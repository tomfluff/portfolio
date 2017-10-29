using System;
using Microsoft.Azure.Mobile.Server;
using Microsoft.Azure.Mobile.Server.Tables;

namespace SunnyDay.Backend.DataObjects
{
    public class UvReading : EntityData
    {
        public string UserId { get; set; }
        public DateTime? ReadTime { get; set; }
        public string ReadLocation { get; set; }
        public int LocalIndex { get; set; }
        public int PublicIndex { get; set; }
        public string Description { get; set; }
        public int LightIntensity { get; set; }
        public int DailyExposure { get; set; }
    }
}