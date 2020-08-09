using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AppV.Models
{
    public class People : Identity
    {

        public long Id { get; set; }

        // Main data, for grid purposes
        public PeoplePaper Paper { get; set; }  
        public ContatctItem ContactData { get; set; }
        public Address Address { get; set; }

        // multiple itens for complete information
        public List<Contact> Contacts { get; set; }
        public List<PeoplePaper> Papers { get; set; }
        
        public Object Image { get; set; }
        public Boolean Active { get; set; } = true;

        public People ()
        {

            Address address = new Address();
            Address = address;

            PeoplePaper paper = new PeoplePaper();
            Paper = paper;

            ContatctItem contactData = new ContatctItem();
            ContactData = contactData;

            List<Contact> contacts = new List<Contact>();
            Contacts = contacts;

            List<PeoplePaper> papers = new List<PeoplePaper>();
            Papers = papers;

        }

    }

}
