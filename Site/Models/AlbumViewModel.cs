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
            IImageRequestStrategy imageAccess
        ) {
            this.ID = getAlbumID(album);
            this.Name = album.Name;
            this.Album = album;
            this.Date = album.Date;
            this.VisibleToGroups = new UserGroupViewModel[0].AsReadOnly();
            this.CurrentUser = currentUser;

            var itemModels = album.Items.Value.ToLookup(item => item.IsProposedToBeDeleted);
            this.Items = itemModels[false].Select((item, index) => ToItemModel(item, index, getAlbumID, imageAccess)).ToList().AsReadOnly();
            this.ProposedToBeDeleted = itemModels[true]
                                            .GroupBy(
                                                item => item.ProposedToBeDeletedBy,
                                                item => ToItemModel(item, 1000, getAlbumID, imageAccess),
                                                (key, models) => new DeleteProposalGroupModel(key, models.ToSet())
                                            )
                                            .ToList()
                                            .AsReadOnly();
        }

        private AlbumItemModel ToItemModel(AlbumItem item, int itemIndex, Func<Album, string> getAlbumID, IImageRequestStrategy imageAccess) {
            return new AlbumItemModel(
                item, itemIndex, this.ID,
                item.PrimaryAlbum.Get(getAlbumID).Value,
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

        public void SetupSecurityManagement(IEnumerable<UserGroupViewModel> visibleToGroups) {
            this.CanManageSecurity = true;
            this.VisibleToGroups = visibleToGroups.ToList().AsReadOnly();
        }
    }
}