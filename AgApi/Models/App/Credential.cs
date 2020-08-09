using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AppV.Models
{
    public class Credential
    {
        public long Id { get; set; }
        public String Code { get; set; }
        public Boolean Active { get; set; } = true;
    }

}
