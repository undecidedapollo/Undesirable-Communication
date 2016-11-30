using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNet.SignalR;

namespace Undesirable_Communication.Infrastructure
{
    public class ChatHub : Hub
    {
        public void Connect(string userName)
        {
            
        }

        public void SendPrivateMessage(string toUserUUID, string message)
        {
            
        }

        //public override System.Threading.Tasks.Task OnDisconnected()
        //{
            
        //}
    }
}