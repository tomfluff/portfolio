using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using SunnyDay.Client.Core.Helpers;
using SunnyDay.Client.Core.Services;
using SunnyDay.Client.Styles;
using SunnyDay.Client.Core.Utils;
using SunnyDay.Client.Views;
using Xamarin.Forms;

namespace SunnyDay.Client.ViewModels
{
    public class LoginViewModel : BaseViewModel
    {
        private ICloudService _service;

        public LoginViewModel()
        {
            Title = "Welcome";
            _service = ServiceLocator.Instance.Resolve<ICloudService>();
        }

        #region Command : FacebookLogin

        /// <summary>
        /// A command property representing the "FacebookLogin" operation
        /// </summary>
        ICommand _facebookLoginCommand;
        public ICommand FacebookLoginCommand =>
            _facebookLoginCommand ?? (_facebookLoginCommand = new Command(async () => await ExecuteFacebookLoginCommandAsync()));

        async Task ExecuteFacebookLoginCommandAsync()
        {
            if (IsBusy)
                return;

            Debug.WriteLine("> Facebook login command received");

            IsBusy = true;

            // Attempt to log-in (if failed return)
            if (!(await LoginAsync(AuthType.Facebook)))
            {
                IsBusy = false;
                return;
            }

            try
            {
                //await ServiceLocator.Instance.Resolve<IBandService>().Start();

                var user = await _service.GetUser(Settings.UserId);

                MessagingCenter.Send(new Core.BeginExtendedExecutionMessage(), "BeginExtendedExecutionMessage");

                if (user == null)
                {
                    // New user
                    var userInfo = await _service.GetVerifiedUserSocialInfo();
                    await _service.AddUser(userInfo["name"], userInfo["email"], -1, 0, true, userInfo["picture"]);

                    Application.Current.MainPage = new TakePictureView();
                }
                else if (user.SkinTone == -1)
                {
                    // User has no picture yet
                    Application.Current.MainPage = new TakePictureView();
                }
                else
                {
                    // User has an account
                    Settings.IsSkinToneDetected = true;
                    Application.Current.MainPage = new MasterPageView();
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine($"{e.Source} : {e} ({e.InnerException})");
            }
            finally
            {
                IsBusy = false;
            }
        }

        #endregion

        public async Task<bool> LoginAsync(AuthType type)
        {
            if (Settings.IsLoggedIn)
                return true;

            Debug.WriteLine("> User isn't logged in yet, attempting to login...");
            switch (type)
            {
                case AuthType.Facebook:
                    Settings.AuthProvider = "facebook";
                    break;
                case AuthType.Google:
                    Settings.AuthProvider = "google";
                    break;
                default:
                    Settings.AuthProvider = string.Empty;
                    break;
            }
            try
            {
                var didLogIn = await _service.LoginAsync();

                if (!didLogIn)
                {
                    InfoColor = Resources.OrangeInfoColor;
                    InfoMessage = "Please login to use the app.";
                    InfoDisplay = true;
                }
                else
                {
                    InfoColor = Resources.GreenInfoColor;
                    InfoMessage = "Logged in!";
                    InfoDisplay = true;
                }
                return didLogIn;
            }
            catch (Exception e)
            {
                InfoColor = Resources.RedInfoColor;
                InfoMessage = "An error has occured, please try again.";
                InfoDisplay = true;
                Debug.WriteLine($"{e.Source} : {e} ({e.InnerException})");
            }
            return false;
        }

    }
}
