using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Plugin.Media;
using Plugin.Media.Abstractions;
using SunnyDay.Client.Core.Authentication;
using SunnyDay.Client.Core.Helpers;
using SunnyDay.Client.Core.Services;
using SunnyDay.Client.Core.Utils;
using SunnyDay.Client.Views;
using Xamarin.Forms;

namespace SunnyDay.Client
{
	public class App : Application
	{
	    public App()
	    {
            // Data service connection
            ServiceLocator.Instance.Add<ICloudService, AzureCloudService>();

            // Weather Service
            ServiceLocator.Instance.Add<IWeatherService, WeatherUnderground>();

            // SPF provider service
            ServiceLocator.Instance.Add<ISkinToneProviderService, SkinToneProviderService>();

            // Picture providing service
            ServiceLocator.Instance.Add<IPictureProviderService, PictureProviderService>();

            // Band service connection
            ServiceLocator.Instance.Add<IBandService, MicrosoftBandService>();

            // Initialize the cross media plugin for photo taking
            CrossMedia.Current.Initialize();
            
            // The root page of your application
            if (Settings.IsLoggedIn == false)
            {
                // Not logged in
                MainPage = new LoginView();
            }
            else if (Settings.IsSkinToneDetected == false)
            {
                MessagingCenter.Send(new Core.BeginExtendedExecutionMessage(), "BeginExtendedExecutionMessage");

                // No skin tone yet
                MainPage = new TakePictureView();
            }
            else
	        {
                Debug.WriteLine("> User logged in : Provider=" + Settings.AuthProvider + ", UserId="+ Settings.UserId);
                
                MainPage = new MasterPageView();
            }
        }

	    protected override async void OnStart ()
		{
            base.OnStart();
            Debug.WriteLine("Application OnStart()");

		    Settings.MonitorInBackground = true;
            //await ServiceLocator.Instance.Resolve<IBandService>().Start();

            // Install (if needed) alarm mechanism
            MessagingCenter.Send(new Core.StartAlarmAlertTaskMessage(), "StartAlarmAlertTaskMessage");

            // Handle when your app starts
        }

		protected override async void OnSleep ()
        {
            base.OnSleep();

            Debug.WriteLine("Application OnSleep()");
            // Handle when your app sleeps

            //await ServiceLocator.Instance.Resolve<IBandService>().Stop();

            Settings.MonitorInBackground = false;
        }

		protected override async void OnResume ()
        {
            base.OnResume();

            Debug.WriteLine("Application OnResume()");
            // Handle when your app resumes

            //await ServiceLocator.Instance.Resolve<IBandService>().Start();
        }
    }
}
