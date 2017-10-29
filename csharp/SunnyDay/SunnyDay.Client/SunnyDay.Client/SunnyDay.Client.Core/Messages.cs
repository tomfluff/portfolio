namespace SunnyDay.Client.Core
{
    public class StartAlarmAlertTaskMessage { }

    public class StartUvMonitorTaskMessage { }

    public class AmbientLightReadingMessage
    {
        public int Light { get; set; }
    }

    public class UvReadingMessage
    {
        public int Index { get; set; }
        public int Exposure { get; set; }
    }

    public class StopAlarmAlertTaskMessage { }

    public class StopUvMonitorTaskMessage { }

    public class BeginExtendedExecutionMessage { }
}
