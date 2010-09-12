using Autofac.Builder;

using AshMind.Web.Gallery.Site.OpenIdAbstraction;

namespace AshMind.Web.Gallery.Site {
    public class WebModule : Module {
        protected override void Load(ContainerBuilder builder) {
            base.Load(builder);

            builder.Register(new OpenIdAjaxRelyingParty(new DotNetOpenAuth.OpenId.RelyingParty.OpenIdAjaxRelyingParty(null)))
                   .As<IOpenIdAjaxRelyingParty, IOpenIdRelyingParty>()
                   .SingletonScoped();
        }
    }
}
