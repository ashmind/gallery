using System;
using System.Configuration;
using System.Text.RegularExpressions;

using Autofac.Builder;

using AshMind.Extensions;

using AshMind.Gallery.Core.AlbumSupport;
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

            var personNameRegexString = ConfigurationManager.AppSettings["PersonNameRegex"];
            if (personNameRegexString.IsNotNullOrEmpty()) {
                var personNameReplacement = ConfigurationManager.AppSettings["PersonNameReplacement"];
                if (personNameReplacement == null)
                    throw new InvalidOperationException("PersonNameRegex option always requires a PersonNameReplacement.");

                var personNameRegex = new Regex(personNameRegexString);

                builder.Register(new Logic.PeopleAlbumNameRegexTransform(personNameRegex, personNameReplacement))
                       .As<IAlbumNameTransform>();
            }
        }
    }
}
