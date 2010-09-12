using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

using Newtonsoft.Json;

namespace AshMind.Web.Gallery.Core.Security.Internal {
    internal class JsonSecurityRepository : IRepository<User>, IRepository<UserGroup> {
        #region UserStore class

        private class UserStore {
            public UserStore() {
                this.Users = new List<User>();
                this.Groups = new List<UserGroup>();
            }

            public IList<User> Users              { get; private set; }
            public IList<UserGroup> Groups        { get; private set; }
            public IList<Permission> Permissions  { get; private set; }
        }

        #endregion

        private readonly string path;
        private readonly UserStore store;

        public JsonSecurityRepository(string path) {
            this.path = path;
            
            if (File.Exists(path)) {
                this.store = JsonConvert.DeserializeObject<UserStore>(File.ReadAllText(path));
            }
            else {
                this.store = new UserStore {
                    Groups = { new UserGroup { Name = UserGroup.SuperName } }
                };
                File.WriteAllText(path, JsonConvert.SerializeObject(this.store));
            }
        }

        public IQueryable<User> Query() {
            return this.store.Users.AsQueryable();
        }

        IQueryable<UserGroup> IRepository<UserGroup>.Query() {
            return this.store.Groups.AsQueryable();
        }
    }
}
