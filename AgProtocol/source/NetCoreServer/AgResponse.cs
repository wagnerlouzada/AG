using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace AgProtocol
{
    /// <summary>
    /// Ag response is used to create or process parameters of Ag protocol response(status, headers, etc).
    /// </summary>
    /// <remarks>Not thread-safe.</remarks>
    public class AgResponse
    {
        /// <summary>
        /// Initialize an empty Ag response
        /// </summary>
        public AgResponse()
        {
            Clear();
        }
        /// <summary>
        /// Initialize a new Ag response with a given status and protocol
        /// </summary>
        /// <param name="status">Ag status</param>
        /// <param name="protocol">Protocol version (default is "Ag/1.1")</param>
        public AgResponse(int status, string protocol = "Ag/0.1")
        {
            SetBegin(status, protocol);
        }
        /// <summary>
        /// Initialize a new Ag response with a given status, status phrase and protocol
        /// </summary>
        /// <param name="status">Ag status</param>
        /// <param name="statusPhrase">Ag status phrase</param>
        /// <param name="protocol">Protocol version</param>
        public AgResponse(int status, string statusPhrase, string protocol)
        {
            SetBegin(status, statusPhrase, protocol);
        }

        /// <summary>
        /// Is the Ag response empty?
        /// </summary>
        public bool IsEmpty { get { return (_cache.Size > 0); } }
        /// <summary>
        /// Is the Ag response error flag set?
        /// </summary>
        public bool IsErrorSet { get; private set; }

        /// <summary>
        /// Get the Ag response status
        /// </summary>
        public int Status { get; private set; }

        /// <summary>
        /// Get the Ag response status phrase
        /// </summary>
        public string StatusPhrase { get { return _statusPhrase; } }
        /// <summary>
        /// Get the Ag response protocol version
        /// </summary>
        public string Protocol { get { return _protocol; } }
        /// <summary>
        /// Get the Ag response headers count
        /// </summary>
        public long Headers { get { return _headers.Count; } }
        /// <summary>
        /// Get the Ag response header by index
        /// </summary>
        public Tuple<string, string> Header(int i)
        {
            Debug.Assert((i < _headers.Count), "Index out of bounds!");
            if (i >= _headers.Count)
                return new Tuple<string, string>("", "");

            return _headers[i];
        }
        /// <summary>
        /// Get the Ag response Data as string
        /// </summary>
        public string Data { get { return _cache.ExtractString(_DataIndex, _DataSize); } }
        /// <summary>
        /// Get the Ag request Data as byte array
        /// </summary>
        public byte[] DataBytes { get { return _cache.Data[_DataIndex..(_DataIndex + _DataSize)]; } }
        /// <summary>
        /// Get the Ag request Data as read-only byte span
        /// </summary>
        public ReadOnlySpan<byte> DataSpan { get { return new ReadOnlySpan<byte>(_cache.Data, _DataIndex, _DataSize); } }
        /// <summary>
        /// Get the Ag response Data length
        /// </summary>
        public long DataLength { get { return _DataLength; } }

        /// <summary>
        /// Get the Ag response cache content
        /// </summary>
        public Buffer Cache { get { return _cache; } }

        /// <summary>
        /// Get string from the current Ag response
        /// </summary>
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine($"Status: {Status}");
            sb.AppendLine($"Status phrase: {StatusPhrase}");
            sb.AppendLine($"Protocol: {Protocol}");
            sb.AppendLine($"Headers: {Headers}");
            for (int i = 0; i < Headers; ++i)
            {
                var header = Header(i);
                sb.AppendLine($"{header.Item1} : {header.Item2}");
            }
            sb.AppendLine($"Data: {DataLength}");
            sb.AppendLine(Data);
            return sb.ToString();
        }

        /// <summary>
        /// Clear the Ag response cache
        /// </summary>
        public AgResponse Clear()
        {
            IsErrorSet = false;
            Status = 0;
            _statusPhrase = "";
            _protocol = "";
            _headers.Clear();
            _DataIndex = 0;
            _DataSize = 0;
            _DataLength = 0;
            _DataLengthProvided = false;

            _cache.Clear();
            _cacheSize = 0;
            return this;
        }

        /// <summary>
        /// Set the Ag response begin with a given status and protocol
        /// </summary>
        /// <param name="status">Ag status</param>
        /// <param name="protocol">Protocol version (default is "Ag/1.1")</param>
        public AgResponse SetBegin(int status, string protocol = "Ag/0.1")
        {
            string statusPhrase;

            switch (status)
            {
                case 100: statusPhrase = "Continue"; break;
                case 101: statusPhrase = "Switching Protocols"; break;
                case 102: statusPhrase = "Processing"; break;
                case 103: statusPhrase = "Early Hints"; break;

                case 200: statusPhrase = "OK"; break;
                case 201: statusPhrase = "Created"; break;
                case 202: statusPhrase = "Accepted"; break;
                case 203: statusPhrase = "Non-Authoritative Information"; break;
                case 204: statusPhrase = "No Content"; break;
                case 205: statusPhrase = "Reset Content"; break;
                case 206: statusPhrase = "Partial Content"; break;
                case 207: statusPhrase = "Multi-Status"; break;
                case 208: statusPhrase = "Already Reported"; break;

                case 226: statusPhrase = "IM Used"; break;

                case 300: statusPhrase = "Multiple Choices"; break;
                case 301: statusPhrase = "Moved Permanently"; break;
                case 302: statusPhrase = "Found"; break;
                case 303: statusPhrase = "See Other"; break;
                case 304: statusPhrase = "Not Modified"; break;
                case 305: statusPhrase = "Use Proxy"; break;
                case 306: statusPhrase = "Switch Proxy"; break;
                case 307: statusPhrase = "Temporary Redirect"; break;
                case 308: statusPhrase = "Permanent Redirect"; break;

                case 400: statusPhrase = "Bad Request"; break;
                case 401: statusPhrase = "Unauthorized"; break;
                case 402: statusPhrase = "Payment Required"; break;
                case 403: statusPhrase = "Forbidden"; break;
                case 404: statusPhrase = "Not Found"; break;
                case 405: statusPhrase = "Method Not Allowed"; break;
                case 406: statusPhrase = "Not Acceptable"; break;
                case 407: statusPhrase = "Proxy Authentication Required"; break;
                case 408: statusPhrase = "Request Timeout"; break;
                case 409: statusPhrase = "Conflict"; break;
                case 410: statusPhrase = "Gone"; break;
                case 411: statusPhrase = "Length Required"; break;
                case 412: statusPhrase = "Precondition Failed"; break;
                case 413: statusPhrase = "Payload Too Large"; break;
                case 414: statusPhrase = "URI Too Long"; break;
                case 415: statusPhrase = "Unsupported Media Type"; break;
                case 416: statusPhrase = "Range Not Satisfiable"; break;
                case 417: statusPhrase = "Expectation Failed"; break;

                case 421: statusPhrase = "Misdirected Request"; break;
                case 422: statusPhrase = "Unprocessable Entity"; break;
                case 423: statusPhrase = "Locked"; break;
                case 424: statusPhrase = "Failed Dependency"; break;
                case 425: statusPhrase = "Too Early"; break;
                case 426: statusPhrase = "Upgrade Required"; break;
                case 427: statusPhrase = "Unassigned"; break;
                case 428: statusPhrase = "Precondition Required"; break;
                case 429: statusPhrase = "Too Many Requests"; break;
                case 431: statusPhrase = "Request Header Fields Too Large"; break;

                case 451: statusPhrase = "Unavailable For Legal Reasons"; break;

                case 500: statusPhrase = "Internal Server Error"; break;
                case 501: statusPhrase = "Not Implemented"; break;
                case 502: statusPhrase = "Bad Gateway"; break;
                case 503: statusPhrase = "Service Unavailable"; break;
                case 504: statusPhrase = "Gateway Timeout"; break;
                case 505: statusPhrase = "Ag Version Not Supported"; break;
                case 506: statusPhrase = "Variant Also Negotiates"; break;
                case 507: statusPhrase = "Insufficient Storage"; break;
                case 508: statusPhrase = "Loop Detected"; break;

                case 510: statusPhrase = "Not Extended"; break;
                case 511: statusPhrase = "Network Authentication Required"; break;

                default: statusPhrase = "Unknown"; break;
            }

            SetBegin(status, statusPhrase, protocol);
            return this;
        }

        /// <summary>
        /// Set the Ag response begin with a given status, status phrase and protocol
        /// </summary>
        /// <param name="status">Ag status</param>
        /// <param name="statusPhrase"> Ag status phrase</param>
        /// <param name="protocol">Protocol version</param>
        public AgResponse SetBegin(int status, string statusPhrase, string protocol)
        {
            // Clear the Ag response cache
            Clear();

            // Append the Ag response protocol version
            _cache.Append(protocol);
            _protocol = protocol;

            _cache.Append(" ");

            // Append the Ag response status
            _cache.Append(status.ToString());
            Status = status;

            _cache.Append(" ");

            // Append the Ag response status phrase
            _cache.Append(statusPhrase);
            _statusPhrase = statusPhrase;

            _cache.Append("\r\n");
            return this;
        }

        /// <summary>
        /// Set the Ag response content type
        /// </summary>
        /// <param name="extension">Content extension</param>
        public AgResponse SetContentType(string extension)
        {
            // Base content types
            if (extension == ".html")
                return SetHeader("Content-Type", "text/html");
            else if (extension == ".css")
                return SetHeader("Content-Type", "text/css");
            else if (extension == ".js")
                return SetHeader("Content-Type", "text/javascript");
            else if (extension == ".xml")
                return SetHeader("Content-Type", "text/xml");

            // Common content types
            if (extension == ".gzip")
                return SetHeader("Content-Type", "application/gzip");
            else if (extension == ".json")
                return SetHeader("Content-Type", "application/json");
            else if (extension == ".map")
                return SetHeader("Content-Type", "application/json");
            else if (extension == ".pdf")
                return SetHeader("Content-Type", "application/pdf");
            else if (extension == ".zip")
                return SetHeader("Content-Type", "application/zip");
            else if (extension == ".mp3")
                return SetHeader("Content-Type", "audio/mpeg");
            else if (extension == ".jpg")
                return SetHeader("Content-Type", "image/jpeg");
            else if (extension == ".gif")
                return SetHeader("Content-Type", "image/gif");
            else if (extension == ".png")
                return SetHeader("Content-Type", "image/png");
            else if (extension == ".svg")
                return SetHeader("Content-Type", "image/svg+xml");
            else if (extension == ".mp4")
                return SetHeader("Content-Type", "video/mp4");

            // Application content types
            if (extension == ".atom")
                return SetHeader("Content-Type", "application/atom+xml");
            else if (extension == ".fastsoap")
                return SetHeader("Content-Type", "application/fastsoap");
            else if (extension == ".ps")
                return SetHeader("Content-Type", "application/postscript");
            else if (extension == ".soap")
                return SetHeader("Content-Type", "application/soap+xml");
            else if (extension == ".sql")
                return SetHeader("Content-Type", "application/sql");
            else if (extension == ".xslt")
                return SetHeader("Content-Type", "application/xslt+xml");
            else if (extension == ".zlib")
                return SetHeader("Content-Type", "application/zlib");

            // Audio content types
            if (extension == ".aac")
                return SetHeader("Content-Type", "audio/aac");
            else if (extension == ".ac3")
                return SetHeader("Content-Type", "audio/ac3");
            else if (extension == ".ogg")
                return SetHeader("Content-Type", "audio/ogg");

            // Font content types
            if (extension == ".ttf")
                return SetHeader("Content-Type", "font/ttf");

            // Image content types
            if (extension == ".bmp")
                return SetHeader("Content-Type", "image/bmp");
            else if (extension == ".jpm")
                return SetHeader("Content-Type", "image/jpm");
            else if (extension == ".jpx")
                return SetHeader("Content-Type", "image/jpx");
            else if (extension == ".jrx")
                return SetHeader("Content-Type", "image/jrx");
            else if (extension == ".tiff")
                return SetHeader("Content-Type", "image/tiff");
            else if (extension == ".emf")
                return SetHeader("Content-Type", "image/emf");
            else if (extension == ".wmf")
                return SetHeader("Content-Type", "image/wmf");

            // Message content types
            if (extension == ".Ag")
                return SetHeader("Content-Type", "message/Ag");
            else if (extension == ".s-Ag")
                return SetHeader("Content-Type", "message/s-Ag");

            // Model content types
            if (extension == ".mesh")
                return SetHeader("Content-Type", "model/mesh");
            else if (extension == ".vrml")
                return SetHeader("Content-Type", "model/vrml");

            // Text content types
            if (extension == ".csv")
                return SetHeader("Content-Type", "text/csv");
            else if (extension == ".plain")
                return SetHeader("Content-Type", "text/plain");
            else if (extension == ".richtext")
                return SetHeader("Content-Type", "text/richtext");
            else if (extension == ".rtf")
                return SetHeader("Content-Type", "text/rtf");
            else if (extension == ".rtx")
                return SetHeader("Content-Type", "text/rtx");
            else if (extension == ".sgml")
                return SetHeader("Content-Type", "text/sgml");
            else if (extension == ".strings")
                return SetHeader("Content-Type", "text/strings");
            else if (extension == ".key")
                return SetHeader("Content-Type", "text/uri-list");

            // Video content types
            if (extension == ".H264")
                return SetHeader("Content-Type", "video/H264");
            else if (extension == ".H265")
                return SetHeader("Content-Type", "video/H265");
            else if (extension == ".mpeg")
                return SetHeader("Content-Type", "video/mpeg");
            else if (extension == ".raw")
                return SetHeader("Content-Type", "video/raw");

            return this;
        }

        /// <summary>
        /// Set the Ag response header
        /// </summary>
        /// <param name="key">Header key</param>
        /// <param name="value">Header value</param>
        public AgResponse SetHeader(string key, string value)
        {
            // Append the Ag response header's key
            _cache.Append(key);

            _cache.Append(": ");

            // Append the Ag response header's value
            _cache.Append(value);

            _cache.Append("\r\n");

            // Add the header to the corresponding collection
            _headers.Add(new Tuple<string, string>(key, value));
            return this;
        }

        /// <summary>
        /// Set the Ag response cookie
        /// </summary>
        /// <param name="name">Cookie name</param>
        /// <param name="value">Cookie value</param>
        /// <param name="maxAge">Cookie age in seconds until it expires (default is 86400)</param>
        /// <param name="path">Cookie path (default is "")</param>
        /// <param name="domain">Cookie domain (default is "")</param>
        /// <param name="secure">Cookie secure flag (default is true)</param>
        /// <param name="AgOnly">Cookie Ag-only flag (default is false)</param>
        public AgResponse SetCookie(string name, string value, int maxAge = 86400, string path = "", string domain = "", bool secure = true, bool AgOnly = false)
        {
            string key = "Set-Cookie";

            // Append the Ag response header's key
            _cache.Append(key);

            _cache.Append(": ");

            // Append the Ag response header's value
            int valueIndex = (int)_cache.Size;

            // Append cookie
            _cache.Append(name);
            _cache.Append("=");
            _cache.Append(value);
            _cache.Append("; Max-Age=");
            _cache.Append(maxAge.ToString());
            if (!string.IsNullOrEmpty(domain))
            {
                _cache.Append("; Domain=");
                _cache.Append(domain);
            }
            if (!string.IsNullOrEmpty(path))
            {
                _cache.Append("; Path=");
                _cache.Append(path);
            }
            if (secure)
                _cache.Append("; Secure");
            if (AgOnly)
                _cache.Append("; AgOnly");

            int valueSize = (int)_cache.Size - valueIndex;

            string cookie = _cache.ExtractString(valueIndex, valueSize);

            _cache.Append("\r\n");

            // Add the header to the corresponding collection
            _headers.Add(new Tuple<string, string>(key, cookie));
            return this;
        }

        /// <summary>
        /// Set the Ag response Data
        /// </summary>
        /// <param name="Data">Data string content (default is "")</param>
        public AgResponse SetData(string Data = "")
        {
            int length = string.IsNullOrEmpty(Data) ? 0 : Encoding.UTF8.GetByteCount(Data);

            // Append content length header
            SetHeader("Content-Length", length.ToString());

            _cache.Append("\r\n");

            int index = (int)_cache.Size;

            // Append the Ag response Data
            _cache.Append(Data);
            _DataIndex = index;
            _DataSize = length;
            _DataLength = length;
            _DataLengthProvided = true;
            return this;
        }

        /// <summary>
        /// Set the Ag response Data
        /// </summary>
        /// <param name="Data">Data binary content</param>
        public AgResponse SetData(byte[] Data)
        {
            // Append content length header
            SetHeader("Content-Length", Data.Length.ToString());

            _cache.Append("\r\n");

            int index = (int)_cache.Size;

            // Append the Ag response Data
            _cache.Append(Data);
            _DataIndex = index;
            _DataSize = Data.Length;
            _DataLength = Data.Length;
            _DataLengthProvided = true;
            return this;
        }

        /// <summary>
        /// Set the Ag response Data
        /// </summary>
        /// <param name="Data">Data buffer content</param>
        public AgResponse SetData(Buffer Data)
        {
            // Append content length header
            SetHeader("Content-Length", Data.Size.ToString());

            _cache.Append("\r\n");

            int index = (int)_cache.Size;

            // Append the Ag response Data
            _cache.Append(Data.Data, Data.Offset, Data.Size);
            _DataIndex = index;
            _DataSize = (int)Data.Size;
            _DataLength = (int)Data.Size;
            _DataLengthProvided = true;
            return this;
        }

        /// <summary>
        /// Set the Ag response Data length
        /// </summary>
        /// <param name="length">Data length</param>
        public AgResponse SetDataLength(int length)
        {
            // Append content length header
            SetHeader("Content-Length", length.ToString());

            _cache.Append("\r\n");

            int index = (int)_cache.Size;

            // Clear the Ag response Data
            _DataIndex = index;
            _DataSize = 0;
            _DataLength = length;
            _DataLengthProvided = true;
            return this;
        }

        /// <summary>
        /// Make OK response
        /// </summary>
        /// <param name="status">OK status (default is 200 (OK))</param>
        public AgResponse MakeOkResponse(int status = 200)
        {
            Clear();
            SetBegin(status);
            SetHeader("Content-Type", "text/html; charset=UTF-8");
            SetData();
            return this;
        }

        /// <summary>
        /// Make ERROR response
        /// </summary>
        /// <param name="error">Error content (default is "")</param>
        /// <param name="status">OK status (default is 200 (OK))</param>
        public AgResponse MakeErrorResponse(string error = "", int status = 500)
        {
            Clear();
            SetBegin(status);
            SetHeader("Content-Type", "text/html; charset=UTF-8");
            SetData(error);
            return this;
        }

        /// <summary>
        /// Make HEAD response
        /// </summary>
        public AgResponse MakeHeadResponse()
        {
            Clear();
            SetBegin(200);
            SetHeader("Content-Type", "text/html; charset=UTF-8");
            SetData();
            return this;
        }

        /// <summary>
        /// Make GET response
        /// </summary>
        /// <param name="Data">Data string content (default is "")</param>
        public AgResponse MakeGetResponse(string Data = "")
        {
            Clear();
            SetBegin(200);
            SetHeader("Content-Type", "text/html; charset=UTF-8");
            SetData(Data);
            return this;
        }

        /// <summary>
        /// Make GET response
        /// </summary>
        /// <param name="Data">Data binary content</param>
        public AgResponse MakeGetResponse(byte[] Data)
        {
            Clear();
            SetBegin(200);
            SetHeader("Content-Type", "text/html; charset=UTF-8");
            SetData(Data);
            return this;
        }

        /// <summary>
        /// Make GET response
        /// </summary>
        /// <param name="Data">Data buffer content</param>
        public AgResponse MakeGetResponse(Buffer Data)
        {
            Clear();
            SetBegin(200);
            SetHeader("Content-Type", "text/html; charset=UTF-8");
            SetData(Data);
            return this;
        }

        /// <summary>
        /// Make OPTIONS response
        /// </summary>
        /// <param name="allow">Allow methods (default is "HEAD,GET,POST,PUT,DELETE,OPTIONS,TRACE")</param>
        public AgResponse MakeOptionsResponse(string allow = "HEAD,GET,SAVE, UPDATE,UPLOAD,DOWNLOAD,RIGHTS,SHARE,PUT,DELETE,OPTIONS,TRACE")
        {
            Clear();
            SetBegin(200);
            SetHeader("Allow", allow);
            SetData();
            return this;
        }

        /// <summary>
        /// Make TRACE response
        /// </summary>
        /// <param name="request">Request string content</param>
        public AgResponse MakeTraceResponse(string request)
        {
            Clear();
            SetBegin(200);
            SetHeader("Content-Type", "message/Ag");
            SetData(request);
            return this;
        }

        /// <summary>
        /// Make TRACE response
        /// </summary>
        /// <param name="request">Request binary content</param>
        public AgResponse MakeTraceResponse(byte[] request)
        {
            Clear();
            SetBegin(200);
            SetHeader("Content-Type", "message/Ag");
            SetData(request);
            return this;
        }

        /// <summary>
        /// Make TRACE response
        /// </summary>
        /// <param name="request">Request buffer content</param>
        public AgResponse MakeTraceResponse(Buffer request)
        {
            Clear();
            SetBegin(200);
            SetHeader("Content-Type", "message/Ag");
            SetData(request);
            return this;
        }

        // Ag response status phrase
        private string _statusPhrase;
        // Ag response protocol
        private string _protocol;
        // Ag response headers
        private List<Tuple<string, string>> _headers = new List<Tuple<string, string>>();
        // Ag response Data
        private int _DataIndex;
        private int _DataSize;
        private int _DataLength;
        private bool _DataLengthProvided;

        // Ag response cache
        private Buffer _cache = new Buffer();
        private int _cacheSize;

        // Is pending parts of Ag response
        internal bool IsPendingHeader()
        {
            return (!IsErrorSet && (_DataIndex == 0));
        }
        internal bool IsPendingData()
        {
            return (!IsErrorSet && (_DataIndex > 0) && (_DataSize > 0));
        }

        // Receive parts of Ag response
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

                    // Parse protocol version
                    int protocolIndex = index;
                    int protocolSize = 0;
                    while (_cache[index] != ' ')
                    {
                        ++protocolSize;
                        ++index;
                        if (index >= (int)_cache.Size)
                            return false;
                    }
                    ++index;
                    if ((index >= (int)_cache.Size))
                        return false;
                    _protocol = _cache.ExtractString(protocolIndex, protocolSize);

                    // Parse status code
                    int statusIndex = index;
                    int statusSize = 0;
                    while (_cache[index] != ' ')
                    {
                        if ((_cache[index] < '0') || (_cache[index] > '9'))
                            return false;
                        ++statusSize;
                        ++index;
                        if (index >= (int)_cache.Size)
                            return false;
                    }
                    Status = 0;
                    for (int j = statusIndex; j < (statusIndex + statusSize); ++j)
                    {
                        Status *= 10;
                        Status += _cache[j] - '0';
                    }
                    ++index;
                    if (index >= (int)_cache.Size)
                        return false;

                    // Parse status phrase
                    int statusPhraseIndex = index;
                    int statusPhraseSize = 0;
                    while (_cache[index] != '\r')
                    {
                        ++statusPhraseSize;
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
                    _statusPhrase = _cache.ExtractString(statusPhraseIndex, statusPhraseSize);

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
