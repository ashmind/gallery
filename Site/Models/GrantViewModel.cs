using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Web;

using AshMind.Extensions;
using AshMind.Gallery.Core.Security;

namespace AshMind.Gallery.Site.Models {
    public class GrantViewModel {
        private readonly HashSet<IUserGroup> grantedUserGroups;

        public GrantViewModel(
            string albumID,
            HashSet<IUserGroup> grantedUserGroups,
            IList<UserGroupViewModel> allUserGroups
        ) {
            this.AlbumID = albumID;
            this.grantedUserGroups = grantedUserGroups;
            this.AllUserGroups = allUserGroups.AsReadOnly();
        }

        public string AlbumID { get; private set; }
        public ReadOnlyCollection<UserGroupViewModel> AllUserGroups { get; private set; }

        public bool HasAccess(UserGroupViewModel model) {
            return this.grantedUserGroups.Contains(model.UserGroup);
        }
    }
}