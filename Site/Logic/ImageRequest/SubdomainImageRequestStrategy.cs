using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

using AshMind.Gallery.Core;
using System.Threading;

namespace AshMind.Gallery.Site.Logic.ImageRequest {
    public class SubdomainImageRequestStrategy : CookielessImageRequestStrategy {
        private int currentIndex;
        private readonly string subdomainPattern;

        public SubdomainImageRequestStrategy(IAlbumFacade gallery) : this(gallery, "img{0}") {
        }

        public SubdomainImageRequestStrategy(IAlbumFacade gallery, string subdomainPattern) : base(gallery) {
            this.currentIndex = 1;
            this.subdomainPattern = subdomainPattern.TrimEnd('.');
        }

        protected override string GetActionUrl(UrlHelper urlHelper, object routeValues) {
            var urlString = urlHelper.RouteUrl(this.RouteName, routeValues, urlHelper.RequestContext.HttpContext.Request.Url.Scheme);
            var urlBuilder = new UriBuilder(urlString);

            if (!urlBuilder.Host.Contains(".")) // this should be overridable in future, but not now
                return urlBuilder.ToString();

            Interlocked.Increment(ref currentIndex);
            Interlocked.CompareExchange(ref currentIndex, 1, 6);

            urlBuilder.Host = string.Format(subdomainPattern, currentIndex) + "." + urlBuilder.Host;
            return urlBuilder.ToString();
        }
    }
}