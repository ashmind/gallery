using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Web;

using AshMind.Web.Gallery.Core;

namespace AshMind.Web.Gallery.Site.Models {
    public class GalleryViewModel {
        public GalleryViewModel(
            PagedListViewModel<Album> albums,
            AlbumViewModel selected
        ) {
            this.Albums = albums;
            this.Selected = selected;
        }

        public PagedListViewModel<Album> Albums { get; private set; }
        public AlbumViewModel Selected { get; private set; }

        public bool IsSelected(Album album) {
            return this.Selected != null
                && this.Selected.Album == album;
        }
    }
}