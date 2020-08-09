namespace AgProtocol
{
    /// <summary>
    /// Ag session is used to receive/send Ag requests/responses from the connected Ag client.
    /// </summary>
    /// <remarks>Thread-safe.</remarks>
    public class AgSession : TcpSession
    {
        public AgSession(AgServer server) : base(server)
        {
            Cache = server.Cache;
            Request = new AgRequest();
            Response = new AgResponse();
        }

        /// <summary>
        /// Get the static content cache
        /// </summary>
        public FileCache Cache { get; }

        /// <summary>
        /// Get the Ag request
        /// </summary>
        protected AgRequest Request { get; }

        /// <summary>
        /// Get the Ag response
        /// </summary>
        public AgResponse Response { get; }

        #region Send response / Send response Data

        /// <summary>
        /// Send the current Ag response (synchronous)
        /// </summary>
        /// <returns>Size of sent data</returns>
        public long SendResponse() { return SendResponse(Response); }
        /// <summary>
        /// Send the Ag response (synchronous)
        /// </summary>
        /// <param name="response">Ag response</param>
        /// <returns>Size of sent data</returns>
        public long SendResponse(AgResponse response) { return Send(response.Cache.Data, response.Cache.Offset, response.Cache.Size); }

        /// <summary>
        /// Send the Ag response Data (synchronous)
        /// </summary>
        /// <param name="Data">Ag response Data</param>
        /// <returns>Size of sent data</returns>
        public long SendResponseData(string Data) { return Send(Data); }
        /// <summary>
        /// Send the Ag response Data (synchronous)
        /// </summary>
        /// <param name="buffer">Ag response Data buffer</param>
        /// <returns>Size of sent data</returns>
        public long SendResponseData(byte[] buffer) { return Send(buffer); }
        /// <summary>
        /// Send the Ag response Data (synchronous)
        /// </summary>
        /// <param name="buffer">Ag response Data buffer</param>
        /// <param name="offset">Ag response Data buffer offset</param>
        /// <param name="size">Ag response Data size</param>
        /// <returns>Size of sent data</returns>
        public long SendResponseData(byte[] buffer, long offset, long size) { return Send(buffer, offset, size); }

        /// <summary>
        /// Send the current Ag response (asynchronous)
        /// </summary>
        /// <returns>'true' if the current Ag response was successfully sent, 'false' if the session is not connected</returns>
        public bool SendResponseAsync() { return SendResponseAsync(Response); }
        /// <summary>
        /// Send the Ag response (asynchronous)
        /// </summary>
        /// <param name="response">Ag response</param>
        /// <returns>'true' if the current Ag response was successfully sent, 'false' if the session is not connected</returns>
        public bool SendResponseAsync(AgResponse response) { return SendAsync(response.Cache.Data, response.Cache.Offset, response.Cache.Size); }

        /// <summary>
        /// Send the Ag response Data (asynchronous)
        /// </summary>
        /// <param name="Data">Ag response Data</param>
        /// <returns>'true' if the Ag response Data was successfully sent, 'false' if the session is not connected</returns>
        public bool SendResponseDataAsync(string Data) { return SendAsync(Data); }
        /// <summary>
        /// Send the Ag response Data (asynchronous)
        /// </summary>
        /// <param name="buffer">Ag response Data buffer</param>
        /// <returns>'true' if the Ag response Data was successfully sent, 'false' if the session is not connected</returns>
        public bool SendResponseDataAsync(byte[] buffer) { return SendAsync(buffer); }
        /// <summary>
        /// Send the Ag response Data (asynchronous)
        /// </summary>
        /// <param name="buffer">Ag response Data buffer</param>
        /// <param name="offset">Ag response Data buffer offset</param>
        /// <param name="size">Ag response Data size</param>
        /// <returns>'true' if the Ag response Data was successfully sent, 'false' if the session is not connected</returns>
        public bool SendResponseDataAsync(byte[] buffer, long offset, long size) { return SendAsync(buffer, offset, size); }

        #endregion

        #region Session handlers

        protected override void OnReceived(byte[] buffer, long offset, long size)
        {
            // Receive Ag request header
            if (Request.IsPendingHeader())
            {
                if (Request.ReceiveHeader(buffer, (int)offset, (int)size))
                    OnReceivedRequestHeader(Request);

                size = 0;
            }

            // Check for Ag request error
            if (Request.IsErrorSet)
            {
                OnReceivedRequestError(Request, "Invalid Ag request!");
                Request.Clear();
                Disconnect();
                return;
            }

            // Receive Ag request Data
            if (Request.ReceiveData(buffer, (int)offset, (int)size))
            {
                OnReceivedRequestInternal(Request);
                Request.Clear();
                return;
            }

            // Check for Ag request error
            if (Request.IsErrorSet)
            {
                OnReceivedRequestError(Request, "Invalid Ag request!");
                Request.Clear();
                Disconnect();
                return;
            }
        }

        protected override void OnDisconnected()
        {
            // Receive Ag request Data
            if (Request.IsPendingData())
            {
                OnReceivedRequestInternal(Request);
                Request.Clear();
                return;
            }
        }

        /// <summary>
        /// Handle Ag request header received notification
        /// </summary>
        /// <remarks>Notification is called when Ag request header was received from the client.</remarks>
        /// <param name="request">Ag request</param>
        protected virtual void OnReceivedRequestHeader(AgRequest request) {}

        /// <summary>
        /// Handle Ag request received notification
        /// </summary>
        /// <remarks>Notification is called when Ag request was received from the client.</remarks>
        /// <param name="request">Ag request</param>
        protected virtual void OnReceivedRequest(AgRequest request) {}

        /// <summary>
        /// Handle Ag request error notification
        /// </summary>
        /// <remarks>Notification is called when Ag request error was received from the client.</remarks>
        /// <param name="request">Ag request</param>
        /// <param name="error">Ag request error</param>
        protected virtual void OnReceivedRequestError(AgRequest request, string error) {}

        #endregion

        private void OnReceivedRequestInternal(AgRequest request)
        {
            // Try to get the cached response
            if (request.Method == "GET")
            {
                var response = Cache.Find(request.key);
                if (response.Item1)
                {
                    SendAsync(response.Item2);
                    return;
                }
            }

            // Process the request
            OnReceivedRequest(request);
        }
    }
}
