using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.Background;
using Windows.ApplicationModel.ExtendedExecution;
using Windows.Devices.Bluetooth.Rfcomm;
using Windows.Devices.Enumeration;
using Microsoft.Band;

namespace SunnyDay.Client.UWP.Background
{
    public sealed class Tasks
    {
        //
        // Register a background task with the specified taskEntryPoint, name, trigger,
        // and condition (optional).
        //
        // taskEntryPoint: Task entry point for the background task.
        // taskName: A name for the background task.
        // trigger: The trigger for the background task.
        // condition: Optional parameter. A conditional event that must be true for the task to fire.
        //
        private static BackgroundTaskRegistration RegisterBackgroundTask(string taskEntryPoint, string taskName, IBackgroundTrigger trigger,IBackgroundCondition condition = null)
        {
            // Check for existing registrations of this background task.
            foreach (var cur in BackgroundTaskRegistration.AllTasks)
            {
                if (cur.Value.Name == taskName)
                {
                    // The task is already registered.
                    Debug.WriteLine($"> Task already exists, {cur.Value.TaskId}");
                    return (BackgroundTaskRegistration) (cur.Value);
                }
            }
            if (trigger == null)
                return null;

            // Register the background task.
            var builder = new BackgroundTaskBuilder();

            builder.Name = taskName;
            builder.TaskEntryPoint = taskEntryPoint;
            builder.SetTrigger(trigger);
            builder.IsNetworkRequested = true;

            if (condition != null)
            {
                builder.AddCondition(condition);
            }

            BackgroundTaskRegistration task = builder.Register();
            Debug.WriteLine($"> Registered a new task, {task.TaskId}");
            return task;
        }

        public static async void StartAlarmAlertTask()
        {
            Debug.WriteLine($"> Attempting to register a background task, asking permission");
            BackgroundAccessStatus accessStatus = await BackgroundExecutionManager.RequestAccessAsync();
            if (accessStatus != BackgroundAccessStatus.DeniedBySystemPolicy && 
                accessStatus != BackgroundAccessStatus.DeniedBySystemPolicy)
            {
                Debug.WriteLine($"> Permission granted!");
                RegisterBackgroundTask(typeof(AlarmAlertTask).FullName, typeof(AlarmAlertTask).Name,
                    new TimeTrigger(15, false));
            }
        }

        public static void StopAlarmAlertTask()
        {
            Debug.WriteLine($"> Attempting to un-register a background task");
            var task = RegisterBackgroundTask(typeof(AlarmAlertTask).FullName, typeof(AlarmAlertTask).Name,
                    null);
            if (task != null)
            {
                task.Unregister(true);
                Debug.WriteLine($"> Un-registered task!");
            }
            else
            {
                Debug.WriteLine($"> Task not found!");
            }
        }

        public static async void StartUvMonitorTask()
        {
            Debug.WriteLine($"> Attempting to register a background task, asking permission");

            BackgroundAccessStatus accessStatus = await BackgroundExecutionManager.RequestAccessAsync();
            if (accessStatus != BackgroundAccessStatus.DeniedBySystemPolicy &&
                accessStatus != BackgroundAccessStatus.DeniedBySystemPolicy)
            {
                Debug.WriteLine($"> Permission granted!");
                var trigger = new DeviceUseTrigger();

                var task = RegisterBackgroundTask(typeof(UvMonitorTask).FullName, typeof(UvMonitorTask).Name,
                    trigger);

                // task.Completed += TaskOnCompleted;
                
                var device =
                    (await
                        DeviceInformation.FindAllAsync(
                            RfcommDeviceService.GetDeviceSelector(
                                RfcommServiceId.FromUuid(
                                    new Guid("A502CA9A-2BA5-413C-A4E0-13804E47B38F"))))).FirstOrDefault();

                
                if (device == null)
                {
                    Debug.WriteLine($"> Device wasn't found!");
                    return;
                }
                else
                {
                    Debug.WriteLine($"> Device: {device.Name}, {device.Id}");
                }

                DeviceTriggerResult deviceTriggerResult = await trigger.RequestAsync(device.Id);

                switch (deviceTriggerResult)
                {
                    case DeviceTriggerResult.Allowed:
                        Debug.WriteLine("> Background task started");
                        break;

                    case DeviceTriggerResult.LowBattery:
                        Debug.WriteLine("> Insufficient battery to run the background task");
                        break;

                    case DeviceTriggerResult.DeniedBySystem:
                        // The system can deny a task request if the system-wide DeviceUse task limit is reached.
                        Debug.WriteLine($"> The system has denied the background task request");
                        break;

                    default:
                        Debug.WriteLine("> Could not start the background task: " + deviceTriggerResult);
                        break;
                }
            }
        }

        private static void TaskOnCompleted(BackgroundTaskRegistration sender, BackgroundTaskCompletedEventArgs args)
        {
            Debug.WriteLine($"> Background task completed\n");
            try
            {
                args.CheckResult();
            }
            catch (Exception e)
            {
                Debug.WriteLine($"> {e.GetType()} : {e.Message} ({e.Source})");
            }
        }

        public static void StopUvMonitorTask()
        {
            BackgroundTaskRegistration task;
            while ((task = RegisterBackgroundTask(typeof(UvMonitorTask).FullName, typeof(UvMonitorTask).Name,null)) != null)
            {
                task.Unregister(true);
                Debug.WriteLine($"> Un-registered task!");
            }
            Debug.WriteLine($"> Unregistered all needed tasks!");
        }
    }
}
