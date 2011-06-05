using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

using Newtonsoft.Json;

using AshMind.Extensions;

using AshMind.IO.Abstraction;
using AshMind.Gallery.Core.Security.Actions;

namespace AshMind.Gallery.Core.Security.Internal {
    internal class JsonKeyPermissionProvider : AbstractPermissionProvider<ViewAction<Album>> {
        private readonly IFile permissionStore;
        private readonly IRepository<IUserGroup> userGroupRepository;

        private readonly ReaderWriterLockSlim rawPermissionsLock = new ReaderWriterLockSlim();
        private readonly IDictionary<string, IDictionary<string, IList<string>>> rawPermissions = new Dictionary<string, IDictionary<string, IList<string>>>();

        public JsonKeyPermissionProvider(
            IFile permissionStore,
            IRepository<IUserGroup> userGroupRepository
        ) {
            this.permissionStore = permissionStore;
            this.userGroupRepository = userGroupRepository;

            if (!this.permissionStore.Exists)
                return;
            
            JsonConvert.PopulateObject(this.permissionStore.ReadAllText(), rawPermissions);
        }

        public override bool CanGetPermissions(ViewAction<Album> action) {
            return base.CanGetPermissions(action) && action.Target.ProviderData != null;
        }

        public override bool CanSetPermissions(ViewAction<Album> action) {
            return base.CanSetPermissions(action) && this.CanGetPermissions(action);
        }

        public override IEnumerable<IUserGroup> GetPermissions(ViewAction<Album> action) {
            var groupKeys = (IList<string>)null;

            rawPermissionsLock.EnterReadLock();
            try {
                var permissionDictionary = this.rawPermissions.GetValueOrDefault(action.Target.ProviderData.ToString());
                if (permissionDictionary == null)
                    return Enumerable.Empty<IUserGroup>();

                groupKeys = permissionDictionary.GetValueOrDefault("View");
            }
            finally {
                rawPermissionsLock.ExitReadLock();
            }

            if (groupKeys == null)
                return Enumerable.Empty<IUserGroup>();

            return from userGroupKey in groupKeys
                   select this.userGroupRepository.Load(userGroupKey);
        }

        public override void SetPermissions(ViewAction<Album> action, IEnumerable<IUserGroup> userGroups) {
            rawPermissionsLock.EnterWriteLock();
            try {
                var permissionsOfKey = this.rawPermissions.GetValueOrDefault(action.Target.ProviderData.ToString());
                if (permissionsOfKey == null) {
                    permissionsOfKey = new Dictionary<string, IList<string>>();
                    this.rawPermissions.Add(action.Target.ProviderData.ToString(), permissionsOfKey);
                }

                permissionsOfKey["View"] = userGroups.Select(g => (string)userGroupRepository.GetKey(g)).ToList();
                this.permissionStore.WriteAllText(
                    JsonConvert.SerializeObject(rawPermissions, Formatting.Indented)
                );
            }
            finally {
                rawPermissionsLock.ExitWriteLock();
            }
        }
    }
}
