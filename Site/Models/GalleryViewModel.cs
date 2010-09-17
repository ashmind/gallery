using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Web;

using AshMind.Web.Gallery.Core;

namespace AshMind.Web.Gallery.Site.Models {
    public class GalleryViewModel {
        public GalleryViewModel(
            AlbumListViewModel albums,
            AlbumViewModel selected
        ) {
            this.Albums = albums;
            this.Selected = selected;
        }

        public AlbumListViewModel Albums { get; private set; }
        public AlbumViewModel Selected { get; private set; }

        public bool IsSelected(Album album) {
            return this.Selected != null
                && this.Selected.Album == album;
        }
    }
}