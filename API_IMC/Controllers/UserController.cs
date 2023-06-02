using API_IMC.Helpers;
using API_IMC.Models;
using API_IMC.Models.Entities;
using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text.Json;
using System.Web.Http;
using System.Web.Http.Description;

namespace API_IMC.Controllers
{
    [AllowAnonymous]
    [RoutePrefix("api/User")]
    public class UserController : ApiController
    {
        private HttpResponseMessage res = new HttpResponseMessage();
        private resGeneric resContent = new resGeneric();

        /// <summary>
        ///     Metodo que permite obtener un usuario por medio de su id.
        /// </summary>
        /// <param name="id">Identificador unico del usuario</param>
        /// <returns>Retorna un estatus y un mensaje que indican la respuesta a la peticion.</returns>
        [Authorize(Roles = "Administrator,Evaluator")]
        [HttpGet]
        [Route("getUserById/{id}")]
        [ResponseType(typeof(tblUsers))]
        public dynamic getUserById(string id)
        {
            try
            {
                tblUsers resData = new tblUsers();
                using (IMCEntities db = new IMCEntities())
                {
                    resData = db.tblUsers.Where(x => x.strIdentification == id).FirstOrDefault();
                    db.SaveChanges();
                }

                if (resData != null)
                {
                    resData.strPassword = "****";
                    return new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent(JsonSerializer.Serialize(resData), System.Text.Encoding.UTF8, "application/json") };
                }
                else
                {
                    resContent = new resGeneric { status = 404, message = "No existe ningun usuario que cumpla con los parametros proporcionados" };
                    return new HttpResponseMessage(HttpStatusCode.NotFound) { Content = new StringContent(JsonSerializer.Serialize(resContent), System.Text.Encoding.UTF8, "application/json") };
                }
            }
            catch (Exception ex)
            {
                resContent = new resGeneric { status = 500, message = "Ocurrio un error al consultar la informacion, detalles -> " + ex.Message };
                return new HttpResponseMessage(HttpStatusCode.InternalServerError) { Content = new StringContent(JsonSerializer.Serialize(resContent), System.Text.Encoding.UTF8, "application/json") };
            }
        }

        /// <summary>
        ///     Metodo que permite obtener un usuario por medio de su nombre de usuario.
        /// </summary>
        /// <param name="username">Campo nombre de usuario</param>
        /// <returns>Retorna un estatus y un mensaje que indican la respuesta a la peticion.</returns>
        [Authorize(Roles = "Administrator,Evaluator")]
        [HttpGet]
        [Route("getUserByUsername/{username}")]
        [ResponseType(typeof(tblUsers))]
        public dynamic getUserByUsername(string username)
        {
            try
            {
                tblUsers resData = new tblUsers();
                using (IMCEntities db = new IMCEntities())
                {
                    resData = db.tblUsers.Where(x => x.strUserName == username).FirstOrDefault();
                    db.SaveChanges();
                }

                if (resData != null)
                {
                    resData.strPassword = "****";
                    return new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent(JsonSerializer.Serialize(resData), System.Text.Encoding.UTF8, "application/json") };
                }
                else
                {
                    resContent = new resGeneric { status = 404, message = "No existe ningun usuario que cumpla con los parametros proporcionados" };
                    return new HttpResponseMessage(HttpStatusCode.NotFound) { Content = new StringContent(JsonSerializer.Serialize(resContent), System.Text.Encoding.UTF8, "application/json") };
                }
            }
            catch (Exception ex)
            {
                resContent = new resGeneric { status = 500, message = "Ocurrio un error al consultar la informacion, detalles -> " + ex.Message };
                return new HttpResponseMessage(HttpStatusCode.InternalServerError) { Content = new StringContent(JsonSerializer.Serialize(resContent), System.Text.Encoding.UTF8, "application/json") };
            }
        }

        /// <summary>
        ///     Metodo que permite la creacion de un nuevo usuario.
        /// </summary>
        /// <param name="req">Datos del nuevo cliente para guardar</param>
        /// <returns>Retorna un estatus y un mensaje que indican la respuesta a la peticion.</returns>
        [Authorize(Roles = "Administrator")]
        [HttpPost]
        [Route("postUser")]
        [ResponseType(typeof(resGeneric))]
        public HttpResponseMessage postUser([FromBody]tblUsers req)
        {
            try
            {
                //Validacion de los datos entregados por parte del usuario
                var properties = req.GetType().GetProperties();
                foreach (var property in properties)
                {
                    var value = property.GetValue(req);
                    if (value == null || string.IsNullOrWhiteSpace(value.ToString()) || value.Equals(0) && property.Name != "intPkIdUser")
                    {
                        if (property.Name == "intFkIdRol" && (Convert.ToInt32(value) < 0 || Convert.ToInt32(value) > 3))
                        {
                            resContent = new resGeneric { status = 400, message = "Debes proporcionar un valor valido para la propiedad " + property.Name };
                            return new HttpResponseMessage(HttpStatusCode.BadRequest) { Content = new StringContent(JsonSerializer.Serialize(resContent), System.Text.Encoding.UTF8, "application/json") };
                        }
                        else
                        {
                            resContent = new resGeneric { status = 400, message = "Debes proporcionar un valor valido para la propiedad " + property.Name };
                            return new HttpResponseMessage(HttpStatusCode.BadRequest) { Content = new StringContent(JsonSerializer.Serialize(resContent), System.Text.Encoding.UTF8, "application/json") };
                        }
                    }
                }

                req.strPassword = security.HashPassword(req.strPassword);

                tblUsers userNameValidation = new tblUsers();
                using (IMCEntities db = new IMCEntities())
                {
                    userNameValidation = db.tblUsers.Where(x => x.strUserName == req.strUserName).FirstOrDefault();

                    if (userNameValidation == null)
                    {
                        db.tblUsers.Add(req);
                        db.SaveChanges();
                        resContent = new resGeneric { status = 200, message = "Usuario creado correctamente." };
                        return new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent(JsonSerializer.Serialize(resContent), System.Text.Encoding.UTF8, "application/json") };
                    }
                    else
                    {
                        resContent = new resGeneric { status = 205, message = "El nombre de usuario proporcionado ya se encuentra en uso." };
                        return new HttpResponseMessage(HttpStatusCode.ResetContent) { Content = new StringContent(JsonSerializer.Serialize(resContent), System.Text.Encoding.UTF8, "application/json") };
                    }
                }
            }
            catch (Exception ex)
            {
                resContent = new resGeneric { status = 500, message = "Ocurrio un error al intentar crear el usuario, detalles -> " + ex.Message };
                return new HttpResponseMessage(HttpStatusCode.InternalServerError) { Content = new StringContent(JsonSerializer.Serialize(resContent), System.Text.Encoding.UTF8, "application/json") };
            }
        }

        /// <summary>
        ///     Metodo que permite la actualizacion de datos de un usuario.
        /// </summary>
        /// <param name="req">Datos del nuevo cliente para guardar</param>
        /// <returns>Retorna un estatus y un mensaje que indican la respuesta a la peticion.</returns>
        [Authorize(Roles = "Administrator")]
        [HttpPut]
        [Route("putUser")]
        [ResponseType(typeof(resGeneric))]
        public dynamic putUser([FromBody]tblUsers req)
        {
            try
            {
                //Validacion de los datos entregados por parte del usuario
                var properties = req.GetType().GetProperties();
                foreach (var property in properties)
                {
                    var value = property.GetValue(req);
                    if (value == null || string.IsNullOrWhiteSpace(value.ToString()) || value.Equals(0))
                    {
                        if (property.Name == "intFkIdRol" && (Convert.ToInt32(value) < 0 || Convert.ToInt32(value) > 3))
                        {
                            resContent = new resGeneric { status = 400, message = "Debes proporcionar un valor valido para la propiedad " + property.Name };
                            return new HttpResponseMessage(HttpStatusCode.BadRequest) { Content = new StringContent(JsonSerializer.Serialize(resContent), System.Text.Encoding.UTF8, "application/json") };
                        }
                        else
                        {
                            resContent = new resGeneric { status = 400, message = "Debes proporcionar un valor valido para la propiedad " + property.Name };
                            return new HttpResponseMessage(HttpStatusCode.BadRequest) { Content = new StringContent(JsonSerializer.Serialize(resContent), System.Text.Encoding.UTF8, "application/json") };
                        }
                    }
                }

                req.strPassword = security.HashPassword(req.strPassword);
                resGeneric res;
                tblUsers updData = new tblUsers();
                using (IMCEntities db = new IMCEntities())
                {
                    updData = db.tblUsers.Where(x => x.intPkIdUser == req.intPkIdUser).FirstOrDefault();
                    if (updData != null)
                    {
                        req.intPkIdUser = updData.intPkIdUser;
                        updData = req;
                        db.SaveChanges();
                        resContent = new resGeneric { status = 200, message = "Usuario actualizado correctamente." };
                        return new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent(JsonSerializer.Serialize(resContent), System.Text.Encoding.UTF8, "application/json") };
                    }
                    else
                    {
                        resContent = new resGeneric { status = 404, message = "No existe ningun usuario que cumpla con los parametros proporcionados" };
                        return new HttpResponseMessage(HttpStatusCode.NotFound) { Content = new StringContent(JsonSerializer.Serialize(resContent), System.Text.Encoding.UTF8, "application/json") };
                    }
                }
            }
            catch (Exception ex)
            {
                resContent = new resGeneric { status = 500, message = "Ocurrio un error al intentar actualizar el usuario, detalles -> " + ex.Message };
                return new HttpResponseMessage(HttpStatusCode.InternalServerError) { Content = new StringContent(JsonSerializer.Serialize(resContent), System.Text.Encoding.UTF8, "application/json") };
            }
        }

        /// <summary>
        ///     Metodo que permite el borrado de un usuario.
        /// </summary>
        /// <param name="id">Identificacion del cliente a eliminar</param>
        /// <returns>Retorna un estatus y un mensaje que indican la respuesta a la peticion.</returns>
        [Authorize(Roles = "Administrator")]
        [HttpDelete]
        [Route("deleteUserById/{id}")]
        [ResponseType(typeof(resGeneric))]
        public dynamic deleteUserById(string id)
        {
            try
            {
                //Validacion de los datos entregados por parte del usuario
                if (id == null || string.IsNullOrWhiteSpace(id) || id.Equals(0))
                {
                    resContent = new resGeneric { status = 400, message = "Debes proporcionar un valor valido para el id de la persona" };
                    return new HttpResponseMessage(HttpStatusCode.BadRequest) { Content = new StringContent(JsonSerializer.Serialize(resContent), System.Text.Encoding.UTF8, "application/json") };
                }

                resGeneric res = new resGeneric();
                tblUsers delData = new tblUsers();
                using (IMCEntities db = new IMCEntities())
                {
                    delData = db.tblUsers.Where(x => x.strIdentification == id).FirstOrDefault();
                    if (delData != null)
                    {
                        db.tblUsers.Remove(delData);
                        db.SaveChanges();
                        resContent = new resGeneric { status = 200, message = "Usuario borrado correctamente." };
                        return new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent(JsonSerializer.Serialize(resContent), System.Text.Encoding.UTF8, "application/json") };
                    }
                    else
                    {
                        resContent = new resGeneric { status = 404, message = "No existe ningun usuario que cumpla con los parametros proporcionados" };
                        return new HttpResponseMessage(HttpStatusCode.NotFound) { Content = new StringContent(JsonSerializer.Serialize(resContent), System.Text.Encoding.UTF8, "application/json") };
                    }
                }
            }
            catch (Exception ex)
            {
                resContent = new resGeneric { status = 500, message = "Ocurrio un error al intentar borrar el usuario, detalles -> " + ex.Message };
                return new HttpResponseMessage(HttpStatusCode.InternalServerError) { Content = new StringContent(JsonSerializer.Serialize(resContent), System.Text.Encoding.UTF8, "application/json") };
            }
        }
    }
}
