using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

using DotNetOpenAuth.Messaging;
using DotNetOpenAuth.OpenId;
using DotNetOpenAuth.OpenId.RelyingParty;

namespace AshMind.Web.Gallery.Site.OpenIdAbstraction {
    public class OpenIdAjaxRelyingParty : IOpenIdAjaxRelyingParty {
        private readonly DotNetOpenAuth.OpenId.RelyingParty.OpenIdAjaxRelyingParty actual;

        public OpenIdAjaxRelyingParty(DotNetOpenAuth.OpenId.RelyingParty.OpenIdAjaxRelyingParty actual) {
            this.actual = actual;
        }

        public Channel Channel {
            get { return this.actual.Channel; }
        }

        public IAuthenticationRequest CreateRequest(Identifier userSuppliedIdentifier, Realm realm, Uri returnToUrl) {
            return this.actual.CreateRequest(userSuppliedIdentifier, realm, returnToUrl);
        }

        public IEnumerable<IAuthenticationRequest> CreateRequests(Identifier userSuppliedIdentifier, Realm realm, Uri returnToUrl) {
            return this.actual.CreateRequests(userSuppliedIdentifier, realm, returnToUrl);
        }

        public OutgoingWebResponse AsAjaxDiscoveryResult(IEnumerable<IAuthenticationRequest> requests) {
            return this.actual.AsAjaxDiscoveryResult(requests);
        }

        public string AsAjaxPreloadedDiscoveryResult(IEnumerable<IAuthenticationRequest> requests) {
            return this.actual.AsAjaxPreloadedDiscoveryResult(requests);
        }

        public OutgoingWebResponse ProcessResponseFromPopup() {
            return this.actual.ProcessResponseFromPopup();
        }

        public IAuthenticationResponse GetResponse() {
            return this.actual.GetResponse();
        }

        public IAuthenticationResponse GetResponse(HttpRequestInfo request) {
            return this.actual.GetResponse(request);
        }
    }
}
