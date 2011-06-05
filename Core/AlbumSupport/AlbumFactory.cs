﻿using System;
using System.Collections.Generic;
using System.Linq;

namespace AshMind.Gallery.Core.AlbumSupport {
    public class AlbumFactory {
        private readonly IAlbumNameTransform[] nameTransforms;

        public AlbumFactory(IAlbumNameTransform[] nameTransforms) {
            this.nameTransforms = nameTransforms;
        }

        public Album Create(AlbumDescriptor descriptor, string proposedName, object providerData, Func<IList<AlbumItem>> itemsFactory) {
            var name = nameTransforms.Aggregate(proposedName, (n, t) => t.Transform(n, descriptor));
            return new Album(descriptor, name, providerData, itemsFactory);
        }
    }
}
