using System;
using System.Globalization;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Web.Http;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using REST_KUNDER.Models;

namespace REST_KUNDER.Controllers
{
    [RoutePrefix("")]
    public class DefaultController : ApiController
    {
        [Route("time")]
        public HttpResponseMessage Get([FromUri] string value)
        {
            try
            {
                var time = value;
                time = time.Replace(@"\", "").Replace("\"", "");
                var dt = DateTime.ParseExact(time, "HH:mm:ss", CultureInfo.InvariantCulture);

                var d = new DataRest
                {
                    code = "00",
                    description = "OK",
                    data = dt.ToUniversalTime().ToString("o")
                };
                var response = Request.CreateResponse(HttpStatusCode.OK);
                response.Content = new StringContent(JsonConvert.SerializeObject(d), Encoding.UTF8, "application/json");
                return response;
            }
            catch (Exception e)
            {
                if (e.TargetSite.Name == "ParseExact")
                {
                    var dataToModel = new DataRest
                    {
                        code = "00",
                        description = "Solo se admite la hora en el siguiente formato HH:mm:ss  Ejemplo: 15:30:45",
                        data = ""
                    };
                    var responseParseExact = Request.CreateResponse(HttpStatusCode.BadRequest);
                    responseParseExact.Content = new StringContent(JsonConvert.SerializeObject(dataToModel),
                        Encoding.UTF8, "application/json");
                    return responseParseExact;
                }
                var responseException = Request.CreateResponse(HttpStatusCode.InternalServerError);
                responseException.Content = new StringContent(JsonConvert.SerializeObject(new DataRest
                {
                    code = "00",
                    description = "Error interno del sistema",
                    data = ""
                }), Encoding.UTF8, "application/json");
                return responseException;
            }
        }

        [Route("word")]
        public HttpResponseMessage Post([FromBody] JObject data)
        {
            try
            {
                if (data != null)
                {
                    var dataToString = JsonConvert.SerializeObject(data);
                    var dataToModel = JsonConvert.DeserializeObject<DataRest>(dataToString);
                    if ((dataToModel.data.Length <= 4) && Regex.IsMatch(dataToModel.data, @"^[a-zA-Z]+$"))
                    {
                        dataToModel.code = "00";
                        dataToModel.description = "OK";
                        dataToModel.data = dataToModel.data.ToUpper();
                        var response = Request.CreateResponse(HttpStatusCode.OK);
                        response.Content = new StringContent(JsonConvert.SerializeObject(dataToModel), Encoding.UTF8,
                            "application/json");
                        return response;
                    }
                    dataToModel.code = "00";
                    dataToModel.description = "Solo se admiten letras y maximo 4 caracteres";
                    dataToModel.data = "";
                    var responsee = Request.CreateResponse(HttpStatusCode.BadRequest);
                    responsee.Content = new StringContent(JsonConvert.SerializeObject(dataToModel), Encoding.UTF8,
                        "application/json");
                    return responsee;
                }
                else
                {
                    var obj = new DataRest
                    {
                        code = "00",
                        description = "Se debe enviar los datos en formato JSON",
                        data = ""
                    };
                    var responsee = Request.CreateResponse(HttpStatusCode.BadRequest);
                    responsee.Content = new StringContent(JsonConvert.SerializeObject(obj), Encoding.UTF8,
                        "application/json");
                    return responsee;
                }
            }
            catch
            {
                var responsee = Request.CreateResponse(HttpStatusCode.InternalServerError);
                responsee.Content = new StringContent(JsonConvert.SerializeObject(new DataRest
                {
                    code = "00",
                    description = "Error interno del sistema",
                    data = ""
                }), Encoding.UTF8, "application/json");
                return responsee;
            }
        }
    }
}