using System;
using System.Diagnostics;
using System.Linq;
using Windows.ApplicationModel.Background;
using Windows.Data.Xml.Dom;
using Windows.Security.Cryptography.Core;
using Windows.UI.Notifications;
using Windows.UI.Xaml;
using Microsoft.Band;
using Microsoft.Band.Sensors;
using SunnyDay.Client.Core;
using SunnyDay.Client.Core.Helpers;
using SunnyDay.Client.Core.Services;

namespace SunnyDay.Client.UWP.Background
{
    public sealed class UvMonitorTask : IBackgroundTask, IDisposable
    {
        private BackgroundTaskDeferral _deferral;
        private IBandClient _band;
        private MicrosoftBandService _bandService;
        //private ICloudService _service;
        private int[] _ambientLightReads;
        private int i;

        public async void Run(IBackgroundTaskInstance taskInstance)
        {
            Debug.WriteLine($"> Configuring Band connection in background");
            _deferral = taskInstance.GetDeferral();
            taskInstance.Canceled += TaskInstanceOnCanceled;

            _ambientLightReads = new int[10];

            try
            {
                //_service = new AzureCloudService();
                //await _service.InitializeAsync();
                //Debug.WriteLine($"> Cloude service initialized");

                while (!Settings.MonitorInBackground) { }

                Debug.WriteLine($"> Background trying to connect band");

                _bandService = new MicrosoftBandService();
                await _bandService.Initialize(true);

                await _bandService.StartReadingAmbientLight();
                await _bandService.StartReadingUV();
                
                /*var userBands = await BandClientManager.Instance.GetBandsAsync(isBackground: true);
                if (userBands.Length < 1)
                {
                    throw new InvalidOperationException("No Microsoft Band available to connect to.");
                }
                Debug.WriteLine("> Got bands list!");
                _band = await BandClientManager.Instance.ConnectAsync(userBands[0]);

                Debug.WriteLine("> Connected to user band");

                #region Check Band Sensors Are Supported

                if (!_band.SensorManager.UV.IsSupported)
                {
                    throw new InvalidOperationException("No UV available to connect to.");
                }
                if (!_band.SensorManager.AmbientLight.IsSupported)
                {
                    throw new InvalidOperationException("No AmbientLight available to connect to.");
                }

                #endregion
                
                // Add read change listener for the band sensors
                _band.SensorManager.UV.ReadingChanged += UV_ReadingChanged;
                _band.SensorManager.AmbientLight.ReadingChanged += AmbLight_ReadingChanged;

                await _band.SensorManager.UV.StartReadingsAsync();
                await _band.SensorManager.AmbientLight.StartReadingsAsync();

                // Store a setting so that the app knows that the task is running.
                Debug.WriteLine("> Background Task reading...");*/
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"> Exception from Sensor Task:\n{ex}");
            }
        }

        private async void TaskInstanceOnCanceled(IBackgroundTaskInstance sender, BackgroundTaskCancellationReason reason)
        {
            // Stop reading and remove the listener
            /*await _band.SensorManager.UV.StopReadingsAsync();
            await _band.SensorManager.AmbientLight.StopReadingsAsync();

            _band.SensorManager.UV.ReadingChanged -= UV_ReadingChanged;
            _band.SensorManager.AmbientLight.ReadingChanged -= AmbLight_ReadingChanged;

            _band.Dispose();*/
            Debug.WriteLine($"> Background task canceld!");
            
            _bandService.Dispose();

            _deferral.Complete();
        }

        private void AmbLight_ReadingChanged(object sender, BandSensorReadingEventArgs<IBandAmbientLightReading> e)
        {
            var ambLight = e.SensorReading;
            _ambientLightReads[i] = ambLight.Brightness;
            i = (i + 1) % _ambientLightReads.Length;
            /*Device.BeginInvokeOnMainThread( () =>
            MessagingCenter.Send(new AmbientLightReadingMessage() { Light = ambLight.Brightness}, "AmbientLightReadingMessage"));
            */
        }

        private void UV_ReadingChanged(object sender, BandSensorReadingEventArgs<IBandUVReading> e)
        {
            var uvRead = e.SensorReading;
            Debug.WriteLine($"> UV Background read! {(int)uvRead.IndexLevel} ({uvRead.IndexLevel}), light {_ambientLightReads.Average():F1}");
            /*Device.BeginInvokeOnMainThread(() =>
            MessagingCenter.Send(new UvReadingMessage() {Index = (int)uvRead.IndexLevel, Exposure = (int)uvRead.ExposureToday}, "UvReadingMessage"));
            */
            // Add UV read to list of reads
            /*Position position = null;
            var locator = CrossGeolocator.Current;
            Debug.WriteLine("> Attempting to get position...");
            if (locator.IsGeolocationAvailable && locator.IsGeolocationEnabled)
            {
                position = await locator.GetPositionAsync(5000);
                Debug.WriteLine($"> Got position! {position.Latitude},{position.Longitude}");
            }
            var loc = position != null ? $"{position.Latitude},{position.Longitude}" : string.Empty;
            var r = await _service.AddUserUvReading(Settings.UserId, DateTime.UtcNow, loc, CurrentUV, (int)AverageAmbientLight);*/
            /*_service.AddUserUvReading(Settings.UserId, DateTime.UtcNow, string.Empty,
                    (int) uvRead.IndexLevel + 1, (int) _ambientLightReads.Average(), (int)uvRead.ExposureToday);*/
        }

        public void Dispose()
        {
            // TODO: Maybe implement something
        }
    }
}