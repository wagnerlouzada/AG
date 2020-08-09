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

    public class DeletePeopleHandler : IRequestHandler<DeletePeopleRequest, DeletePeopleResponse>
    {

        // msaccessdbpeople ... Startup.DATABASE ... PEOPLE
        public object CreateInstance(string modelPrefix, string modelItem)
        {


            // msaccess
            string className = "AppV.DataAccess." + Startup.DATABASE + "Db" + modelItem;
            var result = System.Reflection.Assembly.GetExecutingAssembly().CreateInstance(className);

            return result;

        }

        public Task<DeletePeopleResponse> Handle(DeletePeopleRequest request, CancellationToken cancellationToken)
        {

            DeletePeopleResponse response = new DeletePeopleResponse();

            // mocking

            response.people.Street = "address fake";
            response.people.Name = "name fake";
            response.people.ContactData.Value = "one or other fake phone";
            response.people.Paper.Description = "another fake data for papers";

            AppV.Models.People ppl = new AppV.Models.People();
            // AppV.DataAccess.MsAccessDbPeople dtPeoples = new AppV.DataAccess.MsAccessDbPeople();

            IPeople dtPeoples = (IPeople)CreateInstance(Startup.DATABASE, "People");

            request.people.Active = false;
            ppl = dtPeoples.SavePeople(request.people);
            response.people = ppl;
            // --------------------------------------------------
            //#endif
            response.success = true;
            response.message = "";

            return Task.FromResult(response);
        }
    }
}
