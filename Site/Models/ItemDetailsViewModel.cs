using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using AshMind.Gallery.Core;
using AshMind.Gallery.Core.Security;

namespace AshMind.Gallery.Site.Models {
    public class ItemDetailsViewModel {
        public ItemDetailsViewModel(string albumID, AlbumItem item, IUser currentUser) {
            this.AlbumID = albumID;
            this.Item = item;
            this.CurrentUser = currentUser;
        }

        public string AlbumID   { get; private set; }
        public AlbumItem Item   { get; private set; }
        public IUser CurrentUser { get; private set; }
    }
}