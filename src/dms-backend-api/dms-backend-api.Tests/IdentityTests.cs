using AutoMapper;
using dms_backend_api.Controllers;
using dms_backend_api.Services.Identity;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace dms_backend_api.Tests
{
    [TestClass]
    public class IdentityTests
    {
        [TestMethod]
        public void GetAllProducts_ShouldReturnAllProducts()
        {
            string testText = "Echo";
            var controller = new IdentityController(
                new Mock<IIdentityService>().Object,
                new Mock<ILogger<IdentityController>>().Object,
                new Mock<IHttpContextAccessor>().Object,
                new Mock<IMapper>().Object,
                new Mock<ITokenService>().Object,
                new Mock<IEmailSender>().Object);

            var result = controller.Echo();
            Assert.AreEqual(result, testText);
        }
    }
}
