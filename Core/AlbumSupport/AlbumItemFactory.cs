using System;
using System.Collections.Generic;
using System.Linq;

using AshMind.Extensions;

using AshMind.IO.Abstraction;

using AshMind.Gallery.Core.Metadata;
using AshMind.Gallery.Imaging;

namespace AshMind.Gallery.Core.AlbumSupport {
    public class AlbumItemFactory {
        private readonly ISet<string> imageExtensions;
        private readonly IMetadataStore<AlbumItem>[] metadataStores;

        public AlbumItemFactory(
            IEnumerable<IImageFormat> imageFormats,
            IMetadataStore<AlbumItem>[] metadataStores
        ) {
            this.imageExtensions = imageFormats.SelectMany(f => f.FileExtensions).ToSet(StringComparer.InvariantCultureIgnoreCase);
            this.metadataStores = metadataStores;
        }

        public AlbumItemType GetItemType(IFile file) {
            return this.imageExtensions.Contains(file.Extension)
                 ? AlbumItemType.Image
                 : AlbumItemType.Unknown;
        }

        public AlbumItem CreateFrom(IFile file, AlbumItemType itemType) {
            var item = new AlbumItem(
                file,
                file.Name,
                itemType,
                file.GetLastWriteTime()
            );
            foreach (var store in this.metadataStores) {
                store.LoadMetadataTo(item);
            }

            return item;
        }
    }
}
