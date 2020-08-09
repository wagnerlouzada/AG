using AppV.Interfaces;
using AppV.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using DB;
using Models;

namespace AppV.DataAccess
{

    public class MsAccessPeople
    {
        public long Id { get; set; }
        public String Nome { get; set; }
        public String Documento { get; set; }
        public String Telefone { get; set; }
        public String Endereco { get; set; }
        public String Imagem { get; set; }
        public Boolean Ativo { get; set; }

        public MsAccessPeople()
        {
        }

        public MsAccessPeople(People People)
        {
            FromPeople(People);
        }

        public MsAccessPeople FromPeople(People From)
        {

            if (From != null)
            {
                try
                {
                    this.Id = From.Id;
                }
                catch
                {
                    this.Id = 0;
                }
                this.Imagem = (String)From.Image;
                this.Nome = From.Name;
                this.Documento = From.Paper.Description;
                this.Endereco = From.FullAddress();

                foreach (var Contact in From.Contacts)
                {
                    if (Contact.ContactData.Type == contactType.Celular || Contact.ContactData.Type == contactType.Phone)
                    {
                        this.Telefone = Contact.ContactData.Value;
                        break;
                    }
                }
            }

            return this;
        }

        public People ToPeople()
        {

            if (this != null)
            {
                People people = new People();

                people.Id = Convert.ToUInt32(this.Id);
                people.Image = this.Imagem;
                people.Name = this.Nome;
                people.Address.Street = this.Endereco;
                people.Paper.Description = this.Documento;
                people.Contacts = new List<Contact>();
                if (this.Telefone !=null) {
                    Contact Phone = new Contact();
                    Phone.ContactData.Type = contactType.Phone;
                    Phone.ContactData.Value = this.Telefone;
                    people.Contacts.Add(Phone);
                    if (people.ContactData.Value==null) { people.ContactData.Value = this.Telefone;  }
                }
                return people;
            }

            return null;
        }

    }

    public class MsAccessDbPeople : IPeople
    {

        public String MapFields(String Field)
        {
            switch (Field.ToLower())
            {
                case "id":
                    return "Id";
                case "image":
                    return "Imagem";
                case "name":
                    return "Nome";
                case "address":
                    return "Endereco";
                case "paper":
                    return "Documento";
                case "phone":
                    return "Telefone";
                default:
                    return "";
            }
        }

        public List<People> ToPeople(List<MsAccessPeople> From)
        {
            List<People> Peoples = new List<People>();
            foreach (MsAccessPeople pessoa in From)
            {
                Peoples.Add(pessoa.ToPeople());
            }
            return Peoples;
        }

        public List<People> GetPeoples(List<DbFilter> Filter)
        {

            List<MsAccessPeople> Pessoas = new List<MsAccessPeople>();
            DBHelper pessoa = new DBHelper("MsAccess", "Pessoa", "Id", "", typeof(MsAccessPeople));
            pessoa.NoLock = false;
            if (Filter != null && Filter.Count() > 0)
            {
                foreach (DbFilter Flt in Filter)
                {
                    string fld = MapFields(Flt.FieldName);
                    if (fld != "")
                    {
                        pessoa.AddWhere(fld, Flt.Operator, Flt.Value);
                    }
                }
            }
            Pessoas = pessoa.Select<MsAccessPeople>(true);

            return ToPeople(Pessoas);

        }

        public People GetPeople(object Id)
        {

            List<MsAccessPeople> ppl = new List<MsAccessPeople>();
            DBHelper PPL = new DBHelper("MsAccess", "Pessoa", "Id", "", typeof(MsAccessPeople));
            PPL.NoLock = false;
            PPL.AddWhere("Id", DbOperator.Equal, Id);
            ppl = PPL.Select<MsAccessPeople>(true);

            // localizar os contatos
            //DBHelper PPLContacts = new DBHelper("MsAccess", "Pessoa", "Id", "", typeof(MsAccessPeople));

            // localizar os documentos

            return ToPeople(ppl).First();

        }

        public People DeletePeople(People People)
        {

            MsAccessPeople pessoa = new MsAccessPeople(People);
            DBHelper pessoaDb = new DBHelper("MsAccess", "Pessoa", "Id", "", typeof(MsAccessPeople));
            pessoaDb.NoLock = false;
            pessoaDb.PkField = People.Id.ToString();
            List<MsAccessPeople> Pessoas = pessoaDb.Select<MsAccessPeople>(true).ToList();
            if (Pessoas.Count() > 0)
            {           
                pessoa.Ativo = false;
                long PessoaId = pessoaDb.Update<MsAccessPeople>(pessoa, true, false);
                People.Active = false;
                return People;
            }
            return null;

        }

        public People SavePeople(People People)
        {

            long PessoaId = 0;

            if (People != null)
            {
                MsAccessPeople Pessoa = new MsAccessPeople(People);

                DBHelper pessoaDb = new DBHelper("MsAccess", "Pessoa", "Id", "", typeof(MsAccessPeople));
                pessoaDb.NoLock = false;

                pessoaDb.AditionalUpdateEndClause = "WITH OWNERACCESS OPTION";
                pessoaDb.AditionalInsertEndClause = "WITH OWNERACCESS OPTION";

                pessoaDb.PkValue = Pessoa.Id.ToString();
                pessoaDb.AddWhere("Id", DbOperator.Equal, Pessoa.Id);

                List<MsAccessPeople> pessoas = pessoaDb.Select<MsAccessPeople>(true).ToList();
                if (pessoas.Count() > 0)
                {
                    PessoaId = pessoaDb.Update<MsAccessPeople>(Pessoa, true, false);
                    People.Id = PessoaId;
                }
                else
                {
                    PessoaId = pessoaDb.Insert<MsAccessPeople>(Pessoa);
                    People.Id = PessoaId;
                }
                
            }

            return People;

        }

    }

}
