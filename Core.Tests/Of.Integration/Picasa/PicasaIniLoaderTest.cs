using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Gallio.Framework;
using MbUnit.Framework;
using MbUnit.Framework.ContractVerifiers;

using Moq;

using AshMind.Web.Gallery.Core.IO;
using AshMind.Web.Gallery.Core.Integration.Picasa;
using AshMind.Web.Gallery.Core.Tests.IO.Resources;

namespace AshMind.Web.Gallery.Core.Tests.Of.Integration.Picasa {
    [TestFixture]
    public class PicasaIniLoaderTest {
        [Test]
        public void TestLoadFromReturnsNullWhenFileDoesNotExists() {
            var locationMock = new Mock<ILocation>();
            locationMock.Setup(x => x.GetFile(It.IsAny<string>(), It.IsAny<bool>()))
                        .Returns((IFile)null);

            Assert.IsNull(
                new PicasaIniLoader().LoadFrom(locationMock.Object)
            );
        }

        [Test]
        [Row("picasa.simple.ini",  1, "8f7eee7d92388080", "ashmind_lh", "108")]
        [Row("picasa.complex.ini", 1, "ba3dd2b88e998b4e", "ashmind_lh", "4bfdd6100df50504")]
        [Row("picasa.complex.ini", 2, "8f7eee7d92388080", "ashmind_lh", "108")]
        public void TestLoadFromLoadsContactsCorrectly(string resourceName, int index, string hash, string userCode, string id) {
            var picasaIni = new PicasaIniLoader().LoadFrom(LocationOfResource(resourceName));
            var contact = picasaIni.Contacts.Select(x => new { x.Hash, x.UserCode, x.ID }).ElementAtOrDefault(index - 1);

            Assert.AreEqual(
                new { Hash = hash, UserCode = userCode, ID = id },
                contact                
            );
        }

        [Test]
        public void TestLoadFromLoadsFacesCorrectly() {
            var picasaIni = new PicasaIniLoader().LoadFrom(LocationOfResource("picasa.simple.ini"));
            Assert.AreElementsEqual(
                picasaIni.Items.Select(x => new { x.FileName, Faces = string.Join(";", x.Faces.Select(f => f.ContactHash)) }),
                new[] { new { FileName = "DSC00369.JPG", Faces = "8f7eee7d92388080" } }
            );
        }

        private ILocation LocationOfResource(string name) {
            var locationMock = new Mock<ILocation>();
            locationMock.Setup(x => x.GetFile(".picasa.ini", It.IsAny<bool>()))
                        .Returns(new Resource(this.GetType().Namespace + ".Resources." + name));

            return locationMock.Object;
        }
    }
}
