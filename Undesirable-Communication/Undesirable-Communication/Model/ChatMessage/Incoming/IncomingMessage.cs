using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Undesirable_Communication.Model.ChatMessage.Incoming
{
    public class IncomingMessage
    {
        public Guid ChatId { get; set; }
        public Guid CurrentUserId { get; set; }
        public string Content { get; set; }
    }
}