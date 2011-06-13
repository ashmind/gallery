using System;
using System.Collections.Generic;
using System.Linq;

using AshMind.IO.Abstraction;

using AshMind.Gallery.Core.Metadata;

namespace AshMind.Gallery.Core.AlbumSupport {
    public class AlbumItemFactory {
        private readonly IMetadataStore<AlbumItem>[] metadataStores;

        public AlbumItemFactory(IMetadataStore<AlbumItem>[] metadataStores) {
            this.metadataStores = metadataStores;
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
