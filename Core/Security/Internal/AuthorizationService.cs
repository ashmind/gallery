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
            var authorizedByProviders = (
                from provider in this.providers
                where provider.CanGetPermissions(action)
                from @group in provider.GetPermissions(action)
                select @group
            ).ToSet();

            var allGroups = this.userGroupRepository.Query().ToList();
            foreach (var group in allGroups) {
                var groupAuthorization = GetAuthorization(@group, action, authorizedByProviders);
                if (groupAuthorization == Authorization.Denied)
                    continue;

                var allMembers = ((IUserGroup) group).GetUsers().ToArray();
                var authorizedMembers = (
                    from member in allMembers
                    let memberAuthorization = GetAuthorization(member, action, authorizedByProviders)
                    where memberAuthorization != Authorization.Denied
                    where memberAuthorization == Authorization.Allowed || groupAuthorization == Authorization.Allowed
                    select member
                ).ToList();

                if (authorizedMembers.Count == 0)
                    continue;

                if (authorizedMembers.Count == allMembers.Length) {
                    yield return @group;
                    continue;
                }

                foreach (var member in authorizedMembers) {
                    yield return member;
                }
            }
        }

        private Authorization GetAuthorization(IUserGroup @group, ISecurableAction action, HashSet<IUserGroup> authorizedByProviders) {
            var authorization = GetAuthorizationAccordingToRules(group, action);
            if (authorization == Authorization.Unknown && authorizedByProviders.Contains(@group))
                authorization = Authorization.Allowed;

            return authorization;
        }

        private Authorization GetAuthorizationAccordingToRules(IUserGroup @group, ISecurableAction action) {
            var authorization = Authorization.Unknown;
            foreach (var rule in this.rules) {
                var result = rule.GetAuthorization(@group, action);
                if (result == Authorization.Unknown)
                    continue;

                if (rule.OverridesAllOther)
                    return result;

                if (result == Authorization.Allowed && authorization == Authorization.Unknown)
                    authorization = result;

                if (result == Authorization.Denied)
                    authorization = result;
            }

            return authorization;
        }

        public bool IsAuthorized(IUser user, ISecurableAction action) {
            return user == KnownUser.System
                || GetAuthorizedTo(action).Any(g => g.GetUsers().Contains(user));
        }

        public void AuthorizeTo(ISecurableAction action, IEnumerable<IUserGroup> userGroups) {
            var provider = this.providers.FirstOrDefault(p => p.CanSetPermissions(action));

            if (provider == null)
                throw new NotSupportedException();

            provider.SetPermissions(action, userGroups);
        }
    }
}
