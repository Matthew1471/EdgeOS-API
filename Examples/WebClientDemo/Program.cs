using EdgeOS.API;
using EdgeOS.API.Types.Configuration;
using EdgeOS.API.Types.REST;
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

            // Edge - General
            //EdgeGeneralTests(webClient);

            // Edge - Configuration
            //EdgeConfigurationTests(webClient);

            // Edge - Optical Network Unit (ONU)
            //EdgeONUTests(webClient);

            // Edge - Operations
            //EdgeOperationsTests(webClient);

            // Optical Line Terminal (OLT) - General
            //OLTGeneralTests(webClient);

            // Optical Line Terminal (OLT) - Optical Network Unit (ONU)
            //OLTConnectedONUTests(webClient);

            // Wizards

            // Logout of the router.
            webClient.Logout();
        }

        private static void EdgeGeneralTests(WebClient webClient)
        {
            // Test the Authenticate method.
            AuthenticateResponse authenticateResponse = webClient.Authenticate(ConfigurationManager.AppSettings["Username"], ConfigurationManager.AppSettings["Password"]);

            EdgeOS.API.Types.Configuration.Configuration exampleConfiguration = new EdgeOS.API.Types.Configuration.Configuration()
            {
                Firewall = new Firewall()
                {
                    Group = new FirewallGroup()
                    {
                        AddressGroup = new Dictionary<string, AddressGroupEntry>()
                            {
                                {
                                    "APITestAddresses", new AddressGroupEntry()
                                    {
                                        Address = new[] { "4.3.2.1" }
                                    }
                                }
                            }
                    }
                }
            };

            // Add an IP to a FirewallAddressGroup (creating it if it doesn't already exist).
            ConfigurationSettingsBatchResponse configurationSettingsBatchResponse = webClient.ConfigurationSettingsBatch(new ConfigurationSettingsBatchRequest
            {
                // Add an IP address group.
                Set = exampleConfiguration,

                // Cut down on the amount of bytes being returned by selecting a hopefully empty node.
                Get = new EdgeOS.API.Types.Configuration.Configuration() { CustomAttribute = new[] { "" } }
            });

            // Delete APITestAddresses.
            exampleConfiguration.Firewall.Group.AddressGroup["APITestAddresses"] = null;

            // Remove the FirewallAddressGroup.
            ConfigurationSettingsDeleteResponse configurationSettingsDeleteResponse = webClient.ConfigurationSettingsDelete(exampleConfiguration);

            // Get predefined config list.
            ConfigurationSettingsGetResponse configurationSettingsGetPredefinedListResponse = webClient.ConfigurationSettingsGetPredefinedList();

            // Get sections of partial config.
            ConfigurationSettingsGetResponse configurationSettingsGetSectionsResponse = webClient.ConfigurationSettingsGetSections("{\"firewall\":null, \"protocols\":null}");

            // Get tree config.
            ConfigurationSettingsGetTreeResponse configurationSettingsGetTreeResponse = webClient.ConfigurationSettingsGetTree(new[] { "system", "ntp" });
            configurationSettingsGetTreeResponse = webClient.ConfigurationSettingsGetTree(new[] { "system" });
            configurationSettingsGetTreeResponse = webClient.ConfigurationSettingsGetTree(new[] { "firewall", "group", "address-group" });

            // Set a FirewallAddressGroup.
            ConfigurationSettingsSetResponse configurationSettingsSetResponse = webClient.ConfigurationSettingsSet(exampleConfiguration);

            // Data methods.
            DataDefaultConfigurationStatusResponse dataDefaultConfigurationStatusResponse = webClient.DataDefaultConfigurationStatus();
            DataDHCPLeasesResponse dataDHCPLeasesResponse = webClient.DataDHCPLeases();
            DataDHCPStatisticsResponse dataDHCPStatisticsResponse = webClient.DataDHCPStatistics();
            DataFirewallStatisticsResponse dataFirewallStatisticsResponse = webClient.DataFirewallStatistics();
            DataNATStatisticsResponse dataNATStatisticsResponse = webClient.DataNATStatistics();
            DataRoutesResponse dataRoutesResponse = webClient.DataRoutes();
            DataSystemInformationResponse dataSystemInformationResponse = webClient.DataSystemInformation();

            // Heartbeat.
            webClient.Heartbeat();

            // Firmware update.
            UpgradeResponse upgradeFirmware = webClient.UpgradeFirmware("EdgeOS.API.dll");
            upgradeFirmware = webClient.UpgradeFirmware(new Uri("http://localhost/firmware.bin"));

            //TODO: Wizard Feature.
            //TODO: Wizard Setup.
        }

        private static void EdgeConfigurationTests(WebClient webClient)
        {
            // Download the configuration.
            File.Delete("Config.tar.gz");
            ConfigurationDownloadPrepareResponse configurationDownloadResponse = webClient.ConfigurationDownloadPrepare();
            webClient.ConfigurationDownload("Config.tar.gz");

            // Restore the configuration.
            ConfigurationResponse configurationRestoreResponse = webClient.ConfigurationRestore("Config.tar.gz");
        }

        private static void EdgeONUTests(WebClient webClient)
        {
            ONURebootResponse onuRebootResponse = webClient.ONUReboot("000000000000");

            //TODO: ONU Upgrade.
        }

        private static void EdgeOperationsTests(WebClient webClient)
        {
            // Test operations.
            OperationResponse operationCheckForFirmwareUpdatesResponse = webClient.OperationCheckForFirmwareUpdates();
            OperationResponse operationClearTrafficAnalysisResponse = webClient.OperationClearTrafficAnalysis();

            // Obtain support file.
            OperationSupportFileDownloadPrepareResponse operationSupportFileDownloadPrepareResponse = webClient.OperationSupportFileDownloadPrepare();
            webClient.OperationSupportFileDownload("Support.tar.gz");

            // Test more operations.
            OperationResponse operationFactoryResetResponse = webClient.OperationFactoryReset();
            OperationResponse operationRebootResponse = webClient.OperationReboot();

            // Release and Renew DHCP lease.
            OperationResponse operationReleaseDHCPResponse = webClient.OperationReleaseDHCP("eth0");
            OperationResponse operationRenewDHCPResponse = webClient.OperationRenewDHCP("eth0");

            // Test other operations.
            OperationResponse operationShutdownResponse = webClient.OperationShutdown();
            OperationResponse operationResetDefaultConfigurationResponse = webClient.OperationResetDefaultConfiguration();
        }

        private static void OLTGeneralTests(WebClient webClient)
        {
            OperationResponse oltConnectedDevicesResponse = webClient.OLTConnectedONUDevices("000000000000");
        }

        private static void OLTConnectedONUTests(WebClient webClient)
        {
            OperationSupportFileDownloadPrepareResponse operationSupportFileDownloadPrepareResponse = webClient.OLTConnectedONUSupportFileDownloadPrepare("000000000000");
            webClient.OLTConnectedONUSupportFileDownload(operationSupportFileDownloadPrepareResponse.Operation.Path, "SupportFile.tar.gz");

            OperationResponse oltConnectedONUWiFiClientsResponse = webClient.OLTConnectedONUWiFiClients("000000000000");
            OperationResponse oltConnectedONULocateResponse = webClient.OLTConnectedONULocate("000000000000");
            OperationResponse oltConnectedONUResetResponse = webClient.OLTConnectedONUReset("000000000000");
        }
    }
}