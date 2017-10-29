using System;

namespace SunnyDay.Client.Core.Utils
{
    public abstract class SunnyDayException : Exception
    {
    }

    public class CameraNotAvailableException : SunnyDayException
    {
        public override string Message => "No Camera available";
    }

    public class FailedTakingPictureException : SunnyDayException
    {
        public override string Message => "Failed to take a picture";
    }

    public class NotEnoughLightException : SunnyDayException
    {
        public override string Message => "There is not enough light to tkae a viable picture";
    }

    public class FailedPickingPictureException : SunnyDayException
    {
        public override string Message => "Failed to pick a picture";
    }

    public class CannotConnectToTheDestination : SunnyDayException
    {
        public override string Message => "Cannot reach the remote server";
    }

    public class FailtToConnectToBandException : SunnyDayException
    {
        public override string Message => "Cannot connect to an active band";
    }

    public class LocationServicesDisabled : SunnyDayException
    {
        public override string Message => "Location services are unavailable";
    }

    public class BadSkinTonePictureException : SunnyDayException
    {
        public int ErrorCode { get; }

        public BadSkinTonePictureException(int errorCode)
        {
            ErrorCode = errorCode;
        }

        public override string Message => "Could not detect SPF with the given picture";
    }
}
