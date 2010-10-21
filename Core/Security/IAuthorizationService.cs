using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AshMind.Gallery.Core.Security {
    public interface IAuthorizationService {
        bool IsAuthorized(IUser user, SecurableAction action, object target);
        void AuthorizeTo(SecurableAction action, object target, IEnumerable<IUserGroup> userGroups);
        IEnumerable<IUserGroup> GetAuthorizedTo(SecurableAction action, object target);
    }
}
