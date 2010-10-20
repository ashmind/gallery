using System;
using System.Collections.Generic;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Runtime.Remoting.Metadata.W3cXsd2001; // this is for byte-string conversion only
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

using AshMind.Gallery.Core;
using AshMind.Gallery.Core.IO;
using AshMind.Gallery.Core.Security;
using AshMind.Gallery.Site.Models;
using AshMind.Gallery.Site.Fixes;
using System.IO;

namespace AshMind.Gallery.Site.Logic.ImageRequest {
    public class CookielessImageRequestStrategy : IImageRequestStrategy {        
        private readonly AlbumFacade gallery;
        protected string RouteName { get; private set; }
        
        public CookielessImageRequestStrategy(AlbumFacade gallery) {
            this.gallery = gallery;
            this.RouteName = "Images (" + this.GetType() + ")";
        }

        public virtual void MapRoute(RouteCollection routes, string imageControllerName, string getImageActionName) {
            routes.MapRoute(
                this.RouteName,
                "image/{key}/{size}",
                new { controller = imageControllerName, action = getImageActionName, size = ImageSize.Original }
            );
        }

        public virtual string GetActionUrl(RequestContext requestContext, string albumID, string itemID) {
            var bytes = Encoding.UTF8.GetBytes(albumID + "/" + itemID);
            var keyBytes = EncryptOrDecrypt(bytes, encrypt: true);
            var key = BytesToString(keyBytes);

            return GetActionUrl(new UrlHelper(requestContext), new { key });
        }

        protected virtual string GetActionUrl(UrlHelper urlHelper, object routeValues) {
            return urlHelper.RouteUrl(this.RouteName, routeValues);
        }

        public virtual bool IsAuthorized(RequestContext requestContext) {
            return true;
        }

        public virtual IFile GetImageFile(RequestContext requestContext) {
            var key = (string)requestContext.RouteData.Values["key"];

            var keyBytes = StringToBytes(key);
            var bytes = EncryptOrDecrypt(keyBytes, encrypt: false);
            var arguments = Encoding.UTF8.GetString(bytes).Split('/');

            return this.gallery.GetItem(arguments[0], arguments[1], User.System).File;
        }

        private static byte[] EncryptOrDecrypt(byte[] bytes, bool encrypt) {
            return MachineKeySectionMethods.EncryptOrDecryptData(
                encrypt, bytes,
                modifier: null,
                start: 0,
                length: bytes.Length,
                useValidationSymAlgo: false,
                useLegacyMode: false,
                ivType: IVType.Hash,
                signData: true
            );
        }

        private string BytesToString(byte[] bytes) {
            return Convert.ToBase64String(bytes)
                          .Replace('/', '_');
        }

        private byte[] StringToBytes(string value) {
            return Convert.FromBase64String(value.Replace('_', '/'));
        }
    }
}