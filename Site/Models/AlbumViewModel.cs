using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Web;

using AshMind.Extensions;

using AshMind.Gallery.Core;
using AshMind.Gallery.Core.Security;

namespace AshMind.Gallery.Site.Models {
    public class AlbumViewModel {
        public AlbumViewModel(
            Album album, Func<Album, string> getAlbumID
        ) {
            this.ID = getAlbumID(album);
            this.Album = album;
            this.GetAdditionalAlbumID = getAlbumID;
            this.VisibleToGroups = new UserGroupViewModel[0].AsReadOnly();
        }

        public AlbumViewModel(
            Album album, Func<Album, string> getAlbumID,
            bool canManageSecurity, IList<UserGroupViewModel> visibleToGroups            
        ) {
            if (!canManageSecurity)
                throw new ArgumentException();

            this.ID = getAlbumID(album);
            this.Album = album;
            this.GetAdditionalAlbumID = getAlbumID;
            this.CanManageSecurity = canManageSecurity;
            this.VisibleToGroups = visibleToGroups.AsReadOnly();
        }

        public string ID { get; private set; }
        public Album Album { get; private set; }

        public bool CanManageSecurity { get; private set; }
        public ReadOnlyCollection<UserGroupViewModel> VisibleToGroups { get; private set; }

        public Func<Album, string> GetAdditionalAlbumID { get; private set; }
    }
}