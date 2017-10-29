using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.MobileServices;
using SunnyDay.Client.Core.Authentication;
using SunnyDay.Client.Core.Helpers;

[assembly:Xamarin.Forms.Dependency(typeof(SunnyDay.Client.UWP.Services.UWPAuthentication))]
namespace SunnyDay.Client.UWP.Services
{
    public class UWPAuthentication : IAuthentication
    {
        public Task<MobileServiceUser> LoginAsync(IMobileServiceClient client, MobileServiceAuthenticationProvider provider, IDictionary<string, string> parameters = null)
        {
            Debug.WriteLine("> Platform : UWP");
            try
            {
                return client.LoginAsync(provider, parameters);
            }
            catch { }

            return null;
        }

        public void ClearCookies()
        {

        }

        public virtual async Task<bool> RefreshUser(IMobileServiceClient client)
        {
            try
            {
                var user = await client.RefreshUserAsync();

                if (user != null)
                {
                    client.CurrentUser = user;
                    Settings.AuthToken = user.MobileServiceAuthenticationToken;
                    Settings.UserId = user.UserId;
                    return true;
                }
            }
            catch (System.Exception e)
            {
                Debug.WriteLine("Unable to refresh user: " + e);
            }

            return false;
        }
    }
}
