using MediatR;
using Models.Requests.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using AppV.Models;


namespace Models.Requests.Controllers
{

    public class DeletePeopleRequest : IRequest<DeletePeopleResponse>
    {
        public People people { get; set; }
    }

    public class DeletePeopleResponse : ResponseBase
    {

        public People people { get; set; }

        // only for create a empty container for accesses
        public DeletePeopleResponse()
        {

            People people = new People();
            this.people = people;

        }
    }

}
