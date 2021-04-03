using EdgeOS.API.Types.REST;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;

namespace EdgeOS.API
{
    /// <summary>Provides an API into EdgeOS based off the official API.</summary>
    public class WebClient : IDisposable
    {
        /// <summary>The EdgeOS SessionID returned after logging in.</summary>
        public string SessionID;

        /// <summary>The EdgeOS Cross-Site Request Forgery (CSRF) token returned after logging in.</summary>
        public string CSRFToken;

        /// <summary>The HTTP Client object that all requests will be performed from. It may have valid credentials pre-configured if <see cref="Login"/> is invoked.</summary>
        private readonly HttpClient _httpClient;

        /// <summary>Creates an instance of the WebClient which can be used to call EdgeOS API methods.</summary>
        /// <param name="host">The EdgeOS hostname this instance will contact.</param>
        public WebClient(string host)
        {
            // Prevent .NET from consuming the HTTP 303 that contains our authentication session.
            _httpClient = new HttpClient(new HttpClientHandler() { AllowAutoRedirect = false, CookieContainer = new CookieContainer() })
            {
                // A EdgeOS API endpoint is the hostname.
                BaseAddress = new Uri(host)
            };

            // Be a good net citizen and reveal who we are.
            _httpClient.DefaultRequestHeaders.Add("User-Agent", "C#-EdgeOS-API");
        }

        /// <summary>Allows a local .crt certificate file to be used to validate a host.</summary>
        public void AllowLocalCertificates()
        {
            // Ignore certificate trust errors if there is a saved public key pinned.
            ServicePointManager.ServerCertificateValidationCallback = ServerCertificateValidationCallback.PinPublicKey;
        }

        /// <summary>Attempt to authenticate with the EdgeOS device and will internally create a session but will not return session tokens to allow further requests. See <see cref="Login(string, string)"/> to actually login to obtain a session.</summary>
        public AuthenticateResponse Authenticate(string username, string password)
        {
            // Build up the HTML Form.
            List<KeyValuePair<string, string>> loginForm = new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>("username", username),
                new KeyValuePair<string, string>("password", password)
            };

            // Perform the HTTP POST.
            HttpResponseMessage httpResponse = _httpClient.PostAsync("/api/edge/auth.json", new FormUrlEncodedContent(loginForm)).Result;

            // Check the result is what we are expecting (and throw an exception if not).
            httpResponse.EnsureSuccessStatusCode();

            // If the response contains content we want to read it.
            if (httpResponse.Content != null)
            {
                string responseContent = httpResponse.Content.ReadAsStringAsync().Result;

                // Deserialize the responseContent to a AuthenticateResponse.
                return JsonConvert.DeserializeObject<AuthenticateResponse>(responseContent);
            }
            else
            {
                // No content returned.
                return null;
            }
        }

        /// <summary>Attempt to login to the EdgeOS device and configure the <seealso cref="HttpClient"/> with the session credentials for future usage.</summary>
        /// <param name="username">The username this instance will use to login to the EdgeOS device.</param>
        /// <param name="password">The password this instance will use to login to the EdgeOS device.</param>
        public void Login(string username, string password)
        {
            // Teardown any previous session.
            SessionID = null;
            CSRFToken = null;

            // Build up the HTML Form.
            List<KeyValuePair<string, string>> loginForm = new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>("username", username),
                new KeyValuePair<string, string>("password", password)
            };

            // Perform the HTTP POST.
            HttpResponseMessage httpResponse = _httpClient.PostAsync("/", new FormUrlEncodedContent(loginForm)).Result;

            // The server does not correctly use HTTP Status codes.
            switch (httpResponse.StatusCode)
            {
                case HttpStatusCode.OK:
                    string response = httpResponse.Content.ReadAsStringAsync().Result;

                    // Check if the login failed (it likely did).
                    if (response.Contains("The username or password you entered is incorrect")) { throw new FormatException("The username or password you entered is incorrect"); }

                    break;
                case HttpStatusCode.SeeOther:
                    // The response headers will contain the session in a cookie if successful.
                    HttpResponseHeaders headers = httpResponse.Headers;

                    // If for whatever reason login fails then a cookie will not be present.
                    if (!(headers.Contains("Set-Cookie"))) { throw new FormatException("Expected header used for Authentication was not present in the response message back from the server."); }

                    // The stats connection requires the session ID for authentication.
                    const string sessionNeedle = "PHPSESSID=";

                    // The X-CSRF-TOKEN is used to validate sensitive HTTP POSTs.
                    const string csrfNeedle = "X-CSRF-TOKEN=";

                    foreach (string cookie in headers.GetValues("Set-Cookie"))
                    {
                        // We are only interested in the PHPSESSID and X-CSRF-TOKEN.
                        if (cookie.StartsWith(sessionNeedle))
                        {
                            int semicolon = cookie.IndexOf(';');
                            SessionID = semicolon == -1 ? cookie.Substring(sessionNeedle.Length) : cookie.Substring(sessionNeedle.Length, semicolon - sessionNeedle.Length);
                        }
                        else if (cookie.StartsWith(csrfNeedle))
                        {
                            int semicolon = cookie.IndexOf(';');
                            CSRFToken = semicolon == -1 ? cookie.Substring(csrfNeedle.Length) : cookie.Substring(csrfNeedle.Length, semicolon - sessionNeedle.Length);
                        }

                        // Do we have everything to break out of the loop?
                        if (SessionID != null && CSRFToken != null) { break; }
                    }

                    // There's a chance the authentication has changed and we are no longer reliant on a PHPSESSID.
                    if (SessionID == null) { throw new FormatException("Unable to find session credentials."); }

                    break;
            }
        }

        /// <summary>Attempt to keep the session alive on the EdgeOS device.</summary>
        public void Heartbeat()
        {
            _httpClient.GetAsync("/api/edge/heartbeat.json?_=" + (DateTime.UtcNow - new DateTime(1970, 1, 1)).TotalSeconds);
        }

        /// <summary>Log out of the EdgeOS device.</summary>
        public void Logout()
        {
            _httpClient.GetAsync("/logout");
        }

        /// <summary>Make a batch query/deletion/update request to specific parts of the device’s configuration.</summary>
        /// <param name="batchRequest"></param>
        public BatchResponse Batch(BatchRequest batchRequest)
        {
            // Serialize our concrete class into a JSON String.
            string requestContent = JsonConvert.SerializeObject(batchRequest, new JsonSerializerSettings() { NullValueHandling = NullValueHandling.Ignore });

            // We build up our request.
            HttpRequestMessage httpRequest = new HttpRequestMessage(HttpMethod.Post, "/api/edge/batch.json") { Content = new StringContent(requestContent, Encoding.UTF8, "application/json") };

            // This end-point is protected with a Cross-Site Request Forgery (CSRF) token.
            httpRequest.Headers.Add("X-CSRF-TOKEN", CSRFToken);

            // Send it to the Configuration Settings Batch end-point with the appropriate CSRF header.
            HttpResponseMessage httpResponse = _httpClient.SendAsync(httpRequest).Result;

            // Check the result is what we are expecting (and throw an exception if not).
            httpResponse.EnsureSuccessStatusCode();

            // If the response contains content we want to read it.
            if (httpResponse.Content != null)
            {
                string responseContent = httpResponse.Content.ReadAsStringAsync().Result;

                // Deserialize the responseContent to a BatchResponse.
                return JsonConvert.DeserializeObject<BatchResponse>(responseContent);
            }
            else
            {
                // No content returned.
                return null;
            }
        }

        /// <summary>Ensures proper clean up of the resources.</summary>
        public void Dispose()
        {
            if (_httpClient != null) { _httpClient.Dispose(); }
        }
    }
}