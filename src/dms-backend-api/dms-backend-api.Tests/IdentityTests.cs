using AutoMapper;
using dms_backend_api.Controllers;
using dms_backend_api.Domain.Identity;
using dms_backend_api.Factories;
using dms_backend_api.Services.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace dms_backend_api.Tests
{
    [TestClass]
    public class IdentityTests
    {
        [TestMethod]
        public void EchoTest()
        {
            string testText = "Echo";
            var controller = new IdentityController(
                new Mock<IIdentityService>().Object,
                new Mock<ILogger<IdentityController>>().Object,
                new Mock<IMapper>().Object,
                new Mock<IEmailSender>().Object,
                new Mock<IErrorFactory>().Object,
                new Mock<IUserTwoFactorTokenProvider<ApplicationUser>>().Object);

            var result = controller.Echo();
            Assert.AreEqual(result, testText);
        }
    }
}
