using MediatR;
using Models.Requests.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using AppV.Models;
using CustomExtensions;

namespace Models.Requests.Controllers
{

    public class PeopleListRequest : IRequest<PeopleListResponse>
    {
        public String filter { get; set; }
    }

    public class PeopleListResponse : ResponseBase
    {        
        public List<AppV.Models.People> peoples { get; set; }

        public PeopleListResponse()
        {
            List<AppV.Models.People> peoples = new List<People>();
            this.peoples = peoples;
        }
    }

}