using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using AshMind.Extensions;

namespace AshMind.Web.Gallery.Core.Security {
    public class User : IUserGroup {
        public static User System { get; private set; }

        static User() {
            System = new User("system@gallery.local");
        }

        public User(string email) {
            this.Email = email;
        }

        public string Email { get; private set; }

        public bool IsSystem {
            get { return this == System; }
        }

        HashSet<User> IUserGroup.GetUsers() {
            return new HashSet<User> { this };
        }

        string IUserGroup.Name {
            get { return this.Email.SubstringBefore("@"); }
        }
    }
}
