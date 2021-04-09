using EdgeOS.API;
using EdgeOS.API.Types.Configuration;
using EdgeOS.API.Types.REST.Requests;
using EdgeOS.API.Types.REST.Responses;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;

namespace WebClientDemo
{
    class Program
    {
        static void Main()
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
            WebClient webClient = new WebClient("https://" + ConfigurationManager.AppSettings["Host"] + "/");

            // Ignore TLS certificate errors if there is a ".crt" file present that matches this host.
            webClient.AllowLocalCertificates();

            // Login to the router.
            webClient.Login(ConfigurationManager.AppSettings["Username"], ConfigurationManager.AppSettings["Password"]);

            // Test the Authenticate method.
            //AuthenticateResponse authenticateResponse = webClient.Authenticate(ConfigurationManager.AppSettings["Username"], ConfigurationManager.AppSettings["Password"]);

            EdgeOS.API.Types.Configuration.Configuration exampleConfiguration = new EdgeOS.API.Types.Configuration.Configuration()
            {
                Firewall = new Firewall()
                {
                    Group = new FirewallGroup()
                    {
                        AddressGroup = new Dictionary<string, FirewallAddressGroupEntry>()
                            {
                                {
                                    "APITestAddresses", new FirewallAddressGroupEntry()
                                    {
                                        Address = new[] { "4.3.2.1" }
                                    }
                                }
                            }
                    }
                }
            };

            // Add an IP to a FirewallAddressGroup (creating it if it doesn't already exist).
            //ConfigurationSettingsBatchTest(webClient, exampleConfiguration);

            // Delete APITestAddresses.
            //exampleConfiguration.Firewall.Group.AddressGroup["APITestAddresses"] = null;

            // Remove the FirewallAddressGroup.
            //ConfigurationSettingsDeleteTest(webClient, exampleConfiguration);

            // Get predefined config list.
            //ConfigurationSettingsGetResponse configurationSettingsGetPredefinedListResponse = webClient.ConfigurationSettingsGetPredefinedList();

            // Get sections of partial config.
            //ConfigurationSettingsGetResponse configurationSettingsGetSectionsResponse = webClient.ConfigurationSettingsGetSections("{\"firewall\":null, \"protocols\":null}");

            // Get tree config.
            //ConfigurationSettingsGetTreeResponse configurationSettingsGetTreeResponse = webClient.ConfigurationSettingsGetTree(new[] { "system", "ntp" });
            //configurationSettingsGetTreeResponse = webClient.ConfigurationSettingsGetTree(new[] { "system" });
            //configurationSettingsGetTreeResponse = webClient.ConfigurationSettingsGetTree(new[] { "firewall", "group", "address-group" });

            // Set a FirewallAddressGroup.
            //ConfigurationSettingsSetResponse configurationSettingsSetResponse = webClient.ConfigurationSettingsSet(exampleConfiguration);

            // Test operations.
            //OperationResponse operationCheckForFirmwareUpdates = webClient.OperationCheckForFirmwareUpdates();
            //OperationResponse operationClearTrafficAnalysis = webClient.OperationClearTrafficAnalysis();
            //OperationResponse operationFactoryReset = webClient.OperationFactoryReset();
            //OperationResponse operationReboot = webClient.OperationReboot();

            // Release and Renew DHCP lease.
            //OperationResponse operationReleaseResponse = webClient.OperationReleaseDHCP("eth0");
            //OperationResponse operationRenewResponse = webClient.OperationRenewDHCP("eth0");

            // Test other operations.
            //OperationResponse operationShutdown = webClient.OperationShutdown();
            //OperationResponse operationResetDefaultConfiguration = webClient.OperationResetDefaultConfiguration();

            // Download the configuration.
            /*
            ConfigurationDownloadPrepareResponse configurationDownloadResponse = webClient.ConfigurationDownloadPrepare();

            using (FileStream fileStream = new FileStream("Config.tar.gz", FileMode.Create, FileAccess.Write))
            using (Stream stream = webClient.ConfigurationDownload())
            {
                stream.CopyTo(fileStream);
            }
            */

            // Logout of the router.
            webClient.Logout();
        }

        private static void ConfigurationSettingsBatchTest(WebClient webClient, EdgeOS.API.Types.Configuration.Configuration exampleConfiguration)
        {
            ConfigurationSettingsBatchRequest configurationSettingsBatchRequest = new ConfigurationSettingsBatchRequest
            {
                // Add an IP address group.
                Set = exampleConfiguration,

                // Cut down on the amount of bytes being returned by selecting a hopefully empty node.
                Get = new EdgeOS.API.Types.Configuration.Configuration() { CustomAttribute = new[] { "" } }
            };

            // Add the new data.
            ConfigurationSettingsBatchResponse configurationSettingsBatchResponse = webClient.ConfigurationSettingsBatch(configurationSettingsBatchRequest);
        }

        private static void ConfigurationSettingsDeleteTest(WebClient webClient, EdgeOS.API.Types.Configuration.Configuration exampleConfiguration)
        {
            // Delete the new data.
            ConfigurationSettingsDeleteResponse configurationSettingsDeleteResponse = webClient.ConfigurationSettingsDelete(exampleConfiguration);
        }
    }
}