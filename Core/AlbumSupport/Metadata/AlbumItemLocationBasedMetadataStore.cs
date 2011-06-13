using System;
using System.Collections.Generic;
using System.Linq;

using AshMind.Gallery.Core.Metadata;
using AshMind.Gallery.Core.Security;

namespace AshMind.Gallery.Core.AlbumSupport.Metadata {
    public class AlbumItemLocationBasedMetadataStore : IMetadataStore<AlbumItem> {
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

        public AlbumItemLocationBasedMetadataStore(
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

            item.ProposedToBeDeletedBy = (
                metadata.DeleteProposals.Select(
                    p => userGroupReferenceSupport.ResolveReference(p, userGroups)
                ).OfType<KnownUser>()
            ).FirstOrDefault();
        }

        public void SaveMetadata(AlbumItem item) {
            var metadata = locationMetadataProvider.GetMetadata<AlbumItemRawMetadata>(
                item.File.Location, item.File.Name
            );
            if (metadata == null)
                metadata = new AlbumItemRawMetadata();

            metadata.DeleteProposals.Clear();
            if (item.ProposedToBeDeletedBy != null)
                metadata.DeleteProposals.Add(this.userGroupReferenceSupport.GetReference(item.ProposedToBeDeletedBy));

            locationMetadataProvider.ApplyMetadata(item.File.Location, item.File.Name, metadata);
        }
    }
}
