using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Net.Mime;

using AshMind.Web.Gallery.Core;
using AshMind.Web.Gallery.Site.Models;
using AshMind.Web.Gallery.Core.IO;

namespace AshMind.Web.Gallery.Site.Controllers {
    [Authorize]
    public class ImageController : Controller {
        private IDictionary<string, string> knownMimeTypes = new Dictionary<string, string> {
            {".jpg",    MediaTypeNames.Image.Jpeg},
            {".jpeg",   MediaTypeNames.Image.Jpeg},
            {".png",    "image/png"}
        };

        private readonly AlbumFacade gallery;
        private readonly PreviewFacade preview;

        public ImageController(AlbumFacade gallery, PreviewFacade preview) {
            this.gallery = gallery;
            this.preview = preview;
        }

        //[OutputCache(Duration = 60, VaryByParam = "*")]
        public ActionResult Get(string album, string item, string size) {
            var imageSize = ImageSize.Parse(size);
            var path = this.gallery.GetItemFile(album, item).Path;
            if (imageSize != ImageSize.Original)
                path = this.preview.GetPreviewPath(path, imageSize.Size);

            var ifModifiedSince = Request.Headers["If-Modified-Since"];
            var lastModifiedDate = (DateTimeOffset)System.IO.File.GetLastWriteTimeUtc(path);
            if (!string.IsNullOrEmpty(ifModifiedSince)) {
                var ifModifiedSinceDate = DateTimeOffset.Parse(ifModifiedSince);

                if ((lastModifiedDate - ifModifiedSinceDate).TotalSeconds < 1) {
                    Response.StatusCode = 304;
                    return new EmptyResult();
                }
            }

            Response.AddFileDependency(path);
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
                path,
                knownMimeTypes[Path.GetExtension(path).ToLower()],
                Path.GetFileName(path)
            );
        }
    }
}
