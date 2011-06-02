using System;
using System.Collections.Generic;
using System.Linq;

using AshMind.Gallery.Core.IO;
using AshMind.Gallery.Core.Metadata;

namespace AshMind.Gallery.Core.Security.Internal {
    internal class JsonLocationPermissionProvider : AbstractPermissionProvider<ILocation> {
        private const string PermissionsMetadataKey = "Permissions";

        private readonly ILocationMetadataProvider metadataProvider;
        private readonly IRepository<IUserGroup> userGroupRepository;
        private readonly Func<IUserGroupSecureReferenceStrategy> getUserGroupReferenceSupport;

        public JsonLocationPermissionProvider(
            ILocationMetadataProvider metadataProvider,
            IRepository<IUserGroup> userGroupRepository,
            Func<IUserGroupSecureReferenceStrategy> getUserGroupReferenceSupport
        ) {
            this.metadataProvider = metadataProvider;
            this.userGroupRepository = userGroupRepository;
            this.getUserGroupReferenceSupport = getUserGroupReferenceSupport;
        }

        public override IEnumerable<Permission> GetPermissions(ILocation location) {
            var permissionSet = this.metadataProvider.GetMetadata<Dictionary<SecurableAction, IList<string>>>(
                location, PermissionsMetadataKey
            );
            if (permissionSet == null)
                return Enumerable.Empty<Permission>();

            var lazyUserGroups = new Lazy<IList<IUserGroup>>(() => this.userGroupRepository.Query().ToList());
            var referenceSupport = getUserGroupReferenceSupport();

            return from pair in permissionSet
                   from key in pair.Value
                   let @group = referenceSupport.ResolveReference(key, lazyUserGroups.Value)
                   where @group != null
                   select new Permission {
                       Action = pair.Key,
                       Group = @group
                   };
        }
       
        public override void SetPermissions(ILocation location, IEnumerable<Permission> permissions) {
            var referenceSupport = getUserGroupReferenceSupport();
            var permissionSet = permissions.GroupBy(p => p.Action)
                                            .ToDictionary(
                                                g => g.Key,
                                                g => g.Select(p => referenceSupport.GetReference(p.Group)).ToList()
                                            );

            this.metadataProvider.ApplyMetadata(location, PermissionsMetadataKey, permissionSet);
        }
    }
}
