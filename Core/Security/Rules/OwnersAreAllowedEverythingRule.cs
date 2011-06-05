using System;
using System.Collections.Generic;
using System.Linq;

using AshMind.Gallery.Core.Security.Actions;

namespace AshMind.Gallery.Core.Security.Rules {
    public class OwnersAreAllowedEverythingRule : IAuthorizationRule {
        public const string Owners = "*Owners";

        public bool? IsAuthorized(IUserGroup group, ISecurableAction action) {
            return group.Name == Owners ? (bool?)true : null;
        }
    }
}
