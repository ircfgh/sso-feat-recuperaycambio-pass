using Cdmx.Scg.Sso.Auth.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Web.Http;

namespace Cdmx.Scg.Sso.Auth.Controllers
{
    public class CambiarPassController : ApiController
    {
        [HttpPost]
        public IHttpActionResult CambiarPass(Models.ViewModels.CambiaPassVM model)

        {

            int mensaje = 0;
            using (var bd = new DbContextAuth())
            {
                {
                    var oCambiaPass = bd.Usuario.Find(model.Iduser);

                    if (oCambiaPass.Clave == model.OldPassword)
                    {
                        oCambiaPass.Clave = model.NewPassword;
                        bd.SaveChanges();
                        mensaje = 1;
                    }
                    else
                    {
                        mensaje = 0;
                    }
                }
                return Ok(mensaje);
            }
        }

        public static string GetMD5(string str)
        {
            MD5 md5 = MD5CryptoServiceProvider.Create();
            ASCIIEncoding encoding = new ASCIIEncoding();
            byte[] stream = null;
            StringBuilder sb = new StringBuilder();
            stream = md5.ComputeHash(encoding.GetBytes(str));
            for (int i = 0; i < stream.Length; i++) sb.AppendFormat("{0:x2}", stream[i]);
            return sb.ToString();
        }
    }
}