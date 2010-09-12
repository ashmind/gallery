using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AshMind.Web.Gallery.Core.Security {
    public class Permission {
        public Permission() {
        }

        public string TargetTag         { get; set; }
        public SecurableAction Action   { get; set; }
        public PermissionStatus Status  { get; set; }
    }
}
