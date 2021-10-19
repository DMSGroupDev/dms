using dms_backend_api.Controllers;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace dms_backend_api.Tests
{
    [TestClass]
    public class IdentityTests
    {
        [TestMethod]
        public void GetAllProducts_ShouldReturnAllProducts()
        {
            string testText = "echo";
            var controller = new IdentityController();

            var result = controller.Echo();
            Assert.AreEqual(result, testText);
        }
    }
}
