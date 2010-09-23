using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AshMind.Gallery.Core.Security {
    public class Permission {
        public Permission() {
        }

        public IUserGroup Group         { get; set; }
        public SecurableAction Action   { get; set; }
    }
}
