namespace SunnyDay.Client.Core.Models
{
    /// <summary>
    /// This entity represents the data returned by the server when calculating the skin type based on an image
    /// </summary>
    public class SkinToneResponse
    {
        /// <summary>
        /// Currently holds a server error message on failure
        /// </summary>
        public string Comment { get; set; }

        /// <summary>
        /// An integer values which represents the skin type with a value betweenb 0 and 6 on success, or -1 otherwise
        /// </summary>
        public string SkinType { get; set; }

        /// <summary>
        /// The name of the image file as represented on server side
        /// </summary>
        public string FileName { get; set; }

        /// <summary>
        /// True if and only if the image was uploaded successfuly and the skin type calculation finished with no errors
        /// </summary>
        public bool Success { get; set; }
    }
}
