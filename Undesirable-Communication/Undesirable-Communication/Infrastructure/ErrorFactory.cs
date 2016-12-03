using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using Undesirable_Communication.Model.Errors;
using Undesirable_Communication.Model.Message.Outgoing;

namespace Undesirable_Communication.Infrastructure
{
    public class ErrorFactory
    {
        public static HttpResponseMessage Handle(Func<HttpResponseMessage> functionToEncapsulate, HttpRequestMessage Request, string userId = null)
        {
            try
            {
                return functionToEncapsulate();
            }
            catch (ChatGroupNotFoundException e)
            {
                return JsonFactory.CreateJsonMessage(new OutgoingHttpMessage { Message = "Your group could not be found.", Action = "groupNotFound" }, HttpStatusCode.BadRequest, Request);
            }
            catch (UserNotFoundException e)
            {
                return JsonFactory.CreateJsonMessage(new OutgoingHttpMessage { Message = "Your user Id could not be found.", Action = "userNotFound" }, HttpStatusCode.BadRequest, Request);
            }
            catch (InvalidModelException e)
            {
                return JsonFactory.CreateJsonMessage(new OutgoingHttpMessage { Message = "Invalid model exception.", Action = "invalidModel" }, HttpStatusCode.BadRequest, Request);
            }
            catch (NotImplementedException e)
            {
                return JsonFactory.CreateJsonMessage(new OutgoingHttpMessage { Message = "Invalid API Endpoint. We have not yet implemented this documented feature.", Action = "invalidImplementation" }, HttpStatusCode.BadRequest, Request);
            }
            catch (UnauthorizedAccessException e)
            {
                return JsonFactory.CreateJsonMessage(new OutgoingHttpMessage { Message = "You are not authorized to get this content. ", Action = "unauthorized" }, HttpStatusCode.Forbidden, Request);
            }
            catch (Exception e)
            {
                return JsonFactory.CreateJsonMessage(new OutgoingHttpMessage { Message = "An unknown error has occured" }, HttpStatusCode.InternalServerError, Request);
            }
        }

        public async static Task<HttpResponseMessage> Handle(Func<Task<HttpResponseMessage>> functionToEncapsulate, HttpRequestMessage Request, string userId = null)
        {
            try
            {
                return await functionToEncapsulate();
            }
            catch (NotImplementedException e)
            {
                return JsonFactory.CreateJsonMessage(new OutgoingHttpMessage { Message = "Invalid API Endpoint. We have not yet implemented this documented feature.", Action = "invalidImplementation" }, HttpStatusCode.BadRequest, Request);
            }
            catch (UnauthorizedAccessException e)
            {
                return JsonFactory.CreateJsonMessage(new OutgoingHttpMessage { Message = "You are not authorized to get this content. ", Action = "unauthorized" }, HttpStatusCode.Forbidden, Request);
            }
            catch (Exception e)
            {
                return JsonFactory.CreateJsonMessage(new OutgoingHttpMessage { Message = "An unknown error has occured" }, HttpStatusCode.InternalServerError, Request);
            }
        }
    }
}