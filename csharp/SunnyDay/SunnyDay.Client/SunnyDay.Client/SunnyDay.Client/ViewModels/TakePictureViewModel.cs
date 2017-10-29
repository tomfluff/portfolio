using Plugin.Media.Abstractions;
using SunnyDay.Client.Core.Services;
using SunnyDay.Client.Core.Utils;
using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Microsoft.WindowsAzure.Storage.Blob;
using PCLStorage;
using Plugin.Media;
using SunnyDay.Client.Core.Helpers;
using SunnyDay.Client.Core.Models;
using SunnyDay.Client.Styles;
using SunnyDay.Client.Views;
using Xamarin.Forms;

namespace SunnyDay.Client.ViewModels
{
    public class TakePictureViewModel : BaseViewModel
    {
        private readonly ICloudService _service;
        private readonly IPictureProviderService _pictureProvider;
        private readonly ISkinToneProviderService _skinToneProvider;
        private readonly IBandService _band;
        public ObservableCollection<SkinToneCaptureInstruction> Instructions { get; set; }

        public TakePictureViewModel()
        {
            Title = "Take a picture";

            _service = ServiceLocator.Instance.Resolve<ICloudService>();
            _pictureProvider = ServiceLocator.Instance.Resolve<IPictureProviderService>();
            _skinToneProvider = ServiceLocator.Instance.Resolve<ISkinToneProviderService>();
            _band = ServiceLocator.Instance.Resolve<IBandService>();

            Instructions = new ObservableCollection<SkinToneCaptureInstruction>()
            {
                {new SkinToneCaptureInstruction() {ImageUrl = "Images/instruction_01.png",Text = "Take a picture of your arm's bottom area, so that it takes a sagnificant portion of the frame.",Index = 1} },
                {new SkinToneCaptureInstruction() {ImageUrl = "Images/instruction_02.png",Text = "Use a well lit environemnt, prefferably an ambient bright light.",Index = 2} },
                {new SkinToneCaptureInstruction() {ImageUrl = "Images/instruction_03.png",Text = "Make sure nothing is casting shadow on your skin.",Index = 3} }
            };
        }

        #region Property : SkinTone

        private int _skinTone = -1;
        public const string SkinTonePropertyName = "SkinTone";
        /// <summary>
        /// Gets or sets the "SkinTone" property
        /// </summary>
        /// <value>The property value.</value>
        public int SkinTone
        {
            get { return _skinTone; }
            set { SetProperty(ref _skinTone, value, SkinTonePropertyName); }
        }

        #endregion

        #region Property : ShowResults

        private bool _showResults = false;
        public const string ShowResultsPropertyName = "ShowResults";
        /// <summary>
        /// Gets or sets the "ShowResults" property
        /// </summary>
        /// <value>The property value.</value>
        public bool ShowResults
        {
            get { return _showResults; }
            set { SetProperty(ref _showResults, value, ShowResultsPropertyName); }
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

        #region Property : SpfLevel

        private int _spfLevel = 0;
        public const string SpfLevelPropertyName = "SpfLevel";
        /// <summary>
        /// Gets or sets the "SpfLevel" property
        /// </summary>
        /// <value>The property value.</value>
        public int SpfLevel
        {
            get { return _spfLevel; }
            set { SetProperty(ref _spfLevel, value, SpfLevelPropertyName); }
        }

        #endregion

        #region Command : TakePicture

        /// <summary>
        /// A command property representing the "TakePicture" operation
        /// </summary>
        ICommand _takePictureCommand;
        public ICommand TakePictureCommand =>
            _takePictureCommand ?? (_takePictureCommand = new Command(async () => await ExecuteTakePictureCommandAsync()));

        async Task ExecuteTakePictureCommandAsync()
        {
            if (IsBusy)
                return;

            Debug.WriteLine("> Attempting to take picture");

            IsBusy = true;

            try
            {
                int skinTone = -1;
                string filePath;
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
                    Debug.WriteLine(
                        $"> Current Light index = {_band.AverageAmbientLight} (atm={_band.CurrentAmbientLight})");

                    // TODO: maybe send light indication as well
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
                        SkinTone = skinTone;
                        SkinToneColor = SunnyDay.Client.Core.Utils.DataConvert.SkinTone.SkinTone_ToColor(skinTone);
                        SkinToneRemarks = SunnyDay.Client.Core.Utils.DataConvert.SkinTone.SkiTone_ToRemarks(skinTone);
                        SkinToneText = SunnyDay.Client.Core.Utils.DataConvert.SkinTone.SkinTone_ToDescription(skinTone);
                        SpfLevel = SunnyDay.Client.Core.Utils.DataConvert.SkinTone.SkinTone_ToSpfRecommendation(skinTone);
                        ShowResults = true;
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
                InfoMessage = "An unknown error occured. Try again later.";
                InfoDisplay = true;
                Debug.WriteLine($"{e.Source} : {e} ({e.InnerException})");
            }
            finally
            {
                IsBusy = false;
            }
        }

        #endregion

        #region Command : CancelResultDisplay

        /// <summary>
        /// A command property representing the "CancelResultDisplay" operation
        /// </summary>
        ICommand _cancelResultDisplayCommand;
        public ICommand CancelResultDisplayCommand =>
            _cancelResultDisplayCommand ?? (_cancelResultDisplayCommand = new Command(async () => await ExecuteCancelResultDisplayCommandAsync()));

        async Task ExecuteCancelResultDisplayCommandAsync()
        {
            ShowResults = false;
            SkinTone = -1;
            SpfLevel = 0;
        }

        #endregion

        #region Command : SaveAndContinue

        /// <summary>
        /// A command property representing the "SaveAndContinue" operation
        /// </summary>
        ICommand _saveAndContinueCommand;
        public ICommand SaveAndContinueCommand =>
            _saveAndContinueCommand ?? (_saveAndContinueCommand = new Command(async () => await ExecuteSaveAndContinueCommandAsync()));

        async Task ExecuteSaveAndContinueCommandAsync()
        {
            if (IsBusy)
                return;

            IsBusy = true;

            var user = await _service.GetUser(Settings.UserId);
            user.SkinTone = SkinTone;
            user.SpfLevel = SpfLevel;
            await _service.UpdateUser(user);
            Settings.IsSkinToneDetected = true;

            Application.Current.MainPage = new MasterPageView();

            IsBusy = false;
        }

        #endregion

    }
}
