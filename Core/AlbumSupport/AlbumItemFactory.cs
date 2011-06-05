using System;
using System.Collections.Generic;
using System.Linq;

using AshMind.IO.Abstraction;

namespace AshMind.Gallery.Core.AlbumSupport {
    public class AlbumItemFactory {
        private readonly IAlbumItemMetadataProvider[] metadataProviders;

        public AlbumItemFactory(IAlbumItemMetadataProvider[] metadataProviders) {
            this.metadataProviders = metadataProviders;
        }

        public AlbumItem CreateFrom(IFile file, AlbumItemType itemType) {
            var item = new AlbumItem(
                file,
                file.Name,
                itemType,
                file.GetLastWriteTime()
            );
            foreach (var provider in this.metadataProviders) {
                provider.LoadMetadataTo(item);
            }

            return item;
        }
    }
}
