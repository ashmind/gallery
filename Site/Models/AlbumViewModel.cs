using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Web;

using AshMind.Extensions;

using AshMind.Gallery.Core;
using AshMind.Gallery.Core.Security;
using AshMind.Gallery.Site.Logic;

namespace AshMind.Gallery.Site.Models {
    public class AlbumViewModel {       
        public AlbumViewModel(
            Album album,
            Func<Album, string> getAlbumID,
            IUser currentUser,
            IImageRequestStrategy imageAccess,
            bool canManageSecurity,
            IList<UserGroupViewModel> visibleToGroups
        ) {
            if (!canManageSecurity && visibleToGroups != null && visibleToGroups.Count > 0)
                throw new ArgumentException();

            this.Album = album;
            this.ID = getAlbumID(album);
            this.CanManageSecurity = canManageSecurity;
            this.VisibleToGroups = (visibleToGroups ?? new UserGroupViewModel[0]).AsReadOnly();
            this.CurrentUser = currentUser;

            var itemModels = album.Items.ToLookup(
                item => item.IsProposedToBeDeleted,
                item => ToItemModel(item, getAlbumID, imageAccess)
            );
            this.Items = itemModels[false].ToList().AsReadOnly();
            this.ProposedToBeDeleted = itemModels[true].ToList().AsReadOnly();
        }

        private AlbumItemModel ToItemModel(AlbumItem item, Func<Album, string> getAlbumID, IImageRequestStrategy imageAccess) {
            return new AlbumItemModel(
                item, this.ID,
                item.PrimaryAlbum != null ? getAlbumID(item.PrimaryAlbum) : null,
                request => imageAccess.GetActionUrl(request, this.ID, item.Name),
                this.CurrentUser
            );
        }

        public string ID { get; private set; }
        public Album Album { get; private set; }
        public IUser CurrentUser { get; private set; }
        public ReadOnlyCollection<AlbumItemModel> Items { get; private set; }
        public ReadOnlyCollection<AlbumItemModel> ProposedToBeDeleted { get; private set; }

        public bool CanManageSecurity { get; private set; }
        public ReadOnlyCollection<UserGroupViewModel> VisibleToGroups { get; private set; }
    }
}