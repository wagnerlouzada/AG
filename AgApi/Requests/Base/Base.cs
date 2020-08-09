using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Models.Requests.Base
{
    public class ResponseBase
    {
        public Boolean success { get; set; }
        public String message { get; set; }
        public List<Error> errors { get; set; }
    }

    public class Error
    {
        public string key { get; set; }
        public string message { get; set; }
    }
}