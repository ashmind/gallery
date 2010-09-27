﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

using Newtonsoft.Json;

using AshMind.Extensions;

using AshMind.Gallery.Core.Internal;
using AshMind.Gallery.Core.IO;
using AshMind.Gallery.Core.Security;
using System.IO;

namespace AshMind.Gallery.Core.Security.Internal {
    [Obsolete("Use JsonLocationPermissionProvider instead.")]
    internal class ObsoleteJsonLocationPermissionProvider : AbstractPermissionProvider<ILocation> {
        private readonly IRepository<User> userRepository;
        private readonly IRepository<UserGroup> groupRepository;

        public ObsoleteJsonLocationPermissionProvider(
            IRepository<User> userRepository,
            IRepository<UserGroup> groupRepository
        ) {
            this.userRepository = userRepository;
            this.groupRepository = groupRepository;
        }

        public override IEnumerable<Permission> GetPermissions(ILocation location) {
            var securityFile = GetSecurityFile(location, true);
            if (securityFile == null)
                return Enumerable.Empty<Permission>();

            var json = securityFile.ReadAllText();
            var permissionSet = JsonConvert.DeserializeObject<
                Dictionary<SecurableAction, IList<string>>
            >(json);

            var lazyUsers = new Lazy<IList<User>>(() => this.userRepository.Query().ToList());
            var lazyGroups = new Lazy<IDictionary<string, UserGroup>>(() => this.groupRepository.Query().ToDictionary(g => g.Name));

            return Using(MD5.Create(), md5 =>
                from pair in permissionSet
                from key in pair.Value
                let @group = ResolveGroup(key, md5, lazyUsers, lazyGroups)
                where @group != null
                select new Permission {
                    Action = pair.Key,
                    Group = @group
                }
            );
        }

        private IEnumerable<T> Using<TDisposable, T>(TDisposable disposable, Func<TDisposable, IEnumerable<T>> enumerateWith) 
            where TDisposable : IDisposable
        {
            using (disposable) {
                foreach (var element in enumerateWith(disposable)) {
                    yield return element;
                }
            }
        }
       
        public override void SetPermissions(ILocation location, IEnumerable<Permission> permissions) {
            throw new NotSupportedException();
        }

        public override bool CanSetPermissions(ILocation target) {
            return false;
        }

        private IUserGroup ResolveGroup(string key, MD5 md5, Lazy<IList<User>> lazyUsers, Lazy<IDictionary<string, UserGroup>> lazyGroups) {
            if (!key.StartsWith("u:"))
                return lazyGroups.Value.GetValueOrDefault(key);

            key = key.SubstringAfter("u:");
            return lazyUsers.Value.SingleOrDefault(
                u => md5.ComputeHashAsString(Encoding.UTF8.GetBytes(u.Email)) == key
            );
        }

        private IFile GetSecurityFile(ILocation location, bool nullUnlessExists) {
            return location.GetFile(".album.security", nullUnlessExists);
        }
    }
}