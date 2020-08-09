using AppV.Models;
using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AppV.Interfaces
{
    interface IPeople
    {
        People GetPeople(object ID);
        List<People> GetPeoples(List<DbFilter> Filter);
        People SavePeople(People People);
        People DeletePeople(People People);       
    }

    interface ICompany
    {
        List<Company> GetCompanies(List<DbFilter> Filter);
        ICompany SaveCompany(Company Company);
        ICompany DeleteCompany(Company Company);
    }

    interface IActivity
    {
        List<Access> GetActivities(People People, List<DbFilter> Filter = null);
        List<Access> GetActivities(Company Company, List<DbFilter> Filter = null);
        Access SaveActivity(Access Activity);
        Access FinishActivity(Access Activity);
    }
}
