using System;
using System.Net;
using System.Threading;
using System.Web.Http;
using Cdmx.Scg.Sso.Auth.Models;
using Cdmx.Scg.Sso.Auth.Security;
using System.Data.Entity;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace Cdmx.Scg.Sso.Auth.Controllers
{
    /// <summary>
    /// login controller class for authenticate users
    /// </summary>
    [AllowAnonymous]
    [RoutePrefix("api/login")]
    public class LoginController : ApiController
    {
        [HttpGet]
        [Route("echoping")]
        public IHttpActionResult EchoPing()
        {
            return Ok(true);
        }

        [HttpGet]
        [Route("echouser")]
        public IHttpActionResult EchoUser()
        {
            var identity = Thread.CurrentPrincipal.Identity;
            return Ok($" IPrincipal-user: {identity.Name} - IsAuthenticated: {identity.IsAuthenticated}");
        }

        [HttpPost]
        [Route("authenticate")]
        public IHttpActionResult Authenticate(LoginRequest login)
        {
            if (login == null)
                throw new HttpResponseException(HttpStatusCode.BadRequest);

            DbContextAuth db = new DbContextAuth();

            var password = Encrypt.GetMD5(login.Password);

            var cUsuario = db.Usuario.Where(x => x.NombreUsuario == login.Username && x.Clave == password).SingleOrDefault();

            //TODO: This code is only for demo - extract method in new class & validate correctly in your application !!
            //var isUserValid = (login.Username == "user" && login.Password == "123456");

            bool isUserValid = (cUsuario != null);

            if (isUserValid && cUsuario.RolUsuario.Where(x => x.Rol.TipoRol == "CONSULTA").Count() >= 1)
            {
                var rolename = "CONSULTA";
                var token = TokenGenerator.GenerateTokenJwt(cUsuario.IdUsuario , cUsuario.NombreUsuario, rolename, "roledos");
                return Ok(token);
            }

            //TODO: This code is only for demo - extract method in new class & validate correctly in your application !!
            //var isTesterValid = (login.Username == "test" && login.Password == "123456");
            if (isUserValid && cUsuario.RolUsuario.Where(x => x.Rol.TipoRol == "CAPTURA").Count() >= 1)
            {
                var rolename = "CAPTURA";
                var token = TokenGenerator.GenerateTokenJwt(cUsuario.IdUsuario, cUsuario.NombreUsuario, rolename, "roledos");
                return Ok(token);
            }

            //TODO: This code is only for demo - extract method in new class & validate correctly in your application !!
            //var isAdminValid = (login.Username == "admin" && login.Password == "123456");

            if (isUserValid && cUsuario.RolUsuario.Where(x => x.Rol.TipoRol == "ADMINISTRADOR").Count() >= 1)
            {
                var rolename = "ADMINISTRADOR";
                var token = TokenGenerator.GenerateTokenJwt(cUsuario.IdUsuario, cUsuario.NombreUsuario, rolename, "roledos");
                return Ok(token);
            }

            // Unauthorized access 
            return Unauthorized();
        }
        public class Encrypt
        {
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
}
