using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AshMind.Gallery.Core.Security {
    public abstract class AbstractPermissionProvider<TTarget> : IPermissionProvider {
        public abstract IEnumerable<Permission> GetPermissions(TTarget target);
        public abstract void SetPermissions(TTarget target, IEnumerable<Permission> permissions);

        #region IPermissionProvider Members

        IEnumerable<Permission> IPermissionProvider.GetPermissions(object target) {
            return this.GetPermissions((TTarget)target);
        }

        void IPermissionProvider.SetPermissions(object target, IEnumerable<Permission> permissions) {
            this.SetPermissions((TTarget)target, permissions);
        }

        bool IPermissionProvider.CanGetOrSetPermissions(object target) {
            return target is TTarget;
        }

        #endregion
    }
}
