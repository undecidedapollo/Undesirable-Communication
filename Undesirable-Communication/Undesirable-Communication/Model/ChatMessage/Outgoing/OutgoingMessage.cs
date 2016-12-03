using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Undesirable_Communication.Model.User.Outgoing;

namespace Undesirable_Communication.Model.ChatMessage.Outgoing
{
    public class OutgoingMessage
    {
        public Guid Id { get; set; }
        public OutgoingMinimalUser Author { get; set; }
        public DateTime TimeSent { get; set; }
        public string Content { get; set; }

        public bool IsRequestingUser { get; set; }
        public static OutgoingMessage Parse(Message x, Guid currentUserId)
        {
            if (x == null)
            {
                return null;
            }

            return new OutgoingMessage
            {
                TimeSent = x.TimeSent,
                Content = x.Content,
                Author = OutgoingMinimalUser.Parse(x.Author),
                Id = x.Id,
                IsRequestingUser = x.Author.Id.Equals(currentUserId)
            };

        }
    }
}