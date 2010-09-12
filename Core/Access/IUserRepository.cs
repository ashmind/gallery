using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AshMind.Web.Gallery.Core.Access {
    public interface IUserRepository {
        User Load(string email);
    }
}
