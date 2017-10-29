using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;
using SunnyDay.Client.Views;
using SunnyDay.Client.Core.Utils;
using System.Collections;
using System.Diagnostics;
using System.IO;
using Microsoft.WindowsAzure.Storage.Blob;
using PCLStorage;
using SunnyDay.Client.Core.Helpers;
using SunnyDay.Client.Core.Models;
using SunnyDay.Client.Core.Services;
using SunnyDay.Client.Styles;

namespace SunnyDay.Client.ViewModels
{
    public class SettingsViewModel : BaseViewModel
    {
        private readonly ICloudService _service;
        private readonly IPictureProviderService _pictureProvider;
        private readonly ISkinToneProviderService _skinToneProvider;
        private readonly IBandService _band;
        private readonly User _user;

        public List<string> AllSpfValues { get; }

        #region Property : UserSpfValue

        private int _userSpfValue = 15;
        public const string UserSpfValuePropertyName = "UserSpfValue";
        /// <summary>
        /// Gets or sets the "UserSpfValue" property
        /// </summary>
        /// <value>The property value.</value>
        public int UserSpfValue
        {
            get { return _userSpfValue; }
            set { SetProperty(ref _userSpfValue, value, UserSpfValuePropertyName); }
        }

        #endregion

        #region Property : UserName

        private string _userName = string.Empty;
        public const string UserNamePropertyName = "UserName";
        /// <summary>
        /// Gets or sets the "UserName" property
        /// </summary>
        /// <value>The property value.</value>
        public string UserName
        {
            get { return _userName; }
            set { SetProperty(ref _userName, value, UserNamePropertyName); }
        }

        #endregion

        #region Property : UserEmail

        private string _userEmail = string.Empty;
        public const string UserEmailPropertyName = "UserEmail";
        /// <summary>
        /// Gets or sets the "UserEmail" property
        /// </summary>
        /// <value>The property value.</value>
        public string UserEmail
        {
            get { return _userEmail; }
            set { SetProperty(ref _userEmail, value, UserEmailPropertyName); }
        }

        #endregion

        #region Property : ProfileImage

        private string _profileImage = string.Empty;
        public const string ProfileImagePropertyName = "ProfileImage";
        /// <summary>
        /// Gets or sets the "ProfileImage" property
        /// </summary>
        /// <value>The property value.</value>
        public string ProfileImage
        {
            get { return _profileImage; }
            set { SetProperty(ref _profileImage, value, ProfileImagePropertyName); }
        }

        #endregion

        #region Property : SkinToneText

        private string _skinToneText = string.Empty;
        public const string SkinToneTextPropertyName = "SkinToneText";
        /// <summary>
        /// Gets or sets the "SkinToneText" property
        /// </summary>
        /// <value>The property value.</value>
        public string SkinToneText
        {
            get { return _skinToneText; }
            set { SetProperty(ref _skinToneText, value, SkinToneTextPropertyName); }
        }

        #endregion

        #region Property : SkinToneRemarks

        private string _skinToneRemarks = string.Empty;
        public const string SkinToneRemarksPropertyName = "SkinToneRemarks";
        /// <summary>
        /// Gets or sets the "SkinToneRemarks" property
        /// </summary>
        /// <value>The property value.</value>
        public string SkinToneRemarks
        {
            get { return _skinToneRemarks; }
            set { SetProperty(ref _skinToneRemarks, value, SkinToneRemarksPropertyName); }
        }

        #endregion

        #region Property : SkinToneColor

        private Color _skinToneColor = Color.Transparent;
        public const string SkinToneColorPropertyName = "SkinToneColor";
        /// <summary>
        /// Gets or sets the "SkinToneColor" property
        /// </summary>
        /// <value>The property value.</value>
        public Color SkinToneColor
        {
            get { return _skinToneColor; }
            set { SetProperty(ref _skinToneColor, value, SkinToneColorPropertyName); }
        }

        #endregion

        #region Property : IsMetric

        private bool _isMetric = false;
        public const string IsMetricPropertyName = "IsMetric";
        /// <summary>
        /// Gets or sets the "IsMetric" property
        /// </summary>
        /// <value>The property value.</value>
        public bool IsMetric
        {
            get { return _isMetric; }
            set { SetProperty(ref _isMetric, value, IsMetricPropertyName); }
        }

        #endregion

        public SettingsViewModel()
        {
            Title = "Settings";
            AllSpfValues = new List<string>();
            _service = ServiceLocator.Instance.Resolve<ICloudService>();
            _pictureProvider = ServiceLocator.Instance.Resolve<IPictureProviderService>();
            _skinToneProvider = ServiceLocator.Instance.Resolve<ISkinToneProviderService>();
            _band = ServiceLocator.Instance.Resolve<IBandService>();

            foreach (var value in Enum.GetNames(typeof(SpfValues)))
            {
                AllSpfValues.Add($"{(int)Enum.Parse(typeof(SpfValues),value)} ({Enum.Parse(typeof(SpfValues), value)})");
            }

            Debug.WriteLine("> Initiating the Settings Page");

            if (Settings.IsLoggedIn && !string.IsNullOrEmpty(Settings.UserId))
            {
                _user = Task.Run(async () => await _service.GetUser(Settings.UserId)).Result;
            }

            Debug.WriteLine($"> User received, {_user.Name}, {_user.SpfLevel}, {_user.SkinTone}");

            UserName = _user.Name;
            UserEmail = _user.Email;
            ProfileImage = _user.ImageUrl;
            SkinToneText = SunnyDay.Client.Core.Utils.DataConvert.SkinTone.SkinTone_ToDescription(_user.SkinTone);
            SkinToneRemarks = SunnyDay.Client.Core.Utils.DataConvert.SkinTone.SkiTone_ToRemarks(_user.SkinTone);
            SkinToneColor = SunnyDay.Client.Core.Utils.DataConvert.SkinTone.SkinTone_ToColor(_user.SkinTone);
            UserSpfValue = _user.SpfLevel;

            IsMetric = Settings.IsMetric;
        }


        #region Command : SaveChanges

        /// <summary>
        /// A command property representing the "SaveChanges" operation
        /// </summary>
        ICommand _saveChangesCommand;
        public ICommand SaveChangesCommand =>
            _saveChangesCommand ?? (_saveChangesCommand = new Command(async () => await ExecuteSaveChangesCommandAsync()));

        async Task ExecuteSaveChangesCommandAsync()
        {
            if (IsBusy)
                return;

            IsBusy = true;

            try
            {
                _user.SpfLevel = UserSpfValue;
                _user.Name = UserName;
                // skin tone is changed when retaking a picture
                _user.Email = UserEmail;
                Settings.IsMetric = IsMetric;
                await _service.UpdateUser(_user);
                InfoDisplay = false;
            }
            catch (Exception e)
            {
                Debug.WriteLine($"{e.Message}\n{e}\n{e.StackTrace}");
                InfoColor = Resources.RedInfoColor;
                InfoMessage = e.Message;
                InfoDisplay = true;
            }
            finally
            {
                IsBusy = false;
            }
        }

        #endregion

        #region Command : RetakePhoto

        /// <summary>
        /// A command property representing the "RetakePhoto" operation
        /// </summary>
        ICommand _retakePhotoCommand;
        public ICommand RetakePhotoCommand =>
            _retakePhotoCommand ?? (_retakePhotoCommand = new Command(async () => await ExecuteRetakePhotoCommandAsync()));

        async Task ExecuteRetakePhotoCommandAsync()
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
                        _user.SkinTone = skinTone;
                        SkinToneText = SunnyDay.Client.Core.Utils.DataConvert.SkinTone.SkinTone_ToDescription(_user.SkinTone);
                        SkinToneRemarks = SunnyDay.Client.Core.Utils.DataConvert.SkinTone.SkiTone_ToRemarks(_user.SkinTone);
                        SkinToneColor = SunnyDay.Client.Core.Utils.DataConvert.SkinTone.SkinTone_ToColor(_user.SkinTone);
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
            catch (Exception ex)
            {
                InfoColor = Resources.RedInfoColor;
                InfoMessage = ex.Message;
                InfoDisplay = true;
                Debug.WriteLine($"{ex}\n{ex.Message}\n{ex.StackTrace}");
            }
            finally
            {
                IsBusy = false;
            }
        }

        #endregion

        #region Command : LogOut

        /// <summary>
        /// A command property representing the "LogOut" operation
        /// </summary>
        ICommand _logOutCommand;
        public ICommand LogOutCommand =>
            _logOutCommand ?? (_logOutCommand = new Command(async () => await ExecuteLogOutCommandAsync()));

        async Task ExecuteLogOutCommandAsync()
        {
            if (IsBusy)
                return;

            IsBusy = true;

            try
            {
                // TODO: If any time left -> implement
            }
            finally
            {
                IsBusy = false;
            }
        }

        #endregion
    }
}
