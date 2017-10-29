using System.Threading.Tasks;

namespace SunnyDay.Client.Core.Services
{
    public interface IBandService
    {
        /// <summary>
        /// The last Ambient Light reading value
        /// </summary>
        int CurrentAmbientLight { get; }

        /// <summary>
        /// The average ambient light from the last 10 reads
        /// </summary>
        double AverageAmbientLight { get; }

        /// <summary>
        /// The last UV reading value
        /// </summary>
        int CurrentUV { get; }

        /// <summary>
        /// The current daily exposure (minutes)
        /// </summary>
        int CurrentDailyExposure { get; }

        /// <summary>
        /// True iff the current connection is initialized
        /// </summary>
        bool IsInitialized { get; }
        
        Task Start();

        Task Stop();
    }
}
