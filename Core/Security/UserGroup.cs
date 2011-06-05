using System;
using System.Collections.Generic;
using System.Linq;

namespace AshMind.Gallery.Core.Security {
    public class UserGroup : IUserGroup {
        public UserGroup() {
            this.Users = new HashSet<KnownUser>();
            this.Keys = new HashSet<string>();
        }

        public string Name                     { get; set; }
        public HashSet<KnownUser> Users        { get; private set; }
        public HashSet<string> Keys            { get; private set; }

        IEnumerable<IUser> IUserGroup.GetUsers() {
            return Enumerable.Concat(
                this.Users,
                new IUser[] { new AnonymousMember(this) }
            );
        }
    }
}
