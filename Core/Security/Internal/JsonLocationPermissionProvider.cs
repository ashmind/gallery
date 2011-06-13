using System;
using System.Collections.Generic;
using System.Linq;

using AshMind.Extensions;

using AshMind.IO.Abstraction;

using AshMind.Gallery.Core.Albums;
using AshMind.Gallery.Core.Metadata;
using AshMind.Gallery.Core.Security.Actions;

namespace AshMind.Gallery.Core.Security.Internal {
    internal class JsonLocationPermissionProvider : AbstractPermissionProvider<ViewAction<Album>> {
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

        public override bool CanGetPermissions(ViewAction<Album> action) {
            return base.CanGetPermissions(action) && action.Target is FileSystemAlbum;
        }

        public override IEnumerable<IUserGroup> GetPermissions(ViewAction<Album> action) {
            var permissionSet = this.metadataProvider.GetMetadata<IDictionary<string, IList<string>>>(
                ((FileSystemAlbum)action.Target).Location, PermissionsMetadataKey
            );
            if (permissionSet == null)
                return Enumerable.Empty<IUserGroup>();

            var lazyUserGroups = new Lazy<IList<IUserGroup>>(() => this.userGroupRepository.Query().ToList());
            var referenceSupport = getUserGroupReferenceSupport();

            return from key in (permissionSet.GetValueOrDefault("View") ?? new string[0])
                   let @group = referenceSupport.ResolveReference(key, lazyUserGroups.Value)
                   where @group != null
                   select @group;
        }

        public override bool CanSetPermissions(ViewAction<Album> action) {
            return base.CanSetPermissions(action) && action.Target is FileSystemAlbum;
        }

        public override void SetPermissions(ViewAction<Album> action, IEnumerable<IUserGroup> userGroups) {
            var referenceSupport = getUserGroupReferenceSupport();
            var permissionSet = new Dictionary<string, IList<string>> {
                { "View", userGroups.Select(referenceSupport.GetReference).ToList() }
            };

            this.metadataProvider.ApplyMetadata(
                ((FileSystemAlbum)action.Target).Location, PermissionsMetadataKey, permissionSet
            );
        }
    }
}
