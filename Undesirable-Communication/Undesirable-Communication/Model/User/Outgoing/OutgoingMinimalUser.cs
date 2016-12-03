using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Undesirable_Communication.Model.User.Outgoing
{
    public class OutgoingMinimalUser
    {
        public string Username { get; set; }
        
        public static OutgoingMinimalUser Parse(RegisteredUser x)
        {
            if(x == null)
            {
                return null;
            }

            return new OutgoingMinimalUser
            {
                Username = x.Username
            };
        }
    }
}