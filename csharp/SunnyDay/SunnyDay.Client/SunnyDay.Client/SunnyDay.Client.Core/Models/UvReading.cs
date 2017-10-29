
using System;

namespace SunnyDay.Client.Core.Models
{
    public class UvReading : EntityData
    {
        public string UserId { get; set; }
        public DateTime ReadTime { get; set; }
        public string ReadLocation { get; set; }
        public int LocalIndex { get; set; }
        public int PublicIndex { get; set; }
        public string Description { get; set; }
        public int LightIntensity { get; set; }
        public int DailyExposure { get; set; }
    }
}
