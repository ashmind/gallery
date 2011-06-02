using System;
using System.Collections.Generic;
using System.Linq;

using Newtonsoft.Json;

using AshMind.Extensions;

using AshMind.Gallery.Core.IO;

namespace AshMind.Gallery.Core.Security.Internal {
    internal class JsonSecurityRepository : IRepository<User>, IRepository<UserGroup>, IRepository<IUserGroup> {
        #region UserStore class

        private class UserStore {
            public UserStore() {
                this.Users = new List<User>();
                this.Groups = new List<UserGroup>();
            }

            public IList<User> Users              { get; private set; }
            public IList<UserGroup> Groups        { get; private set; }
        }

        #endregion

        private readonly IFile file;
        private readonly UserStore store;

        public JsonSecurityRepository(IFile file) {
            this.file = file;
            
            if (this.file.Exists) {
                this.store = JsonConvert.DeserializeObject<UserStore>(this.file.ReadAllText());
            }
            else {
                this.store = new UserStore {
                    Groups = { new UserGroup { Name = UserGroup.SuperName } }
                };
                this.file.WriteAllText(JsonConvert.SerializeObject(this.store));
            }
        }

        public IQueryable<User> Query() {
            return this.store.Users.AsQueryable();
        }
                
        public object GetKey(User user) {
            return user.Email;
        }

        public User Load(object key) {
            return this.store.Users.SingleOrDefault(u => u.Email == (string)key);
        }

        #region IRepository<UserGroup> Members

        IQueryable<UserGroup> IRepository<UserGroup>.Query() {
            return this.store.Groups.AsQueryable();
        }

        object IRepository<UserGroup>.GetKey(UserGroup entity) {
            return entity.Name;
        }

        UserGroup IRepository<UserGroup>.Load(object key) {
            return this.store.Groups.SingleOrDefault(g => g.Name == (string)key);
        }

        #endregion

        #region IRepository<IUserGroup> Members

        IQueryable<IUserGroup> IRepository<IUserGroup>.Query() {
            return Enumerable.Concat<IUserGroup>(
                this.store.Users,
                this.store.Groups
            ).AsQueryable();
        }

        object IRepository<IUserGroup>.GetKey(IUserGroup entity) {
            if (entity is User)
                return "user:" + (this as IRepository<User>).GetKey(entity as User);

            if (entity is UserGroup)
                return "group:" + (this as IRepository<UserGroup>).GetKey(entity as UserGroup);

            throw new NotSupportedException();
        }

        IUserGroup IRepository<IUserGroup>.Load(object key) {
            var keyString = (string)key;
            if (keyString.StartsWith("user:"))
                return (this as IRepository<User>).Load(keyString.RemoveStart("user:"));

            if (keyString.StartsWith("group:"))
                return (this as IRepository<UserGroup>).Load(keyString.RemoveStart("group:"));

            throw new NotSupportedException();
        }

        #endregion
    }
}
