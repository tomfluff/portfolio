using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.Background;
using Windows.Data.Xml.Dom;
using Windows.UI.Notifications;
using SunnyDay.Client.Core.Helpers;
using SunnyDay.Client.Core.Services;
using SunnyDay.Client.Core.Utils;

namespace SunnyDay.Client.UWP.Background
{
    public sealed class AlarmAlertTask : IBackgroundTask
    {
        private BackgroundTaskDeferral _deferral;
        private ICloudService _service;

        public async void Run(IBackgroundTaskInstance taskInstance)
        {
            _deferral = taskInstance.GetDeferral();

            if (ServiceLocator.Instance.Resolve<ICloudService>() == null)
                _service = new AzureCloudService();
            else
                _service = ServiceLocator.Instance.Resolve<ICloudService>();

            var alarms = await _service.GetActiveGroupAlarms(Settings.UserId);
            var now = DateTime.UtcNow;

            var notifyNames = "";

            foreach (var a in alarms)
            {
                var span = a.EndTime.ToUniversalTime().Subtract(now);
                if (a.EndTime >= DateTime.Now && span.TotalMinutes <= 15)
                {
                    Debug.WriteLine($"> Alarm near expiration detected, {a.Text} : {span} left");
                    if (!string.IsNullOrEmpty(notifyNames)) notifyNames += ", ";
                    notifyNames += a.Text;
                    await _service.DisableAlarm(a.Id);
                }
            }

            string xml;

            if (!string.IsNullOrEmpty(notifyNames))
            {
                xml = $@"<toast scenario=""alarm"">
                            <visual>
                                <binding template=""ToastGeneric"">
                                    <text>SunnyDay safety reminder!</text>
                                    <text>{notifyNames} will get burn in less than 15 minutes! Be careful :)</text>
                                    <text>(log back into the app to continue alerting)</text>
                                </binding>
                            </visual>
                            <audio src=""ms-winsoundevent:Notification.Looping.Alarm""/>
                            <action activationType=""system"" arguments=""dismiss"" content=""""/>
                          </toast>";

                XmlDocument doc = new XmlDocument();
                doc.LoadXml(xml);
                ScheduledToastNotification toast = new ScheduledToastNotification(doc, DateTimeOffset.Now.AddSeconds(5));
                ToastNotificationManager.CreateToastNotifier().AddToSchedule(toast);
            }

            _deferral.Complete();
        }
    }
}
