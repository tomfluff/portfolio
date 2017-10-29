using System;
using System.Security.Claims;
using System.Security.Principal;
using System.Threading.Tasks;
using System.Web.Http;
using Microsoft.Azure.Mobile.Server.Authentication;
using Microsoft.Azure.Mobile.Server.Config;

namespace SunnyDay.Backend.Controllers
{
    [MobileAppController]
    public class SocialInfoController : ApiController
    {
        // GET api/SocialInfo
        public async Task<string> Get()
        {
            try
            {
                // Get the credentials for the logged-in user.
                var credentials =
                    await this.User
                    .GetAppServiceIdentityAsync<FacebookCredentials>(this.Request);

                if (credentials?.Provider == "Facebook")
                {
                    // Create a query string with the Facebook access token.
                    var fbRequestUrl = "https://graph.facebook.com/me?fields=id,name,picture,email&access_token="
                                       + credentials.AccessToken;

                    // Create an HttpClient request.
                    var client = new System.Net.Http.HttpClient();

                    // Request the current user info from Facebook.
                    var resp = await client.GetAsync(fbRequestUrl);
                    resp.EnsureSuccessStatusCode();

                    // Do something here with the Facebook user information.
                    var fbInfo = await resp.Content.ReadAsStringAsync();

                    return fbInfo;
                }
                else
                {
                    return "Unknown";
                }
            }
            catch (Exception e)
            {
                return e.StackTrace;
            }
        }
    }
}
