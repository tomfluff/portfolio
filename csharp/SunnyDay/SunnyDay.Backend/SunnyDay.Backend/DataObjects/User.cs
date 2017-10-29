using Microsoft.Azure.Mobile.Server;

namespace SunnyDay.Backend.DataObjects
{
    public class User : EntityData
    {
        public string UserId { get; set; }
        public string GroupId { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public int SkinTone { get; set; }
        public int SpfLevel { get; set; }
        public bool IsVerified { get; set; }
        public string ImageUrl { get; set; }
    }
}