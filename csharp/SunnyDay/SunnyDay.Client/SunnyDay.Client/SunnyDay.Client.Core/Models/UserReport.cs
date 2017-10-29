using System.Collections.Generic;

namespace SunnyDay.Client.Core.Models.UserReport
{

    public class UvReading
    {
        public int ReadTime { get; set; }
        public string ReadLocation { get; set; }
        public int UVRead { get; set; }
    }

    public class UvPerDay
    {
        public int ReadTime { get; set; }
        public int UVRead { get; set; }
    }

    public class ChildUser
    {
        public string userName { get; set; }
        public int SkinTone { get; set; }
        public int SpfLevel { get; set; }
        public List<int> Alarms { get; set; }
        public string ImageUrl { get; set; }
    }

    public class RootObject
    {
        public string userName { get; set; }
        public int SkinTone { get; set; }
        public int SpfLevel { get; set; }
        public string ImageUrl { get; set; }
        public string email { get; set; }
        public List<UvReading> UvReadings { get; set; }
        public List<int> Alarms { get; set; }
        public List<UvPerDay> UvPerDay { get; set; }
        public List<ChildUser> childUsers { get; set; }
    }

}