using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

using AshMind.Gallery.Core;
using AshMind.Gallery.Core.IO;
using AshMind.Gallery.Core.Security;
using AshMind.Gallery.Site.Models;
using AshMind.Gallery.Site.Fixes;

namespace AshMind.Gallery.Site.Logic.ImageRequest {
    public class CookielessImageRequestStrategy : IImageRequestStrategy {        
        private readonly IAlbumFacade gallery;
        protected string RouteName { get; private set; }
        
        public CookielessImageRequestStrategy(IAlbumFacade gallery) {
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

        private static readonly char[] Base64CharsUnsafeForUrl = { '/', '=', '+' };

        private string BytesToString(byte[] bytes) {
            var base64 = Convert.ToBase64String(bytes);
            return Regex.Replace(
                base64,
                "[" + new string(Base64CharsUnsafeForUrl) + "]",
                match => {
                    var @char = match.Value[0];
                    return "_" + (char)('a' + Array.IndexOf(Base64CharsUnsafeForUrl, @char));
                }
            );
        }

        private byte[] StringToBytes(string value) {
            var base64 = Regex.Replace(
                value, "_[a-z]",
                match => {
                    var @char = match.Value[1];
                    return Base64CharsUnsafeForUrl[@char - 'a'].ToString();
                }
            );

            return Convert.FromBase64String(base64);
        }
    }
}