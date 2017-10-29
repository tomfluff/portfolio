
using System;

namespace SunnyDay.Client.Core.Models
{
    public class Alarm : EntityData
    {
        public string GroupId { get; set; }
        public string UserId { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public int DurationSeconds { get; set; }
        public bool IsActive { get; set; }
        public string Text { get; set; }
    }
}
