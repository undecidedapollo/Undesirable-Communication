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
                var manager = ChatManager.GetInstance();

                var newUserGuid = Guid.NewGuid();

                if(newUser.Username == null || newUser.Username.Length < 6 || newUser.Username.Length > 25)
                {
                    throw new InvalidModelException();
                }


                var theNewUser = new RegisteredUser { Id = newUserGuid, Username = newUser.Username, TimeRegistered = DateTime.Now, LastCheckIn = DateTime.Now };

                manager.EnqueuePendingUser(theNewUser);
                var returnObject = new OutgoingUserRegistrationResult { UserId = newUserGuid };


                return JsonFactory.CreateJsonMessage(returnObject, HttpStatusCode.OK, this.Request);
            }), this.Request);
        }

        [HttpPost]
        [Route("api/Chat/CheckStatus")]
        public HttpResponseMessage CheckStatus(IncomingRegisteredUser newUser)
        {
            return ErrorFactory.Handle((Func<HttpResponseMessage>)(() =>
            {
                var manager = ChatManager.GetInstance();

                var existsInPending = manager.QueryPendingUsers((x) =>
                {
                    var user = x.FirstOrDefault(y => y.Id == newUser.Id);

                    if(user != null)
                    {
                        user.LastCheckIn = DateTime.Now;
                    }

                    return user;
                });

                var existsInKnown = manager.QueryKnownConnections((x) =>
                {
                    var con =  x.FirstOrDefault(y => y.User1.Id == newUser.Id || y.User2.Id == newUser.Id);

                    if(con != null)
                    {
                        if(con.User1.Id == newUser.Id)
                        {
                            con.User1.LastCheckIn = DateTime.Now;
                        }else if(con.User2.Id == newUser.Id)
                        {
                            con.User2.LastCheckIn = DateTime.Now;
                        }
                    }

                    return con;
                    
                });

                var isPending = existsInPending != null;
                var isRegistered = existsInPending != null || existsInKnown != null;
                string otherName = "";
                if (existsInKnown != null)
                {
                    otherName = existsInKnown.User1.Id == newUser.Id ? existsInKnown.User2.Username : existsInKnown.User1.Username;
                }
                var status = new OutgoingChatStatus { IsRegistered = isRegistered, IsPending = isPending, connection = OutgoingChatConnection.Parse(existsInKnown, otherName) };

                return JsonFactory.CreateJsonMessage(status, HttpStatusCode.OK, this.Request);
            }), this.Request);
        }

        [HttpGet]
        [Route("api/Chat/GetRecentMessages")]
        public HttpResponseMessage GetRecentMessages(Guid chatId, Guid currentUserId)
        {
            return ErrorFactory.Handle((Func<HttpResponseMessage>)(() =>
            {
                var manager = ChatManager.GetInstance();

                var outgoingMessages = manager.QueryKnownConnections((x) =>
                {
                    var connection = x.FirstOrDefault(y => y.Id == chatId);

                    if (connection == null)
                    {
                        throw new ChatGroupNotFoundException();
                    }

                    if(connection.User1.Id == currentUserId)
                    {
                        connection.User1.LastCheckIn = DateTime.Now;
                    }else if(connection.User2.Id == currentUserId)
                    {
                        connection.User2.LastCheckIn = DateTime.Now;
                    }
                    else
                    {
                        throw new UserNotFoundException();
                    }


                    var messages = connection.Messages.OrderByDescending(m => m.TimeSent).Take(100).ToList();

                    var outgoingResponse = OutgoingMessageList.Parse(messages, connection, currentUserId);

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
                var manager = ChatManager.GetInstance();

                var outgoingMess = manager.QueryKnownConnections((x) =>
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
                        connection.User1.LastCheckIn = DateTime.Now;
                        
                    }else if (connection.User2.Id == incMessage.CurrentUserId)
                    {
                        UserInList = connection.User2;
                        connection.User2.LastCheckIn = DateTime.Now;
                    }
                    else
                    {
                        throw new UserNotFoundException();
                    }

                    UserInList.LastCheckIn = DateTime.Now;

                    var newMessage = new Message { Id = Guid.NewGuid(), Author = UserInList, TimeSent = DateTime.Now, Content = incMessage.Content };

                    connection.Messages.Add(newMessage);

                    var outgoingMessage = OutgoingMessage.Parse(newMessage, incMessage.CurrentUserId);

                    return outgoingMessage;
                });

                return JsonFactory.CreateJsonMessage(outgoingMess, HttpStatusCode.OK, this.Request);
            }), this.Request);
        }

        [HttpPost]
        [Route("api/Chat/ExitChat")]
        public HttpResponseMessage ExitChat(IncomingLeaveChat leaveChatRequest)
        {
            return ErrorFactory.Handle((Func<HttpResponseMessage>)(() =>
            {
                var manager = ChatManager.GetInstance();

                manager.QueryKnownConnections((x) =>
                {
                    var connection = x.FirstOrDefault(y => y.Id == leaveChatRequest.Id && (y.User1.Id == leaveChatRequest.CurrentUserId || y.User2.Id == leaveChatRequest.CurrentUserId));

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