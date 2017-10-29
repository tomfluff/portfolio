using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Microsoft.WindowsAzure.Storage.Blob;
using MvvmHelpers;
using PCLStorage;
using SunnyDay.Client.Core.Helpers;
using SunnyDay.Client.Core.Models;
using SunnyDay.Client.Core.Services;
using SunnyDay.Client.Styles;
using SunnyDay.Client.Core.Utils;
using Xamarin.Forms;

namespace SunnyDay.Client.ViewModels
{
    public class MainPageViewModel : BaseViewModel
    {
        private const double _uvDisplayBoxBaseHeight = 25;
        private bool _isNewUserCreated = false;
        private readonly TimeSpan _timerTickSpan = TimeSpan.FromMilliseconds(1000);

        private readonly ICloudService _service;
        private readonly IBandService _band;
        private readonly IWeatherService _weather;
        private readonly IPictureProviderService _pictureProvider;
        private readonly ISkinToneProviderService _skinToneProvider;
        
        #region Property : MainUser

        private UserViewModel _mainUser = null;
        public const string MainUserPropertyName = "MainUser";
        /// <summary>
        /// Gets or sets the "MainUser" property
        /// </summary>
        /// <value>The property value.</value>
        public UserViewModel MainUser
        {
            get { return _mainUser; }
            set { SetProperty(ref _mainUser, value, MainUserPropertyName); }
        }

        #endregion

        public ObservableRangeCollection<UserViewModel> SubUsers { get; set; } // Sub-users of the main user
        public List<string> AllSpfValues { get; }

        public MainPageViewModel()
		{
            // Will be called each time the page is displayed
			Title = "Real-Time Protection";
			CurrUvLevel = 0;
            AllSpfValues = new List<string>();
            SubUsers = new ObservableRangeCollection<UserViewModel>();

            _weather = ServiceLocator.Instance.Resolve<IWeatherService>();
			_service = ServiceLocator.Instance.Resolve<ICloudService>();
			_band = ServiceLocator.Instance.Resolve<IBandService>();
            _pictureProvider = ServiceLocator.Instance.Resolve<IPictureProviderService>();
            _skinToneProvider = ServiceLocator.Instance.Resolve<ISkinToneProviderService>();

            foreach (var value in Enum.GetNames(typeof(SpfValues)))
            {
                AllSpfValues.Add($"{(int)Enum.Parse(typeof(SpfValues), value)} ({Enum.Parse(typeof(SpfValues), value)})");
            }

            Task.Run(async () =>
			{
				await Initialize();
            });
		}

        private async Task Initialize()
        {
            if (IsBusy)
                return;

            IsBusy = true;

            try
            {
                Debug.WriteLine($"> Initialization started, IsBusy={IsBusy}");

                //await ServiceLocator.Instance.Resolve<IBandService>().Start();
                await GetUserDetails();

                if (Settings.MonitorInBackground)
                {
                    // Request extended execution for band tracking
                    MessagingCenter.Send(new Core.BeginExtendedExecutionMessage(), "BeginExtendedExecutionMessage");
                    // disactivate since user can navigate between pages
                    Settings.MonitorInBackground = false;
                }

                await GetWeatherInformation();

                SetExposureInfo();

                CurrentSubUserEdit = _mainUser;

                InfoDisplay = false;
            }
            catch (FailtToConnectToBandException)
            {
                InfoColor = Resources.RedInfoColor;
                InfoMessage =
                    $"Could not connect to your band. Make sure it is activated and in reach and refresh to try again.";
                InfoDisplay = true;
            }
            catch (LocationServicesDisabled)
            {
                InfoColor = Resources.RedInfoColor;
                InfoMessage =
                    $"Location services unavailable. Enable and refresh to try again.";
                InfoDisplay = true;
            }
            catch (CannotConnectToTheDestination)
            {
                InfoColor = Resources.RedInfoColor;
                InfoMessage =
                    $"Cannot reach the remote destination. Try again later.";
                InfoDisplay = true;
            }
            catch (Exception e)
            {
                InfoColor = Resources.RedInfoColor;
                InfoMessage = $"An error has occured. Try again later.";
                InfoDisplay = true;
                Debug.WriteLine($"{e.Source} : {e} ({e.InnerException})");
            }
            finally
            {
                await Task.Delay(500);
                IsBusy = false;
            }

            Debug.WriteLine("> Finished Initialization");
        }

        private async void SetExposureInfo()
        {
            DailyExposure = _band.IsInitialized ? _band.CurrentDailyExposure : 0;
            //var uv = (await _service.GetUserLastUvReadingsByCount(Settings.UserId, 1)).FirstOrDefault();
            if (_band.CurrentUV > 1)
            {
                CurrUvLevel = SunnyDay.Client.Core.Utils.DataConvert.Uv.UV_ToPublic(_band.CurrentUV);
                CurrUvColor = SunnyDay.Client.Core.Utils.DataConvert.Uv.UV_ToColor(_band.CurrentUV);
                CurrUvDisplayHeight = _uvDisplayBoxBaseHeight * (_band.CurrentUV - 1);
            }
            else
            {
                CurrUvLevel = _weather.UvLevel != -1 ? _weather.UvLevel : 0;
                CurrUvColor =
                    SunnyDay.Client.Core.Utils.DataConvert.Uv.UV_ToColor(
                        SunnyDay.Client.Core.Utils.DataConvert.Uv.UV_ToLocal(_weather.UvLevel));
                CurrUvDisplayHeight = _uvDisplayBoxBaseHeight *
                                      (SunnyDay.Client.Core.Utils.DataConvert.Uv.UV_ToLocal(_weather.UvLevel) - 1);
            }
        }

        private bool MainTimerCallback()
        {
            SetExposureInfo();
            if (MainUser.IsAlarmActive)
            {
                MainUser.UpdateTimer();
            }
            else
            {
                MainUser.ZeroDisplayAlarm();
            }
            foreach (var user in SubUsers)
            {
                if (user.IsAlarmActive)
                {
                    user.UpdateTimer();
                }
                else
                {
                    user.ZeroDisplayAlarm();
                }
            }
            return true;
        }

        private async Task GetUserDetails()
        {
            MainUser = new UserViewModel(await _service.GetUser(Settings.UserId), await _service.GetActiveUserAlarm(Settings.UserId), true);

            var userGroup = await _service.GetUserSubGroup(MainUser.UserItem.GroupId);
            if (userGroup.Any())
            {
                Device.BeginInvokeOnMainThread(() => SubUsers.Clear() );
                Debug.WriteLine($"> Found other users in group");
                foreach (var usr in userGroup)
                {
                    Debug.WriteLine($"> Found sub-user: {usr.Name}, {usr.SpfLevel}spf, {usr.IsVerified}");
                    Device.BeginInvokeOnMainThread(async () => SubUsers.Add(new UserViewModel(usr,await _service.GetActiveUserAlarm(usr.UserId), false)));
                }
            }

            Device.BeginInvokeOnMainThread(() => Device.StartTimer(_timerTickSpan, MainTimerCallback));
        }

        private async Task GetWeatherInformation()
        {
            Debug.WriteLine("> Refreshing weather information");
            await _weather.UpdateWeather();

            WeatherLocation = _weather.Location;
            WeatherText = _weather.Temperture;
            WeatherDate = _weather.Date;
            WeatherImage = _weather.Icon;

            Debug.WriteLine($"> Got Weather: Loc={_weather.Location}, Temp={_weather.Temperture}" +
                            $" Date={_weather.Date}, UV={_weather.UvLevel} Alt={_weather.Altitude}, Icon={_weather.Icon}");
        }

        #region Property : WeatherImage

        private string _weatherImage = string.Empty;
        public const string WeatherImagePropertyName = "WeatherImage";

        /// <summary>
        /// Gets or sets the "WeatherImage" property
        /// </summary>
        /// <value>The property value.</value>
        public string WeatherImage
        {
            get { return _weatherImage; }
            set { SetProperty(ref _weatherImage, value, WeatherImagePropertyName); }
        }

        #endregion

        #region Property : WeatherLocation

        private string _weatherLocation = string.Empty;
        public const string WeatherLocationPropertyName = "WeatherLocation";

        /// <summary>
        /// Gets or sets the "WeatherLocation" property
        /// </summary>
        /// <value>The property value.</value>
        public string WeatherLocation
        {
            get { return _weatherLocation; }
            set { SetProperty(ref _weatherLocation, value, WeatherLocationPropertyName); }
        }

        #endregion

        #region Property : WeatherDate

        private string _weatherDate = string.Empty;
        public const string WeatherDatePropertyName = "WeatherDate";

        /// <summary>
        /// Gets or sets the "WeatherDate" property
        /// </summary>
        /// <value>The property value.</value>
        public string WeatherDate
        {
            get { return _weatherDate; }
            set { SetProperty(ref _weatherDate, value, WeatherDatePropertyName); }
        }

        #endregion

        #region Property : WeatherText

        private string _weatherText = string.Empty;
        public const string WeatherTextPropertyName = "WeatherText";

        /// <summary>
        /// Gets or sets the "WeatherText" property
        /// </summary>
        /// <value>The property value.</value>
        public string WeatherText
        {
            get { return _weatherText; }
            set { SetProperty(ref _weatherText, value, WeatherTextPropertyName); }
        }

        #endregion

        #region Property : CurrUvLevel

        private int _currUvLevel = 0;
        public const string CurrUvLevelPropertyName = "CurrUvLevel";

        /// <summary>
        /// Gets or sets the "CurrUvLevel" property
        /// </summary>
        /// <value>The property value.</value>
        public int CurrUvLevel
        {
            get { return _currUvLevel; }
            set { SetProperty(ref _currUvLevel, value, CurrUvLevelPropertyName); }
        }

        #endregion

        #region Property : CurrUvColor

        private Color _currUvColor = Color.Orange;
        public const string CurrUvColorPropertyName = "CurrUvColor";

        /// <summary>
        /// Gets or sets the "CurrUvColor" property
        /// </summary>
        /// <value>The property value.</value>
        public Color CurrUvColor
        {
            get { return _currUvColor; }
            set { SetProperty(ref _currUvColor, value, CurrUvColorPropertyName); }
        }

        #endregion

        #region Property : DailyExposure

        private int _dailyExposure = 0;
        public const string DailyExposurePropertyName = "DailyExposure";
        /// <summary>
        /// Gets or sets the "DailyExposure" property
        /// </summary>
        /// <value>The property value.</value>
        public int DailyExposure
        {
            get { return _dailyExposure; }
            set { SetProperty(ref _dailyExposure, value, DailyExposurePropertyName); }
        }

        #endregion

        #region Property : CurrUvDisplayHeight

        private double _currUvDisplayHeight = 0;
        public const string CurrUvDisplayHeightPropertyName = "CurrUvDisplayHeight";

        /// <summary>
        /// Gets or sets the "CurrUvDisplayWidth" property
        /// </summary>
        /// <value>The property value.</value>
        public double CurrUvDisplayHeight
        {
            get { return _currUvDisplayHeight; }
            set { SetProperty(ref _currUvDisplayHeight, value, CurrUvDisplayHeightPropertyName); }
        }

        #endregion

        #region Property : IsSubUserEditVisible

        private bool _isSubUserEditVisible = false;
        public const string IsSubUserEditVisiblePropertyName = "IsSubUserEditVisible";
        /// <summary>
        /// Gets or sets the "IsSubUserEditVisible" property
        /// </summary>
        /// <value>The property value.</value>
        public bool IsSubUserEditVisible
        {
            get { return _isSubUserEditVisible; }
            set { SetProperty(ref _isSubUserEditVisible, value, IsSubUserEditVisiblePropertyName); }
        }

        #endregion

        #region Property : CurrentSubUserEdit

        private UserViewModel _currentSubUserEdit = null;
        public const string CurrentSubUserEditPropertyName = "CurrentSubUserEdit";
        /// <summary>
        /// Gets or sets the "CurrentSubUserEdit" property
        /// </summary>
        /// <value>The property value.</value>
        public UserViewModel CurrentSubUserEdit
        {
            get { return _currentSubUserEdit; }
            set { SetProperty(ref _currentSubUserEdit, value, CurrentSubUserEditPropertyName); }
        }

        #endregion

        #region Command : EditSubUser

        /// <summary>
        /// A command property representing the "EditSubUser" operation
        /// </summary>
        ICommand _editSubUserCommand;
        public ICommand EditSubUserCommand =>
            _editSubUserCommand ?? (_editSubUserCommand = new Command(async (obj) => await ExecuteEditSubUserCommandAsync(obj)));

        async Task ExecuteEditSubUserCommandAsync(Object obj)
        {
            if (IsBusy)
                return;

            IsBusy = true;

            var user = obj as UserViewModel;
            if (user == null)
            {
                IsBusy = false;
                return;
            }

            CurrentSubUserEdit = user;
            CurrentSubUserEdit.InitializeEditView();
            Debug.WriteLine($"> {CurrentSubUserEdit.Name}, {CurrentSubUserEdit.SkinTone}, {CurrentSubUserEdit.SpfLevel}");
            IsSubUserEditVisible = true;

            IsBusy = false;
        }

        #endregion

        #region Command : SaveEditSubUser

        /// <summary>
        /// A command property representing the "SaveEditSubUser" operation
        /// </summary>
        ICommand _saveEditSubUserCommand;
        public ICommand SaveEditSubUserCommand =>
            _saveEditSubUserCommand ?? (_saveEditSubUserCommand = new Command(async () => await ExecuteSaveEditSubUserCommandAsync()));

        async Task ExecuteSaveEditSubUserCommandAsync()
        {
            if (IsBusy)
                return;

            IsBusy = true;

            await CurrentSubUserEdit.UpdateUser();
            IsSubUserEditVisible = false;
            await GetUserDetails();

            _isNewUserCreated = false;
            IsBusy = false;
        }

        #endregion

        #region Command : CancelEditSubUser

        /// <summary>
        /// A command property representing the "CancelEditSubUser" operation
        /// </summary>
        ICommand _cancelEditSubUserCommand;
        public ICommand CancelEditSubUserCommand =>
            _cancelEditSubUserCommand ?? (_cancelEditSubUserCommand = new Command(async () => await ExecuteCancelEditSubUserCommandAsync()));

        async Task ExecuteCancelEditSubUserCommandAsync()
        {
            if (_isNewUserCreated)
            {
                await ExecuteDeleteSubUserCommandAsync(CurrentSubUserEdit);
                _isNewUserCreated = false;
            }

            IsSubUserEditVisible = false;
        }

        #endregion

        #region Command : DeleteSubUser

        /// <summary>
        /// A command property representing the "DeleteSubUser" operation
        /// </summary>
        ICommand _deleteSubUserCommand;
        public ICommand DeleteSubUserCommand =>
            _deleteSubUserCommand ?? (_deleteSubUserCommand = new Command(async (obj) => await ExecuteDeleteSubUserCommandAsync(obj)));

        async Task ExecuteDeleteSubUserCommandAsync(Object obj)
        {
            if (IsBusy)
                return;

            IsBusy = true;

            var userModel = obj as UserViewModel;
            if (userModel == null)
                return; // TODO: maybe handle error?
            await _service.DeleteUser(userModel.UserItem);
            SubUsers.Remove(userModel);

            IsBusy = false;
        }

        #endregion

        #region Command : SubUserRetakePhoto

        /// <summary>
        /// A command property representing the "SubUserRetakePhoto" operation
        /// </summary>
        ICommand _subUserRetakePhotoCommand;
        public ICommand SubUserRetakePhotoCommand =>
            _subUserRetakePhotoCommand ?? (_subUserRetakePhotoCommand = new Command(async () => await ExecuteSubUserRetakePhotoCommandAsync()));

        async Task ExecuteSubUserRetakePhotoCommandAsync()
        {
            if (IsBusy)
                return;

            Debug.WriteLine("> Attempting to take picture");

            IsBusy = true;
            string filePath = "";
            try
            {
                int skinTone = -1;
                //var light = (await _service.GetUserLastUvReadingsByCount(Settings.UserId, 1)).FirstOrDefault().LightIntensity;
                var light = (int)_band.AverageAmbientLight;
                using (var file = await _pictureProvider.TakePicture(light))
                {
                    InfoDisplay = false;

                    // Get the storage token from the custom API
                    var storageToken = await _service.GetStorageToken(Path.GetFileName(file.Path));
                    var storageUri = new Uri($"{storageToken.Uri}{storageToken.MainSasToken}");
                    var readUri = new Uri($"{storageToken.Uri}{storageToken.ReadSasToken}");

                    // Store the MediaFile to the storage token
                    var blob = new CloudBlockBlob(storageUri);
                    var stream = file.GetStream();
                    await blob.UploadFromStreamAsync(stream);

                    Debug.WriteLine($"> mainstorageuri={storageUri.AbsoluteUri}\n> readstorageuri={readUri.AbsoluteUri}");

                    skinTone = await _skinToneProvider.GetSkinToneFromImageUri(readUri.AbsoluteUri,
                        Path.GetFileName(file.Path), light);
                    Debug.WriteLine($"> Got response!");

                    filePath = file.Path;
                }

                await (await FileSystem.Current.GetFileFromPathAsync(filePath)).DeleteAsync();
                Debug.WriteLine($"> File deleted {filePath}");

                switch (skinTone)
                {
                    case -1:
                        InfoColor = Resources.RedInfoColor;
                        InfoMessage = "An error occured, please try again.";
                        InfoDisplay = true;
                        break;
                    case -2:
                        InfoColor = Resources.OrangeInfoColor;
                        InfoMessage =
                            "Skin tone couldn't be detected correctly, please re-take the picture. Pay attention to the above instructions.";
                        InfoDisplay = true;
                        break;
                    default:
                        CurrentSubUserEdit.UpdateSkinToneEdit(skinTone);
                        break;
                }
            }
            catch (NotEnoughLightException)
            {
                InfoColor = Resources.RedInfoColor;
                InfoMessage =
                    "There isn't enough light around for your skin tone to be detected.";
                InfoDisplay = true;
            }
            catch (CameraNotAvailableException)
            {
                InfoColor = Resources.RedInfoColor;
                InfoMessage =
                    "The camera wasn't available, please make sure your device has a camera and it can be used.";
                InfoDisplay = true;
            }
            catch (FailedTakingPictureException)
            {
                InfoColor = Resources.OrangeInfoColor;
                InfoMessage =
                    "Please take a picture according to the instructions for correct skin tone analysis.";
                InfoDisplay = true;
            }
            catch (TaskCanceledException)
            {
                InfoColor = Resources.OrangeInfoColor;
                InfoMessage =
                    "Please take a picture according to the instructions for correct skin tone analysis.";
                InfoDisplay = true;
            }
            catch (CannotConnectToTheDestination)
            {
                InfoColor = Resources.RedInfoColor;
                InfoMessage =
                    "Please make sure the device is connected to the internet and try again.";
                InfoDisplay = true;
            }
            catch (Exception e)
            {
                InfoColor = Resources.RedInfoColor;
                InfoMessage = "An error has occured. Please try again.";
                InfoDisplay = true;

                Debug.WriteLine($"{e.Source} : {e} ({e.InnerException})");
            }
            finally
            {
                IsBusy = false;
            }
        }

        #endregion

        #region Command : SubUserCreate

        /// <summary>
        /// A command property representing the "SubUserCreate" operation
        /// </summary>
        ICommand _subUserCreateCommand;
        public ICommand SubUserCreateCommand =>
            _subUserCreateCommand ?? (_subUserCreateCommand = new Command(async () => await ExecuteSubUserCreateCommandAsync()));

        async Task ExecuteSubUserCreateCommandAsync()
        {
            if (IsBusy)
                return;

            IsBusy = true;

            var user = new UserViewModel(await _service.AddUser(string.Empty, string.Empty, -1, _mainUser.SpfLevel, false, string.Empty), null, false);
            SubUsers.Add(user);
            IsBusy = false;
            _isNewUserCreated = true;

            await ExecuteEditSubUserCommandAsync(user);
        }

        #endregion
        
        #region Command : Refresh

        /// <summary>
        /// A command property representing the "RefreshCommand" operation
        /// </summary>
        ICommand _refreshCommand;

        public ICommand RefreshCommand =>
            _refreshCommand ?? (_refreshCommand = new Command(async () => {await ExecuteRefreshCommandAsync();}));

        async Task ExecuteRefreshCommandAsync()
        {
            Debug.WriteLine($"> Refreshing started");

            await Initialize();
            Debug.WriteLine($"> Finished refreshing, IsBusy={IsBusy}");
        }

        #endregion

    }
}
