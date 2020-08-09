using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AppV.Models
{
    public enum LinkType
    {
        Dweller,
        Employee,
        Visitor
    }

    public class Link
    {
        public long Id { get; set;  }
        public People People { get; set; }
        public Company Company { get; set; }
        public Credential Credential { get; set; }
        public LinkType Type { get; set; }
        public DateTime Start { get; set; }
        public DateTime Finish { get; set; }
        public Boolean Active { get; set; }

        public Link ()
        {
            People people = new People();
            People = people;

            Company company = new Company();
            Company = company;

            Credential credential = new Credential();
            Credential = credential;
        }
    }
}
