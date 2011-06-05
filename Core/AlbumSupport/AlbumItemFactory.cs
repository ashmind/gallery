using System;
using System.Collections.Generic;
using System.Linq;

using AshMind.IO.Abstraction;
using AshMind.Gallery.Core.Commenting;

namespace AshMind.Gallery.Core.AlbumSupport {
    public class AlbumItemFactory {
        private readonly ICommentRepository commentRepository;
        private readonly IAlbumItemMetadataProvider[] metadataProviders;

        public AlbumItemFactory(
            ICommentRepository commentRepository,
            IAlbumItemMetadataProvider[] metadataProviders
        ) {
            this.commentRepository = commentRepository;
            this.metadataProviders = metadataProviders;
        }

        public AlbumItem CreateFrom(IFile file, AlbumItemType itemType) {
            var item = new AlbumItem(
                file,
                file.Name,
                itemType,
                file.GetLastWriteTime(),
                () => this.commentRepository.LoadCommentsOf(file.Path)
            );
            foreach (var provider in this.metadataProviders) {
                provider.LoadMetadataTo(item);
            }

            return item;
        }
    }
}
