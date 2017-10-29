using System;
using System.Reflection;
using Acr.UserDialogs;
using Android.App;
using Android.Content.PM;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using CarouselView.FormsPlugin.Android;
using Plugin.Permissions;
using Refractored.XamForms.PullToRefresh.Droid;
using SunnyDay.Client.Droid.Services;
using SunnyDay.Client.Core.Services;
using Xamarin.Forms;

namespace SunnyDay.Client.Droid
{
	[Activity (Label = "SunnyDay.Client", Icon = "@drawable/icon", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
	public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsApplicationActivity
	{
		protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);

			global::Xamarin.Forms.Forms.Init (this, bundle);

            // prevents linker errors
            Microsoft.WindowsAzure.MobileServices.CurrentPlatform.Init();
            PullToRefreshLayoutRenderer.Init();
            CarouselViewRenderer.Init();
            UserDialogs.Init(this);

            LoadApplication (new SunnyDay.Client.App ());
		}

        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, Permission[] grantResults)
        {
            PermissionsImplementation.Current.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }
    }
}

