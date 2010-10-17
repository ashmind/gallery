using System;
using System.Collections.Generic;
using System.Text;
using System.Web;
using System.Web.Routing;

using Gallio.Framework;

using MbUnit.Framework;
using MbUnit.Framework.ContractVerifiers;

using Moq;

using AshMind.Gallery.Site.Logic.ImageRequest;

namespace AshMind.Gallery.Site.Tests.Of.Logic.ImageRequest {
    [TestFixture]
    public class CookielessImageRequestStrategyTest {
        [Test]
        public void TestUrlIsConsistent() {
            var strategy = new CookielessImageRequestStrategy(null);

            var context = new RequestContext { 
                RouteData = new RouteData(),
                HttpContext = MockHttpContextForUrlHelper()
            };

            strategy.MapRoute(RouteTable.Routes, "Image", "Get");

            var url1 = strategy.GetActionUrl(context, "TestAlbum", "TestItem");
            var url2 = strategy.GetActionUrl(context, "TestAlbum", "TestItem");

            Assert.AreEqual(url1, url2);
        }

        private HttpContextBase MockHttpContextForUrlHelper() {
            var httpContextMock = new Mock<HttpContextBase>();

            httpContextMock.Setup(m => m.Request.ApplicationPath)
                           .Returns("test");
            httpContextMock.Setup(m => m.Response.ApplyAppPathModifier(It.IsAny<string>()))
                           .Returns((string value) => value);

            return httpContextMock.Object;
        }
    }
}
