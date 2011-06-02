using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;

using AshMind.Gallery.Core.Security;

namespace AshMind.Gallery.Site.Logic {
    public interface IUserAuthentication {
        bool AuthenticateByEmail(string email);
        bool AuthenticateByKey(string key);

        IUser GetUser(IPrincipal principal);
    }
}