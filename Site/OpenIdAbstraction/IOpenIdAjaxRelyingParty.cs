using System;
using System.Collections.Generic;

using DotNetOpenAuth.Messaging;
using DotNetOpenAuth.OpenId.RelyingParty;

namespace AshMind.Web.Gallery.Site.OpenIdAbstraction {
    public interface IOpenIdAjaxRelyingParty : IOpenIdRelyingParty {
        OutgoingWebResponse AsAjaxDiscoveryResult(IEnumerable<IAuthenticationRequest> requests);
        string AsAjaxPreloadedDiscoveryResult(IEnumerable<IAuthenticationRequest> requests);
    }
}