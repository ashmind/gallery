using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AshMind.Web.Gallery.Core.Access {
    public class UserGroup {
        public UserGroup() {
            this.Users = new HashSet<User>();
        }

        public string Name { get; set; }
        public HashSet<User> Users { get; private set; }
    }
}
