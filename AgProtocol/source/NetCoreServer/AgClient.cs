using System;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace AgProtocol
{
    /// <summary>
    /// Ag client is used to communicate with Ag Web server. It allows to send GET, POST, PUT, DELETE requests and receive Ag result.
    /// </summary>
    /// <remarks>Thread-safe.</remarks>
    public class AgClient : TcpClient
    {
        /// <summary>
        /// Initialize Ag client with a given IP address and port number
        /// </summary>
        /// <param name="address">IP address</param>
        /// <param name="port">Port number</param>
        public AgClient(IPAddress address, int port) : base(address, port) { Request = new AgRequest(); Response = new AgResponse(); }
        /// <summary>
        /// Initialize Ag client with a given IP address and port number
        /// </summary>
        /// <param name="address">IP address</param>
        /// <param name="port">Port number</param>
        public AgClient(string address, int port) : base(address, port) { Request = new AgRequest(); Response = new AgResponse(); }
        /// <summary>
        /// Initialize Ag client with a given IP endpoint
        /// </summary>
        /// <param name="endpoint">IP endpoint</param>
        public AgClient(IPEndPoint endpoint) : base(endpoint) { Request = new AgRequest(); Response = new AgResponse(); }

        /// <summary>
        /// Get the Ag request
        /// </summary>
        public AgRequest Request { get; protected set; }

        /// <summary>
        /// Get the Ag response
        /// </summary>
        protected AgResponse Response { get; set; }

        #region Send request / Send request Data

        /// <summary>
        /// Send the current Ag request (synchronous)
        /// </summary>
        /// <returns>Size of sent data</returns>
        public long SendRequest() { return SendRequest(Request); }
        /// <summary>
        /// Send the Ag request (synchronous)
        /// </summary>
        /// <param name="request">Ag request</param>
        /// <returns>Size of sent data</returns>
        public long SendRequest(AgRequest request) { return Send(request.Cache.Data, request.Cache.Offset, request.Cache.Size); }

        /// <summary>
        /// Send the Ag request Data (synchronous)
        /// </summary>
        /// <param name="Data">Ag request Data</param>
        /// <returns>Size of sent data</returns>
        public long SendRequestData(string Data) { return Send(Data); }
        /// <summary>
        /// Send the Ag request Data (synchronous)
        /// </summary>
        /// <param name="buffer">Ag request Data buffer</param>
        /// <returns>Size of sent data</returns>
        public long SendRequestData(byte[] buffer) { return Send(buffer); }
        /// <summary>
        /// Send the Ag request Data (synchronous)
        /// </summary>
        /// <param name="buffer">Ag request Data buffer</param>
        /// <param name="offset">Ag request Data buffer offset</param>
        /// <param name="size">Ag request Data size</param>
        /// <returns>Size of sent data</returns>
        public long SendRequestData(byte[] buffer, long offset, long size) { return Send(buffer, offset, size); }

        /// <summary>
        /// Send the current Ag request (asynchronous)
        /// </summary>
        /// <returns>'true' if the current Ag request was successfully sent, 'false' if the session is not connected</returns>
        public bool SendRequestAsync() { return SendRequestAsync(Request); }
        /// <summary>
        /// Send the Ag request (asynchronous)
        /// </summary>
        /// <param name="request">Ag request</param>
        /// <returns>'true' if the current Ag request was successfully sent, 'false' if the session is not connected</returns>
        public bool SendRequestAsync(AgRequest request) { return SendAsync(request.Cache.Data, request.Cache.Offset, request.Cache.Size); }

        /// <summary>
        /// Send the Ag request Data (asynchronous)
        /// </summary>
        /// <param name="Data">Ag request Data</param>
        /// <returns>'true' if the Ag request Data was successfully sent, 'false' if the session is not connected</returns>
        public bool SendRequestDataAsync(string Data) { return SendAsync(Data); }
        /// <summary>
        /// Send the Ag request Data (asynchronous)
        /// </summary>
        /// <param name="buffer">Ag request Data buffer</param>
        /// <returns>'true' if the Ag request Data was successfully sent, 'false' if the session is not connected</returns>
        public bool SendRequestDataAsync(byte[] buffer) { return SendAsync(buffer); }
        /// <summary>
        /// Send the Ag request Data (asynchronous)
        /// </summary>
        /// <param name="buffer">Ag request Data buffer</param>
        /// <param name="offset">Ag request Data buffer offset</param>
        /// <param name="size">Ag request Data size</param>
        /// <returns>'true' if the Ag request Data was successfully sent, 'false' if the session is not connected</returns>
        public bool SendRequestDataAsync(byte[] buffer, long offset, long size) { return SendAsync(buffer, offset, size); }

        #endregion

        #region Session handlers

        protected override void OnReceived(byte[] buffer, long offset, long size)
        {
            // Receive Ag response header
            if (Response.IsPendingHeader())
            {
                if (Response.ReceiveHeader(buffer, (int)offset, (int)size))
                    OnReceivedResponseHeader(Response);

                size = 0;
            }

            // Check for Ag response error
            if (Response.IsErrorSet)
            {
                OnReceivedResponseError(Response, "Invalid Ag response!");
                Response.Clear();
                Disconnect();
                return;
            }

            // Receive Ag response Data
            if (Response.ReceiveData(buffer, (int)offset, (int)size))
            {
                OnReceivedResponse(Response);
                Response.Clear();
                return;
            }

            // Check for Ag response error
            if (Response.IsErrorSet)
            {
                OnReceivedResponseError(Response, "Invalid Ag response!");
                Response.Clear();
                Disconnect();
                return;
            }
        }

        protected override void OnDisconnected()
        {
            // Receive Ag response Data
            if (Response.IsPendingData())
            {
                OnReceivedResponse(Response);
                Response.Clear();
                return;
            }
        }

        /// <summary>
        /// Handle Ag response header received notification
        /// </summary>
        /// <remarks>Notification is called when Ag response header was received from the server.</remarks>
        /// <param name="response">Ag request</param>
        protected virtual void OnReceivedResponseHeader(AgResponse response) { }

        /// <summary>
        /// Handle Ag response received notification
        /// </summary>
        /// <remarks>Notification is called when Ag response was received from the server.</remarks>
        /// <param name="response">Ag response</param>
        protected virtual void OnReceivedResponse(AgResponse response) { }

        /// <summary>
        /// Handle Ag response error notification
        /// </summary>
        /// <remarks>Notification is called when Ag response error was received from the server.</remarks>
        /// <param name="response">Ag response</param>
        /// <param name="error">Ag response error</param>
        protected virtual void OnReceivedResponseError(AgResponse response, string error) { }

        #endregion
    }

    /// <summary>
    /// Ag extended client make requests to Ag Web server with returning Task as a synchronization primitive.
    /// </summary>
    /// <remarks>Thread-safe.</remarks>
    public class AgClientEx : AgClient
    {
        /// <summary>
        /// Initialize Ag client with a given IP address and port number
        /// </summary>
        /// <param name="address">IP address</param>
        /// <param name="port">Port number</param>
        public AgClientEx(IPAddress address, int port) : base(address, port) {}
        /// <summary>
        /// Initialize Ag client with a given IP address and port number
        /// </summary>
        /// <param name="address">IP address</param>
        /// <param name="port">Port number</param>
        public AgClientEx(string address, int port) : base(address, port) {}
        /// <summary>
        /// Initialize Ag client with a given IP endpoint
        /// </summary>
        /// <param name="endpoint">IP endpoint</param>
        public AgClientEx(IPEndPoint endpoint) : base(endpoint) {}

        #region Send request

        /// <summary>
        /// Send current Ag request
        /// </summary>
        /// <param name="timeout">Current Ag request timeout (default is 1 minute)</param>
        /// <returns>Ag request Task</returns>
        public Task<AgResponse> SendRequest(TimeSpan? timeout = null) { return SendRequest(Request, timeout); }
        /// <summary>
        /// Send Ag request
        /// </summary>
        /// <param name="request">Ag request</param>
        /// <param name="timeout">Ag request timeout (default is 1 minute)</param>
        /// <returns>Ag request Task</returns>
        public Task<AgResponse> SendRequest(AgRequest request, TimeSpan? timeout = null)
        {
            timeout ??= TimeSpan.FromMinutes(1);

            _tcs = new TaskCompletionSource<AgResponse>();
            Request = request;

            // Check if the Ag request is valid
            if (Request.IsEmpty || Request.IsErrorSet)
            {
                SetPromiseError("Invalid Ag request!");
                return _tcs.Task;
            }

            if (!IsConnected)
            {
                // Connect to the Web server
                if (!ConnectAsync())
                {
                    SetPromiseError("Connection failed!");
                    return _tcs.Task;
                }
            }
            else
            {
                // Send prepared Ag request
                if (!SendRequestAsync())
                {
                    SetPromiseError("Failed to send Ag request!");
                    return _tcs.Task;
                }
            }

            void TimeoutHandler(object state)
            {
                // Disconnect on timeout
                OnReceivedResponseError(Response, "Timeout!");
                Response.Clear();
                DisconnectAsync();
            }

            // Create a new timeout timer
            if (_timer == null)
                _timer = new Timer(TimeoutHandler, null, Timeout.Infinite, Timeout.Infinite);

            // Start the timeout timer
            _timer.Change((int)timeout.Value.TotalMilliseconds, Timeout.Infinite);

            return _tcs.Task;
        }

        /// <summary>
        /// Send HEAD request
        /// </summary>
        /// <param name="key">key to request</param>
        /// <param name="timeout">Current Ag request timeout (default is 1 minute)</param>
        /// <returns>Ag request Task</returns>
        public Task<AgResponse> SendHeadRequest(string key, TimeSpan? timeout = null) { return SendRequest(Request.MakeHeadRequest(key), timeout); }
        /// <summary>
        /// Send GET request
        /// </summary>
        /// <param name="key">key to request</param>
        /// <param name="timeout">Current Ag request timeout (default is 1 minute)</param>
        /// <returns>Ag request Task</returns>
        public Task<AgResponse> SendGetRequest(string key, TimeSpan? timeout = null) { return SendRequest(Request.MakeGetRequest(key), timeout); }
        /// <summary>
        /// Send POST request
        /// </summary>
        /// <param name="key">key to request</param>
        /// <param name="content">Content</param>
        /// <param name="timeout">Current Ag request timeout (default is 1 minute)</param>
        /// <returns>Ag request Task</returns>
        public Task<AgResponse> SendPostRequest(string key, string content, TimeSpan? timeout = null) { return SendRequest(Request.MakePostRequest(key, content), timeout); }
        /// <summary>
        /// Send PUT request
        /// </summary>
        /// <param name="key">key to request</param>
        /// <param name="content">Content</param>
        /// <param name="timeout">Current Ag request timeout (default is 1 minute)</param>
        /// <returns>Ag request Task</returns>
        public Task<AgResponse> SendPutRequest(string key, string content, TimeSpan? timeout = null) { return SendRequest(Request.MakePutRequest(key, content), timeout); }
        /// <summary>
        /// Send DELETE request
        /// </summary>
        /// <param name="key">key to request</param>
        /// <param name="timeout">Current Ag request timeout (default is 1 minute)</param>
        /// <returns>Ag request Task</returns>
        public Task<AgResponse> SendDeleteRequest(string key, TimeSpan? timeout = null) { return SendRequest(Request.MakeDeleteRequest(key), timeout); }
        /// <summary>
        /// Send OPTIONS request
        /// </summary>
        /// <param name="key">key to request</param>
        /// <param name="timeout">Current Ag request timeout (default is 1 minute)</param>
        /// <returns>Ag request Task</returns>
        public Task<AgResponse> SendOptionsRequest(string key, TimeSpan? timeout = null) { return SendRequest(Request.MakeOptionsRequest(key), timeout); }
        /// <summary>
        /// Send TRACE request
        /// </summary>
        /// <param name="key">key to request</param>
        /// <param name="timeout">Current Ag request timeout (default is 1 minute)</param>
        /// <returns>Ag request Task</returns>
        public Task<AgResponse> SendTraceRequest(string key, TimeSpan? timeout = null) { return SendRequest(Request.MakeTraceRequest(key), timeout); }

        #endregion

        #region Session handlers

        protected override void OnConnected()
        {
            // Send prepared Ag request on connect
            if (!Request.IsEmpty && !Request.IsErrorSet)
                if (!SendRequestAsync())
                    SetPromiseError("Failed to send Ag request!");
        }

        protected override void OnDisconnected()
        {
            // Cancel timeout check timer
            _timer?.Change(Timeout.Infinite, Timeout.Infinite);

            base.OnDisconnected();
        }

        protected override void OnReceivedResponse(AgResponse response)
        {
            // Cancel timeout check timer
            _timer?.Change(Timeout.Infinite, Timeout.Infinite);

            SetPromiseValue(response);
        }

        protected override void OnReceivedResponseError(AgResponse response, string error)
        {
            // Cancel timeout check timer
            _timer?.Change(Timeout.Infinite, Timeout.Infinite);

            SetPromiseError(error);
        }

        #endregion

        private TaskCompletionSource<AgResponse> _tcs = new TaskCompletionSource<AgResponse>();
        private Timer _timer;

        private void SetPromiseValue(AgResponse response)
        {
            Response = new AgResponse();
            _tcs.SetResult(response);
            Request.Clear();
        }

        private void SetPromiseError(string error)
        {
            _tcs.SetException(new Exception(error));
            Request.Clear();
        }

        #region IDisposable implementation

        // Disposed flag.
        private bool _disposed;

        protected override void Dispose(bool disposingManagedResources)
        {
            if (!_disposed)
            {
                if (disposingManagedResources)
                {
                    // Dispose managed resources here...
                    _timer?.Dispose();
                }

                // Dispose unmanaged resources here...

                // Set large fields to null here...

                // Mark as disposed.
                _disposed = true;
            }

            // Call Dispose in the base class.
            base.Dispose(disposingManagedResources);
        }

        // The derived class does not have a Finalize method
        // or a Dispose method without parameters because it inherits
        // them from the base class.

        #endregion
    }
}
