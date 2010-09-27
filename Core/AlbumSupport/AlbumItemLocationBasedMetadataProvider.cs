using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using AshMind.Extensions;

using AshMind.Gallery.Core.Metadata;
using AshMind.Gallery.Core.Security;

namespace AshMind.Gallery.Core.AlbumSupport {
    public class AlbumItemLocationBasedMetadataProvider : IAlbumItemMetadataProvider {
        #region AlbumItemRawMetadata Class

        private class AlbumItemRawMetadata {
            public AlbumItemRawMetadata() {
                this.DeleteProposals = new List<string>();
            }

            public IList<string> DeleteProposals { get; private set; }
        }

        #endregion

        private readonly ILocationMetadataProvider locationMetadataProvider;
        private readonly IRepository<IUserGroup> userGroupRepository;
        private readonly IUserGroupSecureReferenceStrategy userGroupReferenceSupport;

        public AlbumItemLocationBasedMetadataProvider(
            ILocationMetadataProvider locationMetadataProvider,
            IRepository<IUserGroup> userGroupRepository,
            IUserGroupSecureReferenceStrategy userGroupReferenceSupport
        ) {
            this.locationMetadataProvider = locationMetadataProvider;
            this.userGroupRepository = userGroupRepository;
            this.userGroupReferenceSupport = userGroupReferenceSupport;
        }

        public void LoadMetadataTo(AlbumItem item) {
            var metadata = locationMetadataProvider.GetMetadata<AlbumItemRawMetadata>(
                item.File.Location, item.File.Name
            );
            if (metadata == null)
                return;

            var userGroups = userGroupRepository.Query().ToList();

            item.DeleteProposals.AddRange(
                metadata.DeleteProposals.Select(
                    p => userGroupReferenceSupport.ResolveReference(p, userGroups)
                ).OfType<User>()
            );
        }

        public void SaveMetadata(AlbumItem item) {
            var metadata = locationMetadataProvider.GetMetadata<AlbumItemRawMetadata>(
                item.File.Location, item.File.Name
            );
            if (metadata == null)
                metadata = new AlbumItemRawMetadata();

            metadata.DeleteProposals.Clear();
            metadata.DeleteProposals.AddRange(
                item.DeleteProposals.Select(this.userGroupReferenceSupport.GetReference)
            );

            locationMetadataProvider.ApplyMetadata<AlbumItemRawMetadata>(
                item.File.Location, item.File.Name, metadata
            );
        }
    }
}
