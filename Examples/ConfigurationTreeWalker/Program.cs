using EdgeOS.API;
using EdgeOS.API.Types.REST;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.IO;
using System.Text;

namespace ConfigurationTreeWalker
{
    class Program
    {
        static void Main(string[] args)
        {
            // Set the window title to something a bit more interesting.
            if (!Console.IsOutputRedirected) { Console.Title = "ConfigurationTreeWalker V0.1"; }

            // Check the credentials are provided in the application's configuration file.
            if (ConfigurationManager.AppSettings["Username"] == null || ConfigurationManager.AppSettings["Password"] == null || ConfigurationManager.AppSettings["Host"] == null)
            {
                Console.WriteLine("Program cannot start, some credentials were missing in the program's configuration file.");

                // Exit the application.
                Environment.Exit(1610);
            }

            // EdgeOS requires logins and session heartbeats to be sent via the REST API.
            using (WebClient webClient = new WebClient("https://" + ConfigurationManager.AppSettings["Host"] + "/"))
            {
                // Ignore TLS certificate errors if there is a ".crt" file present that matches this host.
                webClient.AllowLocalCertificates();

                // Login to the router.
                webClient.Login(ConfigurationManager.AppSettings["Username"], ConfigurationManager.AppSettings["Password"]);

                // If the folder exists, delete it then recreate it.
                if (Directory.Exists("Configuration")) { Directory.Delete("Configuration", true); }

                // Start walking the classes.
                CreateClass(webClient, null);

                // Create the Configuration Boolean helper class.
                CreateConfigurationBoolClass();
            }
        }

        private static void CreateConfigurationBoolClass()
        {
            File.WriteAllText(@"Configuration\ConfigurationBool.cs",
@"using System.Runtime.Serialization;

namespace EdgeOS.API.Types.Configuration
{
    /// <summary>EdgeOS Configuration Boolean.</summary>
    public enum ConfigurationBool : byte
    {
        /// <summary>This setting has not been configured.</summary>
        [EnumMember(Value = null)]
        NotConfigured,

        /// <summary>This setting is enabled.</summary>
        [EnumMember(Value = ""enable"")]
        Enable,

        /// <summary>This setting is disabled.</summary>
        [EnumMember(Value = ""disable"")]
        Disable
    }
}");
        }

        private static void CreateClass(WebClient webClient, string[] startKey)
        {
            // Get the tree.
            ConfigurationSettingsGetTreeResponse configurationSettingsGetTreeResponse = webClient.ConfigurationSettingsGetTree(startKey);

            // We remember further sections or tags for later rather than process immediately because we want to finish writing the current class before moving onto them.
            List<string[]> furtherSectionsOrTags = new List<string[]>();

            // There may not be any further definitions.
            if (configurationSettingsGetTreeResponse.Get.Definitions != null)
            {
                StringBuilder stringBuilder = new StringBuilder();
                stringBuilder.AppendLine("using Newtonsoft.Json;");

                foreach (KeyValuePair<string, ConfigurationSettingsGetTreeResponse.GetConfig.Definition> definition in configurationSettingsGetTreeResponse.Get.Definitions)
                {
                    if (definition.Value.Tag)
                    {
                        stringBuilder.AppendLine("using System.Collections.Generic;");
                        break;
                    }
                }

                stringBuilder.AppendLine();
                stringBuilder.AppendLine("namespace EdgeOS.API.Types.Configuration");
                stringBuilder.AppendLine("{");
                stringBuilder.AppendLine("    /// <summary>A class representing an EdgeOS " + GetNamingConventionString(startKey != null ? string.Join(" ", startKey) + " " : "") + "configuration tree.</summary>");
                stringBuilder.AppendLine("    public class " + (startKey != null ? GetNamingConventionString(string.Join("-", startKey)) : "Configuration"));
                stringBuilder.AppendLine("    {");

                // To work out when to add new lines in the class once a previous item has been added.
                bool alreadyAddedItem = false;

                // Take each of the definitions.
                foreach (KeyValuePair<string, ConfigurationSettingsGetTreeResponse.GetConfig.Definition> definition in configurationSettingsGetTreeResponse.Get.Definitions)
                {
                    // Write the separator to the class if this is not the first iteration.
                    if (alreadyAddedItem) { stringBuilder.AppendLine(); }

                    // Write the metadata to the class.
                    stringBuilder.AppendLine("        /// <summary>" + definition.Value.Help.TrimEnd() + "</summary>");
                    stringBuilder.AppendLine("        [JsonProperty(PropertyName = \"" + definition.Key + "\")]");

                    string namingConventionKey = GetNamingConventionString(definition.Key);

                    // Is this a list of further values (will not have a type declared)?
                    if (definition.Value.Type == ConfigurationSettingsGetTreeResponse.GetConfig.Definition.ValueType.Unknown)
                    {
                        // Get the full path to this key.
                        string[] newArray = AppendArray(startKey, definition.Key);

                        // Write out the section path.
                        //Console.WriteLine("Section: " + string.Join(" -> ", newArray));

                        // Write this section to the class.
                        stringBuilder.AppendLine("        public " + namingConventionKey + " " + namingConventionKey + ";");

                        // We will recursively call this method again (but after we have finished processing all Definitions for this class).
                        furtherSectionsOrTags.Add(newArray);
                    }
                    // User defined tags are present.
                    else if (definition.Value.Tag)
                    {
                        // Get the full path to this key.
                        string[] newArray = AppendArray(startKey, definition.Key);

                        // Write out the tag path.
                        //Console.WriteLine("Tag: " + string.Join(" -> ", newArray));

                        // Write this tag to the class.
                        stringBuilder.AppendLine("        public Dictionary<string, " + namingConventionKey + "Entry> " + namingConventionKey + ";");

                        // Query an undefined tag to get the definitions.
                        newArray = AppendArray(newArray, "Entry");

                        // We will recursively call this method again (but after we have finished processing all Definitions for this class).
                        furtherSectionsOrTags.Add(newArray);
                    }
                    // This is a setting 
                    else
                    {
                        // Write out the value.
                        //Console.WriteLine("Value: " + definition.Key);

                        // Write this value to the class.
                        stringBuilder.AppendLine("        public " + GetTypeString(definition.Value) + " " + namingConventionKey + ";");
                    }

                    // Mark that we have now added an item.
                    alreadyAddedItem = true;
                }

                // Write the class footer.
                stringBuilder.AppendLine("    }");
                stringBuilder.Append("}");

                // Create a path and filename for this class.
                string fileName = @"Configuration\" + (startKey != null ? GetFilepathString(startKey) : "Configuration") + ".cs";

                // Get just the directory
                string directory = Path.GetDirectoryName(fileName);
                if (!Directory.Exists(directory)) { Directory.CreateDirectory(directory); }

                // Write this class to a file.
                File.WriteAllText(fileName, stringBuilder.ToString());

                // Update the user on the progress.
                Console.WriteLine(fileName);

                // More children (tags and sections) to resolve.
                foreach (string[] furtherSectionOrTag in furtherSectionsOrTags)
                {
                    CreateClass(webClient, furtherSectionOrTag);
                }
            }
        }

        private static string GetFilepathString(string[] startKey)
        {
            // We start with the first key.
            string filePath = startKey[0];

            // If there are multiple keys.
            if (startKey.Length > 1)
            {
                // Append all of the remaining but final key.
                for (int count = 1; count < startKey.Length - 1; count++)
                {
                    filePath += (!startKey[count].Equals("Entry") ? "\\" : "-") + startKey[count];
                }

                // Is the final key an "Entry"?
                if (startKey[startKey.Length - 1].Equals("Entry"))
                {
                    // Add on the last one as part of the filename.
                    filePath += '-' + startKey[startKey.Length - 1];
                }
                else
                {
                    // Add on the last one as a new directory.
                    filePath += '\\' + startKey[startKey.Length - 2] + '-' + startKey[startKey.Length - 1];
                }
            }

            filePath = GetNamingConventionString(filePath);
            return filePath;
        }

        private static string GetTypeString(ConfigurationSettingsGetTreeResponse.GetConfig.Definition value)
        {
            string type;

            // Multiple types have to be string.
            if (value.Type != ConfigurationSettingsGetTreeResponse.GetConfig.Definition.ValueType.Unknown && value.Type2 != ConfigurationSettingsGetTreeResponse.GetConfig.Definition.ValueType.Unknown)
            {
                type = "string";
            }
            else if (value.Default != null && (value.Default.Equals("enable") || value.Default.Equals("disable")))
            {
                type = "ConfigurationBool";
            }
            else
            {
                switch (value.Type)
                {
                    case ConfigurationSettingsGetTreeResponse.GetConfig.Definition.ValueType.@bool:
                        type = "ConfigurationBool";
                        break;

                    case ConfigurationSettingsGetTreeResponse.GetConfig.Definition.ValueType.ipv4:
                    case ConfigurationSettingsGetTreeResponse.GetConfig.Definition.ValueType.ipv4net:
                    case ConfigurationSettingsGetTreeResponse.GetConfig.Definition.ValueType.ipv6:
                    case ConfigurationSettingsGetTreeResponse.GetConfig.Definition.ValueType.ipv6net:
                    case ConfigurationSettingsGetTreeResponse.GetConfig.Definition.ValueType.macaddr:
                    case ConfigurationSettingsGetTreeResponse.GetConfig.Definition.ValueType.txt:
                        type = "string";
                        break;

                    case ConfigurationSettingsGetTreeResponse.GetConfig.Definition.ValueType.u32:
                        type = "uint";
                        break;

                    // Bad values.
                    case ConfigurationSettingsGetTreeResponse.GetConfig.Definition.ValueType.Unknown:
                    default:
                        throw new InvalidOperationException("Cannot determine type for Unknown");
                }
            }

            return type + (value.Multiple ? "[]" : null);
        }

        private static string GetNamingConventionString(string key)
        {
            // Convert to Title-Case.
            string convertedString = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(key);

            // Replacing this afterwards means that TitleCase can be properly determined.
            convertedString = convertedString.Replace("-", "");

            // Accepted acronyms.
            convertedString = convertedString.Replace("Arp", "ARP");
            convertedString = convertedString.Replace("Bgp", "BGP");
            convertedString = convertedString.Replace("Bfd", "BFD");
            convertedString = convertedString.Replace("Dhcpv6Pd", "DHCPv6PD");
            convertedString = convertedString.Replace("Dhcp", "DHCP");
            convertedString = convertedString.Replace("Dns", "DNS");
            convertedString = convertedString.Replace("Dscp", "DSCP");
            convertedString = convertedString.Replace("Ftn", "FTN");
            convertedString = convertedString.Replace("Igmp", "IGMP");
            convertedString = convertedString.Replace("Ike", "IKE");
            convertedString = convertedString.Replace("Ipv4", "IPv4");
            convertedString = convertedString.Replace("Ipv6", "IPv6");
            convertedString = convertedString.Replace("Ip", "IP");
            convertedString = convertedString.Replace("Ldp", "LDP");
            convertedString = convertedString.Replace("L2tp", "L2TP");
            convertedString = convertedString.Replace("L2vpn", "L2VPN");
            convertedString = convertedString.Replace("Lsp", "LSP");
            convertedString = convertedString.Replace("Mpls", "MPLS");
            convertedString = convertedString.Replace("Mspw", "MSPW");
            convertedString = convertedString.Replace("Mtu", "MTU");
            convertedString = convertedString.Replace("Orf", "ORF");
            convertedString = convertedString.Replace("Ospf", "OSPF");
            convertedString = convertedString.Replace("Openvpn", "OpenVPN");
            convertedString = convertedString.Replace("Pppoe", "PPPoE");
            convertedString = convertedString.Replace("Pptp", "PPTP");
            convertedString = convertedString.Replace("Radius", "RADIUS");
            convertedString = convertedString.Replace("Rip", "RIP");
            convertedString = convertedString.Replace("Rsa", "RSA");
            convertedString = convertedString.Replace("Rsvp", "RSVP");
            convertedString = convertedString.Replace("Sip", "SIP");
            convertedString = convertedString.Replace("Snmp", "SNMP");
            convertedString = convertedString.Replace("Syn", "SYN");
            convertedString = convertedString.Replace("Tcp", "TCP");
            convertedString = convertedString.Replace("Tls", "TLS");
            convertedString = convertedString.Replace("Ttl", "TTL");
            convertedString = convertedString.Replace("Ttfp", "TTFP");
            convertedString = convertedString.Replace("Ubnt", "UBNT");
            convertedString = convertedString.Replace("Udp", "UDP");
            convertedString = convertedString.Replace("Unms", "UNMS");
            convertedString = convertedString.Replace("Upnp", "UPnP");
            convertedString = convertedString.Replace("Url", "URL");
            convertedString = convertedString.Replace("Vc", "VC");
            convertedString = convertedString.Replace("Vpls", "VPLS");
            convertedString = convertedString.Replace("Vpn", "VPN");
            convertedString = convertedString.Replace("Vif", "VIF");
            convertedString = convertedString.Replace("Vrrp", "VRRP");

            // Return the result.
            return convertedString;
        }

        private static string[] AppendArray(string[] startKey, string key)
        {
            string[] newArray;

            // No array yet means just use this as the first item.
            if (startKey == null || startKey.Length == 0)
            {
                newArray = new[] { key };
            }
            // Append this to the array.
            else
            {
                int newArrayLength = startKey.Length + 1;
                newArray = new string[newArrayLength];
                startKey.CopyTo(newArray, 0);
                newArray[newArrayLength - 1] = key;
            }

            return newArray;
        }
    }
}