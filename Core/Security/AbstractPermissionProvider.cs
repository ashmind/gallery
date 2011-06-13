using System;
using System.Collections.Generic;
using System.Linq;
using AshMind.Gallery.Core.Security.Actions;

namespace AshMind.Gallery.Core.Security {
    public abstract class AbstractPermissionProvider<TAction> : IPermissionProvider
        where TAction : ISecurableAction
    {
        public virtual bool CanGetPermissions(TAction action) {
            return true;
        }

        public virtual bool CanSetPermissions(TAction action) {
            return true;
        }

        public abstract IEnumerable<IUserGroup> GetPermissions(TAction action);
        public abstract void SetPermissions(TAction action, IEnumerable<IUserGroup> userGroups);

        public virtual int Priority {
            get { return 0; }
        }

        #region IPermissionProvider Members

        IEnumerable<IUserGroup> IPermissionProvider.GetPermissions(ISecurableAction action) {
            return this.GetPermissions((TAction)action);
        }

        void IPermissionProvider.SetPermissions(ISecurableAction action, IEnumerable<IUserGroup> userGroups) {
            this.SetPermissions((TAction)action, userGroups);
        }

        bool IPermissionProvider.CanGetPermissions(ISecurableAction action) {
            return action is TAction
                && this.CanGetPermissions((TAction)action);
        }

        bool IPermissionProvider.CanSetPermissions(ISecurableAction action) {
            return action is TAction
                && this.CanSetPermissions((TAction)action);
        }

        #endregion
    }
}
