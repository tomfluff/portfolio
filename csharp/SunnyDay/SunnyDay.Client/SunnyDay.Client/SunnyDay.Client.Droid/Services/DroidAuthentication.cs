using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Android.OS;
using Microsoft.WindowsAzure.MobileServices;
using SunnyDay.Client.Core.Authentication;
using SunnyDay.Client.Core.Helpers;
using Xamarin.Forms;
using Debug = System.Diagnostics.Debug;

[assembly:Xamarin.Forms.Dependency(typeof(SunnyDay.Client.Droid.Services.DroidAuthentication))]
namespace SunnyDay.Client.Droid.Services
{
    public class DroidAuthentication : IAuthentication
    {
        public Task<MobileServiceUser> LoginAsync(IMobileServiceClient client, MobileServiceAuthenticationProvider provider, IDictionary<string, string> parameters = null)
        {
            System.Diagnostics.Debug.WriteLine("> Platform : Android");
            try
            {
                return client.LoginAsync(Forms.Context, provider, parameters);
            }
            catch { }

            return null;
        }

        public void ClearCookies()
        {
            System.Diagnostics.Debug.WriteLine("> Platform : Android");
            try
            {
                if ((int)global::Android.OS.Build.VERSION.SdkInt >= 21)
                    global::Android.Webkit.CookieManager.Instance.RemoveAllCookies(null);
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine("Error clearing cookies : " + e);
                throw;
            }
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