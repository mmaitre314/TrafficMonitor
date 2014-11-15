using Microsoft.WindowsAzure.Mobile.Service.Security;
using System.Web.Http;

namespace TrafficMonitorMobileService
{
    // Note: classes exposing Web APIs must end with a "Controller" suffix
    /// <summary></summary>
    [AuthorizeLevel(AuthorizationLevel.User)]
    public class MiscController : ApiController
    {
        /// <summary>
        /// Test that the WebApi service is alive
        /// </summary>
        [HttpGet, Route("api/ping")]
        public IHttpActionResult Ping()
        {
            return Ok();
        }
   }
}