using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using AshMind.Gallery.Core.Security;
using AshMind.Gallery.Core;

namespace AshMind.Gallery.Site.Controllers
{
    public abstract class ControllerBase : Controller
    {
        private readonly IRepository<User> userRepository;

        public ControllerBase(
            IRepository<User> userRepository
        ) {
            this.userRepository = userRepository;
        }

        public new User User {
            get { 
                if (!base.User.Identity.IsAuthenticated)
                    return null;

                return this.userRepository.Load(base.User.Identity.Name);
            }
        }
    }
}
