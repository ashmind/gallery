using System;
using System.Collections.Generic;
using System.Linq;

namespace AshMind.Gallery.Core.Security.Actions {
    public static class SecurableActions {
        public static ISecurableAction ManageSecurity { get; private set; }

        static SecurableActions() {
            ManageSecurity = new ManageSecurityAction();
        }

        public static ISecurableAction View<T>(T target) {
            return new ViewAction<T>(target);
        }
    }
}
