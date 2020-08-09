namespace AgProtocol
{
    /// <summary>
    /// Ag client is used to communicate with Ag Web server. It allows to send GET, POST, PUT, DELETE requests and receive Ag result.
    /// </summary>
    /// <remarks>Thread-safe.</remarks>
    public static class AgProtocol 
    {
        /// <summary>
        public static string Address { get; set; } = "127.0.0.1"; // 127 0 0 1
        public static int Port { get; set; } = 5555; // 8 0 8 0
    }
}
