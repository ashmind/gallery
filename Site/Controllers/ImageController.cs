using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Net.Mime;

using AshMind.Extensions;

using AshMind.Gallery.Core;
using AshMind.Gallery.Core.IO;
using AshMind.Gallery.Core.Security;
using AshMind.Gallery.Site.Logic;
using AshMind.Gallery.Site.Models;

namespace AshMind.Gallery.Site.Controllers {
    [Authorize]
    public class ImageController : ControllerBase {
        private IDictionary<string, string> knownMimeTypes = new Dictionary<string, string> {
            {".jpg",    MediaTypeNames.Image.Jpeg},
            {".jpeg",   MediaTypeNames.Image.Jpeg},
            {".png",    "image/png"}
        };

        private readonly AlbumFacade gallery;
        private readonly PreviewFacade preview;

        public ImageController(AlbumFacade gallery, PreviewFacade preview, UserAuthentication authentication)
            : base(authentication)
        {
            this.gallery = gallery;
            this.preview = preview;
        }

        //[OutputCache(Duration = 60, VaryByParam = "*")]
        public ActionResult Get(string album, string item, string size) {
            var imageSize = ImageSize.Parse(size);

            var file = this.gallery.GetItem(album, item, this.User).File;
            var fileName = file.Name;
            if (imageSize != ImageSize.Original) {
                file = this.preview.GetPreview(file, imageSize.Size);
                fileName = string.Format(
                    "{0}_{1}px.{2}",
                    fileName.SubstringBefore("."),
                    imageSize.Size,
                    fileName.SubstringAfter(".")
                );
            }

            var ifModifiedSince = Request.Headers["If-Modified-Since"];
            var lastModifiedDate = file.GetLastWriteTime();
            if (!string.IsNullOrEmpty(ifModifiedSince)) {
                var ifModifiedSinceDate = DateTimeOffset.Parse(ifModifiedSince);

                if ((lastModifiedDate - ifModifiedSinceDate).TotalSeconds < 1) {
                    Response.StatusCode = 304;
                    return new EmptyResult();
                }
            }

            Response.AddFileDependency(file.Path);
            Response.Cache.SetCacheability(HttpCacheability.Private);
            Response.Cache.SetETagFromFileDependencies();
            Response.Cache.SetLastModifiedFromFileDependencies();

            var maxAge = TimeSpan.Zero;
            if ((DateTimeOffset.Now - lastModifiedDate).TotalDays > 30) { // I think I will not edit these
                maxAge = TimeSpan.FromDays(90);
            }
            else {
                maxAge = TimeSpan.FromHours(1);
            }

            Response.Cache.SetExpires((DateTimeOffset.Now + maxAge).UtcDateTime);
            Response.Cache.SetMaxAge(maxAge);

            return File(
                file.Path,
                knownMimeTypes[Path.GetExtension(file.Name).ToLower()],
                fileName
            );
        }
    }
}
