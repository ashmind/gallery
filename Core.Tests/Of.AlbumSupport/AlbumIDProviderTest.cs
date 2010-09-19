using System;
using System.Collections.Generic;
using System.Text;

using Gallio.Framework;
using MbUnit.Framework;
using MbUnit.Framework.ContractVerifiers;

using AshMind.Web.Gallery.Core.AlbumSupport;
using AshMind.Web.Gallery.Core.IO;
using AshMind.Web.Gallery.Core.Tests.IO;

namespace AshMind.Web.Gallery.Core.Tests.Of.AlbumSupport {
    [TestFixture]
    public class AlbumIDProviderTest {
        [Test]
        public void TestGetAlbumIDIsConsistent() {
            var idmap = new MemoryFile();
            
            var albumPath = "testpath";

            var firstProvider = CreateProvider(idmap);
            var id = firstProvider.GetAlbumID(new MemoryLocation { Path = albumPath });
            var secondProvider = CreateProvider(idmap);
            var result = secondProvider.GetAlbumLocation(id);

            Assert.AreEqual(albumPath, result.Path);
        }

        private AlbumIDProvider CreateProvider(IFile idmap) {
            return new AlbumIDProvider(
                new MemoryLocation {
                    { "idmap", idmap }
                },
                new MemoryFileSystem()
            );
        }
    }
}
