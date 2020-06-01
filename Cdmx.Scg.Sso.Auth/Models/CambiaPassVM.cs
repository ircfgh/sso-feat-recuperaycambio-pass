using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Cdmx.Scg.Sso.Auth.Models.ViewModels
{
    public class CambiaPassVM
    {
        public int Iduser { get; set; }
        public string OldPassword { get; set; }
        public string NewPassword { get; set; }
    }
}