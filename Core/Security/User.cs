using System;
using System.Collections.Generic;
using System.Linq;

using AshMind.Extensions;

namespace AshMind.Gallery.Core.Security {
    public class User : IUser {
        public static User System { get; private set; }

        static User() {
            System = new User("system@gallery.local");
        }

        public User(string email) {
            this.Email = email;
        }

        public string Email { get; private set; }
        
        IEnumerable<IUser> IUserGroup.GetUsers() {
            yield return this;
        }

        string IUserGroup.Name {
            get { return this.Email.SubstringBefore("@"); }
        }
    }
}
