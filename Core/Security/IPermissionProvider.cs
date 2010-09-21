using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AshMind.Web.Gallery.Core.Security {
    public interface IPermissionProvider {
        bool CanGetOrSetPermissions(object target);

        IEnumerable<Permission> GetPermissions(object target);
        void SetPermissions(object target, IEnumerable<Permission> permissions);
    }
}
