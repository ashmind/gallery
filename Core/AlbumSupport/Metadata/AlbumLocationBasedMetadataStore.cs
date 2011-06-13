using System;
using System.Collections.Generic;
using System.Linq;

using AshMind.Extensions;

using AshMind.Gallery.Core.Albums;
using AshMind.Gallery.Core.Metadata;
using AshMind.Gallery.Core.Security;

namespace AshMind.Gallery.Core.AlbumSupport.Metadata {
    public class AlbumLocationBasedMetadataStore : IMetadataStore<FileSystemAlbum> {
        private static class Keys {
            public const string ViewedBy = "ViewedBy";
        }

        private readonly ILocationMetadataProvider locationMetadataProvider;
        private readonly IRepository<IUserGroup> userGroupRepository;
        private readonly IUserGroupSecureReferenceStrategy userReferenceSupport;

        public AlbumLocationBasedMetadataStore(
            ILocationMetadataProvider locationMetadataProvider,
            IRepository<IUserGroup> userGroupRepository,
            IUserGroupSecureReferenceStrategy userReferenceSupport
        ) {
            this.locationMetadataProvider = locationMetadataProvider;
            this.userGroupRepository = userGroupRepository;
            this.userReferenceSupport = userReferenceSupport;
        }

        public void LoadMetadataTo(FileSystemAlbum target) {
            var viewedBy = locationMetadataProvider.GetMetadata<string[]>(target.Location, Keys.ViewedBy);
            if (viewedBy == null)
                return;

            var userGroups = userGroupRepository.Query().ToList();
            target.ViewedBy.Clear();
            target.ViewedBy.AddRange(
                viewedBy.Select(key => this.userReferenceSupport.ResolveReference(key, userGroups))
                        .Cast<IUser>()
            );
        }

        public void SaveMetadata(FileSystemAlbum target) {
            locationMetadataProvider.ApplyMetadata(
                target.Location, Keys.ViewedBy, target.ViewedBy.Select(user => this.userReferenceSupport.GetReference(user))
            );
        }
    }
}
