using EdgeOS.API.Types.SubscriptionRequests;
using EdgeOS.API.Types.SubscriptionResponses;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.WebSockets;
using System.Text;
using System.Threading;

namespace EdgeOS.API
{
    /// <summary>Represents a WebSocket connection to EdgeOS and provides methods for requesting and receiving data.</summary>
    public class StatsConnection : IDisposable
    {
        /// <summary>The ClientWebSocket used to connect to EdgeOS.</summary>
        private readonly ClientWebSocket _clientWebSocket = new ClientWebSocket();

        /// <summary>The session ID used to authenticate with the socket.</summary>
        public string SessionID;

        /// <summary>A cancellation token used to propagate notification that the operation should be cancelled.</summary>
        private readonly CancellationTokenSource _cancellationTokenSource = new CancellationTokenSource();

        /// <summary>Size of the receive buffer.</summary>
        public const int BufferSize = 1024;

        /// <summary>Event that gets raised when a full message has been received from EdgeOS.</summary>
        public event EventHandler<SubscriptionDataEvent> DataReceived;

        /// <summary>Event that gets raised when the connection state of the underlying ClientWebSocket changes.</summary>
        public event EventHandler<ConnectionStatus> ConnectionStatusChanged;

        /// <summary>To detect redundant calls.</summary>
        private bool _disposed;

        private readonly Dictionary<string, Type> responseTypeMappings = new Dictionary<string, Type>() {
            { "config-change", typeof(ConfigurationChangeResponse) },
            { "discover", typeof(DiscoverResponse) },
            { "export", typeof(TrafficAnalysisResponse) },
            { "interfaces", typeof(InterfacesResponse) },
            { "num-routes", typeof(NumberOfRoutesResponse) },
            { "system-stats", typeof(SystemStatsResponse) },
            { "udapi-statistics", typeof(UDAPIStatisticsResponse) },
            { "users", typeof(UserResponse) }
        };

        /// <summary>The various states a StatsConnection connection may be in.</summary>
        public enum ConnectionStatus {
            /// <summary>The socket is currently connecting.</summary>
            Connecting,
            
            /// <summary>The socket is currently connected.</summary>
            Connected,
            
            /// <summary>The user has closed the connection.</summary>
            DisconnectedByUser,
            
            /// <summary>The remote server has closed the connection.</summary>
            DisconnectedByHost,
            
            /// <summary>The host did not respond to our connection in the specified time.</summary>
            ConnectFail_Timeout,
            
            /// <summary>The host did not send data in the specified time.</summary>
            ReceiveFail_Timeout,
            
            /// <summary>The message failed to send in time.</summary>
            SendFail_Timeout,

            /// <summary>An error has occurred.</summary>
            Error
        };
        
        /// <summary>Creates an instance of the StatsConnection for connecting to a single EdgeOS device.</summary>
        public StatsConnection()
        {
            // Implementation of timeout of 5000ms.
            //_cancellationTokenSource.CancelAfter(5000);
        }

        /// <summary>Allows a local .crt certificate file to be used to validate a host.</summary>
        public void AllowLocalCertificates()
        {
            // Ignore certificate trust errors if there is a saved public key pinned.
            ServicePointManager.ServerCertificateValidationCallback += ServerCertificateValidationCallback.PinPublicKey;
        }

        /// <summary>Tells the underlying ClientWebSocket to connect and raises various connection events.</summary>
        /// <param name="uri">The requested WebSocket Uri to connect to.</param>
        public async void ConnectAsync(Uri uri)
        {
            // Raise an event.
            ConnectionStatusChanged?.Invoke(this, ConnectionStatus.Connecting);

            // Perform the connection.
            await _clientWebSocket.ConnectAsync(uri, _cancellationTokenSource.Token);

            // Raise an event.
            ConnectionStatusChanged?.Invoke(this, ConnectionStatus.Connected);
        }

        /// <summary>EdgeOS requires a subscription on its WebSocket for anything to be sent to it, this method will request a subscription to start seeing requested events.</summary>
        /// <param name="subscriptionRequest">The event types to subscribe/unsubscribe.</param>
        public async void SubscribeForEvents(SubscriptionRequest subscriptionRequest)
        {
            // We need the SubscriptionMessage in JSON format.
            string subscriptionRequestJson = subscriptionRequest.ToJson();

            // The client web socket is expecting bytes and we also need to include the subscription message length.
            ArraySegment<byte> bytesToSend = new ArraySegment<byte>(Encoding.UTF8.GetBytes(subscriptionRequestJson.Length + "\r\n" + subscriptionRequestJson));

            // Send to the EdgeOS device our request.
            await _clientWebSocket.SendAsync(bytesToSend, WebSocketMessageType.Text, true, _cancellationTokenSource.Token);

            // Listen for messages being received due to this subscription.
            BeginReceive();
        }

        /// <summary>A receive message loop for processing obtained data from the WebSocket.</summary>
        private async void BeginReceive()
        {
            // The clientWebSocket.ReceiveAsync needs a buffer to store the frames coming in.
            byte[] buffer = new byte[BufferSize];

            try
            {
                // This will loop while the ClientWebSocket is still open.
                while (_clientWebSocket.State == WebSocketState.Open)
                {
                    // Frames arrive with partial messages, the FrameReassembler ensures that the messages are appropriately chunked.
                    FrameReassembler frameReassembler = new FrameReassembler();

                    WebSocketReceiveResult result;
                    do
                    {
                        // Ask the ClientWebSocket for any data.
                        result = await _clientWebSocket.ReceiveAsync(new ArraySegment<byte>(buffer), _cancellationTokenSource.Token);

                        switch (result.MessageType)
                        {
                            case WebSocketMessageType.Text:
                                // If we are in the middle of an incomplete message then we need to provide additional frame data.
                                frameReassembler.HandleReceivedData(Encoding.UTF8.GetString(buffer, 0, result.Count));

                                // Does the frameReassembler have any complete messages?
                                while (frameReassembler.HasCompleteMessages())
                                {
                                    // Raise an event containing the full message.
                                    DataReceived?.Invoke(this, new SubscriptionDataEvent(frameReassembler.GetNextCompleteMessage(), responseTypeMappings));
                                }

                                break;
                            case WebSocketMessageType.Close:
                                // We have been asked to close the socket gracefully.
                                await _clientWebSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, string.Empty, CancellationToken.None);

                                // Raise an event.
                                ConnectionStatusChanged?.Invoke(this, ConnectionStatus.DisconnectedByHost);
                                break;
                            default:
                                throw new NotImplementedException(result.MessageType.ToString() + " is not implemented.");
                        }

                    // Although EdgeOS does set the EndOfMessage flag, it lies.
                    } while (!result.EndOfMessage || frameReassembler.IsMissingData());
                }
            }
            finally
            {
                _clientWebSocket.Dispose();
            }
        }

        /// <summary>Stops the Websocket by setting its cancellation token that it regularly checks.</summary>
        private void EndReceive()
        {
            _cancellationTokenSource.Cancel();
        }

        /// <summary>Protected implementation of Dispose pattern.</summary>
        /// <param name="disposing">The disposing parameter should be false when called from a finalizer, and true when called from the IDisposable.Dispose method. In other words, it is true when deterministically called and false when non-deterministically called.</param>
        protected virtual void Dispose(bool disposing)
        {
            // Prevent duplicate disposal.
            if (!_disposed)
            {
                // The disposing parameter should be false when called from a finalizer, and true when called from the IDisposable.Dispose method. In other words, it is true when deterministically called and false when non-deterministically called.
                if (disposing)
                {
                    // Close the socket (if open).
                    if (_clientWebSocket.State == WebSocketState.Open) { _clientWebSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Program exiting", _cancellationTokenSource.Token).Wait(); }

                    // Stop receiving data by setting the thread's cancellation token.
                    EndReceive();

                    // Release the socket resources.
                    _clientWebSocket.Dispose();

                    // Release the cancellation token.
                    _cancellationTokenSource.Dispose();
                }

                // We are marked as disposed.
                _disposed = true;
            }
        }

        /// <summary>Public implementation of Dispose pattern callable by consumers.</summary>
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method.
            Dispose(disposing: true);

            // Suppress finalization.
            GC.SuppressFinalize(this);
        }
    }
}