using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using AshMind.Extensions;

using AshMind.Gallery.Core.Security;
using AshMind.Gallery.Core;
using AshMind.Gallery.Site.Logic;

namespace AshMind.Gallery.Site.Controllers {
    public abstract class ControllerBase : Controller {
        private IUser user;
        private readonly IUserAuthentication authentication;

        public ControllerBase(IUserAuthentication authentication) {
            this.authentication = authentication;
        }

        public new IUser User {
            get {
                if (this.user == null)
                    this.user = this.authentication.GetUser(base.User);

                return this.user;
            }
        }

        public HttpUnauthorizedResult Unauthorized() {
            return new HttpUnauthorizedResult();
        }

        public EmptyResult Empty() {
            return new EmptyResult();
        }
    }
}
