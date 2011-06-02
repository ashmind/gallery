using System;
using System.Collections.Generic;
using System.Linq;

namespace AshMind.Gallery.Core.Security {
    public class Permission {
        public IUserGroup Group         { get; set; }
        public SecurableAction Action   { get; set; }
    }
}
