using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using System.Web.Routing;

using AshMind.Gallery.Site.Models;
using AshMind.Gallery.Core;
using AshMind.Gallery.Core.IO;
using AshMind.Gallery.Core.Security;

namespace AshMind.Gallery.Site.Logic.ImageRequest {
    public class FriendlyImageUrlStrategy : IImageRequestStrategy {
        private static readonly string RouteName = "Images (" + typeof(FriendlyImageUrlStrategy) + ")";

        private readonly IAlbumFacade gallery;
        private readonly IUserAuthentication authentication;

        public FriendlyImageUrlStrategy(
            IAlbumFacade gallery,
            IUserAuthentication authentication
        ) {
            this.gallery = gallery;
            this.authentication = authentication;
        }

        public void MapRoute(RouteCollection routes, string imageControllerName, string getImageActionName) {
            routes.MapRoute(
                RouteName,
                "{album}/{item}/{size}",
                new { controller = imageControllerName, action = getImageActionName, size = ImageSize.Original }
            );
        }

        public string GetActionUrl(RequestContext requestContext, string albumID, string itemID) {
            return new UrlHelper(requestContext).RouteUrl(RouteName, new { album = albumID, item = itemID });
        }

        public bool IsAuthorized(RequestContext requestContext) {
            var user = this.authentication.GetUser(requestContext.HttpContext.User);
            if (user == null)
                return false;

            var albumID = (string)requestContext.RouteData.Values["album"];
            var item = (string)requestContext.RouteData.Values["item"];
            
            // TODO: rewrite it as a true authorization check
            return this.gallery.GetItem(albumID, item, user) != null;
        }

        public IFile GetImageFile(RequestContext requestContext) {
            var albumID = (string)requestContext.RouteData.Values["album"];
            var item = (string)requestContext.RouteData.Values["item"];

            return this.gallery.GetItem(albumID, item, KnownUser.System).File;
        }
    }
}