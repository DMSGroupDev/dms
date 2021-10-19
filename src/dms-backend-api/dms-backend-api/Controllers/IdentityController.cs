using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;


namespace dms_backend_api.Controllers
{
    [Authorize]
    [ApiController]
    [Route("/api/identity")]
    public class IdentityController : ControllerBase
    {
        #region Fields

        #endregion

        #region Ctor
        public IdentityController()
        {

        }
        #endregion

        #region Methods
        [HttpGet]
        public string Echo()
        {
            return "echo";
        }
        #endregion
    }
}
