﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Net.Mime;

using AshMind.Extensions;

using AshMind.Gallery.Core;
using AshMind.Gallery.Site.Logic;
using AshMind.Gallery.Site.Models;

namespace AshMind.Gallery.Site.Controllers {
    public class ImageController : ControllerBase {
        private readonly IDictionary<string, string> knownMimeTypes = new Dictionary<string, string> {
            {".jpg",    MediaTypeNames.Image.Jpeg},
            {".jpeg",   MediaTypeNames.Image.Jpeg},
            {".png",    "image/png"}
        };

        private readonly PreviewFacade preview;
        private readonly IImageRequestStrategy strategy;

        public ImageController(
            PreviewFacade preview,
            IUserAuthentication authentication,
            IImageRequestStrategy strategy
        )
            : base(authentication)
        {
            this.preview = preview;
            this.strategy = strategy;
        }

        //[OutputCache(Duration = 60, VaryByParam = "*")]
        public ActionResult Get(string size) {
            if (!this.strategy.IsAuthorized(ControllerContext.RequestContext))
                return Unauthorized();

            var imageSize = ImageSize.Parse(size);                       

            var file = this.strategy.GetImageFile(ControllerContext.RequestContext);
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

            var maxAge = (DateTimeOffset.Now - lastModifiedDate).TotalDays > 30 // I think I will not edit these
                       ? TimeSpan.FromDays(90)
                       : TimeSpan.FromHours(1);

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
