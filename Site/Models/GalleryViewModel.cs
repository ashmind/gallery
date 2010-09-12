using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Web;

using AshMind.Web.Gallery.Core;

namespace AshMind.Web.Gallery.Site.Models {
    public class GalleryViewModel {
        public GalleryViewModel(
            IList<GalleryAlbum> albums,
            GalleryAlbum selectedAlbum
        ) {
            this.Albums = new ReadOnlyCollection<GalleryAlbum>(albums);
            this.SelectedAlbum = selectedAlbum;
        }

        public ReadOnlyCollection<GalleryAlbum> Albums { get; private set; }
        public GalleryAlbum SelectedAlbum { get; private set; }

        public bool IsSelected(GalleryAlbum folder) {
            return folder == this.SelectedAlbum;
        }
    }
}