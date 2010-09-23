using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading;

using Newtonsoft.Json;

using AshMind.Extensions;

using AshMind.Gallery.Core.Internal;
using AshMind.Gallery.Core.IO;
using AshMind.Gallery.Core.Security;

namespace AshMind.Gallery.Core.Security.Internal {
    internal class JsonKeyPermissionProvider : AbstractPermissionProvider<SecurableUniqueKey> {
        private readonly IFile permissionStore;
        private readonly IRepository<IUserGroup> userGroupRepository;

        private readonly ReaderWriterLockSlim rawPermissionsLock = new ReaderWriterLockSlim();
        private readonly IDictionary<string, IDictionary<SecurableAction, IList<string>>> rawPermissions = new Dictionary<string, IDictionary<SecurableAction, IList<string>>>();

        public JsonKeyPermissionProvider(
            IFile permissionStore,
            IRepository<IUserGroup> userGroupRepository
        ) {
            this.permissionStore = permissionStore;
            this.userGroupRepository = userGroupRepository;

            if (this.permissionStore.Exists)
                JsonConvert.PopulateObject(this.permissionStore.ReadAllText(), rawPermissions);
        }

        public override IEnumerable<Permission> GetPermissions(SecurableUniqueKey key) {
            var permissionsOfKey = (IDictionary<SecurableAction, IList<string>>)null;

            rawPermissionsLock.EnterReadLock();
            try {
                permissionsOfKey = this.rawPermissions.GetValueOrDefault(key.Value);
            }
            finally {
                rawPermissionsLock.ExitReadLock();
            }

            if (permissionsOfKey == null)
                return Enumerable.Empty<Permission>();

            return from rawPermission in permissionsOfKey
                   from userGroupKey in rawPermission.Value
                   select new Permission {
                       Action = rawPermission.Key,
                       Group = this.userGroupRepository.Load(userGroupKey)
                   };
        }

        public override void SetPermissions(SecurableUniqueKey key, IEnumerable<Permission> permissions) {
            rawPermissionsLock.EnterWriteLock();
            try {
                var permissionsOfKey = this.rawPermissions.GetValueOrDefault(key.Value);
                if (permissionsOfKey == null) {
                    permissionsOfKey = new Dictionary<SecurableAction, IList<string>>();
                    this.rawPermissions.Add(key.Value, permissionsOfKey);
                }

                foreach (var permissionGroup in permissions.GroupBy(p => p.Action)) {
                    var permissionsOfAction = permissionsOfKey.GetValueOrDefault(permissionGroup.Key);
                    if (permissionsOfAction == null) {
                        permissionsOfAction = new List<string>();
                        permissionsOfKey.Add(permissionGroup.Key, permissionsOfAction);
                    }

                    permissionsOfAction.AddRange(
                        permissionGroup.Select(p => (string)userGroupRepository.GetKey(p.Group))
                    );
                }

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
