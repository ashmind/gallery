using System;
using System.Collections.Generic;
using System.Linq;

using AshMind.Gallery.Core.AlbumSupport;
using AshMind.Gallery.Core.Values;

using AshMind.IO.Abstraction;

namespace AshMind.Gallery.Core.Albums {
    public class FileSystemAlbum : Album {
        public FileSystemAlbum(AlbumDescriptor descriptor, string name, ILocation location, IValue<IEnumerable<AlbumItem>> items)
            : base(descriptor, name, items)
        {
            this.Location = location;
        }

        public ILocation Location { get; private set; }

        protected override Album Recreate(IValue<IList<AlbumItem>> items) {
            return new FileSystemAlbum(this.Descriptor, this.Name, this.Location, items);
        }
    }
}
