using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

using AshMind.Gallery.Core.Security;

namespace AshMind.Gallery.Site.Models {
    public class UserGroupViewModel {
        public UserGroupViewModel(IUserGroup userGroup) {
            var user = userGroup as KnownUser;
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
            this.Users = userGroup.GetUsers().OfType<KnownUser>().ToList().AsReadOnly();
        }

        public string Key { get; private set; }
        public string Name {
            get { return this.UserGroup.Name; }
        }

        public IUserGroup UserGroup { get; private set; }
        public ReadOnlyCollection<KnownUser> Users { get; private set; }
    }
}