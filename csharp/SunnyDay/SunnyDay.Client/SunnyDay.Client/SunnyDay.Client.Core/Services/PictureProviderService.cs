using System;
using System.Threading.Tasks;
using Plugin.Media;
using Plugin.Media.Abstractions;
using SunnyDay.Client.Core.Utils;

namespace SunnyDay.Client.Core.Services
{
    public class PictureProviderService : IPictureProviderService
    {
        private const string SpfDirectory = "temp_skintome";
        private const string SpfNameTemplate = "img_{0}_{1}.jpg";
        
        public async Task<MediaFile> TakePicture(int light)
        {
            if (!CrossMedia.Current.IsCameraAvailable || !CrossMedia.Current.IsTakePhotoSupported)
            {
                throw new CameraNotAvailableException();
            }

            if (light < 40)
            {
                throw new NotEnoughLightException();
            }

            var file = await CrossMedia.Current.TakePhotoAsync(new Plugin.Media.Abstractions.StoreCameraMediaOptions
            {
                PhotoSize = PhotoSize.Large,
                Directory = SpfDirectory,
                Name = string.Format(SpfNameTemplate,DateTime.Now.ToBinary().ToString("X"),light)
            });

            if (file == null)
            {
                throw new FailedTakingPictureException();
            }

            return file;
        }
    }
}
