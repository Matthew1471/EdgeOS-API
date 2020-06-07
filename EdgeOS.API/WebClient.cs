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
        /// <summary>The EdgeOS username used to login.</summary>
        private readonly string username;

        /// <summary>The EdgeOS password used to login.</summary>
        private readonly string password;

        /// <summary>The EdgeOS SessionID returned after logging in.</summary>
        public string SessionID;

        /// <summary>The HTTP Client object that all requests will be performed from. It may have valid credentials pre-configured if <see cref="Login"/> is invoked.</summary>
        private readonly HttpClient _httpClient;

        /// <summary>Creates an instance of the WebClient which can be used to call EdgeOS API methods.</summary>
        /// <param name="username">The username this instance will use to authenticate with the EdgeOS device.</param>
        /// <param name="password">The password this instance will use to authenticate with the EdgeOS device.</param>
        /// <param name="host">The EdgeOS hostname this instance will contact.</param>
        public WebClient(string username, string password, string host)
        {
            // Store the credentials into a private field.
            this.username = username;
            this.password = password;

            // Prevent .NET from consuming the HTTP 303 that contains our authentication session.
            _httpClient = new HttpClient(new HttpClientHandler() { AllowAutoRedirect = false, CookieContainer = new CookieContainer() })
            {
                // A EdgeOS API endpoint is the hostname.
                BaseAddress = new Uri(host)
            };

            // Be a good net citizen and reveal who we are.
            _httpClient.DefaultRequestHeaders.Add("User-Agent", "C#-EdgeOS-API");

            // Allow only our trusted certificate (if there is one) but otherwise reject any certificate errors.
            ServicePointManager.ServerCertificateValidationCallback += ServerCertificateValidationCallback.PinPublicKey;
        }

        /// <summary>Attempt to login to the EdgeOS device and configure the <seealso cref="HttpClient"/> with the session credentials for future usage.</summary>
        public void Login()
        {
            // Teardown any previous session.
            SessionID = null;

            // Build up the HTML Form.
            List<KeyValuePair<string, string>> loginForm = new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>("username", username),
                new KeyValuePair<string, string>("password", password)
            };

            // Perform the HTTP POST.
            HttpResponseMessage httpResponseMessage = _httpClient.PostAsync("/", new FormUrlEncodedContent(loginForm)).Result;

            // The server does not correctly use HTTP Status codes.
            switch (httpResponseMessage.StatusCode)
            {
                case HttpStatusCode.OK:
                    string response = httpResponseMessage.Content.ReadAsStringAsync().Result;

                    // Check if the login failed (it likely did).
                    if (response.Contains("The username or password you entered is incorrect")) { throw new FormatException("The username or password you entered is incorrect"); }

                    break;
                case HttpStatusCode.SeeOther:
                    // The response headers will contain the session in a cookie if successful.
                    HttpResponseHeaders headers = httpResponseMessage.Headers;

                    // If for whatever reason login fails then a cookie will not be present.
                    if (!(headers.Contains("Set-Cookie"))) { throw new FormatException("Expected header used for Authentication was not present in the response message back from the server."); }

                    // The stats connection requires the session ID for authentication.
                    const string sessionNeedle = "PHPSESSID=";
                    foreach (string cookie in headers.GetValues("Set-Cookie"))
                    {
                        // We are only interested in the PHPSESSID.
                        if (cookie.StartsWith(sessionNeedle)) {
                            int semicolon = cookie.IndexOf(';');
                            SessionID = semicolon == -1 ? cookie.Substring(sessionNeedle.Length) : cookie.Substring(sessionNeedle.Length, semicolon - sessionNeedle.Length);
                            break;
                        }
                    }

                    // There's a chance the authentication has changed and we are no longer reliant on a PHPSESSID.
                    if (SessionID == null) { throw new FormatException("Unable to find session credentials."); }

                    break;
            }
        }

        /// <summary>Ensures proper clean up of the resources.</summary>
        public void Dispose()
        {
            if (_httpClient != null) { _httpClient.Dispose(); }
        }
    }
}