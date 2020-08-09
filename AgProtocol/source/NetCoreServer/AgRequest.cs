using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace AgProtocol
{
    /// <summary>
    /// Ag request is used to create or process parameters of Ag protocol request(method, key, headers, etc).
    /// </summary>
    /// <remarks>Not thread-safe.</remarks>
    public class AgRequest
    {
        /// <summary>
        /// Initialize an empty Ag request
        /// </summary>
        public AgRequest()
        {
            Clear();
        }
        /// <summary>
        /// Initialize a new Ag request with a given method, key and protocol
        /// </summary>
        /// <param name="method">Ag method</param>
        /// <param name="key">Requested key</param>
        /// <param name="protocol">Protocol version (default is "Ag/1.1")</param>
        public AgRequest(string method, string key, string protocol = "Ag/0.1") //  "Ag/1.1")
        {
            SetBegin(method, key, protocol);
        }

        /// <summary>
        /// Is the Ag request empty?
        /// </summary>
        public bool IsEmpty { get { return (_cache.Size == 0); } }
        /// <summary>
        /// Is the Ag request error flag set?
        /// </summary>
        public bool IsErrorSet { get; private set; }

        /// <summary>
        /// Get the Ag request method
        /// </summary>
        public string Method { get { return _method; } }
        /// <summary>
        /// Get the Ag request key
        /// </summary>
        public string key { get { return _key; } }
        /// <summary>
        /// Get the Ag request protocol version
        /// </summary>
        public string Protocol { get { return _protocol; } }
        /// <summary>
        /// Get the Ag request headers count
        /// </summary>
        public long Headers { get { return _headers.Count; } }
        /// <summary>
        /// Get the Ag request header by index
        /// </summary>
        public Tuple<string, string> Header(int i)
        {
            Debug.Assert((i < _headers.Count), "Index out of bounds!");
            if (i >= _headers.Count)
                return new Tuple<string, string>("", "");

            return _headers[i];
        }
        /// <summary>
        /// Get the Ag request cookies count
        /// </summary>
        long Cookies { get { return _cookies.Count; } }
        /// <summary>
        /// Get the Ag request cookie by index
        /// </summary>
        Tuple<string, string> Cookie(int i)
        {
            Debug.Assert((i < _cookies.Count), "Index out of bounds!");
            if (i >= _cookies.Count)
                return new Tuple<string, string>("", "");

            return _cookies[i];
        }
        /// <summary>
        /// Get the Ag request Data as string
        /// </summary>
        public string Data { get { return _cache.ExtractString(_DataIndex, _DataSize); } }
        /// <summary>
        /// Get the Ag request Data as byte array
        /// </summary>
        public byte[] DataBytes { get { return _cache.Data[_DataIndex..(_DataIndex + _DataSize)]; } }
        /// <summary>
        /// Get the Ag request Data as byte span
        /// </summary>
        public Span<byte> DataSpan { get { return new Span<byte>(_cache.Data, _DataIndex, _DataSize); } }
        /// <summary>
        /// Get the Ag request Data length
        /// </summary>
        public long DataLength { get { return _DataLength; } }

        /// <summary>
        /// Get the Ag request cache content
        /// </summary>
        public Buffer Cache { get { return _cache; } }

        /// <summary>
        /// Get string from the current Ag request
        /// </summary>
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine($"Request method: {Method}");
            //sb.AppendLine($"Request key: {key}");
            sb.AppendLine($"Request KEY: {key}");
            sb.AppendLine($"Request protocol: {Protocol}");
            sb.AppendLine($"Request headers: {Headers}");
            for (int i = 0; i < Headers; ++i)
            {
                var header = Header(i);
                sb.AppendLine($"{header.Item1} : {header.Item2}");
            }
            sb.AppendLine($"Request DATA: {DataLength}");
            sb.AppendLine(Data);
            return sb.ToString();
        }

        /// <summary>
        /// Clear the Ag request cache
        /// </summary>
        public AgRequest Clear()
        {
            IsErrorSet = false;
            _method = "";
            _key = "";
            _protocol = "";
            _headers.Clear();
            _cookies.Clear();
            _DataIndex = 0;
            _DataSize = 0;
            _DataLength = 0;
            _DataLengthProvided = false;

            _cache.Clear();
            _cacheSize = 0;
            return this;
        }

        /// <summary>
        /// Set the Ag request begin with a given method, key and protocol
        /// </summary>
        /// <param name="method">Ag method</param>
        /// <param name="key">Requested key</param>
        /// <param name="protocol">Protocol version (default is "Ag/1.1")</param>
        public AgRequest SetBegin(string method, string key, string protocol = "Ag/0.1") // = "Ag/1.1")
        {
            // Clear the Ag request cache
            Clear();

            // Append the Ag request method
            _cache.Append(method);
            _method = method;

            _cache.Append(" ");

            // Append the Ag request key
            _cache.Append(key);
            _key = key;

            _cache.Append(" ");

            // Append the Ag request protocol version
            _cache.Append(protocol);
            _protocol = protocol;

            _cache.Append("\r\n");
            return this;
        }

        /// <summary>
        /// Set the Ag request header
        /// </summary>
        /// <param name="key">Header key</param>
        /// <param name="value">Header value</param>
        public AgRequest SetHeader(string key, string value)
        {
            // Append the Ag request header's key
            _cache.Append(key);

            _cache.Append(": ");

            // Append the Ag request header's value
            _cache.Append(value);

            _cache.Append("\r\n");

            // Add the header to the corresponding collection
            _headers.Add(new Tuple<string, string>(key, value));
            return this;
        }

        /// <summary>
        /// Set the Ag request cookie
        /// </summary>
        /// <param name="name">Cookie name</param>
        /// <param name="value">Cookie value</param>
        public AgRequest SetCookie(string name, string value)
        {
            string key = "Cookie";
            string cookie = name + "=" + value;

            // Append the Ag request header's key
            _cache.Append(key);

            _cache.Append(": ");

            // Append Cookie
            _cache.Append(cookie);

            _cache.Append("\r\n");

            // Add the header to the corresponding collection
            _headers.Add(new Tuple<string, string>(key, cookie));
            // Add the cookie to the corresponding collection
            _cookies.Add(new Tuple<string, string>(name, value));
            return this;
        }

        /// <summary>
        /// Add the Ag request cookie
        /// </summary>
        /// <param name="name">Cookie name</param>
        /// <param name="value">Cookie value</param>
        public AgRequest AddCookie(string name, string value)
        {
            // Append Cookie
            _cache.Append("; ");
            _cache.Append(name);
            _cache.Append("=");
            _cache.Append(value);

            // Add the cookie to the corresponding collection
            _cookies.Add(new Tuple<string, string>(name, value));
            return this;
        }

        /// <summary>
        /// Set the Ag request Data
        /// </summary>
        /// <param name="Data">Data string content (default is "")</param>
        public AgRequest SetData(string Data = "")
        {
            int length = string.IsNullOrEmpty(Data) ? 0 : Encoding.UTF8.GetByteCount(Data);

            // Append content length header
            SetHeader("Content-Length", length.ToString());

            _cache.Append("\r\n");

            int index = (int)_cache.Size;

            // Append the Ag request Data
            _cache.Append(Data);
            _DataIndex = index;
            _DataSize = length;
            _DataLength = length;
            _DataLengthProvided = true;
            return this;
        }

        /// <summary>
        /// Set the Ag request Data
        /// </summary>
        /// <param name="Data">Data binary content</param>
        public AgRequest SetData(byte[] Data)
        {
            // Append content length header
            SetHeader("Content-Length", Data.Length.ToString());

            _cache.Append("\r\n");

            int index = (int)_cache.Size;

            // Append the Ag request Data
            _cache.Append(Data);
            _DataIndex = index;
            _DataSize = Data.Length;
            _DataLength = Data.Length;
            _DataLengthProvided = true;
            return this;
        }

        /// <summary>
        /// Set the Ag request Data
        /// </summary>
        /// <param name="Data">Data buffer content</param>
        public AgRequest SetData(Buffer Data)
        {
            // Append content length header
            SetHeader("Content-Length", Data.Size.ToString());

            _cache.Append("\r\n");

            int index = (int)_cache.Size;

            // Append the Ag request Data
            _cache.Append(Data.Data, Data.Offset, Data.Size);
            _DataIndex = index;
            _DataSize = (int)Data.Size;
            _DataLength = (int)Data.Size;
            _DataLengthProvided = true;
            return this;
        }

        /// <summary>
        /// Set the Ag request Data length
        /// </summary>
        /// <param name="length">Data length</param>
        public AgRequest SetDataLength(int length)
        {
            // Append content length header
            SetHeader("Content-Length", length.ToString());

            _cache.Append("\r\n");

            int index = (int)_cache.Size;

            // Clear the Ag request Data
            _DataIndex = index;
            _DataSize = 0;
            _DataLength = length;
            _DataLengthProvided = true;
            return this;
        }

        /// <summary>
        /// Make HEAD request
        /// </summary>
        /// <param name="key">key to request</param>
        public AgRequest MakeHeadRequest(string key)
        {
            Clear();
            SetBegin("HEAD", key);
            SetData();
            return this;
        }

        /// <summary>
        /// Make GET request
        /// </summary>
        /// <param name="key">key to request</param>
        public AgRequest MakeGetRequest(string key)
        {
            Clear();
            SetBegin("GET", key);
            SetData();
            return this;
        }

        /// <summary>
        /// Make POST request
        /// </summary>
        /// <param name="key">key to request</param>
        /// <param name="content">String content</param>
        public AgRequest MakePostRequest(string key, string content)
        {
            Clear();
            SetBegin("POST", key);
            SetData(content);
            return this;
        }

        /// <summary>
        /// Make POST request
        /// </summary>
        /// <param name="key">key to request</param>
        /// <param name="content">Binary content</param>
        public AgRequest MakePostRequest(string key, byte[] content)
        {
            Clear();
            SetBegin("POST", key);
            SetData(content);
            return this;
        }

        /// <summary>
        /// Make POST request
        /// </summary>
        /// <param name="key">key to request</param>
        /// <param name="content">Buffer content</param>
        public AgRequest MakePostRequest(string key, Buffer content)
        {
            Clear();
            SetBegin("POST", key);
            SetData(content);
            return this;
        }

        /// <summary>
        /// Make PUT request
        /// </summary>
        /// <param name="key">key to request</param>
        /// <param name="content">String content</param>
        public AgRequest MakePutRequest(string key, string content)
        {
            Clear();
            SetBegin("PUT", key);
            SetData(content);
            return this;
        }

        /// <summary>
        /// Make PUT request
        /// </summary>
        /// <param name="key">key to request</param>
        /// <param name="content">Binary content</param>
        public AgRequest MakePutRequest(string key, byte[] content)
        {
            Clear();
            SetBegin("PUT", key);
            SetData(content);
            return this;
        }

        /// <summary>
        /// Make PUT request
        /// </summary>
        /// <param name="key">key to request</param>
        /// <param name="content">Buffer content</param>
        public AgRequest MakePutRequest(string key, Buffer content)
        {
            Clear();
            SetBegin("PUT", key);
            SetData(content);
            return this;
        }

        /// <summary>
        /// Make DELETE request
        /// </summary>
        /// <param name="key">key to request</param>
        public AgRequest MakeDeleteRequest(string key)
        {
            Clear();
            SetBegin("DELETE", key);
            SetData();
            return this;
        }

        /// <summary>
        /// Make OPTIONS request
        /// </summary>
        /// <param name="key">key to request</param>
        public AgRequest MakeOptionsRequest(string key)
        {
            Clear();
            SetBegin("OPTIONS", key);
            SetData();
            return this;
        }

        /// <summary>
        /// Make TRACE request
        /// </summary>
        /// <param name="key">key to request</param>
        public AgRequest MakeTraceRequest(string key)
        {
            Clear();
            SetBegin("TRACE", key);
            SetData();
            return this;
        }

        // Ag request method
        private string _method;
        // Ag request key
        private string _key;
        // Ag request protocol
        private string _protocol;
        // Ag request headers
        private List<Tuple<string, string>> _headers = new List<Tuple<string, string>>();
        // Ag request cookies
        private List<Tuple<string, string>> _cookies = new List<Tuple<string, string>>();
        // Ag request Data
        private int _DataIndex;
        private int _DataSize;
        private int _DataLength;
        private bool _DataLengthProvided;

        // Ag request cache
        private Buffer _cache = new Buffer();
        private int _cacheSize;

        // Is pending parts of Ag request
        internal bool IsPendingHeader()
        {
            return (!IsErrorSet && (_DataIndex == 0));
        }
        internal bool IsPendingData()
        {
            return (!IsErrorSet && (_DataIndex > 0) && (_DataSize > 0));
        }

        internal bool ReceiveHeader(byte[] buffer, int offset, int size)
        {
            // Update the request cache
            _cache.Append(buffer, offset, size);

            // Try to seek for Ag header separator
            for (int i = _cacheSize; i < (int)_cache.Size; ++i)
            {
                // Check for the request cache out of bounds
                if ((i + 3) >= (int)_cache.Size)
                    break;

                // Check for the header separator
                if ((_cache[i + 0] == '\r') && (_cache[i + 1] == '\n') && (_cache[i + 2] == '\r') && (_cache[i + 3] == '\n'))
                {
                    int index = 0;

                    // Set the error flag for a while...
                    IsErrorSet = true;

                    // Parse method
                    int methodIndex = index;
                    int methodSize = 0;
                    while (_cache[index] != ' ')
                    {
                        ++methodSize;
                        ++index;
                        if (index >= (int)_cache.Size)
                            return false;
                    }
                    ++index;
                    if (index >= (int)_cache.Size)
                        return false;
                    _method = _cache.ExtractString(methodIndex, methodSize);

                    // Parse key
                    int keyIndex = index;
                    int keySize = 0;
                    while (_cache[index] != ' ')
                    {
                        ++keySize;
                        ++index;
                        if (index >= (int)_cache.Size)
                            return false;
                    }
                    ++index;
                    if (index >= (int)_cache.Size)
                        return false;
                    _key = _cache.ExtractString(keyIndex, keySize);

                    // Parse protocol version
                    int protocolIndex = index;
                    int protocolSize = 0;
                    while (_cache[index] != '\r')
                    {
                        ++protocolSize;
                        ++index;
                        if (index >= (int)_cache.Size)
                            return false;
                    }
                    ++index;
                    if ((index >= (int)_cache.Size) || (_cache[index] != '\n'))
                        return false;
                    ++index;
                    if (index >= (int)_cache.Size)
                        return false;
                    _protocol = _cache.ExtractString(protocolIndex, protocolSize);

                    // Parse headers
                    while ((index < (int)_cache.Size) && (index < i))
                    {
                        // Parse header name
                        int headerNameIndex = index;
                        int headerNameSize = 0;
                        while (_cache[index] != ':')
                        {
                            ++headerNameSize;
                            ++index;
                            if (index >= i)
                                break;
                            if (index >= (int)_cache.Size)
                                return false;
                        }
                        ++index;
                        if (index >= i)
                            break;
                        if (index >= (int)_cache.Size)
                            return false;

                        // Skip all prefix space characters
                        while (char.IsWhiteSpace((char)_cache[index]))
                        {
                            ++index;
                            if (index >= i)
                                break;
                            if (index >= (int)_cache.Size)
                                return false;
                        }

                        // Parse header value
                        int headerValueIndex = index;
                        int headerValueSize = 0;
                        while (_cache[index] != '\r')
                        {
                            ++headerValueSize;
                            ++index;
                            if (index >= i)
                                break;
                            if (index >= (int)_cache.Size)
                                return false;
                        }
                        ++index;
                        if ((index >= (int)_cache.Size) || (_cache[index] != '\n'))
                            return false;
                        ++index;
                        if (index >= (int)_cache.Size)
                            return false;

                        // Validate header name and value
                        if ((headerNameSize == 0) || (headerValueSize == 0))
                            return false;

                        // Add a new header
                        string headerName = _cache.ExtractString(headerNameIndex, headerNameSize);
                        string headerValue = _cache.ExtractString(headerValueIndex, headerValueSize);
                        _headers.Add(new Tuple<string, string>(headerName, headerValue));

                        // Try to find the Data content length
                        if (headerName == "Content-Length")
                        {
                            _DataLength = 0;
                            for (int j = headerValueIndex; j < (headerValueIndex + headerValueSize); ++j)
                            {
                                if ((_cache[j] < '0') || (_cache[j] > '9'))
                                    return false;
                                _DataLength *= 10;
                                _DataLength += _cache[j] - '0';
                                _DataLengthProvided = true;
                            }
                        }

                        // Try to find Cookies
                        if (headerName == "Cookie")
                        {
                            bool name = true;
                            bool token = false;
                            int current = headerValueIndex;
                            int nameIndex = index;
                            int nameSize = 0;
                            int cookieIndex = index;
                            int cookieSize = 0;
                            for (int j = headerValueIndex; j < (headerValueIndex + headerValueSize); ++j)
                            {
                                if (_cache[j] == ' ')
                                {
                                    if (token)
                                    {
                                        if (name)
                                        {
                                            nameIndex = current;
                                            nameSize = j - current;
                                        }
                                        else
                                        {
                                            cookieIndex = current;
                                            cookieSize = j - current;
                                        }
                                    }
                                    token = false;
                                    continue;
                                }
                                if (_cache[j] == '=')
                                {
                                    if (token)
                                    {
                                        if (name)
                                        {
                                            nameIndex = current;
                                            nameSize = j - current;
                                        }
                                        else
                                        {
                                            cookieIndex = current;
                                            cookieSize = j - current;
                                        }
                                    }
                                    token = false;
                                    name = false;
                                    continue;
                                }
                                if (_cache[j] == ';')
                                {
                                    if (token)
                                    {
                                        if (name)
                                        {
                                            nameIndex = current;
                                            nameSize = j - current;
                                        }
                                        else
                                        {
                                            cookieIndex = current;
                                            cookieSize = j - current;
                                        }

                                        // Validate the cookie
                                        if ((nameSize > 0) && (cookieSize > 0))
                                        {
                                            // Add the cookie to the corresponding collection
                                            _cookies.Add(new Tuple<string, string>(_cache.ExtractString(nameIndex, nameSize), _cache.ExtractString(cookieIndex, cookieSize)));

                                            // Resset the current cookie values
                                            nameIndex = j;
                                            nameSize = 0;
                                            cookieIndex = j;
                                            cookieSize = 0;
                                        }
                                    }
                                    token = false;
                                    name = true;
                                    continue;
                                }
                                if (!token)
                                {
                                    current = j;
                                    token = true;
                                }
                            }

                            // Process the last cookie
                            if (token)
                            {
                                if (name)
                                {
                                    nameIndex = current;
                                    nameSize = headerValueIndex + headerValueSize - current;
                                }
                                else
                                {
                                    cookieIndex = current;
                                    cookieSize = headerValueIndex + headerValueSize - current;
                                }

                                // Validate the cookie
                                if ((nameSize > 0) && (cookieSize > 0))
                                {
                                    // Add the cookie to the corresponding collection
                                    _cookies.Add(new Tuple<string, string>(_cache.ExtractString(nameIndex, nameSize), _cache.ExtractString(cookieIndex, cookieSize)));
                                }
                            }
                        }
                    }

                    // Reset the error flag
                    IsErrorSet = false;

                    // Update the Data index and size
                    _DataIndex = i + 4;
                    _DataSize = (int)_cache.Size - i - 4;

                    // Update the parsed cache size
                    _cacheSize = (int)_cache.Size;

                    return true;
                }
            }

            // Update the parsed cache size
            _cacheSize = ((int)_cache.Size >= 3) ? ((int)_cache.Size - 3) : 0;

            return false;
        }

        internal bool ReceiveData(byte[] buffer, int offset, int size)
        {
            // Update the request cache
            _cache.Append(buffer, offset, size);

            // Update the parsed cache size
            _cacheSize = (int)_cache.Size;

            // Update Data size
            _DataSize += size;

            // GET request has no Data
            if ((Method == "HEAD") || (Method == "GET") || (Method == "OPTIONS") || (Method == "TRACE"))
            {
                _DataLength = 0;
                _DataSize = 0;
                return true;
            }

            // Check if the Data was fully parsed
            if (_DataLengthProvided && (_DataSize >= _DataLength))
            {
                _DataSize = _DataLength;
                return true;
            }

            return false;
        }
    }
}
