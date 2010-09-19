using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using AshMind.Web.Gallery.Core;
using AshMind.Web.Gallery.Core.Security;

namespace AshMind.Web.Gallery.Site.Models {
    public class ItemViewModel {
        public ItemViewModel(string albumID, AlbumItem item, User currentUser) {
            this.AlbumID = albumID;
            this.Item = item;
            this.CurrentUser = currentUser;
        }

        public string AlbumID   { get; private set; }
        public AlbumItem Item   { get; private set; }
        public User CurrentUser { get; private set; }
    }
}