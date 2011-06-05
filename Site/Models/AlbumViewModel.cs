using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

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

            this.ID = getAlbumID(album);
            this.Name = album.Name;
            this.Album = album;
            this.Date = album.Date;
            this.CanManageSecurity = canManageSecurity;
            this.VisibleToGroups = (visibleToGroups ?? new UserGroupViewModel[0]).AsReadOnly();
            this.CurrentUser = currentUser;

            var itemModels = album.Items.Value.ToLookup(item => item.IsProposedToBeDeleted);
            this.Items = itemModels[false].Select(item => ToItemModel(item, getAlbumID, imageAccess)).ToList().AsReadOnly();
            this.ProposedToBeDeleted = itemModels[true]
                                            .GroupBy(
                                                item => item.ProposedToBeDeletedBy,
                                                item => ToItemModel(item, getAlbumID, imageAccess),
                                                (key, models) => new DeleteProposalGroupModel(key, models.ToSet())
                                            )
                                            .ToList()
                                            .AsReadOnly();
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
        public string Name { get; private set; }
        internal Album Album { get; private set; }
        public DateTimeOffset Date { get; private set; }
        public IUser CurrentUser { get; private set; }
        public ReadOnlyCollection<AlbumItemModel> Items { get; private set; }
        public ReadOnlyCollection<DeleteProposalGroupModel> ProposedToBeDeleted { get; private set; }

        public bool CanManageSecurity { get; private set; }
        public ReadOnlyCollection<UserGroupViewModel> VisibleToGroups { get; private set; }
    }
}