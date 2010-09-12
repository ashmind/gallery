using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AshMind.Web.Gallery.Core.Security {
    public class User {
        public User(string email) {
            this.Email = email;
            this.Permissions = new HashSet<Permission>();
        }

        public string Email                    { get; private set; }
        public HashSet<Permission> Permissions { get; private set; }
    }
}
