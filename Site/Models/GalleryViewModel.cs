using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Web;

using AshMind.Extensions;

using AshMind.Web.Gallery.Core;

namespace AshMind.Web.Gallery.Site.Models {
    public class GalleryViewModel {
        public GalleryViewModel(
            AlbumViewModel currentUserAlbum,
            IList<AlbumViewModel> otherPersonAlbums,
            AlbumListViewModel standardAlbums,
            AlbumViewModel selected
        ) {
            this.CurrentUserAlbum = currentUserAlbum;
            this.OtherPersonAlbums = otherPersonAlbums.AsReadOnly();
            this.StandardAlbums = standardAlbums;
            this.Selected = selected;
        }

        public AlbumViewModel CurrentUserAlbum      { get; private set; }
        public ReadOnlyCollection<AlbumViewModel> OtherPersonAlbums { get; private set; }
        public AlbumListViewModel StandardAlbums    { get; private set; }
        public AlbumViewModel Selected              { get; private set; }

        public bool IsSelected(Album album) {
            return this.Selected != null
                && this.Selected.Album == album;
        }
    }
}