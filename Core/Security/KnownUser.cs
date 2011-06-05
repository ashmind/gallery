using System;
using System.Collections.Generic;
using System.Linq;

using AshMind.Extensions;

namespace AshMind.Gallery.Core.Security {
    public class KnownUser : IUser {
        public static KnownUser System { get; private set; }

        static KnownUser() {
            System = new KnownUser("system@gallery.local");
        }

        public KnownUser(string email) {
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
