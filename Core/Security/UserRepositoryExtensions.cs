using System;
using System.Collections.Generic;
using System.Linq;

namespace AshMind.Gallery.Core.Security {
    public static class UserRepositoryExtensions {
        public static KnownUser FindByEmail(this IRepository<KnownUser> userRepository, string email) {
            return userRepository.Query().SingleOrDefault(u => u.Email == email);
        }
    }
}
