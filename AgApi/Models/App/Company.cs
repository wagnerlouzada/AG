using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AppV.Models
{
    public class Company : Identity
    {

        public long Id { get; set; }

        public People Responsible { get; set; }
        public CompanyPaper Paper { get; set; }
        public ContatctItem ContactData { get; set; }
        public List<Contact> Contacts { get; set; }
        public Address Address { get; set; }
        public Boolean Active { get; set; } = true;
        public companyType Type { get; set; }
        public List<companyType> Types { get; set; }

        public Company ()
        {
            People responsible = new People();
            Responsible = responsible;

            CompanyPaper paper = new CompanyPaper();
            Paper = paper;

            ContatctItem contactData = new ContatctItem();
            ContactData = contactData;

            Address address = new Address();
            Address = address;

            companyType type = new companyType();
            Type = type;

            List<Contact> contacts = new List<Contact>();
            Contacts = contacts;

            List<companyType> types = new List<companyType>();
            Types = types;


        }

    }

}
