using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Undesirable_Communication.Model.Connection
{
    public class ChatConnection
    {
        public ChatConnection()
        {
            this.Messages = new List<ChatMessage.Message>();
        }

        public Guid Id { get; set; }
        public User.RegisteredUser User1 { get; set; }
        public User.RegisteredUser User2 { get; set; }
        public DateTime StartTime { get; set; }

        public List<ChatMessage.Message> Messages { get; set; }
       

    }
}