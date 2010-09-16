using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using AshMind.Extensions;

namespace AshMind.Web.Gallery.Core.Security {
    public class User : IUserGroup {
        public User(string email) {
            this.Email = email;
        }

        public string Email { get; private set; }

        HashSet<User> IUserGroup.GetUsers() {
            return new HashSet<User> { this };
        }

        string IUserGroup.Name {
            get { return this.Email.SubstringBefore("@"); }
        }
    }
}
