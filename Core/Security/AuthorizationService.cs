using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using AshMind.Extensions;

namespace AshMind.Web.Gallery.Core.Security {
    public class AuthorizationService {
        private readonly HashSet<User> superUsers = new HashSet<User>();
        private readonly IDictionary<User, IList<Permission>> effectivePermissions = new Dictionary<User, IList<Permission>>();

        public AuthorizationService(IRepository<UserGroup> userGroupRepository) {
            foreach (var group in userGroupRepository.Query()) {
                if (group.IsSuper) {
                    superUsers.AddRange(group.Users);
                    continue;
                }

                foreach (var user in group.Users) {
                    var permissions = effectivePermissions.GetValueOrDefault(user);
                    if (permissions == null) {
                        permissions = new List<Permission>();
                        permissions.AddRange(user.Permissions);
                        effectivePermissions.Add(user, permissions);
                    }

                    permissions.AddRange(group.Permissions);
                }
            }

            foreach (var pair in effectivePermissions) {
                var denied = pair.Value.Where(p => p.Status == PermissionStatus.Deny).ToArray();
                pair.Value.RemoveWhere(p => denied.Any(d => d.Action == p.Action && d.TargetTag == p.TargetTag));
            }
        }

        public bool IsAuthorized(User user, SecurableAction action, params string[] targetTags) {
            var targetTagSet = targetTags.ToSet();
            if (superUsers.Contains(user))
                return true;

            return effectivePermissions[user].Any(p => p.Action == action && targetTagSet.Contains(p.TargetTag));
        }
    }
}
