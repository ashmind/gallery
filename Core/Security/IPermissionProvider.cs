using System;
using System.Collections.Generic;
using System.Linq;

namespace AshMind.Gallery.Core.Security {
    public interface IPermissionProvider {
        bool CanGetPermissions(object target);
        bool CanSetPermissions(object target);

        IEnumerable<Permission> GetPermissions(object target);
        void SetPermissions(object target, IEnumerable<Permission> permissions);
    }
}
