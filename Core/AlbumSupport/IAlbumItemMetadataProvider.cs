﻿using System;
using System.Collections.Generic;
using System.Linq;

namespace AshMind.Gallery.Core.AlbumSupport {
    public interface IAlbumItemMetadataProvider {
        void LoadMetadataTo(AlbumItem item);
        void SaveMetadata(AlbumItem item);
    }
}
