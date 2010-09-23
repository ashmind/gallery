﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AshMind.Gallery.Core.Security {
    public interface IUserGroup {
        string Name { get; }
        HashSet<User> GetUsers();
    }
}
