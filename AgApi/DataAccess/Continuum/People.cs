using AppV.Interfaces;
using AppV.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using DB;
using Models;

namespace AppV.DataAccess
{

    public class ContinuumPeople
    {
        public String ObjectIdHi { get; set; }
        public String ObjectIdLo { get; set; }
        public string uiName { get; set; }
        public int OwnerIdHi { get; set; }
        public int OwnerIdLo { get; set; }
        public int DeviceIdHi { get; set; }
        public int DeviceIdLo { get; set; }
        public bool TemplateFlag { get; set; }
        public int TemplateIdHi { get; set; }
        public int TemplateIdLo { get; set; }
        public string ControllerName { get; set; }
        public int AlarmGraphicPage { get; set; }
        public DateTime ActivationDate { get; set; }
        public DateTime SavedActivationDate { get; set; }
        public bool ADA { get; set; }
        public string Address { get; set; }
        public bool AllowEntEntEgr { get; set; }
        public string Blood { get; set; }
        public int CardType { get; set; }
        public int CardType2 { get; set; }
        public int SiteCode { get; set; }
        public int SiteCode2 { get; set; }
        public object CardNumber { get; set; }
        public object CardNumber2 { get; set; }
        public int SavedCardType { get; set; }
        public string City { get; set; }
        public string Country { get; set; }
        public string CustomControl1 { get; set; }
        public string CustomControl2 { get; set; }
        public string CustomControl3 { get; set; }
        public DateTime DateOfBirth { get; set; }
        public bool DeletePending { get; set; }
        public string Department { get; set; }
        public float DepartmentCode { get; set; }
        public bool DistFailed { get; set; }
        public bool Duress { get; set; }
        public string EmergencyContact { get; set; }
        public string EmergencyPhone { get; set; }
        public string EmpNumber { get; set; }
        public bool EntryEgress { get; set; }
        public DateTime ExpirationDate { get; set; }
        public DateTime SavedExpirationDate { get; set; }
        public string EyeColor { get; set; }
        public string FirstName { get; set; }
        public string HairColor { get; set; }
        public string Height { get; set; }
        public string HomePhone { get; set; }
        public int InactiveDisableDays { get; set; }
        public string Info1 { get; set; }
        public string Info2 { get; set; }
        public string Info3 { get; set; }
        public string Info4 { get; set; }
        public string Info5 { get; set; }
        public string Info6 { get; set; }
        public string JobTitle { get; set; }
        public string LastName { get; set; }
        public string LicenseNumber { get; set; }
        public bool LostCard { get; set; }
        public string MiddleName { get; set; }
        public string OfficeLocation { get; set; }
        public string ParkingSticker { get; set; }
        public string PhotoFile { get; set; }
        public int PIN { get; set; }
        public int SavedPIN { get; set; }
        public object SavedCardNumber { get; set; }
        public int SavedSiteCode { get; set; }
        public int Sex { get; set; }
        public string Signature { get; set; }
        public string SocSecNo { get; set; }
        public DateTime StartDate { get; set; }
        public int State { get; set; }
        public int SavedState { get; set; }
        public string StateOfResidence { get; set; }
        public string Supervisor { get; set; }
        public DateTime TimeEntered { get; set; }
        public int ValueHi { get; set; }
        public int ValueLo { get; set; }
        public string VehicalInfo { get; set; }
        public bool Visitor { get; set; }
        public int Weight { get; set; }
        public string WorkPhone { get; set; }
        public string Zip { get; set; }
        public int Zone { get; set; }
        public int ZonePointHi { get; set; }
        public int ZonePointLo { get; set; }
        public int NonABACardNumber { get; set; }
        public int NonABACardNumber2 { get; set; }
        public string BLOB_Template { get; set; }
        public bool ExecutivePrivilege { get; set; }
        public int DefaultClearanceLevel { get; set; }
        public int FipsAgencyCode { get; set; }
        public int FipsOrgId { get; set; }
        public int FipsHmac { get; set; }
        public int FipsSystemCode { get; set; }
        public int FipsCredentialNumber { get; set; }
        public object FipsPersonId { get; set; }
        public int FipsCredentialSeries { get; set; }
        public int FipsCredentialIssue { get; set; }
        public int FipsOrgCategory { get; set; }
        public int FipsPersonOrgAssociation { get; set; }
        public DateTime FipsExpirationDate { get; set; }
        public bool FipsPivControlled { get; set; }
        public int FipsPivState { get; set; }
        public int SavedCardType2 { get; set; }
        public object SavedCardNumber2 { get; set; }
        public int SavedSiteCode2 { get; set; }

        public ContinuumPeople()
        {
        }

        public ContinuumPeople(People People)
        {
            FromPeople(People);
        }

        public ContinuumPeople FromPeople(People From)
        {

            if (From != null)
            {
                this.ObjectIdLo = From.Id.ToString();
                this.PhotoFile = (String)From.Image;
                this.uiName = From.Name;
                this.SocSecNo = From.Paper.Description;
                this.Address = From.FullAddress();

                foreach (var Contact in From.Contacts)
                {
                    if (Contact.ContactData.Type == contactType.Celular || Contact.ContactData.Type == contactType.Phone)
                    {
                        this.HomePhone = Contact.ContactData.Value;
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

                people.Id = Convert.ToUInt32( this.ObjectIdLo);
                people.Image = this.PhotoFile;
                people.Name = this.uiName!=null ? this.uiName.Replace("_"," ") : "";

                people.FirstName = this.FirstName != null ? this.FirstName.Replace("_", " ") : "";
                people.LastName = this.LastName != null ? this.LastName.Replace("_", " ") : "";

                people.Address.Street = this.Address !=null ? this.Address.Replace("_", " ") : "";
                people.Paper.Description = this.SocSecNo!=null ? this.SocSecNo.Replace("_", " ") : "";

                people.Contacts = new List<Contact>();
                if (this.HomePhone !=null) {
                    Contact Phone = new Contact();
                    Phone.ContactData.Type = contactType.Phone;
                    Phone.ContactData.Value = this.HomePhone.Replace("_", " ");
                    people.Contacts.Add(Phone);
                }
                return people;
            }

            return null;
        }

    }

    public class ContinuumDbPeople : IPeople
    {

        public String MapFields(String Field)
        {
            switch (Field.ToLower())
            {
                case "id":
                    return "ObjectIdLo";
                case "image":
                    return "PhotoFile";
                case "name":
                    return "uiName";
                case "firstname":
                    return "FirstName";
                case "lastname":
                    return "LastName";
                case "address":
                    return "Address";
                case "paper":
                    return "SocSecNo";
                //case "phone":
                //    return "Telefone";
                default:
                    return "";
            }
        }

        public List<People> ToPeople(List<ContinuumPeople> From)
        {
            List<People> Peoples = new List<People>();
            foreach (ContinuumPeople pessoa in From)
            {
                Peoples.Add(pessoa.ToPeople());
            }
            return Peoples;
        }

        public List<People> GetPeoples(List<DbFilter> Filter)
        {
            List<ContinuumPeople> Pessoas = new List<ContinuumPeople>();
            DBHelper pessoa = new DBHelper("Continuum", "Personnel", "ObjectIdLo", "", typeof(ContinuumPeople));
            pessoa.Top = 50;
            pessoa.TopMode = DbTopMode.Tuplas;
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
            Pessoas = pessoa.Select<ContinuumPeople>(true);
            return ToPeople(Pessoas);
        }

        public People GetPeople(object ID)
        {
           
            return null;
        }

        public People DeletePeople(People People)
        {
            ContinuumPeople pessoa = new ContinuumPeople(People);
            DBHelper pessoaDb = new DBHelper("Continuum", "Personnnel", "ObjectIdLo", "", typeof(ContinuumPeople));
            pessoaDb.PkField = People.Id.ToString();
            List<ContinuumPeople> Pessoas = pessoaDb.Select<ContinuumPeople>(true).ToList();
            if (Pessoas.Count() > 0)
            {           
                //pessoa.ActivationDate = false;
                long PessoaId = pessoaDb.Update<ContinuumPeople>(pessoa, true, false);
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
                ContinuumPeople Pessoa = new ContinuumPeople(People);

                DBHelper pessoaDb = new DBHelper("Continuum", "Personnel", "ObjectIdLo", "", typeof(ContinuumPeople));
                pessoaDb.PkField = Pessoa.ObjectIdLo.ToString();
                List<ContinuumPeople> pessoas = pessoaDb.Select<ContinuumPeople>(true).ToList();
                if (pessoas.Count() > 0)
                {
                    PessoaId = pessoaDb.Update<ContinuumPeople>(Pessoa, true, false);
                    People.Id = PessoaId;
                }
                else
                {
                    PessoaId = pessoaDb.Insert<ContinuumPeople>(Pessoa);
                    People.Id = PessoaId;
                }
                
            }
            return People;

        }

    }
}
