using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Threading;
using System.Web.Mvc;
using AshMind.Gallery.Core.Security.Actions;
using DotNetOpenAuth.Messaging;
using DotNetOpenAuth.OpenId;
using DotNetOpenAuth.OpenId.Extensions.SimpleRegistration;
using DotNetOpenAuth.OpenId.RelyingParty;

using AshMind.Extensions;

using AshMind.Gallery.Core;
using AshMind.Gallery.Core.Security;
using AshMind.Gallery.Site.Logic;
using AshMind.Gallery.Site.Models;
using AshMind.Gallery.Site.OpenIdAbstraction;

namespace AshMind.Gallery.Site.Controllers {
    [HandleError]
    public class AccessController : ControllerBase {
        private readonly IOpenIdAjaxRelyingParty openId;
        private readonly IUserAuthentication authentication;
        private readonly IRepository<IUserGroup> userGroupRepository;
        private readonly IAuthorizationService authorization;
        private readonly IAlbumFacade gallery;

        public AccessController(
            IOpenIdAjaxRelyingParty openId,
            IUserAuthentication authentication,
            IRepository<IUserGroup> userGroupRepository,
            IAuthorizationService authorization,
            IAlbumFacade gallery
        ) : base(authentication) {
            this.openId = openId;
            this.authentication = authentication;
            this.userGroupRepository = userGroupRepository;
            this.authorization = authorization;
            this.gallery = gallery;
        }
      
        public ActionResult Login(string error, string returnUrl, string key) {
            if (this.authentication.AuthenticateByKey(key))
                return Redirect(returnUrl);

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

                var authenticated = this.authentication.AuthenticateByEmail(claims.Email);
                if (!authenticated)
                    return RedirectToAction("Login", new { error = "Could not authenticate this email." });

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

            var album = this.gallery.GetAlbum(albumID, this.User);
            return PartialView("GrantForm", new GrantViewModel(
                albumID,
                this.authorization.GetAuthorizedTo(SecurableActions.View(album.SecurableToken)).ToSet(),
                this.GetAllGroups().ToList()
            ));
        }

        [HttpPost]
        public ActionResult Grant(string albumID, HashSet<string> groupKeys) {
            if (!Request.IsAjaxRequest())
                throw new NotImplementedException();

            var album = this.gallery.GetAlbum(albumID, this.User);
            var groups = this.GetAllGroups()
                             .Where(g => groupKeys.Contains(g.Key))
                             .Select(g => g.UserGroup);

            this.authorization.AuthorizeTo(SecurableActions.View(album.SecurableToken), groups);
            return new EmptyResult();
        }

        private IEnumerable<UserGroupViewModel> GetAllGroups() {
            return this.userGroupRepository.Query().AsEnumerable().Select(g => new UserGroupViewModel(g));
        }

        private Uri MakeAbsolute(string uri) {
            return new Uri(Request.Url, uri);
        }
    }
}


