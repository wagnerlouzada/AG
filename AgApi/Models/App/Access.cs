using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AppV.Models
{
    public class Access
    {
        public String Id { get; set; }
        public People People { get; set; }
        public Company Company { get; set; }
        public People ReferencePeople { get; set; }
        public DateTime Start { get; set; }
        public DateTime Finish { get; set; }
        public DateTime EstimatedFinish { get; set; }
        public Link Credential { get; set; }
        public Credential ActiveCredential { get; set; }
        public Boolean Closed { get; set; } = false;

        public Access ()
        {
            People people = new People();
            People = people;

            Company company = new Company();
            Company = company;

            People referencePeople = new People();
            ReferencePeople = referencePeople;

            Link credential = new Link();
            Credential = credential;

            Credential activeCredential = new Credential();
            ActiveCredential = activeCredential;
        }

    }

}
