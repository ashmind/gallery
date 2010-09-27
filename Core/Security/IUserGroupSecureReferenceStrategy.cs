using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using AshMind.Gallery.Core.Security;

namespace AshMind.Gallery.Core.Security {
    public interface IUserGroupSecureReferenceStrategy {
        IUserGroup ResolveReference(string reference, IEnumerable<IUserGroup> userGroups);
        string GetReference(IUserGroup userGroup);
    }
}
