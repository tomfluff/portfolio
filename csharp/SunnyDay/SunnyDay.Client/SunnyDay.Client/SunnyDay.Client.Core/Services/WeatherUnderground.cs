using System;
using System.Diagnostics;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Plugin.Connectivity;
using Plugin.Geolocator;
using SunnyDay.Client.Core.Models.WeatherUnderground;
using SunnyDay.Client.Core.Utils;
using MvvmHelpers;

namespace SunnyDay.Client.Core.Services
{
    public class WeatherUnderground : IWeatherService
    {
        private const string WeatherLookupUri = "https://api.wunderground.com/api/{2}/geolookup/conditions/q/{0},{1}.json";

        private readonly TimeSpan _readInterval = TimeSpan.FromMinutes(30);
        private bool _isInitialized = false;
        private WeatherRoot _currentWeather;
        private DateTime _fetchTime;

        public string Location
        {
            get
            {
                if (!_isInitialized) return string.Empty;
                return _currentWeather.DisplayLocation;
            }
        }

        public string Temperture
        {
            get
            {
                if (!_isInitialized) return string.Empty;
                if (SunnyDay.Client.Core.Helpers.Settings.IsMetric)
                {
                    return _currentWeather.DisplayTempC;
                }
                return _currentWeather.DisplayTempF;
            }
        }

        public string Date
        {
            get
            {
                if (!_isInitialized) return string.Empty;
                return _currentWeather.DisplayDate;
            }
        }

        public string Icon
        {
            get
            {
                if (!_isInitialized) return string.Empty;
                return _currentWeather.DisplayIcon;
            }
        }


        public int UvLevel
        {
            get
            {
                if (!_isInitialized) return -1;
                return _currentWeather.UvLevel;
            }
        }

        public double Altitude
        {
            get
            {
                if (!_isInitialized) return 0;
                return _currentWeather.Altitude;
            }
        }

        public bool IsInitialized
        {
            get { return _isInitialized; }
        }

        private async Task GetWeather(double latitude, double longitude)
        {
            Debug.WriteLine("> Trying to get weather");
            var client = new HttpClient();
            var units = SunnyDay.Client.Core.Helpers.Settings.IsMetric ? "metric" : "imperial";
            Debug.WriteLine($"> Units are: {units}");
            var url = string.Format(WeatherLookupUri, latitude, longitude, Static.Keys.WeatherUndergroundKey);
            Debug.WriteLine($"> Weather url: {url}");
            var json = await client.GetStringAsync(url).WithTimeout(10000);

            if (string.IsNullOrWhiteSpace(json))
            {
                client.CancelPendingRequests();
                client.Dispose();
                throw new CannotConnectToTheDestination();
            }

            _currentWeather = JsonConvert.DeserializeObject<WeatherRoot>(json);
            _isInitialized = true;
            _fetchTime = DateTime.Now;
            client.CancelPendingRequests();
            client.Dispose();
        }

        public async Task UpdateWeather()
        {
            if (!_isInitialized || (DateTime.Now.Subtract(_fetchTime) >= _readInterval))
            {
                Debug.WriteLine("> Weather should be fetched");
                var locator = CrossGeolocator.Current;
                Debug.WriteLine("> Attempting to get position...");
                if (locator.IsGeolocationAvailable && locator.IsGeolocationEnabled)
                {
                    var position = await locator.GetPositionAsync(10000);
                    Debug.WriteLine($"> Got position! {position.Latitude},{position.Longitude}");
                    
                    await GetWeather(position.Latitude, position.Longitude);
                }
                else
                {
                    Debug.WriteLine($"> Cannot read location, IsGeolocationAvailable={locator.IsGeolocationAvailable}, IsGeolocationEnabled={locator.IsGeolocationEnabled}");
                    throw new LocationServicesDisabled();
                }
            }
        }
    }
}
