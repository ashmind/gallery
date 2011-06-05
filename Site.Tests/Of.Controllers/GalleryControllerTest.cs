using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using AshMind.Gallery.Core.Values;
using Moq;

using MbUnit.Framework;

using AshMind.Gallery.Core;
using AshMind.Gallery.Core.AlbumSupport;
using AshMind.Gallery.Core.Security;
using AshMind.Gallery.Core.Commenting;
using AshMind.Gallery.Site.Controllers;
using AshMind.Gallery.Site.Logic;
using AshMind.Gallery.Site.Models;

namespace AshMind.Gallery.Site.Tests.Of.Controllers {
    [TestFixture]
    public class GalleryControllerTest {
        [Test]
        public void TestStandardAlbumNamesReturnsAlbumsWithoutGaps() {
            var now = DateTimeOffset.Now;
            var albums = Enumerable.Range(0, 40).Select(
                i => MakeAlbum("Album_" + (i + 1), now.AddDays(-i))
            ).ToArray();
            var facadeMock = new Mock<IAlbumFacade>();
            facadeMock.Setup(x => x.GetAlbums(AlbumProviderKeys.Default, It.IsAny<IUser>()))
                      .Returns(albums);

            var controller = this.CreateController(facadeMock.Object);

            var loadedNames = Enumerable.Concat(
                GetModel<GalleryViewModel>(controller.StandardAlbumNames(0, 20)).StandardAlbums.List,
                GetModel<GalleryViewModel>(controller.StandardAlbumNames(21, 40)).StandardAlbums.List
            ).Select(a => a.Album.Name);

            Assert.AreElementsEqual(albums.Select(a => a.Name), loadedNames);
        }

        private static Album MakeAlbum(string name, DateTimeOffset date) {
            return new Album(
                new AlbumDescriptor(AlbumProviderKeys.Default, ""),
                name, null,
                To.Just(new[] { new AlbumItem(null, "", AlbumItemType.Image, date, () => new Comment[0]) })
            );
        }

        private T GetModel<T>(ActionResult result) {
            return (T)((ViewResultBase)result).ViewData.Model;
        }

        private GalleryController CreateController(IAlbumFacade facade = null) {
            return new GalleryController(
                facade ?? new Mock<IAlbumFacade>().Object,
                new Mock<IAuthorizationService>().Object,
                new Mock<ICommentRepository>().Object,
                new Mock<IUserAuthentication>().Object,
                new Mock<IImageRequestStrategy>().Object
            );
        }
    }
}
