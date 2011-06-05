using System;
using System.Collections.Generic;
using System.Linq;
using AshMind.Extensions;
using AshMind.IO.Abstraction;
using AshMind.Gallery.Core.Metadata;
using AshMind.Gallery.Core.Security.Actions;

namespace AshMind.Gallery.Core.Security.Internal {
    internal class JsonLocationPermissionProvider : AbstractPermissionProvider<ViewAction<ILocation>> {
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

        public override IEnumerable<IUserGroup> GetPermissions(ViewAction<ILocation> action) {
            var permissionSet = this.metadataProvider.GetMetadata<IDictionary<string, IList<string>>>(action.Target, PermissionsMetadataKey);
            if (permissionSet == null)
                return Enumerable.Empty<IUserGroup>();

            var lazyUserGroups = new Lazy<IList<IUserGroup>>(() => this.userGroupRepository.Query().ToList());
            var referenceSupport = getUserGroupReferenceSupport();

            return from key in (permissionSet.GetValueOrDefault("View") ?? new string[0])
                   let @group = referenceSupport.ResolveReference(key, lazyUserGroups.Value)
                   where @group != null
                   select @group;
        }

        public override void SetPermissions(ViewAction<ILocation> action, IEnumerable<IUserGroup> userGroups) {
            var referenceSupport = getUserGroupReferenceSupport();
            var permissionSet = new Dictionary<string, IList<string>> {
                { "View", userGroups.Select(referenceSupport.GetReference).ToList() }
            };

            this.metadataProvider.ApplyMetadata(action.Target, PermissionsMetadataKey, permissionSet);
        }
    }
}
