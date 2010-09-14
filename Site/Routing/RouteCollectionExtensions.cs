using System;
using System.Web.Mvc;
using System.Web.Routing;

namespace AshMind.Web.Gallery.Site.Routing {
    public static class RouteCollectionExtensions {
        public static void MapLowerCaseRoute(this RouteCollection routes, string name, string url, object defaults) {
            routes.MapLowerCaseRoute(name, url, defaults, null);
        }

        public static void MapLowerCaseRoute(this RouteCollection routes, string name, string url, object defaults, object constraints) {
            if (routes == null)
                throw new ArgumentNullException("routes");

            if (url == null)
                throw new ArgumentNullException("url");

            var route = new LowerCaseRoute(url, new MvcRouteHandler()) {
                Defaults = new RouteValueDictionary(defaults),
                Constraints = new RouteValueDictionary(constraints)
            };

            if (string.IsNullOrEmpty(name)) {
                routes.Add(name, route);
            }
            else {
                routes.Add(route);
            }
        }
    }
}