using System;
using System.Diagnostics;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Plugin.Geolocator;
using SunnyDay.Client.Core.Models.OpenWeatherMap;

namespace SunnyDay.Client.Core.Services
{
    public class OpenWeatherMapService : IWeatherService
    {
        private const string WeatherCoordinatesUri = "http://api.openweathermap.org/data/2.5/weather?lat={0}&lon={1}&units={2}&appid={3}";

        private readonly TimeSpan _readInterval = TimeSpan.FromMinutes(30);
        private bool _isInitialized = false;
        private WeatherRoot _currentWeather;
        private DateTime _fetchTime;

        public bool IsInitialized
        {
            get { return _isInitialized; }
        }

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
                return _currentWeather.DisplayTemp;
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


        public int UvLevel => -1;

        public double Altitude => 0;

        public OpenWeatherMapService() { }

        private async Task GetWeather(double latitude, double longitude)
        {
            Debug.WriteLine("> Trying to get weather");
            using (var client = new HttpClient())
            {
                var units = SunnyDay.Client.Core.Helpers.Settings.IsMetric ? "metric" : "imperial";
                Debug.WriteLine($"> Units are: {units}");
                var url = string.Format(WeatherCoordinatesUri, latitude, longitude, units, Static.Keys.OpenWeatherMapKey);
                var json = await client.GetStringAsync(url);
                Debug.WriteLine(json);

                if (string.IsNullOrWhiteSpace(json))
                    return;

                _currentWeather = JsonConvert.DeserializeObject<WeatherRoot>(json);
                _isInitialized = true;
                _fetchTime = DateTime.Now;
            }
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
                    try
                    {
                        var position = await locator.GetPositionAsync(10000);
                        Debug.WriteLine($"> Got position! {position.Latitude},{position.Longitude}");

                        await GetWeather(position.Latitude, position.Longitude);
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine(ex.StackTrace);
                        throw;
                    }
                }
                else
                {
                    Debug.WriteLine($"> Cannot read location, IsGeolocationAvailable={locator.IsGeolocationAvailable}, IsGeolocationEnabled={locator.IsGeolocationEnabled}");
                }
            }
        }
    }
}
