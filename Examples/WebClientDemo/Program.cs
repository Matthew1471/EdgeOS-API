using EdgeOS.API;
using EdgeOS.API.Types.REST;
using System;
using System.Collections.Generic;
using System.Configuration;

namespace WebClientDemo
{
    class Program
    {
        static void Main(string[] args)
        {
            // Set the window title to something a bit more interesting.
            if (!Console.IsOutputRedirected) { Console.Title = "WebClient Demo V0.1"; }

            // Check the credentials are provided in the application's configuration file.
            if (ConfigurationManager.AppSettings["Username"] == null || ConfigurationManager.AppSettings["Password"] == null || ConfigurationManager.AppSettings["Host"] == null)
            {
                Console.WriteLine("Program cannot start, some credentials were missing in the program's configuration file.");

                // Exit the application.
                Environment.Exit(1610);
            }

            // EdgeOS requires logins and session heartbeats to be sent via the REST API.
            WebClient webClient = new WebClient(ConfigurationManager.AppSettings["Username"], ConfigurationManager.AppSettings["Password"], "https://" + ConfigurationManager.AppSettings["Host"] + "/");

            // Ignore TLS certificate errors if there is a ".crt" file present that matches this host.
            webClient.AllowLocalCertificates();

            // Login to the router.
            webClient.Login();

            // Test the Authenticate method.
            AuthenticateResponse authenticateResponse = webClient.Authenticate(ConfigurationManager.AppSettings["Username"], ConfigurationManager.AppSettings["Password"]);

            // Add an IP to a FirewallAddressGroup.
            BatchConfigurationTest(webClient);

            // Logout of the router.
            webClient.Logout();
        }

        private static void BatchConfigurationTest(WebClient webClient)
        {
            BatchRequest batchRequest = new BatchRequest
            {
                Set = new EdgeOS.API.Types.REST.Configuration()
                {
                    Firewall = new Firewall()
                    {
                        Group = new FirewallGroup()
                        {
                            AddressGroup = new Dictionary<string, FirewallAddressGroupEntry>()
                            {
                                {
                                    "BannedAddresses", new FirewallAddressGroupEntry()
                                    {
                                        Address = new[] { "4.3.2.1" }
                                    }
                                }
                            }
                        }
                    }
                },

                // Cut down on the amount of bytes being returned by selecting a hopefully empty node.
                Get = new EdgeOS.API.Types.REST.Configuration() { CustomAttribute = new[] { "" } }
            };

            // Add the new data.
            BatchResponse batchResponse = webClient.Batch(batchRequest);
        }
    }
}