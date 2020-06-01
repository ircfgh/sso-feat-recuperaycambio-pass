using Cdmx.Scg.Sso.Auth.Models;
using System.Web.Http;
using Cdmx.Scg.Sso.Auth.Security;
using System.Linq;

namespace Cdmx.Scg.Sso.Auth.Controllers
{
    public class CambiarPassRecuperadoController : ApiController
    {
        [HttpPost]
        public IHttpActionResult CambiarPassRecuperado(Models.ViewModels.CambiaPassRecuperadoVM model)

      {
        int mensaje = 0;
        using (var bd = new DbContextAuth())
        {
            {                
                var oUser = bd.Usuario.Where(d => d.token_recovery == model.token).FirstOrDefault();
                    if (oUser != null)
                    {                       
                        oUser.Clave = model.Password;
                        oUser.token_recovery = null;
                        bd.Entry(oUser).State = System.Data.Entity.EntityState.Modified;
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
    }
}