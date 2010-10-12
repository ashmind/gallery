using Autofac.Builder;

using AshMind.Gallery.Site.OpenIdAbstraction;

namespace AshMind.Gallery.Site {
    public class WebModule : Module {
        protected override void Load(ContainerBuilder builder) {
            base.Load(builder);

            builder.Register(new OpenIdAjaxRelyingParty(new DotNetOpenAuth.OpenId.RelyingParty.OpenIdAjaxRelyingParty(null)))
                   .As<IOpenIdAjaxRelyingParty, IOpenIdRelyingParty>()
                   .SingletonScoped();

            builder.Register<Logic.UserAuthentication>()
                   .ContainerScoped();
        }
    }
}
