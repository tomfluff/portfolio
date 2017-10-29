using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Acr.UserDialogs;
using Microsoft.Band.Portable.Sensors;
using Microsoft.Data.OData.Query.SemanticAst;
using Plugin.Geolocator;
using Plugin.Geolocator.Abstractions;
using SunnyDay.Client.Core.Helpers;
using SunnyDay.Client.Core.Models;
using SunnyDay.Client.Core.Utils;
using Xamarin.Forms;
using BandClientManager = Microsoft.Band.Portable.BandClientManager;

namespace SunnyDay.Client.Core.Services
{
    public class MicrosoftBandService : IBandService, IDisposable
    {
        private ICloudService _service;
        private bool _isBackground = false;
        private bool _isReadinfUv = false;
        private bool _isReadingAmbientLight = false;
        private Microsoft.Band.Portable.BandClient _bandClient;
        private Microsoft.Band.Portable.BandDeviceInfo _band;
        private Microsoft.Band.Portable.BandClientManager _bandClientManager;
        private int[] _lastAmbientLightReads = new int[10];
        private int _lastAmbientLightIndex = 0;

        public int CurrentUV { get; private set; } = 0;
        public int CurrentAmbientLight { get; private set; } = 0;
        public int CurrentDailyExposure { get; private set; } = 0;
        public bool IsInitialized { get; private set; } = false;

        public double AverageAmbientLight
        {
            get
            {
                double res = 0;
                foreach (var read in _lastAmbientLightReads)
                {
                    res += read;
                }
                res /= _lastAmbientLightReads.Length;
                return res;
            }
        }

        public MicrosoftBandService()
        {
        }

        public async Task Initialize(bool isBackground)
        {
            _isBackground = isBackground;
            UvReading lastR;

            if (!isBackground)
            {
                _service = ServiceLocator.Instance.Resolve<ICloudService>();
                lastR = (await _service.GetUserLastUvReadingsByCount(Settings.UserId, 1)).FirstOrDefault();
                if (lastR != null && lastR.ReadTime.Date == DateTime.Today.Date)
                {
                    CurrentDailyExposure = lastR.DailyExposure;
                }
                var read = from r in (await _service.GetUserLastUvReadingsByCount(Settings.UserId, 10))
                    where r.ReadTime >= DateTime.Now.Subtract(TimeSpan.FromMinutes(30)) && r.PublicIndex > 0
                    orderby r.ReadTime descending select r;
                if (read.Any())
                {
                    var r = read.FirstOrDefault();
                    Debug.WriteLine($"> Recent read found! {r.ReadTime}, ({DateTime.Now})");
                    // recover from last read
                    this.CurrentAmbientLight = r.LightIntensity;
                    this.CurrentUV = r.LocalIndex;
                    this.CurrentDailyExposure = r.DailyExposure;
                    for (int i = 0; i < _lastAmbientLightReads.Length; i++)
                    {
                        _lastAmbientLightReads[i] = CurrentAmbientLight;
                    }
                }
            }

            if (IsInitialized) return;

            Debug.WriteLine($"> Starting Band connection ({isBackground})...");
            try
            {
                _bandClientManager = BandClientManager.Instance;

                var bands = await _bandClientManager.GetPairedBandsAsync(isBackgound: isBackground);

                if (!bands.Any())
                    throw new FailtToConnectToBandException();

                _band = bands.FirstOrDefault();
                _bandClient = await _bandClientManager.ConnectAsync(_band);
                Debug.WriteLine($"> Connected to band {_band.Name} ({isBackground})");
                Debug.WriteLine($"> Started Band connection! ({isBackground})");
                IsInitialized = true;
            }
            catch (Exception e)
            {
                Debug.WriteLine($"({isBackground}) {e.GetType()} in {e.Source} : {e.Message} ({e.InnerException?.Message})");
                throw;
            }
        }

        #region Ambient Light Reading

        public async Task StartReadingAmbientLight()
        {
            if (!IsInitialized || _isReadingAmbientLight) return;

            await _bandClient.SensorManager.AmbientLight.StartReadingsAsync();
            _bandClient.SensorManager.AmbientLight.ReadingChanged += AmbientLight_ReadingChanged;
            _isReadingAmbientLight = true;
        }
        public async Task StopReadingAmbientLight()
        {
            if (!IsInitialized || !_isReadingAmbientLight) return;

            _isReadingAmbientLight = false;
            await _bandClient.SensorManager.AmbientLight.StopReadingsAsync();
            _bandClient.SensorManager.AmbientLight.ReadingChanged -= AmbientLight_ReadingChanged;
        }
        private async void AmbientLight_ReadingChanged(object sender, BandSensorReadingEventArgs<BandAmbientLightReading> e)
        {
            this.CurrentAmbientLight = e.SensorReading.Brightness;
            _lastAmbientLightReads[_lastAmbientLightIndex] = e.SensorReading.Brightness;
            _lastAmbientLightIndex++;
            _lastAmbientLightIndex = _lastAmbientLightIndex % _lastAmbientLightReads.Length;
            if (_lastAmbientLightIndex == 0)
            {
                Debug.WriteLine($"> ({_isBackground}) LIGHT read : {CurrentAmbientLight}, avg={AverageAmbientLight}");
            }
        }

        #endregion

        #region UV Reading

        public async Task StartReadingUV()
        {
            if (!IsInitialized || _isReadinfUv) return;

            await _bandClient.SensorManager.UltravioletLight.StartReadingsAsync();
            _bandClient.SensorManager.UltravioletLight.ReadingChanged += UV_ReadingChanged;
            _isReadinfUv = true;
        }
        public async Task StopReadingUV()
        {
            if (!IsInitialized || !_isReadinfUv) return;

            _isReadinfUv = false;
            await _bandClient.SensorManager.UltravioletLight.StopReadingsAsync();
            _bandClient.SensorManager.UltravioletLight.ReadingChanged -= UV_ReadingChanged;
        }
        private async void UV_ReadingChanged(object sender, BandSensorReadingEventArgs<BandUltravioletLightReading> e)
        {
            this.CurrentUV = (int)e.SensorReading.Level;
            this.CurrentDailyExposure = (int) e.SensorReading.ExposureToday;

            var loc = string.Empty;

            if (CrossGeolocator.Current != null && CrossGeolocator.Current.IsGeolocationAvailable && CrossGeolocator.Current.IsGeolocationEnabled)
            {
                var pos = await CrossGeolocator.Current.GetPositionAsync();
                loc = $"{pos.Latitude},{pos.Longitude}";
            }
            Debug.WriteLine($"> Location: {loc}");

            await _service.AddUserUvReading(Settings.UserId, DateTime.UtcNow, loc, CurrentUV, (int) AverageAmbientLight,
                CurrentDailyExposure);

            Debug.WriteLine($"> ({_isBackground}) UV read : {CurrentUV} ({e.SensorReading.Level}), Exposure : {e.SensorReading.ExposureToday}");
        }

        #endregion

        public async Task Start()
        {
            Debug.WriteLine($"> Starting band connection");
            await this.Initialize(false);
            await this.StartReadingUV();
            await this.StartReadingAmbientLight();
            
            Debug.WriteLine($"> Band is connected!");
        }

        public async Task Stop()
        {
            Debug.WriteLine($"> Stopping band connection");
            Dispose();
            this._isReadinfUv = false;
            this._isReadingAmbientLight = false;
            this.IsInitialized = false;

            Debug.WriteLine($"> Band is stopped!");
        }

        public async void Dispose()
        {
            await _bandClient.DisconnectAsync();
        }
    }
}
