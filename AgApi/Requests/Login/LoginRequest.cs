using MediatR;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Security.Claims;
using System.Web;

namespace AppV.Models
{
    public class LoginModel: IRequest<LoginResponse>
    {
        [Required]
        [Display(Name = "Nome do usuário")]
        public string UserName { get; set; }
        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Senha")]
        public string Password { get; set; }

        public String RefreshToken { get; set; }
        public String ErrorMessage { get; set; }
    }


    public class LoginResponse
    {
        public Boolean Sucesso { get; set; }
        public String[] Erros { get; set; }
        public ClaimsIdentity resultData { get; set; }
        public uint ExpiresIN { get; set; }
    }
}