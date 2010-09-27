using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

using AshMind.Extensions;

using AshMind.Gallery.Core.Security;
using AshMind.Gallery.Core.Internal;

namespace AshMind.Gallery.Core.Security.Internal {
    internal class MD5UserGroupSecureReferenceStrategy : IUserGroupSecureReferenceStrategy, IDisposable {
        private readonly MD5 md5;

        public MD5UserGroupSecureReferenceStrategy(MD5 md5) {
            this.md5 = md5;
        }

        public IUserGroup ResolveReference(string reference, IEnumerable<IUserGroup> userGroups) {
            if (!reference.StartsWith("u:"))
                return userGroups.OfType<UserGroup>().SingleOrDefault(g => g.Name == reference);

            reference = reference.SubstringAfter("u:");
            return userGroups.OfType<User>().SingleOrDefault(
                u => md5.ComputeHashAsString(Encoding.UTF8.GetBytes(u.Email)) == reference
            );
        }

        public string GetReference(IUserGroup userGroup) {
            var concreteUserGroup = userGroup as UserGroup;
            if (concreteUserGroup != null)
                return userGroup.Name;

            var user = userGroup as User;
            if (user != null)
                return "u:" + md5.ComputeHashAsString(Encoding.UTF8.GetBytes(user.Email));

            throw new NotSupportedException();
        }

        #region IDisposable Members

        public void Dispose() {
            this.md5.Dispose();
        }

        #endregion
    }
}
