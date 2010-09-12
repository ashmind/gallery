using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

using Newtonsoft.Json;

namespace AshMind.Web.Gallery.Core.Access {
    internal class JsonUserRepository : IUserRepository {
        #region UserStore class

        private class UserStore {
            public UserStore() {
                this.Users = new List<User>();
                this.Groups = new List<UserGroup>();
            }

            public IList<User> Users        { get; private set; }
            public IList<UserGroup> Groups  { get; private set; }
        }

        #endregion

        private readonly string path;
        private readonly UserStore store;

        public JsonUserRepository(string path) {
            this.path = path;
            
            if (File.Exists(path)) {
                this.store = JsonConvert.DeserializeObject<UserStore>(File.ReadAllText(path));
            }
            else {
                this.store = new UserStore();
                File.WriteAllText(path, JsonConvert.SerializeObject(this.store));
            }
        }

        public User Load(string email) {
            return this.store.Users.FirstOrDefault(u => u.Email == email);
        }
    }
}
