using API_IMC.Models;
using API_IMC.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text.Json;
using System.Web.Http;
using System.Web.Http.Description;

namespace API_IMC.Controllers
{
    [Authorize]
    [RoutePrefix("api/Evaluation")]
    public class EvaluationController : ApiController
    {
        private HttpResponseMessage res = new HttpResponseMessage();
        private resGeneric resContent = new resGeneric();
        /// <summary>
        /// Metodo que permite obtener una evaluacion por medio de su id.
        /// </summary>
        /// <param name="id">Identificador unico de la evaluacion</param>
        /// <returns>Retorna un estatus y un mensaje que indican la respuesta a la peticion.</returns>
        [HttpGet]
        [Route("getEvaluationsById/{id}")]
        [ResponseType(typeof(tblHistoricalEvaluations))]
        public dynamic getEvaluationsById(int id)
        {
            try
            {
                tblHistoricalEvaluations resData = new tblHistoricalEvaluations();
                using (IMCEntities db = new IMCEntities())
                {
                    resData = db.tblHistoricalEvaluations.Where(x => x.intFkIdUser == id).FirstOrDefault();
                }

                if (resData != null)
                {
                    return new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent(JsonSerializer.Serialize(resData), System.Text.Encoding.UTF8, "application/json") };
                }
                else
                {
                    resContent = new resGeneric { status = 404, message = "No existe ninguna evaluacion que cumpla con los parametros proporcionados." };
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
        /// Metodo que permite obtener las evaluaciones de un usuario por medio de su id.
        /// </summary>
        /// <param name="id">Identificador unico del usuario</param>
        /// <returns>Retorna un estatus y un mensaje que indican la respuesta a la peticion.</returns>
        [HttpGet]
        [Route("getEvaluationByUserId/{id}")]
        [ResponseType(typeof(List<tblHistoricalEvaluations>))]
        public dynamic getEvaluationsByUserId(string id)
        {
            try
            {
                tblUsers dataUsers = new tblUsers();
                List<tblHistoricalEvaluations> resData = new List<tblHistoricalEvaluations>();
                using (IMCEntities db = new IMCEntities())
                {
                    dataUsers = db.tblUsers.Where(x => x.strIdentification == id).FirstOrDefault();
                    if (dataUsers != null)
                    {
                        resData = db.tblHistoricalEvaluations.Where(x => x.intFkIdUser == dataUsers.intPkIdUser).ToList();
                    }
                    else
                    {
                        resContent = new resGeneric { status = 404, message = "No existe ningun usuario que cumpla con los parametros proporcionados" };
                        return new HttpResponseMessage(HttpStatusCode.NotFound) { Content = new StringContent(JsonSerializer.Serialize(resContent), System.Text.Encoding.UTF8, "application/json") };
                    }
                }

                if (resData != null)
                {
                    return new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent(JsonSerializer.Serialize(resData), System.Text.Encoding.UTF8, "application/json") };
                }
                else
                {
                    resContent = new resGeneric { status = 404, message = "No existe ninguna evaluacion que cumpla con los parametros proporcionados." };
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
        /// Metodo que permite la creacion de un nuevo usuario.
        /// </summary>
        /// <param name="req">Datos del nuevo cliente para guardar</param>
        /// <returns>Retorna un estatus y un mensaje que indican la respuesta a la peticion.</returns>
        [Authorize(Roles = "Administrator,Evaluator")]
        [HttpPost]
        [Route("postEvaluation")]
        [ResponseType(typeof(resGeneric))]
        public dynamic postEvaluation([FromBody]tblHistoricalEvaluations req)
        {
            try
            {
                //Validacion de los datos entregados por parte del usuario
                var properties = req.GetType().GetProperties();
                foreach (var property in properties)
                {
                    var value = property.GetValue(req);
                    if (value == null || string.IsNullOrWhiteSpace(value.ToString()) || value.Equals(0) && (property.Name != "intPkIdEvaluation" && property.Name != "strObservations"))
                    {
                        resContent = new resGeneric { status = 400, message = "Debes proporcionar un valor valido para la propiedad " + property.Name };
                        return new HttpResponseMessage(HttpStatusCode.BadRequest) { Content = new StringContent(JsonSerializer.Serialize(resContent), System.Text.Encoding.UTF8, "application/json") };
                    }
                }

                using (IMCEntities db = new IMCEntities())
                {
                    req.intPkIdEvaluation = 0;
                    req.dtmEvaluationDate = DateTime.Now;
                    db.tblHistoricalEvaluations.Add(req);
                    db.SaveChanges();
                }

                resContent = new resGeneric { status = 200, message = "Evaluacion creada correctamente." };
                return new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent(JsonSerializer.Serialize(resContent), System.Text.Encoding.UTF8, "application/json") };
            }
            catch (Exception ex)
            {
                resContent = new resGeneric { status = 500, message = "Ocurrio un error al intentar crear la evaluacion, detalles -> " + ex.Message };
                return new HttpResponseMessage(HttpStatusCode.InternalServerError) { Content = new StringContent(JsonSerializer.Serialize(resContent), System.Text.Encoding.UTF8, "application/json") };
            }
        }
    }
}
