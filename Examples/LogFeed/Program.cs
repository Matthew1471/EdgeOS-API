using EdgeOS.API;
using EdgeOS.API.Types.SubscriptionRequests;
using EdgeOS.API.Types.SubscriptionResponses;
using System;
using System.Configuration;
using System.Threading;

namespace LogFeed
{
    class Program
    {
        // Signals when we want to quit.
        private static readonly ManualResetEvent WantToQuit = new ManualResetEvent(false);

        // This holds the StatsConnection for the whole form.
        private static readonly StatsConnection statsConnection = new StatsConnection();

        static void Main(string[] args)
        {
            // Set the window title to something a bit more interesting.
            if (!Console.IsOutputRedirected) { Console.Title = "LogFeed V0.1"; }

            // The WebClient allows us to get a valid SessionID to then use with the StatsConnection.
            using (WebClient webClient = new WebClient(ConfigurationManager.AppSettings["Username"], ConfigurationManager.AppSettings["Password"], "https://" + ConfigurationManager.AppSettings["Host"] + "/"))
            {
                // Login to the router.
                webClient.Login();

                // Share a valid SessionID with the StatsConnection object.
                statsConnection.SessionID = webClient.SessionID;

                // Ignore TLS certificate errors if there is a ".crt" file present that matches this host.
                statsConnection.AllowLocalCertificates();

                // Connect to the router.
                statsConnection.ConnectAsync(new Uri("wss://" + ConfigurationManager.AppSettings["Host"] + "/ws/stats"));

                // Setup an event handler for when data is received.
                statsConnection.DataReceived += Connection_DataReceived;

                // Setup an event handler for when the connection state changes.
                statsConnection.ConnectionStatusChanged += Connection_ConnectionStatusChanged;
            }

            // We want the user (and the program itself) to be able to choose to exit.
            Console.CancelKeyPress += delegate (object sender, ConsoleCancelEventArgs eventArgs)
            {
                // Mark as handled by us, as we will want to clean up.
                eventArgs.Cancel = true;

                // Signal to the program that the user wishes to quit.
                WantToQuit.Set();
            };

            // Wait for something (user requested to quit, program finished..) to signal we should resume.
            WantToQuit.WaitOne();
        }


        /// <summary>Method which when a StatsConnection is established requests Interface statistics.</summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="connectionStatus">The <see cref="StatsConnection"/>'s new <see cref="StatsConnection.ConnectionStatus"/>.</param>
        private static void Connection_ConnectionStatusChanged(object sender, StatsConnection.ConnectionStatus connectionStatus)
        {
            // The sender should be a StatsConnection so that we can interact with that StatsConnection instance.
            StatsConnection statsConnection = sender as StatsConnection;
            if (statsConnection == null) { return; }

            // Specifically what did the ConnectionStatus change to?
            switch (connectionStatus)
            {
                // It was previously not connected and now it is.
                case StatsConnection.ConnectionStatus.Connected:

                    // Compose a subscription request message.
                    SubscriptionRequest subscriptionRequest = new SubscriptionRequest
                    {
                        Subscribe = new Subscription[] { new Subscription() { name = SubscriptionMessageType.LogFeed } },
                        SessionID = statsConnection.SessionID
                    };

                    // Ask for events to be delivered.
                    statsConnection.SubscribeForEvents(subscriptionRequest);
                    break;

                case StatsConnection.ConnectionStatus.DisconnectedByHost:
                    
                    // Just close this program.
                    WantToQuit.Set();

                    break;
            }
        }

        /// <summary>Method which is invoked when new <see cref="SubscriptionDataEvent"/> arrives.</summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="SubscriptionDataEvent"/> instance containing the event data.</param>
        private static void Connection_DataReceived(object sender, SubscriptionDataEvent e)
        {
            // Ignore any data that isn't a Console style response message.
            if (e.rootObject.GetType() != typeof(ConsoleResponse)) { return; }

            ConsoleResponse consoleRoot = (ConsoleResponse)e.rootObject;
            Console.WriteLine(consoleRoot.Message);
        }
    }
}