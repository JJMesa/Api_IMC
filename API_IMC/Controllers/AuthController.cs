using API_IMC.App_Start;
using API_IMC.Helpers;
using API_IMC.Models;
using API_IMC.Models.Entities;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text.Json;
using System.Web.Http;
using System.Web.Http.Description;

namespace API_IMC.Controllers
{
    [AllowAnonymous]
    [RoutePrefix("api/Auth")]
    public class AuthController : ApiController
    {

        /// <summary>
        ///     Metodo para la autenticacion de la API.
        /// </summary>
        /// <param name="req">Credenciales de usuario</param>
        /// <returns>Retorna un estatus y un mensaje que indican la respuesta a la peticion.</returns>
        [HttpGet]
        [Route("getToken")]
        [ResponseType(typeof(resGetToken))]
        public HttpResponseMessage getToken([FromBody]reqGetToken req)
        {
            resGeneric resNonOk = new resGeneric();
            resGetToken resOk = new resGetToken();
            if (string.IsNullOrEmpty(req.username) || string.IsNullOrEmpty(req.username))
            {
                resNonOk = new resGeneric { status = 401, message = "Debes proporcionar todos los datos." };
                return new HttpResponseMessage(HttpStatusCode.Unauthorized) { Content = new StringContent(JsonSerializer.Serialize(resNonOk), System.Text.Encoding.UTF8, "application/json") };
            }

            req.password = security.HashPassword(req.password);

            dynamic dataUser;
            using (IMCEntities db = new IMCEntities())
            {
                dataUser = db.tblUsers.Join(db.tblRoles, u => u.intFkIdRol, r => r.intPkIdRol, (u, r) => new { User = u, Role = r })
                                       .Where(x => x.User.strUserName == req.username && x.User.strPassword == req.password)
                                       .Select(x => new { idUser = x.User.intPkIdUser, strName = x.User.strName, strRole = x.Role.strDescription })
                                       .FirstOrDefault();
            }

            if (dataUser == null)
            {
                resNonOk = new resGeneric { status = 401, message = "No se ha encontrado ningun usuario con los parametros proporcionados, valide sus credenciales y intente nuevamente." };
                return new HttpResponseMessage(HttpStatusCode.NotFound) { Content = new StringContent(JsonSerializer.Serialize(resNonOk), System.Text.Encoding.UTF8, "application/json") };
            }
            else
            {
                var token = TokenGenerator.GenerateTokenJwt(req.username, dataUser.strRole);
                resOk = new resGetToken { status = 200, message = "Bienvenido de nuevo " + dataUser.strName, token = token };
                return new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent(JsonSerializer.Serialize(resOk), System.Text.Encoding.UTF8, "application/json") };
            }
        }
    }
}