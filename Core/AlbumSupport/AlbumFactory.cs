using System;
using System.Collections.Generic;
using System.Linq;

using AshMind.Extensions;
using AshMind.Gallery.Core.Metadata;

namespace AshMind.Gallery.Core.AlbumSupport {
    public class AlbumFactory<TAlbum>
        where TAlbum : Album
    {
        private readonly IMetadataStore<TAlbum>[] metadataStores;
        private readonly IAlbumNameTransform[] nameTransforms;

        public AlbumFactory(IMetadataStore<TAlbum>[] metadataStores, IAlbumNameTransform[] nameTransforms) {
            this.metadataStores = metadataStores;
            this.nameTransforms = nameTransforms;
        }

        public string GetAlbumName(string proposedName, AlbumDescriptor descriptor) {
            return nameTransforms.Aggregate(proposedName, (n, t) => t.Transform(n, descriptor));
        }

        public TAlbum Process(TAlbum album) {
            this.metadataStores.ForEach(s => s.LoadMetadataTo(album));
            return album;
        }
    }
}
