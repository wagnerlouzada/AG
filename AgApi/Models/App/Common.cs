using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AppV.Models
{

    public enum companyType
    {
       Condominio,
       Condomino,
       Prestador
    }

    public enum contactType
    {
        Phone,
        Celular,
        Email,
        Telegram,
        Skype,
        WhatsApp,
        Facebook,
        LinkedIn,
        Messenger
    }

    public enum peoplePaperType
    {
        Rg,
        Cpf,
        TE
    }

    public enum companyPaperType
    {
        Cnpj,
        IE,
        IM
    }

    public class Contact
    {
        public long Id { get; set; }
        public ContatctItem ContactData { get; set; }
        public Boolean Main { get; set; } = false;

        public Contact ()
        {
            ContatctItem contactData = new ContatctItem();
            ContactData = contactData;
        }
    }

    public class ContatctItem
    {
        public contactType Type { get; set; }
        public String Value { get; set; }
        public Boolean Main { get; set; } = false;
    }

    public class PeoplePaper
    {
        public peoplePaperType Type { get; set; }
        public String Description { get; set; }
        public String Extra { get; set; }
        public Boolean Main { get; set; } = false;
    }

    public class CompanyPaper
    {
        public companyPaperType Type { get; set; }
        public String Description { get; set; }
        public String Extra { get; set; }
        public Boolean Main { get; set; } = false;
    }

    public class ConpanyType
    {
        public long Id { get; set; }
        public companyType Type { get; set; }
        public Boolean Main { get; set; } = false;
    }

    public class Address
    {

        public String FullAddress()
        {
            return Street + 
                            (Street == "" ? "" : " ") + 
                            Number + (Number == "" ? "" : " ") + 
                            Complement + (Complement == "" ? "" : " ") + 
                            City + (City == "" ? "" : " ") + 
                            State + (State == "" ? "" : " ") +
                            Neighborhood;
        }
        public String Street { get; set; }
        public String Number { get; set; }
        public String Complement { get; set; }
        public String State { get; set; }
        public String City { get; set; }
        public String Neighborhood { get; set; }
        public Boolean Main { get; set; } = false;
    }

    public class Identity : Address
    {
        public String Name { get; set; } 
        public String FirstName { get; set; }
        public String LastName { get; set; }
    }


}
