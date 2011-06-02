using System;
using System.Collections.Generic;
using System.Linq;

namespace AshMind.Gallery.Core.Security {
    public class UserGroup : IUserGroup {
        public static string SuperName = "*Owners";

        public UserGroup() {
            this.Users = new HashSet<User>();
            this.Keys = new HashSet<string>();
        }

        public string Name                     { get; set; }
        public HashSet<User> Users             { get; private set; }
        public HashSet<string> Keys            { get; private set; }

        public bool IsSuper {
            get { return this.Name == SuperName; }
        }

        IEnumerable<IUser> IUserGroup.GetUsers() {
            return Enumerable.Concat(
                this.Users,
                new IUser[] { new AnonymousMember(this) }
            );
        }
    }
}
