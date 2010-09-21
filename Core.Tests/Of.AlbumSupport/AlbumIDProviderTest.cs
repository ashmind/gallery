﻿using System;
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

            var testDescriptor = new AlbumDescriptor("testKey", "testPath");

            var firstProvider = CreateProvider(idmap);
            var id = firstProvider.GetAlbumID("test", testDescriptor);
            var secondProvider = CreateProvider(idmap);
            var resultDescriptor = secondProvider.GetAlbumDescriptor(id);

            Assert.AreEqual(testDescriptor, resultDescriptor);
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