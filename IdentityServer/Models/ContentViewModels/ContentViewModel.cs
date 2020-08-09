using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Identity.IdentityServer.Models.ManageViewModels
{
    public class ContentViewModel
    {
        [Required]
        [DataType(DataType.Text)]
        [Display(Name = "ContentName")]
        public string Name { get; set; }

        [Required]
        [DataType(DataType.Text)]
        [Display(Name = "ContentLanguage")]
        public string Language { get; set; }

        [Required]
        [DataType(DataType.Text)]
        [Display(Name = "ContentHtml")]
        public string Html { get; set; }

        [Required]
        [DataType(DataType.Text)]
        [Display(Name = "ContentUrl")]
        public string Url { get; set; }

        public Boolean Status { get; set; }
    }
}
