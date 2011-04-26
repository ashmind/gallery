using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

using AshMind.Extensions;

namespace AshMind.Gallery.Site.Extensions {
    public static class GravatarExtension {
        public static IHtmlString Gravatar(this HtmlHelper html, string email) {
            return GetImageTag(GetGravatar(email), new RouteValueDictionary(), null);
        }

        public static IHtmlString Gravatar(this HtmlHelper html, string email, object gravatarAttributes) {
            var attributes = (gravatarAttributes == null ? new RouteValueDictionary()
                                                         : new RouteValueDictionary(gravatarAttributes));

            return GetImageTag(GetGravatar(email, attributes), attributes, null);
        }

        public static IHtmlString Gravatar(this HtmlHelper html, string email, object gravatarAttributes, object htmlAttributes) {
            var attributes = (gravatarAttributes == null ? new RouteValueDictionary()
                                                         : new RouteValueDictionary(gravatarAttributes));

            return GetImageTag(GetGravatar(email, attributes), attributes, htmlAttributes);
        }


        private static IHtmlString GetImageTag(string source, RouteValueDictionary gravatarAttributes, object htmlAttributes) {
            var attributes = (htmlAttributes == null 
                ? new RouteValueDictionary() 
                : new RouteValueDictionary(htmlAttributes)
            );

            var builder = new TagBuilder("img");
            builder.Attributes["src"] = source;

            var size = (
                   gravatarAttributes.GetValueOrDefault("s")
                ?? gravatarAttributes.GetValueOrDefault("size")
                ?? string.Empty
            ).ToString();

            if (size.IsNotNullOrEmpty()) {
                builder.Attributes["width"] = size;
                builder.Attributes["height"] = size;
            }

            builder.MergeAttributes(attributes);

            return new MvcHtmlString(builder.ToString(TagRenderMode.SelfClosing));
        }

        private static string GetGravatar(string email) {
            return string.Format("http://www.gravatar.com/avatar/{0}", EncryptMD5(email));
        }

        private static string GetGravatar(string email, RouteValueDictionary gravatarAttributes) {       
            var returnVal = GetGravatar(email);
            var first = true;
            foreach (var key in gravatarAttributes.Keys) {
                if (first) {
                    first = false;
                    returnVal += string.Format("?{0}={1}", key, gravatarAttributes[key]);
                    continue;
                }

                returnVal += string.Format("&{0}={1}", key, gravatarAttributes[key]);
            }

            return returnVal;
        }

        private static string EncryptMD5(string value){
            var valueArray = Encoding.ASCII.GetBytes(value);
            using (var md5 = MD5.Create()) {
                valueArray = md5.ComputeHash(valueArray);
            }

            var encrypted = new StringBuilder();
            foreach (var @byte in valueArray) {
                encrypted.Append(@byte.ToString("x2").ToLower());
            }
            return encrypted.ToString();
        }
    }
}