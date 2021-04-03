using EdgeOS.API;
using EdgeOS.API.Types.Subscription.Requests;
using EdgeOS.API.Types.Subscription.Responses;
using System;
using System.Configuration;
using System.Threading;

namespace LogFeed
{
    class Program
    {
        // Signals when we want to quit.
        private static readonly ManualResetEvent WantToQuit = new ManualResetEvent(false);

        // EdgeOS requires logins and session heartbeats to be sent via the REST API.
        private static WebClient webClient;

        // EdgeOS requires the session to be renewed or it will expire (we renew every 30s)
        private static readonly System.Timers.Timer sessionHeartbeatTimer = new System.Timers.Timer(30000);

        // This holds the StatsConnection for the whole class.
        private static StatsConnection statsConnection;

        static void Main()
        {
            // Set the window title to something a bit more interesting.
            if (!Console.IsOutputRedirected) { Console.Title = "LogFeed V0.1"; }

            // Check the credentials are provided in the application's configuration file.
            if (ConfigurationManager.AppSettings["Username"] == null || ConfigurationManager.AppSettings["Password"] == null || ConfigurationManager.AppSettings["Host"] == null)
            {
                Console.WriteLine("Program cannot start, some credentials were missing in the program's configuration file.");

                // Exit the application.
                Environment.Exit(1610);
            }

            // This method will be invoked each time the timer has elapsed.
            sessionHeartbeatTimer.Elapsed += (s, a) => webClient.Heartbeat();

            // The WebClient allows us to get a valid SessionID to then use with the StatsConnection.
            webClient = new WebClient("https://" + ConfigurationManager.AppSettings["Host"] + "/");

            // Ignore TLS certificate errors if there is a ".crt" file present that matches this host.
            webClient.AllowLocalCertificates();

            // Login to the router.
            webClient.Login(ConfigurationManager.AppSettings["Username"], ConfigurationManager.AppSettings["Password"]);

            // Share a valid SessionID with a new StatsConnection object.
            statsConnection = new StatsConnection(webClient.SessionID);

            // Ignore TLS certificate errors if there is a ".crt" file present that matches this host.
            statsConnection.AllowLocalCertificates();

            // Connect to the router.
            statsConnection.ConnectAsync(new Uri("wss://" + ConfigurationManager.AppSettings["Host"] + "/ws/stats"));

            // Setup an event handler for when data is received.
            statsConnection.DataReceived += Connection_DataReceived;

            // Setup an event handler for when the connection state changes.
            statsConnection.ConnectionStatusChanged += Connection_ConnectionStatusChanged;


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

                    // Start the heartbeat timer.
                    sessionHeartbeatTimer.Enabled = true;

                    break;

                // The router has disconnected (usually due to session expiry due to lack of heartbeats).
                case StatsConnection.ConnectionStatus.DisconnectedByHost:

                    // Stop the heartbeat timer.
                    sessionHeartbeatTimer.Enabled = false;

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


        /// <summary>Clean up any resources being used.</summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected void Dispose(bool disposing)
        {
            if (disposing)
            {
                // Dispose of the statsConnection.
                if (statsConnection != null) { statsConnection.Dispose(); }

                // Dispose the sessionHeartbeatTimer and webClient.
                if (sessionHeartbeatTimer != null) { sessionHeartbeatTimer.Dispose(); }
                if (webClient != null) { webClient.Dispose(); }
            }
        }
    }
}