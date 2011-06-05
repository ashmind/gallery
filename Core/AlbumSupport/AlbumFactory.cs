using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using AshMind.Gallery.Core.Values;

namespace AshMind.Gallery.Core.AlbumSupport {
    public class AlbumFactory {
        private readonly IAlbumNameTransform[] nameTransforms;

        public AlbumFactory(IAlbumNameTransform[] nameTransforms) {
            this.nameTransforms = nameTransforms;
        }

        public Album Create(AlbumDescriptor descriptor, string proposedName, object providerData, IValue<IEnumerable<AlbumItem>> items) {
            var name = nameTransforms.Aggregate(proposedName, (n, t) => t.Transform(n, descriptor));
            return new Album(descriptor, name, providerData, items);
        }
    }
}
