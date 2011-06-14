using System;
using System.Collections.Generic;
using AshMind.IO.Abstraction;
using MbUnit.Framework;

using Moq;

using AshMind.Extensions;

using AshMind.Gallery.Core.AlbumSupport;
using AshMind.Gallery.Core.Metadata;
using AshMind.Gallery.Imaging;

namespace AshMind.Gallery.Core.Tests.Of.AlbumSupport {
    [TestFixture]
    public class AlbumItemFactoryTest {
        [Test]
        [Row("jpg", AlbumItemType.Image)]
        [Row("wtf", AlbumItemType.Unknown)]
        public void TestGetItemTypeReturnsImageWhenFormatIsPresent(string extension, AlbumItemType expectedItemType) {
            var factory = new AlbumItemFactory(new[] { MockImageFormat("jpg") }, new IMetadataStore<AlbumItem>[0]);
            Assert.AreEqual(expectedItemType, factory.GetItemType(MockFile(extension)));
        }

        private IFile MockFile(string extension) {
            var fileMock = new Mock<IFile>();
            fileMock.SetupGet(x => x.Extension).Returns(extension);
            return fileMock.Object;
        }

        private IImageFormat MockImageFormat(string extension) {
            var formatMock = new Mock<IImageFormat>();
            formatMock.SetupGet(x => x.FileExtensions).Returns(new[] { extension }.AsReadOnly());
            return formatMock.Object;
        }
    }
}
