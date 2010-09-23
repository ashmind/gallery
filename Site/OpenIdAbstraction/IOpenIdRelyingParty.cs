using System;
using System.Collections.Generic;

using DotNetOpenAuth.Messaging;
using DotNetOpenAuth.OpenId;
using DotNetOpenAuth.OpenId.RelyingParty;

namespace AshMind.Gallery.Site.OpenIdAbstraction {
    public interface IOpenIdRelyingParty {
        Channel Channel { get; }

        OutgoingWebResponse ProcessResponseFromPopup();

        IAuthenticationRequest CreateRequest(Identifier userSuppliedIdentifier, Realm realm, Uri returnToUrl);
        IEnumerable<IAuthenticationRequest> CreateRequests(Identifier userSuppliedIdentifier, Realm realm, Uri returnToUrl);

        IAuthenticationResponse GetResponse();
        IAuthenticationResponse GetResponse(HttpRequestInfo request);
    }
}