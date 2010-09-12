using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AshMind.Web.Gallery.Core.Access {
    public class User {
        public User(string email) {
            this.Email = email;
        }

        public string Email { get; private set; }
    }
}
