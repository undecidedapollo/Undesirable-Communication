using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Web;

namespace Undesirable_Communication.Infrastructure
{
    public class JsonFactory
    {
        [DebuggerStepThrough]
        public static HttpResponseMessage CreateJsonMessage(object theObject, HttpStatusCode code, HttpRequestMessage request)
        {
            var jsonString = JsonConvert.SerializeObject(theObject);
            var response = request.CreateResponse(code);
            response.Content = new StringContent(jsonString, Encoding.UTF8, "application/json");
            return response;
        }

        [DebuggerStepThrough]
        public static HttpResponseMessage CreateResponseMessage(string theResponseMessage, HttpStatusCode code, HttpRequestMessage request)
        {
            var response = request.CreateResponse(code);
            response.Content = new StringContent(theResponseMessage, Encoding.UTF8, "application/json");
            return response;
        }
    }
}