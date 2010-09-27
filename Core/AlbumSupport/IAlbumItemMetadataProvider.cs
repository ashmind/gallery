using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AshMind.Gallery.Core.AlbumSupport {
    public interface IAlbumItemMetadataProvider {
        void LoadMetadataTo(AlbumItem item);
        void SaveMetadata(AlbumItem item);
    }
}
