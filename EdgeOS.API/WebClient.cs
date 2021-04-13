using EdgeOS.API.Types.Configuration;
using EdgeOS.API.Types.REST;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Web;

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
            // Prevent .NET from consuming the HTTP 303 that contains our session tokens.
            _httpClient = new HttpClient(new HttpClientHandler() { AllowAutoRedirect = false, CookieContainer = new CookieContainer() })
            {
                // A EdgeOS API endpoint is the hostname.
                BaseAddress = new Uri(host)
            };

            // Be a good net citizen and reveal who we are.
            _httpClient.DefaultRequestHeaders.Add("User-Agent", "C#-EdgeOS-API");

            // Error if a field is missing in a C# class.
            JsonConvert.DefaultSettings = () => new JsonSerializerSettings { MissingMemberHandling = MissingMemberHandling.Error };
        }

        /// <summary>Allows a local .crt certificate file to be used to validate a host.</summary>
        public void AllowLocalCertificates()
        {
            // Ignore certificate trust errors if there is a saved public key pinned.
            ServicePointManager.ServerCertificateValidationCallback = ServerCertificateValidationCallback.PinPublicKey;
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
                    if (!(headers.Contains("Set-Cookie"))) { throw new FormatException("Expected session cookie headers were not present in the response back from the server."); }

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

        /// <summary>Log out of the EdgeOS device.</summary>
        public void Logout()
        {
            _ = _httpClient.GetAsync("/logout").Result;
        }

        #region Edge - General

        /// <summary>Attempt to authenticate with the EdgeOS device and will internally create a session but will not return session tokens to allow further requests. See <see cref="Login"/> to actually login to obtain a session.</summary>
        /// <returns>The response from the device.</returns>
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

        /// <summary>Make a batch query/deletion/update request to specific parts of the device’s configuration.</summary>
        /// <param name="batchRequest">An object containing DELETE/SET/GET operations to perform.</param>
        /// <returns>The response from the device.</returns>
        public ConfigurationSettingsBatchResponse ConfigurationSettingsBatch(ConfigurationSettingsBatchRequest batchRequest)
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

                // Deserialize the responseContent to a ConfigurationSettingsBatchResponse.
                return JsonConvert.DeserializeObject<ConfigurationSettingsBatchResponse>(responseContent);
            }
            else
            {
                // No content returned.
                return null;
            }
        }

        /// <summary>Delete specific parts of the device’s configuration.</summary>
        /// <param name="deleteRequest">The Configuration options to delete from the configuration.</param>
        /// <returns>The response from the device.</returns>
        public ConfigurationSettingsDeleteResponse ConfigurationSettingsDelete(Configuration deleteRequest)
        {
            // Serialize our concrete class into a JSON String.
            string requestContent = JsonConvert.SerializeObject(deleteRequest, new JsonSerializerSettings() { NullValueHandling = NullValueHandling.Ignore });

            // We build up our request.
            HttpRequestMessage httpRequest = new HttpRequestMessage(HttpMethod.Post, "/api/edge/delete.json") { Content = new StringContent(requestContent, Encoding.UTF8, "application/json") };

            // This end-point is protected with a Cross-Site Request Forgery (CSRF) token.
            httpRequest.Headers.Add("X-CSRF-TOKEN", CSRFToken);

            // Send it to the Configuration Settings Delete end-point with the appropriate CSRF header.
            HttpResponseMessage httpResponse = _httpClient.SendAsync(httpRequest).Result;

            // Check the result is what we are expecting (and throw an exception if not).
            httpResponse.EnsureSuccessStatusCode();

            // If the response contains content we want to read it.
            if (httpResponse.Content != null)
            {
                string responseContent = httpResponse.Content.ReadAsStringAsync().Result;

                // Deserialize the responseContent to a ConfigurationSettingsDeleteResponse.
                return JsonConvert.DeserializeObject<ConfigurationSettingsDeleteResponse>(responseContent);
            }
            else
            {
                // No content returned.
                return null;
            }
        }

        /// <summary>Get a predefined part of the device's configuration.</summary>
        /// <returns>The response from the device.</returns>
        public ConfigurationSettingsGetResponse ConfigurationSettingsGetPredefinedList()
        {
            // Send it to the Configuration Settings Get Predefined List end-point.
            HttpResponseMessage httpResponse = _httpClient.GetAsync("/api/edge/get.json").Result;

            // Check the result is what we are expecting (and throw an exception if not).
            httpResponse.EnsureSuccessStatusCode();

            // If the response contains content we want to read it.
            if (httpResponse.Content != null)
            {
                string responseContent = httpResponse.Content.ReadAsStringAsync().Result;

                // Deserialize the responseContent to a ConfigurationSettingsGetResponse.
                return JsonConvert.DeserializeObject<ConfigurationSettingsGetResponse>(responseContent);
            }
            else
            {
                // No content returned.
                return null;
            }
        }

        /// <summary>Get specific part(s) of the device's configuration.</summary>
        /// <returns>The response from the device.</returns>
        public ConfigurationSettingsGetResponse ConfigurationSettingsGetSections(string requestContent)
        {
            // Send it to the Configuration Settings Get Partial end-point.
            HttpResponseMessage httpResponse = _httpClient.GetAsync("/api/edge/partial.json?struct=" + HttpUtility.UrlEncode(requestContent)).Result;

            // Check the result is what we are expecting (and throw an exception if not).
            httpResponse.EnsureSuccessStatusCode();

            // If the response contains content we want to read it.
            if (httpResponse.Content != null)
            {
                string responseContent = httpResponse.Content.ReadAsStringAsync().Result;

                // Deserialize the responseContent to a ConfigurationSettingsGetResponse.
                return JsonConvert.DeserializeObject<ConfigurationSettingsGetResponse>(responseContent);
            }
            else
            {
                // No content returned.
                return null;
            }
        }

        /// <summary>Get specific tree part(s) of the device's configuration.</summary>
        /// <param name="requestPath">The JSON key(s) to filter on, such as ["firewall", "group", "address-group"].</param>
        /// <returns>The response from the device.</returns>
        public ConfigurationSettingsGetTreeResponse ConfigurationSettingsGetTree(string[] requestPath)
        {
            // Build the querystring.
            StringBuilder queryString = new StringBuilder();
            for (int count = 0; count < requestPath.Length; count++)
            {
                if (count > 0) { queryString.Append('&'); }
                queryString.Append("node[]=" + HttpUtility.UrlEncode(requestPath[count]));
            }

            // Send it to the Configuration Settings Get Tree end-point.
            HttpResponseMessage httpResponse = _httpClient.GetAsync("/api/edge/getcfg.json" + (requestPath.Length > 0 ? "?" + queryString.ToString() : null)).Result;

            // Check the result is what we are expecting (and throw an exception if not).
            httpResponse.EnsureSuccessStatusCode();

            // If the response contains content we want to read it.
            if (httpResponse.Content != null)
            {
                string responseContent = httpResponse.Content.ReadAsStringAsync().Result;

                // Deserialize the responseContent to a ConfigurationSettingsGetTreeResponse.
                return JsonConvert.DeserializeObject<ConfigurationSettingsGetTreeResponse>(responseContent);
            }
            else
            {
                // No content returned.
                return null;
            }
        }

        /// <summary>Set specific parts of the device’s configuration.</summary>
        /// <param name="setRequest">The Configuration options to set in the configuration.</param>
        /// <returns>The response from the device.</returns>
        public ConfigurationSettingsSetResponse ConfigurationSettingsSet(Configuration setRequest)
        {
            // Serialize our concrete class into a JSON String.
            string requestContent = JsonConvert.SerializeObject(setRequest, new JsonSerializerSettings() { NullValueHandling = NullValueHandling.Ignore });

            // We build up our request.
            HttpRequestMessage httpRequest = new HttpRequestMessage(HttpMethod.Post, "/api/edge/set.json") { Content = new StringContent(requestContent, Encoding.UTF8, "application/json") };

            // This end-point is protected with a Cross-Site Request Forgery (CSRF) token.
            httpRequest.Headers.Add("X-CSRF-TOKEN", CSRFToken);

            // Send it to the Configuration Settings Set end-point with the appropriate CSRF header.
            HttpResponseMessage httpResponse = _httpClient.SendAsync(httpRequest).Result;

            // Check the result is what we are expecting (and throw an exception if not).
            httpResponse.EnsureSuccessStatusCode();

            // If the response contains content we want to read it.
            if (httpResponse.Content != null)
            {
                string responseContent = httpResponse.Content.ReadAsStringAsync().Result;

                // Deserialize the responseContent to a ConfigurationSettingsSetResponse.
                return JsonConvert.DeserializeObject<ConfigurationSettingsSetResponse>(responseContent);
            }
            else
            {
                // No content returned.
                return null;
            }
        }

        /// <summary>Get information about whether the device is running with default configuration.</summary>
        /// <returns>The response from the device.</returns>
        public DataDefaultConfigurationStatusResponse DataDefaultConfigurationStatus()
        {
            // Send it to the Data end-point.
            HttpResponseMessage httpResponse = _httpClient.GetAsync("/api/edge/data.json?data=default_config").Result;

            // Check the result is what we are expecting (and throw an exception if not).
            httpResponse.EnsureSuccessStatusCode();

            // If the response contains content we want to read it.
            if (httpResponse.Content != null)
            {
                string responseContent = httpResponse.Content.ReadAsStringAsync().Result;

                // Deserialize the responseContent to a DataDefaultConfigurationStatusResponse.
                return JsonConvert.DeserializeObject<DataDefaultConfigurationStatusResponse>(responseContent);
            }
            else
            {
                // No content returned.
                return null;
            }
        }

        /// <summary>Get information about the device's DHCP leases.</summary>
        /// <returns>The response from the device.</returns>
        public DataDHCPLeasesResponse DataDHCPLeases()
        {
            // Send it to the Data end-point.
            HttpResponseMessage httpResponse = _httpClient.GetAsync("/api/edge/data.json?data=dhcp_leases").Result;

            // Check the result is what we are expecting (and throw an exception if not).
            httpResponse.EnsureSuccessStatusCode();

            // If the response contains content we want to read it.
            if (httpResponse.Content != null)
            {
                string responseContent = httpResponse.Content.ReadAsStringAsync().Result;

                // Deserialize the responseContent to a DataDHCPLeasesResponse.
                return JsonConvert.DeserializeObject<DataDHCPLeasesResponse>(responseContent);
            }
            else
            {
                // No content returned.
                return null;
            }
        }

        /// <summary>Get information about the device's DHCP statistics.</summary>
        /// <returns>The response from the device.</returns>
        public DataDHCPStatisticsResponse DataDHCPStatistics()
        {
            // Send it to the Data end-point.
            HttpResponseMessage httpResponse = _httpClient.GetAsync("/api/edge/data.json?data=dhcp_stats").Result;

            // Check the result is what we are expecting (and throw an exception if not).
            httpResponse.EnsureSuccessStatusCode();

            // If the response contains content we want to read it.
            if (httpResponse.Content != null)
            {
                string responseContent = httpResponse.Content.ReadAsStringAsync().Result;

                // Deserialize the responseContent to a DataDHCPStatisticsResponse.
                return JsonConvert.DeserializeObject<DataDHCPStatisticsResponse>(responseContent);
            }
            else
            {
                // No content returned.
                return null;
            }
        }

        /// <summary>Get information about the device's firewall statistics.</summary>
        /// <returns>The response from the device.</returns>
        public DataFirewallStatisticsResponse DataFirewallStatistics()
        {
            // Send it to the Data end-point.
            HttpResponseMessage httpResponse = _httpClient.GetAsync("/api/edge/data.json?data=fw_stats").Result;

            // Check the result is what we are expecting (and throw an exception if not).
            httpResponse.EnsureSuccessStatusCode();

            // If the response contains content we want to read it.
            if (httpResponse.Content != null)
            {
                string responseContent = httpResponse.Content.ReadAsStringAsync().Result;

                // Deserialize the responseContent to a DataFirewallStatisticsResponse.
                return JsonConvert.DeserializeObject<DataFirewallStatisticsResponse>(responseContent);
            }
            else
            {
                // No content returned.
                return null;
            }
        }

        /// <summary>Get information about the device's Network Address Translation (NAT) statistics.</summary>
        /// <returns>The response from the device.</returns>
        public DataNATStatisticsResponse DataNATStatistics()
        {
            // Send it to the Data end-point.
            HttpResponseMessage httpResponse = _httpClient.GetAsync("/api/edge/data.json?data=nat_stats").Result;

            // Check the result is what we are expecting (and throw an exception if not).
            httpResponse.EnsureSuccessStatusCode();

            // If the response contains content we want to read it.
            if (httpResponse.Content != null)
            {
                string responseContent = httpResponse.Content.ReadAsStringAsync().Result;

                // Deserialize the responseContent to a DataNATStatisticsResponse.
                return JsonConvert.DeserializeObject<DataNATStatisticsResponse>(responseContent);
            }
            else
            {
                // No content returned.
                return null;
            }
        }

        /// <summary>Get information about the device's routes.</summary>
        /// <returns>The response from the device.</returns>
        public DataRoutesResponse DataRoutes()
        {
            // Send it to the Data end-point.
            HttpResponseMessage httpResponse = _httpClient.GetAsync("/api/edge/data.json?data=routes").Result;

            // Check the result is what we are expecting (and throw an exception if not).
            httpResponse.EnsureSuccessStatusCode();

            // If the response contains content we want to read it.
            if (httpResponse.Content != null)
            {
                string responseContent = httpResponse.Content.ReadAsStringAsync().Result;

                // Deserialize the responseContent to a DataRoutesResponse.
                return JsonConvert.DeserializeObject<DataRoutesResponse>(responseContent);
            }
            else
            {
                // No content returned.
                return null;
            }
        }


        /// <summary>Get information about the device's system information.</summary>
        /// <returns>The response from the device.</returns>
        public DataSystemInformationResponse DataSystemInformation()
        {
            // Send it to the Data end-point.
            HttpResponseMessage httpResponse = _httpClient.GetAsync("/api/edge/data.json?data=sys_info").Result;

            // Check the result is what we are expecting (and throw an exception if not).
            httpResponse.EnsureSuccessStatusCode();

            // If the response contains content we want to read it.
            if (httpResponse.Content != null)
            {
                string responseContent = httpResponse.Content.ReadAsStringAsync().Result;

                // Deserialize the responseContent to a DataSystemInformationResponse.
                return JsonConvert.DeserializeObject<DataSystemInformationResponse>(responseContent);
            }
            else
            {
                // No content returned.
                return null;
            }
        }

        /// <summary>Attempt to keep the session alive on the EdgeOS device.</summary>
        public void Heartbeat()
        {
            _httpClient.GetAsync("/api/edge/heartbeat.json?_=" + (DateTime.UtcNow - new DateTime(1970, 1, 1)).TotalSeconds);
        }

        /// <summary>Upgrade the device firmware.</summary>
        /// <param name="url">The firmware upgrade url to update the EdgeOS device.</param>
        /// <returns>The response from the device.</returns>
        public UpgradeResponse UpgradeFirmware(Uri url)
        {
            // We build up our request.
            HttpRequestMessage httpRequest = new HttpRequestMessage(HttpMethod.Post, "/api/edge/upgrade.json?action=url_upgrade") { Content = new StringContent("{ url: '" + url.ToString() + "'}", Encoding.UTF8, "application/json") };

            // This end-point is protected with a Cross-Site Request Forgery (CSRF) token.
            //httpRequest.Headers.Add("X-CSRF-TOKEN", CSRFToken);

            // Send it to the Upgrade Firmware end-point with the appropriate CSRF header.
            HttpResponseMessage httpResponse = _httpClient.SendAsync(httpRequest).Result;

            // Check the result is what we are expecting (and throw an exception if not).
            httpResponse.EnsureSuccessStatusCode();

            // If the response contains content we want to read it.
            if (httpResponse.Content != null)
            {
                string responseContent = httpResponse.Content.ReadAsStringAsync().Result;

                // Deserialize the responseContent to a UpgradeResponse.
                return JsonConvert.DeserializeObject<UpgradeResponse>(responseContent);
            }
            else
            {
                // No content returned.
                return null;
            }
        }

        /// <summary>Upgrade the device firmware.</summary>
        /// <param name="filename">The firmware upgrade file to update the EdgeOS device.</param>
        /// <returns>The response from the device.</returns>
        public UpgradeResponse UpgradeFirmware(string filename)
        {
            // Build up the HTML Form.
            using (MultipartFormDataContent content = new MultipartFormDataContent())
            using (StreamContent streamContent = new StreamContent(new FileStream(filename, FileMode.Open, FileAccess.Read)))
            {
                // Set the expected firmware upgrade ContentType.
                streamContent.Headers.ContentType = new MediaTypeHeaderValue("application/x-tar");

                // Add to the MultipartFormData content with the expected form input name.
                content.Add(streamContent, "qqfile", filename);

                // We build up our request (we removed some of the unused params the web UI uses).
                HttpRequestMessage httpRequest = new HttpRequestMessage(HttpMethod.Post, "/api/edge/upgrade.json") { Content = content };

                // This end-point is protected with a Cross-Site Request Forgery (CSRF) token (we supply it as a header rather than in the querystring like the web UI).
                httpRequest.Headers.Add("X-CSRF-TOKEN", CSRFToken);

                // Send it to the Upgrade Firmware end-point with the appropriate CSRF header.
                HttpResponseMessage httpResponse = _httpClient.SendAsync(httpRequest).Result;

                // Check the result is what we are expecting (and throw an exception if not).
                httpResponse.EnsureSuccessStatusCode();

                // If the response contains content we want to read it.
                if (httpResponse.Content != null)
                {
                    string responseContent = httpResponse.Content.ReadAsStringAsync().Result;

                    // Deserialize the responseContent to a UpgradeResponse.
                    return JsonConvert.DeserializeObject<UpgradeResponse>(responseContent);
                }
                else
                {
                    // No content returned.
                    return null;
                }
            }
        }

        //TODO: Wizard Feature method.

        //TODO: Wizard Setup method.

        #endregion

        #region Edge - Configuration

        /// <summary>Save the device's entire configuration to a temporary file on the disk in preparation to download it (see <see cref="ConfigurationDownload()"/>).</summary>
        /// <returns>The response from the device.</returns>
        public ConfigurationDownloadPrepareResponse ConfigurationDownloadPrepare()
        {
            // Send it to the Download Configuration end-point.
            HttpResponseMessage httpResponse = _httpClient.GetAsync("/api/edge/config/save.json").Result;

            // Check the result is what we are expecting (and throw an exception if not).
            httpResponse.EnsureSuccessStatusCode();

            // If the response contains content we want to read it.
            if (httpResponse.Content != null)
            {
                string responseContent = httpResponse.Content.ReadAsStringAsync().Result;

                // Deserialize the responseContent to a ConfigurationDownloadPrepareResponse.
                return JsonConvert.DeserializeObject<ConfigurationDownloadPrepareResponse>(responseContent);
            }
            else
            {
                // No content returned.
                return null;
            }
        }

        /// <summary>Get the device's entire configuration from the temporary file on disk that it was saved into (see <see cref="ConfigurationDownloadPrepare"/>.</summary>
        /// <returns>The response from the device.</returns>
        public Stream ConfigurationDownload()
        {
            // Send it to the Download Configuration end-point.
            HttpResponseMessage httpResponse = _httpClient.GetAsync("/files/config/").Result;

            // Check the result is what we are expecting (and throw an exception if not).
            httpResponse.EnsureSuccessStatusCode();

            // If the response contains content we want to read it.
            if (httpResponse.Content != null)
            {
                // Return the stream.
                return httpResponse.Content.ReadAsStreamAsync().Result;
            }
            else
            {
                // No content returned.
                return null;
            }
        }

        /// <summary>Get the device's entire configuration from the temporary file on disk that it was saved into (see <see cref="ConfigurationDownloadPrepare"/>) and saves it to a file.</summary>
        /// <param name="filename">The filename to save the configuration file to. An exception will occur if it already exists.</param>
        public void ConfigurationDownload(string filename)
        {
            using (FileStream fileStream = new FileStream(filename, FileMode.CreateNew, FileAccess.Write))
            using (Stream stream = ConfigurationDownload())
            {
                stream.CopyTo(fileStream);
            }
        }

        /// <summary>Restore the device configuration from a fileStream.</summary>
        /// <param name="fileStream">The configuration fileStream to restore on the EdgeOS device.</param>
        /// <param name="fileName">The optional filename to share with the EdgeOS device.</param>
        /// <returns>The response from the device.</returns>
        public ConfigurationResponse ConfigurationRestore(Stream fileStream, string fileName = null)
        {
            // Build up the HTML Form.
            using (MultipartFormDataContent content = new MultipartFormDataContent())
            using (StreamContent streamContent = new StreamContent(fileStream))
            {
                // Set the expected firmware upgrade ContentType.
                streamContent.Headers.ContentType = new MediaTypeHeaderValue("application/x-tar");

                // Add to the MultipartFormData content with the expected form input name.
                content.Add(streamContent, "qqfile", fileName);

                // We build up our request (we removed some of the unused params the web UI uses).
                HttpRequestMessage httpRequest = new HttpRequestMessage(HttpMethod.Post, "/api/edge/config/restore.json") { Content = content };

                // This end-point is protected with a Cross-Site Request Forgery (CSRF) token (we supply it as a header rather than in the querystring like the web UI).
                httpRequest.Headers.Add("X-CSRF-TOKEN", CSRFToken);

                // Send it to the Configuration Restore end-point with the appropriate CSRF header.
                HttpResponseMessage httpResponse = _httpClient.SendAsync(httpRequest).Result;

                // Check the result is what we are expecting (and throw an exception if not).
                httpResponse.EnsureSuccessStatusCode();

                // If the response contains content we want to read it.
                if (httpResponse.Content != null)
                {
                    string responseContent = httpResponse.Content.ReadAsStringAsync().Result;

                    // Deserialize the responseContent to a ConfigurationResponse.
                    return JsonConvert.DeserializeObject<ConfigurationResponse>(responseContent);
                }
                else
                {
                    // No content returned.
                    return null;
                }
            }
        }

        /// <summary>Restore the device configuration from a file.</summary>
        /// <param name="fileName">The configuration file to restore on the EdgeOS device.</param>
        /// <returns>The response from the device.</returns>
        public ConfigurationResponse ConfigurationRestore(string fileName)
        {
            using (FileStream fileStream = new FileStream(fileName, FileMode.Open, FileAccess.Read))
            {
                return ConfigurationRestore(fileStream, fileName);
            }
        }

        #endregion

        #region Edge - Optical Network Unit (ONU)

        /// <summary>Reboot the connected Optical Network Unit (ONU) device.</summary>
        /// <param name="serialNumber">The serial number of the ONU you wish to reboot.</param>
        /// <returns>The response from the device.</returns>
        public ONURebootResponse ONUReboot(string serialNumber)
        {
            // We build up our request.
            HttpRequestMessage httpRequest = new HttpRequestMessage(HttpMethod.Post, "/api/edge/onu/reboot.json") { Content = new StringContent("{ serial: '" + serialNumber + "'}", Encoding.UTF8, "application/json") };

            // This end-point is protected with a Cross-Site Request Forgery (CSRF) token.
            //httpRequest.Headers.Add("X-CSRF-TOKEN", CSRFToken);

            // Send it to the ONU Reboot end-point with the appropriate CSRF header.
            HttpResponseMessage httpResponse = _httpClient.SendAsync(httpRequest).Result;

            // Check the result is what we are expecting (and throw an exception if not).
            httpResponse.EnsureSuccessStatusCode();

            // If the response contains content we want to read it.
            if (httpResponse.Content != null)
            {
                string responseContent = httpResponse.Content.ReadAsStringAsync().Result;

                // Deserialize the responseContent to a ONURebootResponse.
                return JsonConvert.DeserializeObject<ONURebootResponse>(responseContent);
            }
            else
            {
                // No content returned.
                return null;
            }
        }

        //TODO: ONU Upgrade method.

        #endregion

        #region Edge - Operations

        /// <summary>Check for firmware updates for the device.</summary>
        /// <returns>The response from the device.</returns>
        public OperationResponse OperationCheckForFirmwareUpdates()
        {
            // We build up our request.
            HttpRequestMessage httpRequest = new HttpRequestMessage(HttpMethod.Post, "/api/edge/operation/refresh-fw-latest-status.json");

            // This end-point is protected with a Cross-Site Request Forgery (CSRF) token.
            httpRequest.Headers.Add("X-CSRF-TOKEN", CSRFToken);

            // Send it to the Operation Check For Firmware Updates end-point with the appropriate CSRF header.
            HttpResponseMessage httpResponse = _httpClient.SendAsync(httpRequest).Result;

            // Check the result is what we are expecting (and throw an exception if not).
            httpResponse.EnsureSuccessStatusCode();

            // If the response contains content we want to read it.
            if (httpResponse.Content != null)
            {
                string responseContent = httpResponse.Content.ReadAsStringAsync().Result;

                // Deserialize the responseContent to a OperationResponse.
                return JsonConvert.DeserializeObject<OperationResponse>(responseContent);
            }
            else
            {
                // No content returned.
                return null;
            }
        }

        /// <summary>Clear Traffic Analysis data.</summary>
        /// <returns>The response from the device.</returns>
        public OperationResponse OperationClearTrafficAnalysis()
        {
            // We build up our request.
            HttpRequestMessage httpRequest = new HttpRequestMessage(HttpMethod.Post, "/api/edge/operation/clear-traffic-analysis.json");

            // This end-point is protected with a Cross-Site Request Forgery (CSRF) token.
            httpRequest.Headers.Add("X-CSRF-TOKEN", CSRFToken);

            // Send it to the Operation Clear Traffic Analysis end-point with the appropriate CSRF header.
            HttpResponseMessage httpResponse = _httpClient.SendAsync(httpRequest).Result;

            // Check the result is what we are expecting (and throw an exception if not).
            httpResponse.EnsureSuccessStatusCode();

            // If the response contains content we want to read it.
            if (httpResponse.Content != null)
            {
                string responseContent = httpResponse.Content.ReadAsStringAsync().Result;

                // Deserialize the responseContent to a OperationResponse.
                return JsonConvert.DeserializeObject<OperationResponse>(responseContent);
            }
            else
            {
                // No content returned.
                return null;
            }
        }

        /// <summary>Reset the device back to its factory-default state (erasing all user-generated files and deleting backup firmware image).</summary>
        /// <returns>The response from the device.</returns>
        public OperationResponse OperationFactoryReset()
        {
            // We build up our request.
            HttpRequestMessage httpRequest = new HttpRequestMessage(HttpMethod.Post, "/api/edge/operation/factory-reset.json");

            // This end-point is protected with a Cross-Site Request Forgery (CSRF) token.
            httpRequest.Headers.Add("X-CSRF-TOKEN", CSRFToken);

            // Send it to the Operation Factory Reset end-point with the appropriate CSRF header.
            HttpResponseMessage httpResponse = _httpClient.SendAsync(httpRequest).Result;

            // Check the result is what we are expecting (and throw an exception if not).
            httpResponse.EnsureSuccessStatusCode();

            // If the response contains content we want to read it.
            if (httpResponse.Content != null)
            {
                string responseContent = httpResponse.Content.ReadAsStringAsync().Result;

                // Deserialize the responseContent to a OperationResponse.
                return JsonConvert.DeserializeObject<OperationResponse>(responseContent);
            }
            else
            {
                // No content returned.
                return null;
            }
        }


        /// <summary>Save the device's support file to a temporary file on the disk in preparation to download it (see <see cref="OperationSupportFileDownload()"/>).</summary>
        /// <returns>The response from the device.</returns>
        public OperationSupportFileDownloadPrepareResponse OperationSupportFileDownloadPrepare()
        {
            // We build up our request.
            HttpRequestMessage httpRequest = new HttpRequestMessage(HttpMethod.Post, "/api/edge/operation/get-support-file.json");

            // This end-point is protected with a Cross-Site Request Forgery (CSRF) token.
            httpRequest.Headers.Add("X-CSRF-TOKEN", CSRFToken);

            // Send it to the Operation Get Support File end-point with the appropriate CSRF header.
            HttpResponseMessage httpResponse = _httpClient.SendAsync(httpRequest).Result;

            // Check the result is what we are expecting (and throw an exception if not).
            httpResponse.EnsureSuccessStatusCode();

            // If the response contains content we want to read it.
            if (httpResponse.Content != null)
            {
                string responseContent = httpResponse.Content.ReadAsStringAsync().Result;

                // Deserialize the responseContent to a OperationSupportFileDownloadPrepareResponse.
                return JsonConvert.DeserializeObject<OperationSupportFileDownloadPrepareResponse>(responseContent);
            }
            else
            {
                // No content returned.
                return null;
            }
        }

        /// <summary>Get the device's support file from the temporary file on disk that it was saved into (see <see cref="OperationSupportFileDownloadPrepare"/>.</summary>
        /// <returns>The response from the device.</returns>
        public Stream OperationSupportFileDownload()
        {
            // Send it to the Operation Get Support File end-point.
            HttpResponseMessage httpResponse = _httpClient.GetAsync("/files/support-file/").Result;

            // Check the result is what we are expecting (and throw an exception if not).
            httpResponse.EnsureSuccessStatusCode();

            // If the response contains content we want to read it.
            if (httpResponse.Content != null)
            {
                // Return the stream.
                return httpResponse.Content.ReadAsStreamAsync().Result;
            }
            else
            {
                // No content returned.
                return null;
            }
        }

        /// <summary>Get the device's support file from the temporary file on disk that it was saved into (see <see cref="OperationSupportFileDownloadPrepare"/>) and saves it to a file.</summary>
        /// <param name="filename">The filename to save the support file to. An exception will occur if it already exists.</param>
        public void OperationSupportFileDownload(string filename)
        {
            using (FileStream fileStream = new FileStream(filename, FileMode.CreateNew, FileAccess.Write))
            using (Stream stream = OperationSupportFileDownload())
            {
                stream.CopyTo(fileStream);
            }
        }


        /// <summary>Reboot the device.</summary>
        /// <returns>The response from the device.</returns>
        public OperationResponse OperationReboot()
        {
            // We build up our request.
            HttpRequestMessage httpRequest = new HttpRequestMessage(HttpMethod.Post, "/api/edge/operation/shutdown.json");

            // This end-point is protected with a Cross-Site Request Forgery (CSRF) token.
            httpRequest.Headers.Add("X-CSRF-TOKEN", CSRFToken);

            // Send it to the Operation Reboot end-point with the appropriate CSRF header.
            HttpResponseMessage httpResponse = _httpClient.SendAsync(httpRequest).Result;

            // Check the result is what we are expecting (and throw an exception if not).
            httpResponse.EnsureSuccessStatusCode();

            // If the response contains content we want to read it.
            if (httpResponse.Content != null)
            {
                string responseContent = httpResponse.Content.ReadAsStringAsync().Result;

                // Deserialize the responseContent to a OperationResponse.
                return JsonConvert.DeserializeObject<OperationResponse>(responseContent);
            }
            else
            {
                // No content returned.
                return null;
            }
        }

        /// <summary>Release the DHCP Lease for a specific interface (defective in most EdgeRouter firmware).</summary>
        /// <param name="interface">The specific interface to request to release the DHCP lease (e.g. eth0).</param>
        /// <returns>The response from the device.</returns>
        public OperationResponse OperationReleaseDHCP(string @interface)
        {
            // Build up the HTML Form.
            List<KeyValuePair<string, string>> releaseDHCPform = new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>("interface", @interface)
            };

            // We build up our request.
            HttpRequestMessage httpRequest = new HttpRequestMessage(HttpMethod.Post, "/api/edge/operation/release-dhcp.json") { Content = new FormUrlEncodedContent(releaseDHCPform) };

            // This end-point is protected with a Cross-Site Request Forgery (CSRF) token.
            httpRequest.Headers.Add("X-CSRF-TOKEN", CSRFToken);

            // Send it to the Operation Release DHCP end-point with the appropriate CSRF header.
            HttpResponseMessage httpResponse = _httpClient.SendAsync(httpRequest).Result;

            // Check the result is what we are expecting (and throw an exception if not).
            httpResponse.EnsureSuccessStatusCode();

            // If the response contains content we want to read it.
            if (httpResponse.Content != null)
            {
                string responseContent = httpResponse.Content.ReadAsStringAsync().Result;

                // Deserialize the responseContent to a OperationResponse.
                return JsonConvert.DeserializeObject<OperationResponse>(responseContent);
            }
            else
            {
                // No content returned.
                return null;
            }
        }

        /// <summary>Renew the DHCP Lease for a specific interface (defective in most EdgeRouter firmware).</summary>
        /// <param name="interface">The specific interface to request to renew the DHCP lease (e.g. eth0).</param>
        /// <returns>The response from the device.</returns>
        public OperationResponse OperationRenewDHCP(string @interface)
        {
            // Build up the HTML Form.
            List<KeyValuePair<string, string>> renewDHCPform = new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>("interface", @interface)
            };

            // We build up our request.
            HttpRequestMessage httpRequest = new HttpRequestMessage(HttpMethod.Post, "/api/edge/operation/renew-dhcp.json") { Content = new FormUrlEncodedContent(renewDHCPform) };

            // This end-point is protected with a Cross-Site Request Forgery (CSRF) token.
            httpRequest.Headers.Add("X-CSRF-TOKEN", CSRFToken);

            // Send it to the Operation Renew DHCP end-point with the appropriate CSRF header.
            HttpResponseMessage httpResponse = _httpClient.SendAsync(httpRequest).Result;

            // Check the result is what we are expecting (and throw an exception if not).
            httpResponse.EnsureSuccessStatusCode();

            // If the response contains content we want to read it.
            if (httpResponse.Content != null)
            {
                string responseContent = httpResponse.Content.ReadAsStringAsync().Result;

                // Deserialize the responseContent to a OperationResponse.
                return JsonConvert.DeserializeObject<OperationResponse>(responseContent);
            }
            else
            {
                // No content returned.
                return null;
            }
        }

        /// <summary>Reset the device configuration back to default values (backup firmware image and user-generated files will remain intact).</summary>
        /// <returns>The response from the device.</returns>
        public OperationResponse OperationResetDefaultConfiguration()
        {
            // We build up our request.
            HttpRequestMessage httpRequest = new HttpRequestMessage(HttpMethod.Post, "/api/edge/operation/reset-default-config.json");

            // This end-point is protected with a Cross-Site Request Forgery (CSRF) token.
            httpRequest.Headers.Add("X-CSRF-TOKEN", CSRFToken);

            // Send it to the Operation Reset Default Configuration end-point with the appropriate CSRF header.
            HttpResponseMessage httpResponse = _httpClient.SendAsync(httpRequest).Result;

            // Check the result is what we are expecting (and throw an exception if not).
            httpResponse.EnsureSuccessStatusCode();

            // If the response contains content we want to read it.
            if (httpResponse.Content != null)
            {
                string responseContent = httpResponse.Content.ReadAsStringAsync().Result;

                // Deserialize the responseContent to a OperationResponse.
                return JsonConvert.DeserializeObject<OperationResponse>(responseContent);
            }
            else
            {
                // No content returned.
                return null;
            }
        }

        /// <summary>Shutdown the device.</summary>
        /// <returns>The response from the device.</returns>
        public OperationResponse OperationShutdown()
        {
            // We build up our request.
            HttpRequestMessage httpRequest = new HttpRequestMessage(HttpMethod.Post, "/api/edge/operation/shutdown.json");

            // This end-point is protected with a Cross-Site Request Forgery (CSRF) token.
            httpRequest.Headers.Add("X-CSRF-TOKEN", CSRFToken);

            // Send it to the Operation Shutdown end-point with the appropriate CSRF header.
            HttpResponseMessage httpResponse = _httpClient.SendAsync(httpRequest).Result;

            // Check the result is what we are expecting (and throw an exception if not).
            httpResponse.EnsureSuccessStatusCode();

            // If the response contains content we want to read it.
            if (httpResponse.Content != null)
            {
                string responseContent = httpResponse.Content.ReadAsStringAsync().Result;

                // Deserialize the responseContent to a OperationResponse.
                return JsonConvert.DeserializeObject<OperationResponse>(responseContent);
            }
            else
            {
                // No content returned.
                return null;
            }
        }

        #endregion

        #region Optical Line Terminal (OLT) - General

        /// <summary>Get the connected ONU devices (MAC Addresses).</summary>
        /// <param name="serialNumber">The serial number of the connected ONU to request the MAC addresses for.</param>
        /// <returns>The response from the device.</returns>
        public OperationResponse OLTConnectedONUDevices(string serialNumber)
        {
            // Send our request to the OLT Get ONU MACs end-point.
            HttpResponseMessage httpResponse = _httpClient.GetAsync("/api/olt/get-onu-macs.json?serial=" + serialNumber).Result;

            // Check the result is what we are expecting (and throw an exception if not).
            httpResponse.EnsureSuccessStatusCode();

            // If the response contains content we want to read it.
            if (httpResponse.Content != null)
            {
                string responseContent = httpResponse.Content.ReadAsStringAsync().Result;

                // Deserialize the responseContent to a OperationResponse.
                return JsonConvert.DeserializeObject<OperationResponse>(responseContent);
            }
            else
            {
                // No content returned.
                return null;
            }
        }

        #endregion

        #region Optical Line Terminal (OLT) - Optical Network Unit (ONU)

        /// <summary>Save the connected ONU's support file to a temporary file on the disk in preparation to download it (see <see cref="OLTConnectedONUSupportFileDownload(string)"/>).</summary>
        /// <param name="serialNumber">The serial number of the connected ONU to request a support file for.</param>
        /// <returns>The response from the device.</returns>
        public OperationSupportFileDownloadPrepareResponse OLTConnectedONUSupportFileDownloadPrepare(string serialNumber)
        {
            // Send our request to the OLT ONU Get Support File end-point.
            HttpResponseMessage httpResponse = _httpClient.GetAsync("/api/olt/onu/get-support-file.json?serial=" + serialNumber).Result;

            // Check the result is what we are expecting (and throw an exception if not).
            httpResponse.EnsureSuccessStatusCode();

            // If the response contains content we want to read it.
            if (httpResponse.Content != null)
            {
                string responseContent = httpResponse.Content.ReadAsStringAsync().Result;

                // Deserialize the responseContent to a OperationSupportFileDownloadPrepareResponse.
                return JsonConvert.DeserializeObject<OperationSupportFileDownloadPrepareResponse>(responseContent);
            }
            else
            {
                // No content returned.
                return null;
            }
        }

        /// <summary>Get the connected ONU's support file from the temporary file on disk that it was saved into (see <see cref="OLTConnectedONUSupportFileDownloadPrepare"/>.</summary>
        /// <param name="fileName">The filename of the OLT ONU support file provided by <see cref="OLTConnectedONUSupportFileDownloadPrepare(string)"/></param>
        /// <returns>The response from the device.</returns>
        public Stream OLTConnectedONUSupportFileDownload(string fileName)
        {
            // Send our request to the OLT ONU Support File end-point.
            HttpResponseMessage httpResponse = _httpClient.GetAsync("/api/olt/onu/support-file/?filename=" + fileName).Result;

            // Check the result is what we are expecting (and throw an exception if not).
            httpResponse.EnsureSuccessStatusCode();

            // If the response contains content we want to read it.
            if (httpResponse.Content != null)
            {
                // Return the stream.
                return httpResponse.Content.ReadAsStreamAsync().Result;
            }
            else
            {
                // No content returned.
                return null;
            }
        }

        /// <summary>Get the connected ONU's support file from the temporary file on disk that it was saved into (see <see cref="OLTConnectedONUSupportFileDownloadPrepare(string)"/>) and saves it to a file.</summary>
        /// <param name="inputFilename">The filename of the OLT ONU support file provided by <see cref="OLTConnectedONUSupportFileDownloadPrepare(string)"/></param>
        /// <param name="outputFilename">The filename to save the support file to. An exception will occur if it already exists.</param>
        public void OLTConnectedONUSupportFileDownload(string inputFilename, string outputFilename)
        {
            using (FileStream fileStream = new FileStream(outputFilename, FileMode.CreateNew, FileAccess.Write))
            using (Stream stream = OLTConnectedONUSupportFileDownload(inputFilename))
            {
                stream.CopyTo(fileStream);
            }
        }

        /// <summary>Get a specific connected ONU's WiFi Clients.</summary>
        /// <param name="serialNumber">The serial number of the connected ONU to request the WiFi Clients for.</param>
        /// <returns>The response from the device.</returns>
        public OperationResponse OLTConnectedONUWiFiClients(string serialNumber)
        {
            // Send it to the OLT ONU Get WiFi Clients end-point.
            HttpResponseMessage httpResponse = _httpClient.GetAsync("/api/olt/onu/get-wifi-clients.json?serial=" + serialNumber).Result;

            // Check the result is what we are expecting (and throw an exception if not).
            httpResponse.EnsureSuccessStatusCode();

            // If the response contains content we want to read it.
            if (httpResponse.Content != null)
            {
                string responseContent = httpResponse.Content.ReadAsStringAsync().Result;

                // Deserialize the responseContent to a OperationResponse.
                return JsonConvert.DeserializeObject<OperationResponse>(responseContent);
            }
            else
            {
                // No content returned.
                return null;
            }
        }

        /// <summary>Locate a specific connected ONU.</summary>
        /// <param name="serialNumber">The serial number of the connected ONU to locate.</param>
        /// <returns>The response from the device.</returns>
        public OperationResponse OLTConnectedONULocate(string serialNumber)
        {
            // Send it to the OLT ONU Locate end-point.
            HttpResponseMessage httpResponse = _httpClient.GetAsync("/api/olt/onu/locate.json?serial=" + serialNumber).Result;

            // Check the result is what we are expecting (and throw an exception if not).
            httpResponse.EnsureSuccessStatusCode();

            // If the response contains content we want to read it.
            if (httpResponse.Content != null)
            {
                string responseContent = httpResponse.Content.ReadAsStringAsync().Result;

                // Deserialize the responseContent to a OperationResponse.
                return JsonConvert.DeserializeObject<OperationResponse>(responseContent);
            }
            else
            {
                // No content returned.
                return null;
            }
        }

        /// <summary>Reset a specific connected ONU.</summary>
        /// <param name="serialNumber">The serial number of the connected ONU to reset.</param>
        /// <returns>The response from the device.</returns>
        public OperationResponse OLTConnectedONUReset(string serialNumber)
        {
            // Send it to the OLT ONU Reset end-point.
            HttpResponseMessage httpResponse = _httpClient.GetAsync("/api/olt/onu/reset.json?serial=" + serialNumber).Result;

            // Check the result is what we are expecting (and throw an exception if not).
            httpResponse.EnsureSuccessStatusCode();

            // If the response contains content we want to read it.
            if (httpResponse.Content != null)
            {
                string responseContent = httpResponse.Content.ReadAsStringAsync().Result;

                // Deserialize the responseContent to a OperationResponse.
                return JsonConvert.DeserializeObject<OperationResponse>(responseContent);
            }
            else
            {
                // No content returned.
                return null;
            }
        }

        #endregion

        #region Wizards

        //TODO: Wizards List All Wizards method.

        //TODO: Wizards Specific Wizard Create method.

        //TODO: Wizards Specific Wizard Download method.

        //TODO: Wizards Specific Wizard Remove method.

        //TODO: Wizards Specific Wizard Upload method.

        #endregion

        /// <summary>Ensures proper clean up of the resources.</summary>
        public void Dispose()
        {
            if (_httpClient != null)
            {

                // Attempt to log out.
                if (SessionID != null) { Logout(); }

                // Dispose of the _httpClient field.
                _httpClient.Dispose();
            }
        }
    }
}