using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Undesirable_Communication.Model.User
{
    public class RegisteredUser
    {
        public Guid Id { get; set; }
        public string Username { get; set; }
        public DateTime TimeRegistered { get; set; }

        public DateTime LastCheckIn { get; set; }
    }
}