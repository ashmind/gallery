using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

using AshMind.Extensions;

namespace AshMind.Gallery.Site.Models {
    public class GalleryViewModel {
        public GalleryViewModel(
            AlbumViewModel currentUserAlbum,
            IList<AlbumViewModel> otherPeopleAlbums,
            AlbumListViewModel standardAlbums,
            AlbumViewModel selected
        ) {
            this.CurrentUserAlbum = currentUserAlbum;
            this.OtherPeopleAlbums = otherPeopleAlbums.AsReadOnly();
            this.StandardAlbums = standardAlbums;
            this.Selected = selected;
        }

        public AlbumViewModel CurrentUserAlbum      { get; private set; }
        public ReadOnlyCollection<AlbumViewModel> OtherPeopleAlbums { get; private set; }
        public AlbumListViewModel StandardAlbums    { get; private set; }
        public AlbumViewModel Selected              { get; private set; }

        public bool IsSelected(AlbumViewModel albumModel) {
            return this.Selected != null
                && this.Selected == albumModel;
        }
    }
}