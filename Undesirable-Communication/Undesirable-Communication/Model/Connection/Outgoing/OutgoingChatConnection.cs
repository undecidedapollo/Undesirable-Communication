using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Undesirable_Communication.Model.User.Outgoing;

namespace Undesirable_Communication.Model.Connection.Outgoing
{
    public class OutgoingChatConnection
    {
        public Guid Id { get; set; }

        public OutgoingMinimalUser User1 { get; set; }
        public OutgoingMinimalUser User2 { get; set; }

        public static OutgoingChatConnection Parse(ChatConnection x)
        {
            if(x == null)
            {
                return null;
            }

            return new OutgoingChatConnection
            {
                Id = x.Id,
                User1 = OutgoingMinimalUser.Parse(x.User1),
                User2 = OutgoingMinimalUser.Parse(x.User2)
            };
        }
    }
}