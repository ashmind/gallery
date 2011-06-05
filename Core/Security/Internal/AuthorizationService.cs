using System;
using System.Collections.Generic;
using System.Linq;

using AshMind.Extensions;
using AshMind.Gallery.Core.Security.Actions;
using AshMind.Gallery.Core.Security.Rules;

namespace AshMind.Gallery.Core.Security.Internal {
    public class AuthorizationService : IAuthorizationService {
        private readonly IRepository<UserGroup> userGroupRepository;
        private readonly IAuthorizationRule[] rules;
        private readonly IPermissionProvider[] providers;

        public AuthorizationService(
            IRepository<UserGroup> userGroupRepository,
            IAuthorizationRule[] rules,
            IPermissionProvider[] providers
        ) {
            this.userGroupRepository = userGroupRepository;
            this.rules = rules;
            this.providers = providers;
        }

        public IEnumerable<IUserGroup> GetAuthorizedTo(ISecurableAction action) {
            var groups = Enumerable.Union(
                from @group in this.userGroupRepository.Query().ToList()
                where this.rules.Any(r => r.IsAuthorized(@group, action) == true)
                select @group,

                from provider in this.providers
                where provider.CanGetPermissions(action)
                from @group in provider.GetPermissions(action)
                select @group
            );

            return groups;
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
