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

using AshMind.Web.Gallery.Site.OpenIdAbstraction;
using AshMind.Web.Gallery.Site.Models;
using AshMind.Web.Gallery.Core.Access;

namespace AshMind.Web.Gallery.Site.Controllers {
    [HandleError]
    public class AccessController : Controller {
        private readonly IOpenIdAjaxRelyingParty openId;
        private readonly IUserRepository userRepository;

        public AccessController(
            IOpenIdAjaxRelyingParty openId,
            IUserRepository userRepository
        ) {
            this.openId = openId;
            this.userRepository = userRepository;
        }
      
        public ActionResult Login() {
            Thread.CurrentThread.CurrentUICulture = CultureInfo.GetCultureInfo("en-us");
            var returnToUrl = MakeAbsolute(Url.Action("OpenIdLoginReturnTo"));
            var requests = new[] { "https://www.google.com/accounts/o8/id" }.SelectMany(
                identifier => openId.CreateRequests(identifier, Realm.AutoDetect, returnToUrl)
            ).ToArray();
            requests.ForEach(r => r.AddExtension(new ClaimsRequest {
                Email = DemandLevel.Require
            }));
            var ajax = this.openId.AsAjaxPreloadedDiscoveryResult(requests);

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
                    ModelState.AddModelError("OpenID", "OpenID service didn't provide an user email as requested.");
                    return RedirectToAction("Login");
                }

                var user = this.userRepository.Load(claims.Email);
                if (user == null) {
                    ModelState.AddModelError("OpenID", "User is not known to access control system.");
                    return RedirectToAction("Login");
                }

                FormsAuthentication.SetAuthCookie(user.Email, false);
                return Redirect(returnUrl);
            }

            if (response.Status == AuthenticationStatus.Canceled) {
                ModelState.AddModelError("OpenID", "OpenID login was cancelled.");
            }
            else if (response.Status == AuthenticationStatus.Failed) {
                ModelState.AddModelError("OpenID", response.Exception.Message);
            }

            return RedirectToAction("Login");
        }

        private Uri MakeAbsolute(string uri) {
            return new Uri(Request.Url, uri);
        }
    }
}


