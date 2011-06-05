using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

using AshMind.Extensions;

using AshMind.Gallery.Core.Internal;

namespace AshMind.Gallery.Core.Security.Internal {
    internal class MD5UserGroupSecureReferenceStrategy : IUserGroupSecureReferenceStrategy {
        public IUserGroup ResolveReference(string reference, IEnumerable<IUserGroup> userGroups) {
            if (!reference.StartsWith("u:"))
                return userGroups.OfType<UserGroup>().SingleOrDefault(g => g.Name == reference);

            reference = reference.SubstringAfter("u:");
            using (var md5 = MD5.Create()) {
                return userGroups.OfType<KnownUser>().SingleOrDefault(
                    u => md5.ComputeHashAsString(Encoding.UTF8.GetBytes(u.Email)) == reference
                );
            }
        }

        public string GetReference(IUserGroup userGroup) {
            var concreteUserGroup = userGroup as UserGroup;
            if (concreteUserGroup != null)
                return userGroup.Name;

            var user = userGroup as KnownUser;
            if (user != null) {
                using (var md5 = MD5.Create()) {
                    return "u:" + md5.ComputeHashAsString(Encoding.UTF8.GetBytes(user.Email));
                }
            }

            throw new NotSupportedException();
        }
    }
}
