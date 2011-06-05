using System;
using System.Collections.Generic;
using System.Linq;

using AshMind.Gallery.Core.Security.Actions;

namespace AshMind.Gallery.Core.Security.Rules {
    // temporary rule that essentially says that item authorization is actually album-based
    public class EveryoneCanSeeAllAlbumItems : IAuthorizationRule {
        public Authorization GetAuthorization(IUserGroup group, ISecurableAction action) {
            return action is ViewAction<AlbumItem> ? Authorization.Allowed : Authorization.Unknown;
        }
    }
}
