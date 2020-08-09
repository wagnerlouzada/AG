using System;
using System.Collections.Concurrent;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using AgProtocol;
using Xunit;

namespace tests
{
    class CommonCache
    {
        public static CommonCache GetInstance()
        {
            if (_instance == null)
                _instance = new CommonCache();
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
        private static CommonCache _instance;
    }

    class HttpCacheSession : HttpSession
    {
        public HttpCacheSession(HttpServer server) : base(server) { }

        protected override void OnReceivedRequest(HttpRequest request)
        {
            string cache;
            // Process HTTP request methods
            switch (request.Method.ToUpper())
            {
                case "HEAD":
                    SendResponseAsync(Response.MakeHeadResponse());
                    break;
                case "GET":
                    if (CommonCache.GetInstance().GetCache(request.Url, out cache))
                    {
                        // Response with the cache value
                        SendResponseAsync(Response.MakeGetResponse(cache));
                    }
                    else
                    {
                        SendResponseAsync(Response.MakeErrorResponse("Required cache value was not found for the key: " + request.Url));
                    }
                    break;
                case "POST":
                case "PUT":
                    // Set the cache value
                    CommonCache.GetInstance().SetCache(request.Url, request.Body);
                    // Response with the cache value
                    SendResponseAsync(Response.MakeOkResponse());
                    break;
                case "DELETE":
                    // Delete the cache value 
                    if (CommonCache.GetInstance().DeleteCache(request.Url, out cache))
                    {
                        // Response with the cache value
                        SendResponseAsync(Response.MakeGetResponse(cache));
                    }
                    else
                    {
                        SendResponseAsync(Response.MakeErrorResponse("Deleted cache value was not found for the key: " + request.Url));
                    }
                    break;
                case "OPTIONS":
                    SendResponseAsync(Response.MakeOptionsResponse());
                    break;
                case "TRACE":
                    SendResponseAsync(Response.MakeTraceResponse(request.Cache));
                    break;
                default:
                    SendResponseAsync(Response.MakeErrorResponse("Unsupported HTTP method: " + request.Method));
                    break;
            }
             
        }

        protected override void OnReceivedRequestError(HttpRequest request, string error)
        {
            Console.WriteLine($"Request error: {error}");
        }

        protected override void OnError(SocketError error)
        {
            Console.WriteLine($"HTTP session caught an error: {error}");
        }
    }

    class HttpCacheServer : HttpServer
    {
        public HttpCacheServer(IPAddress address, int port) : base(address, port) { }

        protected override TcpSession CreateSession() { return new HttpCacheSession(this); }

        protected override void OnError(SocketError error)
        {
            Console.WriteLine($"HTTP session caught an error: {error}");
        }
    }

    public class HttpTests
    {
        [Fact(DisplayName = "HTTP server test")]
        public void HttpServerTest()
        {
            string address = AgProtocol.AgProtocol.Address;
            int port = AgProtocol.AgProtocol.Port;

            // Create and start HTTP server
            var server = new HttpCacheServer(IPAddress.Any, port);
            Assert.True(server.Start());
            while (!server.IsStarted)
                Thread.Yield();

            // Create a new HTTP client
            var client = new HttpClientEx(address, port);

            // Test CRUD operations
            var response = client.SendGetRequest("/test").Result;
            Assert.True(response.Status == 500);
            response = client.SendPostRequest("/test", "old_value").Result;
            Assert.True(response.Status == 200);
            response =  client.SendGetRequest("/test").Result;
            Assert.True(response.Status == 200);
            Assert.True(response.Body == "old_value");
            response = client.SendPutRequest("/test", "new_value").Result;
            Assert.True(response.Status == 200);
            response = client.SendGetRequest("/test").Result;
            Assert.True(response.Status == 200);
            Assert.True(response.Body == "new_value");
            response = client.SendDeleteRequest("/test").Result;
            Assert.True(response.Status == 200);
            response = client.SendGetRequest("/test").Result;
            Assert.True(response.Status == 500);

            // Stop the HTTP server
            Assert.True(server.Stop());
            while (server.IsStarted)
                Thread.Yield();
        }
    }
}
