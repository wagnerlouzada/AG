using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AppV.Models
{
    public class Token
    {
        public string access_token { get; set; }

        public string token_type { get; set; }

        public uint expires_in { get; set; }

        public DateTime issued { get; set; }

        public DateTime expires { get; set; }

        public string RefreshToken { get; set; }

        public Int32 uid { get; set; }

        public Int32 gid { get; set; }

        public string userName { get; set; }

        public string Nome { get; set; }

        public string Sobrenome { get; set; }

        public string Email { get; set; } 

        public string Roles { get; set; }
    }
}