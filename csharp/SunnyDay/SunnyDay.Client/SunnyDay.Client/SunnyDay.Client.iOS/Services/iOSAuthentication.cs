using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.MobileServices;
using SunnyDay.Client.Authentication;

[assembly:Xamarin.Forms.Dependency(typeof(SunnyDay.Client.iOS.Services.iOSAuthentication))]
namespace SunnyDay.Client.iOS.Services
{
    public class iOSAuthentication : IAuthentication
    {
        public Task<MobileServiceUser> LoginAsync(IMobileServiceClient client, MobileServiceAuthenticationProvider provider, IDictionary<string, string> parameters = null)
        {
            Debug.WriteLine("> Platform : iOS");
            try
            {
                return client.LoginAsync(GetController(), provider, parameters);
            }
            catch (Exception e)
            {
                e.Data["method"] = "LoginAsync";
            }
            return null;
        }

        UIKit.UIViewController GetController()
        {
            var window = UIKit.UIApplication.SharedApplication.KeyWindow;
            var root = window.RootViewController;
            if (root == null)
                return null;

            var current = root;
            while (current.PresentedViewController != null)
            {
                current = current.PresentedViewController;
            }

            return current;
        }

        public void ClearCookies()
        {
            try
            {
                var store = Foundation.NSHttpCookieStorage.SharedStorage;
                var cookies = store.Cookies;

                foreach (var c in cookies)
                {
                    store.DeleteCookie(c);
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine("Error Logging in : " + e);
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