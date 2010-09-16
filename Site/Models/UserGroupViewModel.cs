using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Web;

using AshMind.Extensions;

using AshMind.Web.Gallery.Core.Security;

namespace AshMind.Web.Gallery.Site.Models {
    public class UserGroupViewModel {
        public UserGroupViewModel(IUserGroup userGroup) {
            var user = userGroup as User;
            if (user != null) {
                this.Key = "user:" + user.Email;
            }
            else {
                var concreteGroup = userGroup as UserGroup;
                if (concreteGroup == null)
                    throw new ArgumentException("group");

                this.Key = "group:" + concreteGroup.Name;
            }

            this.UserGroup = userGroup;
            this.Users = userGroup.GetUsers().ToList().AsReadOnly();
        }

        public string Key { get; private set; }
        public string Name {
            get { return this.UserGroup.Name; }
        }

        public IUserGroup UserGroup { get; private set; }
        public ReadOnlyCollection<User> Users { get; private set; }
    }
}