using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Undesirable_Communication.Model.User;

namespace Undesirable_Communication.Model.ChatMessage
{
    public class Message
    {
        public RegisteredUser Author { get; set; }
        public DateTime TimeSent { get; set; }
        public string Content {get; set; }
    }
}