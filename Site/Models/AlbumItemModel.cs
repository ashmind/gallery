using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Routing;

using AshMind.Gallery.Core;
using AshMind.Gallery.Core.Security;

namespace AshMind.Gallery.Site.Models {
    public class AlbumItemModel {
        private readonly AlbumItem item;

        public AlbumItemModel(AlbumItem item, int itemIndex, string currentAlbumID, string primaryAlbumID, Func<RequestContext, string> getImageUrlBase, IUser currentUser) {
            this.item = item;
            this.ItemIndex = itemIndex;
            this.CurrentAlbumID = currentAlbumID;
            this.PrimaryAlbumID = primaryAlbumID;
            this.GetImageUrlBase = getImageUrlBase;

            this.WasProposedToBeDeletedByCurrentUser = item.ProposedToBeDeletedBy == currentUser;
        }

        public int ItemIndex { get; private set; }

        public string PrimaryAlbumID { get; private set; }
        public string CurrentAlbumID { get; private set; }

        public string Name {
            get { return item.Name; }
        }

        public DateTimeOffset Date {
            get { return item.Date; }
        }

        public Func<RequestContext, string> GetImageUrlBase { get; private set; }

        public bool IsProposedToBeDeleted {
            get { return item.IsProposedToBeDeleted; }
        }

        public bool WasProposedToBeDeletedByCurrentUser { get; private set; }
    }
}