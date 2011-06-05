using System;
using System.Collections.Generic;
using System.Linq;
using AshMind.Gallery.Core.Security.Actions;
using AshMind.Gallery.Core.Security.Rules;
using Moq;

using MbUnit.Framework;

using AshMind.Gallery.Core.Security;
using AshMind.Gallery.Core.Security.Internal;

namespace AshMind.Gallery.Core.Tests.Of.Security {
    [TestFixture]
    public class AuthorizationServiceTest {
        [Test]
        [Row(Authorization.Unknown, Authorization.Unknown, false)]
        [Row(Authorization.Unknown, Authorization.Denied,  false)]
        [Row(Authorization.Unknown, Authorization.Allowed, true)]
        [Row(Authorization.Allowed, Authorization.Unknown, true)]
        [Row(Authorization.Allowed, Authorization.Allowed, true)]
        [Row(Authorization.Allowed, Authorization.Denied,  false)]
        [Row(Authorization.Denied,  Authorization.Unknown, false)]
        [Row(Authorization.Denied,  Authorization.Allowed, false)]
        [Row(Authorization.Denied,  Authorization.Denied, false)]
        public void TestAuthorizationInRuleHierarchy(Authorization groupAuthorization, Authorization userAuthorization, bool isAuthorized) {
            var user = new KnownUser("");
            var group = new UserGroup { Users = { user } };
            var action = SecurableActions.View(new object());

            var groupRule = Mock<IAuthorizationRule>(m => m.Setup(x => x.GetAuthorization(group, action)).Returns(groupAuthorization));
            var userRule = Mock<IAuthorizationRule>(m => m.Setup(x => x.GetAuthorization(user, action)).Returns(userAuthorization));

            var repository = Mock<IRepository<UserGroup>>(
                m => m.Setup(x => x.Query()).Returns(new[] { group }.AsQueryable())
            );
            
            var service = new AuthorizationService(
                repository,
                new[] { groupRule, userRule }, 
                new IPermissionProvider[0]
            );

            Assert.AreEqual(isAuthorized, service.IsAuthorized(user, action));
        }

        private T Mock<T>(Action<Mock<T>> setup) 
            where T : class
        {
            var mock = new Mock<T>();
            setup(mock);
            return mock.Object;
        }
    }
}
