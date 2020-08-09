using MediatR;
using Models.Requests.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using AppV.Models;


namespace Models.Requests.Controllers
{

    public class PeopleRequest : IRequest<PeopleResponse>
    {
        public int id { get; set; }
    }

    public class PeopleResponse : ResponseBase
    {

        public int id { get; set; }
        public People people { get; set; }
        public List<Access> accesses { get; set; }

        // only for create a empty container for accesses
        public PeopleResponse()
        {

            People people = new People();
            this.people = people;

            List<Access> accesses = new List<Access>();
            this.accesses = accesses;


        }
    }

}