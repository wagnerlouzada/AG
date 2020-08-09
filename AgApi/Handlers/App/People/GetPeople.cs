using DB;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Collections;
using Models.Requests.Controllers;
using AppV.Interfaces;
using AppV;

namespace Models.CQRS.Handlers.Controllers
{

    public class GetPeopleHandler : IRequestHandler<PeopleRequest, PeopleResponse>
    {

        // msaccessdbpeople ... Startup.DATABASE ... PEOPLE
        public object CreateInstance(string modelPrefix, string modelItem)
        {


            // msaccess
            string className = "AppV.DataAccess." + Startup.DATABASE + "Db" + modelItem;
            var result = System.Reflection.Assembly.GetExecutingAssembly().CreateInstance(className);

            return result;

        }

        public Task<PeopleResponse> Handle(PeopleRequest request, CancellationToken cancellationToken)
        {

            PeopleResponse response = new PeopleResponse();

            // mocking

            response.people.Street = "address fake";
            response.people.Name = "name fake";
            response.people.ContactData.Value = "one or other fake phone";
            response.people.Paper.Description = "another fake data for papers";

            // --------------------------------------------------



            // mocking

            //#if DEBUG

            //            AppV.Models.People people = new AppV.Models.People();

            //            people.Name = "name fake";
            //            people.Paper = "another fake data for papers";
            //            people.Address = "address fake";

            //            response.peoples.Add(people);

            //            AppV.Models.People people2 = new AppV.Models.People();
            //            people2.Name = "name fake 2";
            //            people2.Paper = "anotherv2 fake data for papers";
            //            people2.Address = "2 address fake";

            //            response.peoples.Add(people2);
            //#else
            // mock end


            AppV.Models.People ppl = new AppV.Models.People();
            // AppV.DataAccess.MsAccessDbPeople dtPeoples = new AppV.DataAccess.MsAccessDbPeople();

            IPeople dtPeoples = (IPeople)CreateInstance(Startup.DATABASE, "People");

            ppl = dtPeoples.GetPeople(request.id);
            response.people = ppl;
            // --------------------------------------------------
            //#endif
            response.success = true;
            response.message = "";

            return Task.FromResult(response);
        }
    }
}