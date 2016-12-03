using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Undesirable_Communication.Model.Connection;
using Undesirable_Communication.Model.Connection.Outgoing;

namespace Undesirable_Communication.Model.ChatMessage.Outgoing
{
    public class OutgoingMessageList
    {
        public ICollection<OutgoingMessage> Messages { get; set; }
        public OutgoingChatConnection Connection { get; set; }

        public static OutgoingMessageList Parse(ICollection<Message> messages, ChatConnection x, Guid currentUserId)
        {
            if(messages == null || x == null)
            {
                return null;
            }

            return new OutgoingMessageList
            {
                Connection = OutgoingChatConnection.Parse(x, ""),
                Messages = messages.Select(y => OutgoingMessage.Parse(y, currentUserId)).ToList()
            };
        }

    }
}