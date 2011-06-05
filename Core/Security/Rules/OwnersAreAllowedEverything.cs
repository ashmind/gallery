using System;
using System.Collections.Generic;
using System.Linq;

using AshMind.Gallery.Core.Security.Actions;

namespace AshMind.Gallery.Core.Security.Rules {
    public class OwnersAreAllowedEverything : IAuthorizationRule {
        public const string Owners = "*Owners";

        public Authorization GetAuthorization(IUserGroup group, ISecurableAction action) {
            return group.Name == Owners ? Authorization.UndeniablyAllowed : Authorization.Unknown;
        }
    }
}
