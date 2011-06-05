using System;
using System.Collections.Generic;
using System.Linq;

using AshMind.Gallery.Core.Security.Actions;

namespace AshMind.Gallery.Core.Security.Rules {
    public class ProposedToBeDeletedCanBeSeenOnlyByProposedUsers : IAuthorizationRule {
        public Authorization GetAuthorization(IUserGroup group, ISecurableAction action) {
            var viewAlbumItem = action as ViewAction<AlbumItem>;
            if (viewAlbumItem == null || !viewAlbumItem.Target.IsProposedToBeDeleted || !(group is IUser))
                return Authorization.Unknown;

            return viewAlbumItem.Target.ProposedToBeDeletedBy == group ? Authorization.Unknown : Authorization.Denied;
        }
    }
}
