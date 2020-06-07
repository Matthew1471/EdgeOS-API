using EdgeOS.API.Types;
using Newtonsoft.Json;
using System;

namespace EdgeOS.API
{
    /// <summary>A class containing received EdgeOS data from a subscription.</summary>
    public class SubscriptionDataEvent : EventArgs
    {
        /// <summary>A C# object representing all of the data EdgeOS has returned.</summary>
        public RootObject rootObject;
        
        /// <summary>Constructor, deserializes a JSON message into the RootObject.</summary>
        /// <param name="message">The JSON message from EdgeOS.</param>
        public SubscriptionDataEvent(string message)
        {
            // Error if something cannot be deserialized.
            JsonSerializerSettings settings = new JsonSerializerSettings { MissingMemberHandling = MissingMemberHandling.Error };

            // Deserialize the JSON into C# objects.
            rootObject = JsonConvert.DeserializeObject<RootObject>(message, settings);
        }
    }
}