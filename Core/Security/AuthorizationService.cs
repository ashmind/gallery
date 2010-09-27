using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using AshMind.Extensions;

namespace AshMind.Gallery.Core.Security {
    public class AuthorizationService {
        private readonly HashSet<UserGroup> superGroups = new HashSet<UserGroup>();
        private readonly IPermissionProvider[] providers;

        public AuthorizationService(
            IRepository<UserGroup> userGroupRepository,
            IPermissionProvider[] providers
        ) {
            superGroups.AddRange(userGroupRepository.Query().Where(g => g.IsSuper));
            this.providers = providers;
        }
        
        public IEnumerable<IUserGroup> GetAuthorizedTo(SecurableAction action, object target) {
            foreach (var group in superGroups) {
                yield return group;
            }

            if (action == SecurableAction.ManageSecurity)
                yield break;
            
            var otherGroups = (
                from provider in this.providers
                where provider.CanGetPermissions(target)
                from permission in provider.GetPermissions(target)
                where permission.Action == action
                    && !superGroups.Contains(permission.Group)
                select permission.Group
            ).Distinct();

            foreach (var group in otherGroups) {
                yield return group;
            }
        }

        public bool IsAuthorized(User user, SecurableAction action, object target) {
            return GetAuthorizedTo(action, target)
                        .Any(g => g.GetUsers().Contains(user)) || user.IsSystem;
        }

        public void MakeAuthorizedTo(SecurableAction action, object target, IEnumerable<IUserGroup> userGroups) {
            var permissions = userGroups.Select(group => new Permission {
                Action = action,
                Group = group
            });

            var provider = this.providers.FirstOrDefault(p => p.CanSetPermissions(target));

            if (provider == null)
                throw new NotSupportedException();

            provider.SetPermissions(target, permissions);
        }
    }
}
