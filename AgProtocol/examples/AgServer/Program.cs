using System;
using System.Collections.Concurrent;
using System.Net;
using System.Net.Sockets;
using AgProtocol;

namespace AgServer
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

    class AgCacheSession : AgSession
    {
        public AgCacheSession(AgProtocol.AgServer server) : base(server) {}

        protected override void OnReceivedRequest(AgRequest request)
        {
            string cache;
            // Show Ag request content
            Console.WriteLine(request);
            switch (request.Method.ToUpper())
            {
                case "HEAD":
                    SendResponseAsync(Response.MakeHeadResponse());
                    break;
                case "GET":
                    if (CommonCache.GetInstance().GetCache(request.key, out cache))
                    {
                        // Response with the cache value
                        SendResponseAsync(Response.MakeGetResponse(cache));
                    }
                    else
                    {
                        SendResponseAsync(Response.MakeErrorResponse("Required cache value was not found for the key: " + request.key));
                    }
                    break;
                case "LIST":
                    if (CommonCache.GetInstance().GetCache(request.key, out cache))
                    {
                        // Response with the cache value
                        SendResponseAsync(Response.MakeGetResponse(cache));
                    }
                    else
                    {
                        SendResponseAsync(Response.MakeErrorResponse("Required cache value was not found for the key: " + request.key));
                    }
                    break;
                case "UPDATE":
                    // Set the cache value
                    CommonCache.GetInstance().SetCache(request.key, request.Data);
                    // Response with the cache value
                    SendResponseAsync(Response.MakeOkResponse());
                    break;
                case "SAVE":
                    // Set the cache value
                    CommonCache.GetInstance().SetCache(request.key, request.Data);
                    // Response with the cache value
                    SendResponseAsync(Response.MakeOkResponse());
                    break;
                case "UPLOAD":
                    // Set the cache value
                    CommonCache.GetInstance().SetCache(request.key, request.Data);
                    // Response with the cache value
                    SendResponseAsync(Response.MakeOkResponse());
                    break;
                case "DOWNLOAD":
                    if (CommonCache.GetInstance().GetCache(request.key, out cache))
                    {
                        // Response with the cache value
                        SendResponseAsync(Response.MakeGetResponse(cache));
                    }
                    else
                    {
                        SendResponseAsync(Response.MakeErrorResponse("Required cache value was not found for the key: " + request.key));
                    }
                    break;
                case "DELETE":
                    if (CommonCache.GetInstance().DeleteCache(request.key, out cache))
                    {
                        // Response with the cache value
                        SendResponseAsync(Response.MakeGetResponse(cache));
                    }
                    else
                    {
                        SendResponseAsync(Response.MakeErrorResponse("Deleted cache value was not found for the key: " + request.key));
                    }
                    break;
                case "OPTIONS":
                    SendResponseAsync(Response.MakeOptionsResponse());
                    break;
                case "SHARE":
                    SendResponseAsync(Response.MakeOptionsResponse());
                    break;
                case "RIGHTS":
                    SendResponseAsync(Response.MakeOptionsResponse());
                    break;
                case "TRACE":
                    SendResponseAsync(Response.MakeTraceResponse(request.Cache.Data));
                    break;
                default:
                    SendResponseAsync(Response.MakeErrorResponse("Unsupported Ag method: " + request.Method));
                    break;
            }                        
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

    class AgCacheServer : AgProtocol.AgServer
    {
        public AgCacheServer(IPAddress address, int port) : base(address, port) {}

        protected override TcpSession CreateSession() { return new AgCacheSession(this); }

        protected override void OnError(SocketError error)
        {
            Console.WriteLine($"Ag session caught an error: {error}");
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            // Ag server port
            int port = AgProtocol.AgProtocol.Port;
            if (args.Length > 0)
                port = int.Parse(args[0]);
            // Ag server content path
            string www = "../../../../../www/api";
            if (args.Length > 1)
                www = args[1];

            Console.WriteLine($"Ag server port: {port}");
            Console.WriteLine($"Ag server static content path: {www}");
            Console.WriteLine($"Ag server website: Ag://localhost:{port}/api/index.html");

            Console.WriteLine();

            // Create a new Ag server
            var server = new AgCacheServer(IPAddress.Any, port);
            server.AddStaticContent(www, "/api");

            // Start the server
            Console.Write("Server starting...");
            server.Start();
            Console.WriteLine("Done!");

            Console.WriteLine("Press Enter to stop the server or '!' to restart the server...");

            // Perform text input
            for (;;)
            {
                string line = Console.ReadLine();
                if (string.IsNullOrEmpty(line))
                    break;

                // Restart the server
                if (line == "!")
                {
                    Console.Write("Server restarting...");
                    server.Restart();
                    Console.WriteLine("Done!");
                }
            }

            // Stop the server
            Console.Write("Server stopping...");
            server.Stop();
            Console.WriteLine("Done!");
        }
    }
}
