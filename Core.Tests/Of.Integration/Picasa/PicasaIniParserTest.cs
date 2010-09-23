using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Gallio.Framework;
using MbUnit.Framework;
using MbUnit.Framework.ContractVerifiers;

using Moq;

using AshMind.Gallery.Core.IO;
using AshMind.Gallery.Core.Integration.Picasa;
using AshMind.Gallery.Core.Tests.IO;

namespace AshMind.Gallery.Core.Tests.Of.Integration.Picasa {
    [TestFixture]
    public class PicasaIniParserTest {
        [Test]
        [Row("picasa.simple.ini",  1, "8f7eee7d92388080", "ashmind_lh", "108")]
        [Row("picasa.complex.ini", 1, "ba3dd2b88e998b4e", "ashmind_lh", "4bfdd6100df50504")]
        [Row("picasa.complex.ini", 2, "8f7eee7d92388080", "ashmind_lh", "108")]
        public void TestLoadFromLoadsContactsCorrectly(string resourceName, int index, string hash, string userCode, string id) {
            var picasaIni = new PicasaIniParser().Parse(Resource(resourceName));
            var contact = picasaIni.Contacts.Select(x => new { x.Hash, x.UserCode, x.ID }).ElementAtOrDefault(index - 1);

            Assert.AreEqual(
                new { Hash = hash, UserCode = userCode, ID = id },
                contact                
            );
        }

        [Test]
        public void TestLoadFromLoadsFacesCorrectly() {
            var picasaIni = new PicasaIniParser().Parse(Resource("picasa.simple.ini"));
            Assert.AreElementsEqual(
                picasaIni.Items.Select(x => new { FileName = x.Key, Faces = string.Join(";", x.Value.Faces.Select(f => f.ContactHash)) }),
                new[] { new { FileName = "DSC00369.JPG", Faces = "8f7eee7d92388080" } }
            );
        }

        private Resource Resource(string name) {
            return new Resource(this.GetType().Namespace + ".Resources." + name);
        }
    }
}
