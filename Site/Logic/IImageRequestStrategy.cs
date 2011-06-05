using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Routing;

using AshMind.IO.Abstraction;

namespace AshMind.Gallery.Site.Logic {
    public interface IImageRequestStrategy {
        void MapRoute(RouteCollection routes, string imageControllerName, string getImageActionName);
        string GetActionUrl(RequestContext requestContext, string albumID, string itemID);

        bool IsAuthorized(RequestContext requestContext);
        IFile GetImageFile(RequestContext requestContext);
    }
}