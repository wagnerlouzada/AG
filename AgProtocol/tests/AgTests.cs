using System;
using System.Collections.Concurrent;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using AgProtocol;
using Xunit;

namespace tests
{
    class AgCommonCache
    {
        public static AgCommonCache GetInstance()
        {
            if (_instance == null)
                _instance = new AgCommonCache();
            return _instance;
        }

        public bool GetCache(string key, out string value)
        {
            return _cache.TryGetValue(key, out value);
        }

        public void SetCache(string key, string value)
        {
            _cache[key] = value;
        }

        public bool DeleteCache(string key, out string value)
        {
            return _cache.TryRemove(key, out value);
        }

        private readonly ConcurrentDictionary<string, string> _cache = new ConcurrentDictionary<string, string>();
        private static AgCommonCache _instance;
    }

    class AgCacheSession : AgSession
    {
        public AgCacheSession(AgServer server) : base(server) { }

        protected override void OnReceivedRequest(AgRequest request)
        {
            // Process Ag request methods
            if (request.Method == "HEAD")
                SendResponseAsync(Response.MakeHeadResponse());
            else if (request.Method == "GET")
            {
                // Get the cache value
                string cache;
                if (AgCommonCache.GetInstance().GetCache(request.key, out cache))
                {
                    // Response with the cache value
                    SendResponseAsync(Response.MakeGetResponse(cache));
                }
                else
                    SendResponseAsync(Response.MakeErrorResponse("Required cache value was not found for the key: " + request.key));
            }
            else if ((request.Method == "POST") || (request.Method == "PUT"))
            {
                // Set the cache value
                AgCommonCache.GetInstance().SetCache(request.key, request.Data);
                // Response with the cache value
                SendResponseAsync(Response.MakeOkResponse());
            }
            else if (request.Method == "DELETE")
            {
                // Delete the cache value
                string cache;
                if (AgCommonCache.GetInstance().DeleteCache(request.key, out cache))
                {
                    // Response with the cache value
                    SendResponseAsync(Response.MakeGetResponse(cache));
                }
                else
                    SendResponseAsync(Response.MakeErrorResponse("Deleted cache value was not found for the key: " + request.key));
            }
            else if (request.Method == "OPTIONS")
                SendResponseAsync(Response.MakeOptionsResponse());
            else if (request.Method == "TRACE")
                SendResponseAsync(Response.MakeTraceResponse(request.Cache));
            else
                SendResponseAsync(Response.MakeErrorResponse("Unsupported Ag method: " + request.Method));
        }

        protected override void OnReceivedRequestError(AgRequest request, string error)
        {
            Console.WriteLine($"Request error: {error}");
        }

        protected override void OnError(SocketError error)
        {
            Console.WriteLine($"Ag session caught an error: {error}");
        }
    }

    class AgCacheServer : AgServer
    {
        public AgCacheServer(IPAddress address, int port) : base(address, port) { }

        protected override TcpSession CreateSession() { return new AgCacheSession(this); }

        protected override void OnError(SocketError error)
        {
            Console.WriteLine($"Ag session caught an error: {error}");
        }
    }

    public class AgTests
    {
        [Fact(DisplayName = "Ag server test")]
        public void AgServerTest()
        {
            string address = AgProtocol.AgProtocol.Address;
            int port = AgProtocol.AgProtocol.Port;

            // Create and start Ag server
            var server = new AgCacheServer(IPAddress.Any, port);
            Assert.True(server.Start());
            while (!server.IsStarted)
                Thread.Yield();

            // Create a new Ag client
            var client = new AgClientEx(address, port);

            // Test CRUD operations
            var response = client.SendGetRequest("/test").Result;
            Assert.True(response.Status == 500);
            response = client.SendPostRequest("/test", "old_value").Result;
            Assert.True(response.Status == 200);
            response =  client.SendGetRequest("/test").Result;
            Assert.True(response.Status == 200);
            Assert.True(response.Data == "old_value");
            response = client.SendPutRequest("/test", "new_value").Result;
            Assert.True(response.Status == 200);
            response = client.SendGetRequest("/test").Result;
            Assert.True(response.Status == 200);
            Assert.True(response.Data == "new_value");
            response = client.SendDeleteRequest("/test").Result;
            Assert.True(response.Status == 200);
            response = client.SendGetRequest("/test").Result;
            Assert.True(response.Status == 500);

            // Stop the Ag server
            Assert.True(server.Stop());
            while (server.IsStarted)
                Thread.Yield();
        }
    }

}
