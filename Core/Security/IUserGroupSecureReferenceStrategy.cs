using System;
using System.Collections.Generic;
using System.Linq;

namespace AshMind.Gallery.Core.Security {
    public interface IUserGroupSecureReferenceStrategy {
        IUserGroup ResolveReference(string reference, IEnumerable<IUserGroup> userGroups);
        string GetReference(IUserGroup userGroup);
    }
}
