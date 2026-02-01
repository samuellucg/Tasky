using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SocketIOClient;
using SocketIOClient.Newtonsoft.Json;
namespace Tasky.Services.Socket
{
    public class SocketClient 
    {
        private static SocketIOClient.SocketIO _client;
        public SocketClient(string route)
        {
            _client = new SocketIOClient.SocketIO(route, new SocketIOOptions
            {
                EIO = EngineIO.V4,
                Reconnection = true,
                ReconnectionAttempts = int.MaxValue,
                Transport = SocketIOClient.Transport.TransportProtocol.WebSocket,
            });

            _client.JsonSerializer = new NewtonsoftJsonSerializer();

            PopulateRegularEvents();
            PopulateEvents();
        }

        public async Task<bool> InitializeSocket()
        {
            await _client.ConnectAsync();
            if (_client.Connected)
            {
                PopulateRegularEvents();
                PopulateEvents();
                return true;
            }
            return false;
        }



        private async Task UnitializeSocket()
        {
            await _client.DisconnectAsync();
        }

        public async Task EmitEvent(string eventName, params object[] payload)
        {
            await _client.EmitAsync(eventName, payload);
        }

        // For regular events in SocketIoClient library
        protected void PopulateRegularEvents()
        {
            try
            {
                _client.OnConnected += async (sender, e) =>
                {
                    Console.WriteLine($"Connected to socket! \nID: {_client.Id}");
                    Console.WriteLine($"Http Client: {_client.HttpClient}");
                };

                _client.OnDisconnected += async (sender, e) =>
                {
                    Console.WriteLine("Disconnected to socket!");
                };

                _client.OnReconnected += async (sender, e) =>
                {
                    Console.WriteLine($"Reconnected to socket! \nID:{_client.Id}");
                };

                _client.OnReconnectAttempt += async (sender, e) =>
                {
                    Console.WriteLine($"Trying to reconnect to socket.");
                };

                _client.OnReconnectError += async (sender, e) =>
                {
                    Console.WriteLine($"Error reconnecting to socket");
                };

                _client.OnReconnectFailed += async (sender, e) =>
                {
                    Console.WriteLine($"Failed reconnecting to socket");
                };
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception on {nameof(PopulateRegularEvents)}: {ex}");
            }
        }

        protected void PopulateEvents()
        {
            // Write your events here...
            _client.On("Message", (response) =>
            {
                Console.WriteLine("Event: Message");
                Console.WriteLine($"Received: {response}");
            });

            _client.On("test", (response) =>
            {
                Console.WriteLine("Event: test");
                Console.WriteLine($"Received: {response}");
            });

            // Any event that aren't specified
            _client.OnAny((name, response) =>
            {
                Console.WriteLine($"Event: {name}");
                Console.WriteLine($"Received: {response}");
            });            
        }
    }
}
