using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Markup;
using SocketIOClient;
using SocketIOClient.Newtonsoft.Json;
namespace Tasky.Services.Socket
{
    public static class SocketClient
    {
        #region Attributes
        private static SocketIOClient.SocketIO _client;
        #endregion

        #region Constructor
        static SocketClient()
        {
            _client = new SocketIOClient.SocketIO("http://localhost:3000", new SocketIOOptions
            {
                EIO = EngineIO.V4,
                Reconnection = true,
                ReconnectionAttempts = int.MaxValue,
                Transport = SocketIOClient.Transport.TransportProtocol.WebSocket,
            });

            _client.JsonSerializer = new NewtonsoftJsonSerializer();
        }
        #endregion

        #region Functions

        public static async Task InitializeSocket()
        {
            await _client.ConnectAsync();
            if (_client.Connected)
            {
                PopulateRegularEvents();
                PopulateEvents();
            }
        }

        private static async Task<bool> UnitializeSocket()
        {
            await _client.DisconnectAsync();
            if (!_client.Connected)
                return true;
            return false;

        }
        public static async Task EmitEvent(string eventName, params object[] payload)
        {
            await _client.EmitAsync(eventName, payload);
        }

        public static async Task<string> ListenNewEvent(SocketIOResponse response)
        {
            string action = await CheckActionFromServer(response);
            return action;
        }

        public static async Task RegisterNewEvent(string eventName)
        {
            _client.On(eventName, async (response) =>
            {
                await ListenNewEvent(response);
            });
        }

        public static async Task<string> CheckActionFromServer(SocketIOResponse response)
        {
            try
            {
                return response.ToString();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return string.Empty;
            }
        }

        // For regular events in SocketIoClient library
        private static void PopulateRegularEvents()
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

        private static void PopulateEvents()
        {
            // Write your events here...
            _client.On("Message", (response) =>
            {
                Console.WriteLine("Event: Message");
                Console.WriteLine($"Received: {response}");
            });

            _client.On("Task_Created", (response) =>
            {
                Console.WriteLine("Event: Task_Created");
                Console.WriteLine($"Received: {response}");
            });

            _client.On("Test", (response) =>
            {
                Console.WriteLine("Event: Test");
                Console.WriteLine($"Received: {response}");
            });

            _client.On("TestApi", (response) =>
            {
                Console.WriteLine("Event: TestApi");
                Console.WriteLine($"Received: {response}");
            });

            // Any event that aren't specified
            //_client.OnAny((name, response) =>
            //{
            //    Console.WriteLine($"Event: {name}");
            //    Console.WriteLine($"Received: {response}");
            //});            
        }
        #endregion
    }
}
