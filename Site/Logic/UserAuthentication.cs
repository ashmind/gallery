using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Web;
using System.Web.Security;

using AshMind.Extensions;

using AshMind.Gallery.Core;
using AshMind.Gallery.Core.Security;

namespace AshMind.Gallery.Site.Logic {
    public class UserAuthentication {
        private static string GroupMemberNamePrefix = "anonymous-member-of:";

        private readonly IRepository<User> userRepository;
        private readonly IRepository<UserGroup> userGroupRepository;

        public UserAuthentication(
            IRepository<User> userRepository,
            IRepository<UserGroup> userGroupRepository
        ) {
            this.userRepository = userRepository;
            this.userGroupRepository = userGroupRepository;
        }

        public bool AuthenticateByKey(string key) {
            if (key.IsNullOrEmpty())
                return false;

            var group = this.userGroupRepository.Query().SingleOrDefault(u => u.Keys.Contains(key));
            if (group == null)
                return false;

            var groupMemberKey = GroupMemberNamePrefix + this.userGroupRepository.GetKey(group);
            FormsAuthentication.SetAuthCookie(groupMemberKey, false);
            return true;
        }

        public bool AuthenticateByEmail(string email) {
            var user = this.userRepository.FindByEmail(email);
            if (user == null)
                return false;

            FormsAuthentication.SetAuthCookie((string)this.userRepository.GetKey(user), false);
            return true;
        }

        public IUser GetUser(IPrincipal principal) {
            if (!principal.Identity.IsAuthenticated)
                return null;

            var userName = principal.Identity.Name;
            if (userName.StartsWith(GroupMemberNamePrefix))
                return new AnonymousMember(this.userGroupRepository.Load(userName.SubstringAfter(GroupMemberNamePrefix)));

            return this.userRepository.Load(userName);
        }
    }
}