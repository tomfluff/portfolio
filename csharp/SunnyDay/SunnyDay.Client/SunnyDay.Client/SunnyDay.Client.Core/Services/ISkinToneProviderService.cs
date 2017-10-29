using System.Threading.Tasks;

namespace SunnyDay.Client.Core.Services
{
    public interface ISkinToneProviderService
    {
        /// <summary>
        /// The function is given a file path representing a skin image and returns the skin type as calculated by the server
        /// </summary>
        /// <param name="fileUri">The file Uri from storage service</param>
        /// <param name="fileName">The file name of the image</param>
        /// <param name="lightIntensity">The light intensity aroud when the picture was taken</param>
        /// <returns>An integer with a value between 0 and 6 (skin type) on success, or -1 otherwise</returns>
        Task<int> GetSkinToneFromImageUri(string fileUri, string fileName, int lightIntensity = 0);
    }
}

