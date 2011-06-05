using System;
using System.Collections.Generic;
using System.Linq;

using AshMind.Gallery.Core.Security.Actions;

namespace AshMind.Gallery.Core.Security {
    public interface IPermissionProvider {
        bool CanGetPermissions(ISecurableAction action);
        bool CanSetPermissions(ISecurableAction action);

        IEnumerable<IUserGroup> GetPermissions(ISecurableAction action);
        void SetPermissions(ISecurableAction action, IEnumerable<IUserGroup> userGroups);
    }
}
