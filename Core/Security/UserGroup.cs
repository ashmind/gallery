﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AshMind.Web.Gallery.Core.Security {
    public class UserGroup {
        public static string SuperName = "*Owners";

        public UserGroup() {
            this.Users = new HashSet<User>();
            this.Permissions = new HashSet<Permission>();
        }

        public string Name                     { get; set; }
        public HashSet<User> Users             { get; private set; }
        public HashSet<Permission> Permissions { get; private set; }

        public bool IsSuper {
            get { return this.Name == SuperName; }
        }
    }
}
