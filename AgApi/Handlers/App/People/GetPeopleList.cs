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
using Microsoft.Extensions.Options;
using AppV;
using AppV.Interfaces;

namespace Models.CQRS.Handlers.Controllers
{

    public class GetPeopleListHandler : IRequestHandler<PeopleListRequest, PeopleListResponse>
    {

        private readonly IOptions<AppV.Models.ConfigurationString> _appSettings;

        public Task<PeopleListResponse> Handle(PeopleListRequest request, CancellationToken cancellationToken)
        {

            PeopleListResponse response = new PeopleListResponse();



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


            List<AppV.Models.People> ppls = new List<AppV.Models.People>();
            // AppV.DataAccess.MsAccessDbPeople dtPeoples = new AppV.DataAccess.MsAccessDbPeople();

            IPeople dtPeoples = (IPeople)CreateInstance(Startup.DATABASE, "People");

            ppls = dtPeoples.GetPeoples(null);
            response.peoples.AddRange(ppls);
            // --------------------------------------------------
//#endif
            response.success= true;
            response.message = "";

            return Task.FromResult(response);

        }

        // msaccessdbpeople ... Startup.DATABASE ... PEOPLE
        public object CreateInstance(string modelPrefix, string modelItem)
        {
            

            // msaccess
            string className = "AppV.DataAccess." + Startup.DATABASE + "Db" + modelItem;
            var result = System.Reflection.Assembly.GetExecutingAssembly().CreateInstance(className);

            return result;

        }


        //public object GetInstance(string strFullyQualifiedName)
        //{
        //    Type type = Type.GetType(strFullyQualifiedName);
        //    if (type != null)
        //        return Activator.CreateInstance(type);
        //    foreach (var asm in AppDomain.CurrentDomain.GetAssemblies())
        //    {
        //        type = asm.GetType(strFullyQualifiedName);
        //        if (type != null)
        //            return Activator.CreateInstance(type);
        //    }
        //    return null;
        //}


    }
}