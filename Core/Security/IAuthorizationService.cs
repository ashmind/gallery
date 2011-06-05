using System;
using System.Collections.Generic;
using System.Linq;

using AshMind.Gallery.Core.Security.Actions;

namespace AshMind.Gallery.Core.Security {
    public interface IAuthorizationService {
        bool IsAuthorized(IUser user, ISecurableAction action);
        void AuthorizeTo(ISecurableAction action, IEnumerable<IUserGroup> userGroups);
        IEnumerable<IUserGroup> GetAuthorizedTo(ISecurableAction action);
    }
}
