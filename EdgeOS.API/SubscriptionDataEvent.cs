using EdgeOS.API.Types;
using EdgeOS.API.Types.SubscriptionResponses;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;

namespace EdgeOS.API
{
    /// <summary>A class containing received EdgeOS data from a subscription.</summary>
    public class SubscriptionDataEvent : EventArgs
    {
        /// <summary>A C# object representing all of the JSON data EdgeOS has returned.</summary>
        public RootObject rootObject;

        /// <summary>Constructor, deserializes a JSON message into the RootObject.</summary>
        /// <param name="message">The JSON message from EdgeOS.</param>
        /// <param name="responseTypeMappings">The mappings for the JSON result types.</param>
        public SubscriptionDataEvent(string message, Dictionary<string, Type> responseTypeMappings = null)
        {
            using (JsonTextReader reader = new JsonTextReader(new StringReader(message)))
            {
                // JSON streams always follow a specific path, so attempt to keep reading until we have our first property name.
                if (reader.Read() && reader.TokenType == JsonToken.StartObject && reader.Read() && reader.TokenType == JsonToken.PropertyName)
                {
                    string propertyName = reader.Value.ToString();

                    if (responseTypeMappings.ContainsKey(propertyName))
                    {
                        // EdgeOS has a very inconsistent way of returning data that means it is sometimes difficult to work out the type of the returned data.
                        Type requestedType = responseTypeMappings[propertyName];

                        // Error if something cannot be deserialized.
                        JsonSerializerSettings settings = new JsonSerializerSettings { MissingMemberHandling = MissingMemberHandling.Error };

                        // Deserialize the JSON into C# objects.
                        rootObject = (RootObject)JsonConvert.DeserializeObject(message, requestedType, settings);
                    }
                    else
                    {
                        // The property name is dynamic for console types.
                        if (reader.Read() && reader.TokenType == JsonToken.String)
                        {
                            rootObject = new ConsoleRoot
                            {
                                sub_id = propertyName,
                                Message = reader.Value.ToString()
                            };

                            if (reader.Read() && reader.TokenType != JsonToken.EndObject && reader.Read() != false) { throw new NotImplementedException("The \"" + propertyName + "\" response is not implemeneted."); }
                        }

                    }
                }
                else
                {
                    // The data did not follow the expected format or ran out of data too soon.
                    throw new FormatException("Bad JSON data returned.");
                }
            }
        }
    }
}