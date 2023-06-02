using API_IMC.Models.Entities;
using System;
using System.Net;
using System.Net.Http;
using System.Text.Json;
using System.Web.Http;
using System.Web.Http.Description;

namespace API_IMC.Controllers
{
    [Authorize]
    [RoutePrefix("api/Calculator")]
    public class CalculatorController : ApiController
    {
        private HttpResponseMessage res = new HttpResponseMessage();
        private resMeasuringBodyMass resOk = new resMeasuringBodyMass();
        private resGeneric resNonOk = new resGeneric();

        /// <summary>
        /// Metodo que permite obtener la masa corporal de una persona.
        /// </summary>
        /// <param name="Weight">Peso de la persona en kg</param>
        /// <param name="Height">Estatura de la persona en centimetros sin '.' ni ','</param>
        /// <returns>Retorna un estatus y un mensaje que indican la respuesta a la peticion.</returns>
        [HttpGet]
        [Route("getBodyMass/{Weight}/{Height}")]
        [ResponseType(typeof(resMeasuringBodyMass))]
        public dynamic getBodyMass(double Weight, int Height)
        {
            try
            {
                double fltBodyMass = (Weight / Math.Pow(Height, 2)) * 10000;
                resOk = new resMeasuringBodyMass { status = 200, message = "Medicion realizada con exito.", fltBodyMass = fltBodyMass };
                return new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent(JsonSerializer.Serialize(resOk), System.Text.Encoding.UTF8, "application/json") };
            }
            catch (Exception ex)
            {
                resNonOk = new resGeneric { status = 500, message = "Ocurrio un error al realizar el proceso, detalles -> " + ex.Message };
                return new HttpResponseMessage(HttpStatusCode.InternalServerError) { Content = new StringContent(JsonSerializer.Serialize(resNonOk), System.Text.Encoding.UTF8, "application/json") };
            }
        }
    }
}
