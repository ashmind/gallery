using System;
using System.Collections.Generic;
using System.Linq;

using AshMind.Gallery.Core.Security.Actions;

namespace AshMind.Gallery.Core.Security.Rules {
    public interface IAuthorizationRule {
        bool? IsAuthorized(IUserGroup group, ISecurableAction action);
    }
}
