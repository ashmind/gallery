using System;
using System.Collections.Generic;
using System.Linq;

namespace AshMind.Gallery.Core.AlbumSupport {
    public class AlbumFactory {
        private readonly IAlbumNameTransform[] nameTransforms;

        public AlbumFactory(IAlbumNameTransform[] nameTransforms) {
            this.nameTransforms = nameTransforms;
        }

        public string GetAlbumName(string proposedName, AlbumDescriptor descriptor) {
            return nameTransforms.Aggregate(proposedName, (n, t) => t.Transform(n, descriptor));
        }

        public TAlbum Process<TAlbum>(TAlbum album) 
            where TAlbum : Album
        {
            return album;
        }
    }
}
