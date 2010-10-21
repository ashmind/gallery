using System;
using System.Collections.Generic;
using System.Configuration;
using System.Text.RegularExpressions;

using Autofac.Builder;

using AshMind.Extensions;

using AshMind.Gallery.Core.AlbumSupport;
using AshMind.Gallery.Site.OpenIdAbstraction;

namespace AshMind.Gallery.Site {
    public class WebModule : Module {
        private static readonly IDictionary<string, string> knownImageStrategyNameShortcuts = new Dictionary<string, string> {
            { "friendly", typeof(Logic.ImageRequest.FriendlyImageUrlStrategy).AssemblyQualifiedName },
            { "subdomains", typeof(Logic.ImageRequest.SubdomainImageRequestStrategy).AssemblyQualifiedName },
        };

        protected override void Load(ContainerBuilder builder) {
            base.Load(builder);

            builder.Register(new OpenIdAjaxRelyingParty(new DotNetOpenAuth.OpenId.RelyingParty.OpenIdAjaxRelyingParty(null)))
                   .As<IOpenIdAjaxRelyingParty, IOpenIdRelyingParty>()
                   .SingletonScoped();

            builder.Register<Logic.UserAuthentication>()
                   .As<Logic.IUserAuthentication>()
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

            RegisterImageRequestStrategy(builder);
        }

        private void RegisterImageRequestStrategy(ContainerBuilder builder) {
            var imageRequestStrategyName = ConfigurationManager.AppSettings["Gallery.ImageUrls"];
            if (imageRequestStrategyName.IsNullOrEmpty())
                imageRequestStrategyName = typeof(Logic.ImageRequest.FriendlyImageUrlStrategy).AssemblyQualifiedName;

            imageRequestStrategyName = knownImageStrategyNameShortcuts.GetValueOrDefault(
                imageRequestStrategyName, imageRequestStrategyName
            );

            var type = Type.GetType(imageRequestStrategyName, true, true);
            builder.Register(type).As<Logic.IImageRequestStrategy>();
        }
    }
}
