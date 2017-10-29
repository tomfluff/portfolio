using System.Threading.Tasks;

namespace SunnyDay.Client.Core.Services
{
    public interface IWeatherService
    {
        string Location { get; }
        string Temperture { get; }
        string Date { get; }
        string Icon { get; }
        int UvLevel { get; }
        double Altitude { get; }
        bool IsInitialized { get; }

        Task UpdateWeather();
    }
}
