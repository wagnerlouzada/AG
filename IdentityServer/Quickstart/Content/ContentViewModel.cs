using System;
using System.Collections.Generic;

namespace IdentityServer4.Quickstart.UI
{
    public class ContentViewModel
    {
        public IEnumerable<ContentViewModel> Contents { get; set; }
    }

    public class ConentViewModel
    {
        public string Name { get; set; }
        public string Html { get; set; }
        public string Url { get; set; }
        public string Status { get; set; }
    }
}
