using System;
using System.Linq;
using System.Net.Mail;
using System.Security.Cryptography;
using System.Text;
using System.Web.Http;
using Cdmx.Scg.Sso.Auth.Models;

namespace Cdmx.Scg.Sso.Auth.Controllers
{
    public class RecuperaPassController : ApiController
    {
        string urlDomain = "http://localhost:49915";
        [HttpPost]
        public IHttpActionResult RecuperaPass(Models.ViewModels.RecuperaPassMV model)
        //public IHttpActionResult RecuperaPass(int id)
        {
            var correo = "";
            using (Models.DbContextAuth db = new DbContextAuth());
            {
                var oRecuperaPass = new Models.Usuario();
                correo = model.Email;
            }
            int existe = 0;
            int idusuario = 0;
            int mensaje = 0;
            string pass = "";
            using (var bd = new DbContextAuth())
            {
                //existe = bd.Usuarios.Where(p => p.IdUsuario == id).Count();
                existe = bd.Usuario.Where(p => p.Mail == correo).Count();
                if (existe == 0) pass = "";
                else
                {
                    //idusuario = bd.Usuarios.Where(p => p.IdUsuario == id).First().IdUsuario;
                    idusuario = bd.Usuario.Where(p => p.Mail == correo).First().IdUsuario;
                    //Verificamos que existe el usuario
                    int nveces = bd.Usuario.Where(p => p.IdUsuario == idusuario).Count();
                    if (nveces == 0)
                    {
                        pass = "";
                        mensaje = 0;
                    }
                    else
                    {
                        // obtener su ID
                        Usuario ouser = bd.Usuario.Where(p => p.IdUsuario == idusuario).First();
                        pass = ouser.Clave;
                        mensaje = 1;
                    }
                    if (mensaje == 1)
                    {
                        string token = GetMD5(Guid.NewGuid().ToString());

                        using (Models.DbContextAuth db = new Models.DbContextAuth())
                        {
                            var oUser = db.Usuario.Where(d => d.Mail == model.Email).FirstOrDefault();
                            if (oUser != null)
                            {
                                oUser.token_recovery = token;
                                db.Entry(oUser).State = System.Data.Entity.EntityState.Modified;
                                db.SaveChanges();

                                //enviar mail
                                Correo(correo, token);
                            }
                        }
                        
                    }
                    else
                    {
                        mensaje = 0;
                    };
                }
                return Ok(mensaje);
            }

        }
    
    #region HELPERS

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

    private void Correo(string EmailDestino, string token)
    {
        string EmailOrigen = "rcfgh@hotmail.com";
        string Contraseña = "rebcon2019";
        string url = urlDomain + "/Cuenta/Recovery/?token=" + token;
        MailMessage oMailMessage = new MailMessage(EmailOrigen, EmailDestino, "Recuperación de contraseña", "<p>Correo para recuperación de contraseña</p><br>" + "<a href='" + url + "'>Click para recuperar</a>");

        oMailMessage.IsBodyHtml = true;
        SmtpClient oSmtpClient = new SmtpClient("smtp.office365.com");
        oSmtpClient.EnableSsl = true;
        oSmtpClient.UseDefaultCredentials = false;
        oSmtpClient.Port = 25;
        oSmtpClient.Credentials = new System.Net.NetworkCredential(EmailOrigen, Contraseña);

        oSmtpClient.Send(oMailMessage);

        oSmtpClient.Dispose();
    }

    #endregion
}
}

