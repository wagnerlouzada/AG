using System;
using System.Net;
using System.IO;

namespace AgProtocol
{
    /// <summary>
    /// Ag server is used to create Ag Web server and communicate with clients using Ag protocol. It allows to receive GET, POST, PUT, DELETE requests and send Ag responses.
    /// </summary>
    /// <remarks>Thread-safe.</remarks>
    public class AgServer : TcpServer
    {
        /// <summary>
        /// Initialize Ag server with a given IP address and port number
        /// </summary>
        /// <param name="address">IP address</param>
        /// <param name="port">Port number</param>
        public AgServer(IPAddress address, int port) : base(address, port) { Cache = new FileCache(); }
        /// <summary>
        /// Initialize Ag server with a given IP address and port number
        /// </summary>
        /// <param name="address">IP address</param>
        /// <param name="port">Port number</param>
        public AgServer(string address, int port) : base(address, port) { Cache = new FileCache(); }
        /// <summary>
        /// Initialize Ag server with a given IP endpoint
        /// </summary>
        /// <param name="endpoint">IP endpoint</param>
        public AgServer(IPEndPoint endpoint) : base(endpoint) { Cache = new FileCache(); }

        /// <summary>
        /// Get the static content cache
        /// </summary>
        public FileCache Cache { get; }

        /// <summary>
        /// Add static content cache
        /// </summary>
        /// <param name="path">Static content path</param>
        /// <param name="prefix">Cache prefix (default is "/")</param>
        /// <param name="timeout">Refresh cache timeout (default is 1 hour)</param>
        public void AddStaticContent(string path, string prefix = "/", TimeSpan? timeout = null)
        {
            timeout ??= TimeSpan.FromHours(1);

            bool Handler(FileCache cache, string key, byte[] value, TimeSpan timespan)
            {
                AgResponse header = new AgResponse();
                header.SetBegin(200);
                header.SetContentType(Path.GetExtension(key));
                header.SetHeader("Cache-Control", $"max-age={timespan.Seconds}");
                header.SetData(value);
                return cache.Add(key, header.Cache.Data, timespan);
            }

            Cache.InsertPath(path, prefix, timeout.Value, Handler);
        }
        /// <summary>
        /// Remove static content cache
        /// </summary>
        /// <param name="path">Static content path</param>
        public void RemoveStaticContent(string path) { Cache.RemovePath(path); }
        /// <summary>
        /// Clear static content cache
        /// </summary>
        public void ClearStaticContent() { Cache.Clear(); }

        /// <summary>
        /// Watchdog the static content cache
        /// </summary>
        public void Watchdog(DateTime utc) { Cache.Watchdog(utc); }

        protected override TcpSession CreateSession() { return new AgSession(this); }
    }
}
