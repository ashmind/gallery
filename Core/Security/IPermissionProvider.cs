using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AshMind.Web.Gallery.Core.Security {
    public interface IPermissionProvider {
        IEnumerable<Permission> GetPermissions(string token);

        bool CanSetPermissions(string token);
        void SetPermissions(string token, IEnumerable<Permission> permissions);
    }
}
