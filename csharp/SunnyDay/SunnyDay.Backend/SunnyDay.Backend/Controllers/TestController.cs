using System.Web.Http;
using Microsoft.Azure.Mobile.Server.Config;

namespace SunnyDay.Backend.Controllers
{
    [MobileAppController]
    public class TestController : ApiController
    {
        // GET api/Test
        public string Get()
        {
            return "The service is working! (GET)";
        }

        // POST api/Test
        public string Post()
        {
            return "The service is working! (POST)";
        }
    }
}
