using System;
using System.Collections.Generic;
using System.Linq;

namespace AshMind.Gallery.Core.Security {
    public class AnonymousMember : IUser {
        private readonly IUserGroup userGroup;

        public AnonymousMember(IUserGroup userGroup) {
            this.userGroup = userGroup;
        }

        public override bool Equals(object obj) {
            if (obj.GetType() != this.GetType())
                return false;
                        
            return this.userGroup.Equals(((AnonymousMember)obj).userGroup);
        }

        public override int GetHashCode() {
            return this.GetType().GetHashCode() ^ this.userGroup.GetHashCode();
        }

        public string Name {
            get { return "Anonymous"; }
        }

        IEnumerable<IUser> IUserGroup.GetUsers() {
            yield return this;
        }
    }
}
