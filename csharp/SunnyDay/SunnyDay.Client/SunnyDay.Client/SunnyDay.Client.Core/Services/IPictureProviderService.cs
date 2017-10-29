using System.Threading.Tasks;
using Plugin.Media.Abstractions;

namespace SunnyDay.Client.Core.Services
{
    public interface IPictureProviderService
    {
        /// <summary>
        /// Takes a picture of the user, opens the camera, and compresses the image to the required size.
        /// If the user cancells the operation - returns null.
        /// </summary>
        /// <param name="light">The amount of light around While taking a picture</param>
        /// <returns>The MediaFile captured, or null on cancell</returns>
        Task<MediaFile> TakePicture(int light);
    }
}
