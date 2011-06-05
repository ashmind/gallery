using System;
using System.Collections.Generic;
using System.Linq;

using AshMind.Extensions;
using AshMind.Gallery.Core.Security.Actions;

namespace AshMind.Gallery.Core.Security.Internal {
    public class AuthorizationService : IAuthorizationService {
        private readonly HashSet<UserGroup> superGroups = new HashSet<UserGroup>();
        private readonly IPermissionProvider[] providers;

        public AuthorizationService(
            IRepository<UserGroup> userGroupRepository,
            IPermissionProvider[] providers
        ) {
            superGroups.AddRange(userGroupRepository.Query().Where(g => g.IsSuper));
            this.providers = providers;
        }
        
        public IEnumerable<IUserGroup> GetAuthorizedTo(ISecurableAction action) {
            foreach (var group in superGroups) {
                yield return group;
            }

            if (action is ManageSecurityAction)
                yield break;
            
            var otherGroups = (
                from provider in this.providers
                where provider.CanGetPermissions(action)
                from @group in provider.GetPermissions(action)
                where !superGroups.Contains(@group)
                select @group
            ).Distinct();

            foreach (var group in otherGroups) {
                yield return group;
            }
        }

        public bool IsAuthorized(IUser user, ISecurableAction action) {
            return GetAuthorizedTo(action)
                        .Any(g => g.GetUsers().Contains(user)) || user == KnownUser.System;
        }

        public void AuthorizeTo(ISecurableAction action, IEnumerable<IUserGroup> userGroups) {
            var provider = this.providers.FirstOrDefault(p => p.CanSetPermissions(action));

            if (provider == null)
                throw new NotSupportedException();

            provider.SetPermissions(action, userGroups);
        }
    }
}
