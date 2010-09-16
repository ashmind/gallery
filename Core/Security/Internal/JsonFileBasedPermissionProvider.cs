﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

using Newtonsoft.Json;

using AshMind.Extensions;

using AshMind.Web.Gallery.Core.Internal;
using AshMind.Web.Gallery.Core.IO;
using AshMind.Web.Gallery.Core.Security;
using System.IO;

namespace AshMind.Web.Gallery.Core.Metadata.Internal {
    internal class JsonFileBasedFilePermissionProvider : IPermissionProvider {
        private readonly IFileSystem fileSystem;
        private readonly IRepository<User> userRepository;
        private readonly IRepository<UserGroup> groupRepository;

        public JsonFileBasedFilePermissionProvider(
            IFileSystem fileSystem,
            IRepository<User> userRepository,
            IRepository<UserGroup> groupRepository
        ) {
            this.fileSystem = fileSystem;
            this.userRepository = userRepository;
            this.groupRepository = groupRepository;
        }

        public IEnumerable<Permission> GetPermissions(string token) {
            var securityFile = GetSecurityFileName(token);
            if (!fileSystem.FileExists(securityFile))
                return Enumerable.Empty<Permission>();

            var json = fileSystem.ReadAllText(securityFile);
            var permissionSet = JsonConvert.DeserializeObject<
                Dictionary<SecurableAction, IList<string>>
            >(json);

            var lazyUsers = new Lazy<IList<User>>(() => this.userRepository.Query().ToList());
            var lazyGroups = new Lazy<IDictionary<string, UserGroup>>(() => this.groupRepository.Query().ToDictionary(g => g.Name));

            using (var md5 = MD5.Create()) {
                return from pair in permissionSet
                       from key in pair.Value
                       let @group = ResolveGroup(key, md5, lazyUsers, lazyGroups)
                       where @group != null
                       select new Permission {
                           Action = pair.Key,
                           Group = @group
                       };
            }
        }
       
        public bool CanSetPermissions(string token) {
            return this.fileSystem.IsLocation(token);
        }

        public void SetPermissions(string token, IEnumerable<Permission> permissions) {
            var securityFile = GetSecurityFileName(token);
            using (var md5 = MD5.Create()) {
                var permissionSet = permissions.GroupBy(p => p.Action)
                                               .ToDictionary(
                                                    g => g.Key,
                                                    g => g.Select(p => GetKey(p.Group, md5)).ToList()
                                               );

                using (var stream = this.fileSystem.OpenFile(securityFile, FileLockMode.Write, true)) 
                using (var writer = new StreamWriter(stream))
                using (var jsonWriter = new JsonTextWriter(writer) { Formatting = Formatting.Indented }) {
                    new JsonSerializer().Serialize(jsonWriter, permissionSet);
                }
            }
        }

        private IUserGroup ResolveGroup(string key, MD5 md5, Lazy<IList<User>> lazyUsers, Lazy<IDictionary<string, UserGroup>> lazyGroups) {
            if (!key.StartsWith("u:"))
                return lazyGroups.Value.GetValueOrDefault(key);

            key = key.SubstringAfter("u:");
            return lazyUsers.Value.SingleOrDefault(
                u => md5.ComputeHashAsString(Encoding.UTF8.GetBytes(u.Email)) == key
            );
        }

        private string GetKey(IUserGroup group, MD5 md5) {
            var userGroup = group as UserGroup;
            if (userGroup != null)
                return userGroup.Name;

            var user = group as User;
            if (user == null)
                return "u:" + md5.ComputeHashAsString(Encoding.UTF8.GetBytes(user.Email));

            throw new NotSupportedException();
        }

        private string GetSecurityFileName(string token) {
            return fileSystem.BuildPath(token, "album.security");
        }
    }
}