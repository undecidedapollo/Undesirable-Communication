using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Undesirable_Communication.Infrastructure;
using Undesirable_Communication.Model.ChatMessage;
using Undesirable_Communication.Model.ChatMessage.Incoming;
using Undesirable_Communication.Model.ChatMessage.Outgoing;
using Undesirable_Communication.Model.Connection.Incoming;
using Undesirable_Communication.Model.Connection.Outgoing;
using Undesirable_Communication.Model.Errors;
using Undesirable_Communication.Model.Message.Outgoing;
using Undesirable_Communication.Model.User;
using Undesirable_Communication.Model.User.Incoming;
using Undesirable_Communication.Model.User.Outgoing;
using Undesirable_Communication.Queues;

namespace Undesirable_Communication.api
{
    public class ChatController : ApiController
    {
        [HttpPost]
        [Route("api/Chat/Register")]
        public HttpResponseMessage RegisterForChat(IncomingUserRegistration newUser)
        {
            return ErrorFactory.Handle((Func<HttpResponseMessage>) (() =>
            {
                var manager = ChatMananger.GetInstance();

                var newUserGuid = new Guid();

                var theNewUser = new RegisteredUser { Id = newUserGuid, Username = newUser.Username, TimeRegistered = DateTime.Now };

                manager.EnqueuePendingUser(theNewUser);
                var returnObject = new OutgoingUserRegistrationResult { UserId = newUserGuid };


                return JsonFactory.CreateJsonMessage(returnObject, HttpStatusCode.OK, this.Request);
            }), this.Request);
        }

        [HttpGet]
        [Route("api/Chat/CheckStatus")]
        public HttpResponseMessage CheckStatus(IncomingRegisteredUser newUser)
        {
            return ErrorFactory.Handle((Func<HttpResponseMessage>)(() =>
            {
                var manager = ChatMananger.GetInstance();

                var existsInPending = manager.QueryPendingUsers((x) =>
                {
                    return x.FirstOrDefault(y => y.Id == newUser.Id);
                });

                var existsInKnown = manager.QueryKnownConnections((x) =>
                {
                    return x.FirstOrDefault(y => y.User1.Id == newUser.Id || y.User2.Id == newUser.Id);
                });

                var isPending = existsInPending != null;
                var isRegistered = existsInPending == null && existsInKnown == null;

                var status = new OutgoingChatStatus { IsRegistered = isRegistered, IsPending = isPending, connection = OutgoingChatConnection.Parse(existsInKnown) };

                return JsonFactory.CreateJsonMessage(status, HttpStatusCode.OK, this.Request);
            }), this.Request);
        }

        [HttpGet]
        [Route("api/Chat/GetRecentMessages")]
        public HttpResponseMessage GetRecentMessages(Guid chatId, Guid currentUserId)
        {
            return ErrorFactory.Handle((Func<HttpResponseMessage>)(() =>
            {
                var manager = ChatMananger.GetInstance();

                var outgoingMessages = manager.QueryKnownConnections((x) =>
                {
                    var connection = x.FirstOrDefault(y => y.Id == chatId);

                    if (connection == null) return null;

                    var messages = connection.Messages.OrderByDescending(m => m.TimeSent).Take(100).ToList();

                    var outgoingResponse = OutgoingMessageList.Parse(messages, connection);

                    return outgoingResponse;
                });

                return JsonFactory.CreateJsonMessage(outgoingMessages, HttpStatusCode.OK, this.Request);
            }), this.Request);
        }

        [HttpPost]
        [Route("api/Chat/SendMessage")]
        public HttpResponseMessage SendMessage(IncomingMessage incMessage)
        {
            return ErrorFactory.Handle((Func<HttpResponseMessage>)(() =>
            {
                var manager = ChatMananger.GetInstance();

                manager.QueryKnownConnections((x) =>
                {
                    var connection = x.FirstOrDefault(y => y.Id == incMessage.ChatId);

                    if (connection == null)
                    {
                        throw new ChatGroupNotFoundException();
                    }

                    RegisteredUser UserInList = null;

                    if(connection.User1.Id == incMessage.CurrentUserId)
                    {
                        UserInList = connection.User1;
                    }else if (connection.User2.Id == incMessage.CurrentUserId)
                    {
                        UserInList = connection.User2;
                    }
                    else
                    {
                        throw new UserNotFoundException();
                    }

                    var newMessage = new Message { Author = UserInList, TimeSent = DateTime.Now, Content = incMessage.Content };

                    connection.Messages.Add(newMessage);

                    return (object)null;
                });

                return JsonFactory.CreateJsonMessage(new OutgoingHttpMessage {Message = "Message sent" }, HttpStatusCode.OK, this.Request);
            }), this.Request);
        }

        [HttpPost]
        [Route("api/Chat/ExitChat")]
        public HttpResponseMessage ExitChat(IncomingLeaveChat leaveChatRequest)
        {
            return ErrorFactory.Handle((Func<HttpResponseMessage>)(() =>
            {
                var manager = ChatMananger.GetInstance();

                manager.QueryKnownConnections((x) =>
                {
                    var connection = x.FirstOrDefault(y => y.Id == leaveChatRequest.Id);

                    if (connection == null)
                    {
                        throw new ChatGroupNotFoundException();
                    }

                    x.Remove(connection);

                    return (object)null;
                });

                return JsonFactory.CreateJsonMessage(new OutgoingHttpMessage { Message = "Chat group left." }, HttpStatusCode.OK, this.Request);
            }), this.Request);
        }


    }
}