using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Cdmx.Scg.Sso.Auth.Models.ViewModels
{
    public class CambiaPassRecuperadoVM
    {
        public string token { get; set; }
        public string Password { get; set; }
        public string Password2 { get; set; }
    }
}