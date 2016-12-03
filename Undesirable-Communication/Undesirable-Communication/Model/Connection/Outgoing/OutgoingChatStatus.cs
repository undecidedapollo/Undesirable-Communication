using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Undesirable_Communication.Model.Connection.Outgoing
{
    public class OutgoingChatStatus
    {
        public bool IsPending { get; set; }

        public bool IsRegistered { get; set; }

        public OutgoingChatConnection connection { get; set; }
    }
}