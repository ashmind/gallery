using System;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Threading;
using System.Web.Mvc;
using System.Web.Security;

using DotNetOpenAuth.Messaging;
using DotNetOpenAuth.OpenId;
using DotNetOpenAuth.OpenId.Extensions.SimpleRegistration;
using DotNetOpenAuth.OpenId.RelyingParty;

using AshMind.Extensions;

using AshMind.Web.Gallery.Core;
using AshMind.Web.Gallery.Core.Security;
using AshMind.Web.Gallery.Site.Models;
using AshMind.Web.Gallery.Site.OpenIdAbstraction;
using System.Collections.Generic;

namespace AshMind.Web.Gallery.Site.Controllers {
    [HandleError]
    public class AccessController : Controller {
        private readonly IOpenIdAjaxRelyingParty openId;
        private readonly IRepository<User> userRepository;
        private readonly IRepository<UserGroup> userGroupRepository;
        private readonly AuthorizationService authorization;
        private readonly AlbumFacade gallery;

        public AccessController(
            IOpenIdAjaxRelyingParty openId,
            IRepository<User> userRepository,
            IRepository<UserGroup> userGroupRepository,
            AuthorizationService authorization,
            AlbumFacade gallery
        ) {
            this.openId = openId;
            this.userRepository = userRepository;
            this.userGroupRepository = userGroupRepository;
            this.authorization = authorization;
            this.gallery = gallery;
        }
      
        public ActionResult Login(string error) {
            Thread.CurrentThread.CurrentUICulture = CultureInfo.GetCultureInfo("en-us");
            var returnToUrl = MakeAbsolute(Url.Action("OpenIdLoginReturnTo"));
            var requests = new[] { "https://www.google.com/accounts/o8/id" }.SelectMany(
                identifier => openId.CreateRequests(identifier, Realm.AutoDetect, returnToUrl)
            ).ToArray();
            requests.ForEach(r => r.AddExtension(new ClaimsRequest {
                Email = DemandLevel.Require                
            }));
            var ajax = this.openId.AsAjaxPreloadedDiscoveryResult(requests);

            if (error.IsNotNullOrEmpty())
                ModelState.AddModelError("Login", error);

            return View(new LoginViewModel { PreloadedDiscoveryResults = ajax });
        }

        [AcceptVerbs(HttpVerbs.Get | HttpVerbs.Post), ValidateInput(false)]
        public ActionResult OpenIdLoginReturnTo() {
            return this.openId.ProcessResponseFromPopup().AsActionResult();
        }

        [AcceptVerbs(HttpVerbs.Post)]
        [ValidateInput(false)]
        public ActionResult OpenIdLoginPostAssertion(string openid_openidAuthData, string returnUrl) {
            IAuthenticationResponse response;
            if (!string.IsNullOrEmpty(openid_openidAuthData)) {
                var auth = new Uri(openid_openidAuthData);
                var headers = new WebHeaderCollection();
                foreach (string header in Request.Headers) {
                    headers[header] = Request.Headers[header];
                }

                // Always say it's a GET since the payload is all in the URL, even the large ones.
                var clientResponseInfo = new HttpRequestInfo("GET", auth, auth.PathAndQuery, headers, null);
                response = this.openId.GetResponse(clientResponseInfo);
            }
            else {
                response = this.openId.GetResponse();
            }
            
            if (response == null)
                return RedirectToAction("Login");
            
            if (response.Status == AuthenticationStatus.Authenticated) {
                var claims = response.GetExtension<ClaimsResponse>();
                if (claims == null) {
                    return RedirectToAction("Login", new { error = "Email not received." });
                }

                var user = this.userRepository.FindByEmail(claims.Email);
                if (user == null)
                    return RedirectToAction("Login", new { error = "User is not known." });

                FormsAuthentication.SetAuthCookie((string)this.userRepository.GetKey(user), false);
                return Redirect(returnUrl);
            }

            if (response.Status == AuthenticationStatus.Failed) {
                return RedirectToAction("Login", new { error = response.Exception.Message });
            }

            return RedirectToAction("Login");
        }
        
        [HttpGet]
        public ActionResult Grant(string albumID) {
            if (!Request.IsAjaxRequest())
                throw new NotImplementedException();

            var token = this.gallery.GetAlbumToken(albumID);
            return PartialView("GrantForm", new GrantViewModel(
                albumID,
                this.authorization.GetAuthorizedTo(SecurableAction.View, token).ToSet(),
                this.GetAllGroups().ToList()
            ));
        }

        [HttpPost]
        public ActionResult Grant(string albumID, HashSet<string> groupKeys) {
            if (!Request.IsAjaxRequest())
                throw new NotImplementedException();

            var token = this.gallery.GetAlbumToken(albumID);
            var groups = this.GetAllGroups()
                             .Where(g => groupKeys.Contains(g.Key))
                             .Select(g => g.UserGroup);

            this.authorization.MakeAuthorizedTo(SecurableAction.View, token, groups);
            return new EmptyResult();
        }

        private IEnumerable<UserGroupViewModel> GetAllGroups() {
            return Enumerable.Concat<IUserGroup>(
                this.userRepository.Query().AsEnumerable(),
                this.userGroupRepository.Query().AsEnumerable()
            ).Select(g => new UserGroupViewModel(g));
        }

        private Uri MakeAbsolute(string uri) {
            return new Uri(Request.Url, uri);
        }
    }
}


