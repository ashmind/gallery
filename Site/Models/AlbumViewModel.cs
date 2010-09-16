using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Web;

using AshMind.Extensions;

using AshMind.Web.Gallery.Core;
using AshMind.Web.Gallery.Core.Security;

namespace AshMind.Web.Gallery.Site.Models {
    public class AlbumViewModel {
        public AlbumViewModel(Album album) {
            this.Album = album;
            this.VisibleToGroups = new UserGroupViewModel[0].AsReadOnly();
        }

        public AlbumViewModel(
            Album album, bool canManageSecurity,
            IList<UserGroupViewModel> visibleToGroups
        ) {
            if (!canManageSecurity)
                throw new ArgumentException();

            this.Album = album;
            this.CanManageSecurity = canManageSecurity;
            this.VisibleToGroups = visibleToGroups.AsReadOnly();
        }

        public Album Album { get; private set; }
        public bool CanManageSecurity { get; private set; }
        public ReadOnlyCollection<UserGroupViewModel> VisibleToGroups { get; private set; }
    }
}