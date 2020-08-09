using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AppV.Models
{

    public enum DeviceType
    {
        Cam,
        Gate,
        Door
    }

    public class Device
    {
        public long Id { get; set; }
        public DeviceType Type { get; set; }
        public String Description { get; set; }
    }
}
