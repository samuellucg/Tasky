using System;
using System.Threading.Tasks;
using NLog;
using SocketIOClient;
using SocketIOClient.Newtonsoft.Json;

namespace Tasky.Services.Socket
{
    public static class SocketClient
    {
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();
        private static SocketIOClient.SocketIO _client;

        public static event Action<string> OnActionReceived;

        static SocketClient()
        {
            _client = new SocketIOClient.SocketIO("http://localhost:3000", new SocketIOClient.SocketIOOptions
            {
                EIO = EngineIO.V4,
                Reconnection = true,
                ReconnectionAttempts = int.MaxValue,
                Transport = SocketIOClient.Transport.TransportProtocol.WebSocket,
            });

            _client.JsonSerializer = new NewtonsoftJsonSerializer();
        }

        public static async Task InitializeSocket()
        {
            await _client.ConnectAsync();
            if (_client.Connected)
            {
                PopulateRegularEvents();
                PopulateEvents();
            }
        }

        public static async Task DisconnectAsync()
        {
            await _client.DisconnectAsync();
        }

        public static async Task EmitEvent(string eventName, params object[] payload)
        {
            await _client.EmitAsync(eventName, payload);
        }

        public static void RegisterNewEvent(string eventName)
        {
            _client.On(eventName, response =>
            {
                var action = response.ToString();
                OnActionReceived?.Invoke(action);
            });
        }

        private static void PopulateRegularEvents()
        {
            _client.OnConnected += (sender, e) =>
            {
                logger.Info($"Connected to socket! ID: {_client.Id}");
            };

            _client.OnDisconnected += (sender, e) =>
            {
                logger.Info("Disconnected from socket");
            };

            _client.OnReconnected += (sender, e) =>
            {
                logger.Info($"Reconnected to socket! ID: {_client.Id}");
            };

            _client.OnReconnectAttempt += (sender, e) =>
            {
                logger.Debug("Attempting to reconnect to socket...");
            };

            _client.OnReconnectError += (sender, e) =>
            {
                logger.Error("Error reconnecting to socket");
            };

            _client.OnReconnectFailed += (sender, e) =>
            {
                logger.Error("Failed to reconnect to socket");
            };
        }

        private static void PopulateEvents()
        {
            // Add custom events here
        }
    }
}
