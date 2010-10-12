﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AshMind.Gallery.Core.Security {
    public interface IUserGroup {
        string Name { get; }
        IEnumerable<IUser> GetUsers();
    }
}
