using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading;
using System.Threading.Tasks;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.ApplicationModel.ExtendedExecution;
using Windows.Data.Xml.Dom;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Foundation.Metadata;
using Windows.UI;
using Windows.UI.Notifications;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Windows.UI.ViewManagement;
using SunnyDay.Client.Core.Services;
using SunnyDay.Client.Core.Utils;
using Xamarin.Forms;
using Application = Windows.UI.Xaml.Application;
using Color = Windows.UI.Color;
using Frame = Windows.UI.Xaml.Controls.Frame;

namespace SunnyDay.Client.UWP
{
    /// <summary>
    /// Provides application-specific behavior to supplement the default Application class.
    /// </summary>
    sealed partial class App : Application
    {


        /// <summary>
        /// Initializes the singleton application object.  This is the first line of authored code
        /// executed, and as such is the logical equivalent of main() or WinMain().
        /// </summary>
        public App()
        {
            this.InitializeComponent();
            this.Suspending += OnSuspending;
        }

        /// <summary>
        /// Invoked when the application is launched normally by the end user.  Other entry points
        /// will be used such as when the application is launched to open a specific file.
        /// </summary>
        /// <param name="e">Details about the launch request and process.</param>
        protected override void OnLaunched(LaunchActivatedEventArgs e)
        {

#if DEBUG
            if (System.Diagnostics.Debugger.IsAttached)
            {
                this.DebugSettings.EnableFrameRateCounter = true;
            }
#endif
            
            Frame rootFrame = Window.Current.Content as Frame;

            // Do not repeat app initialization when the Window already has content,
            // just ensure that the window is active
            if (rootFrame == null)
            {
                // Create a Frame to act as the navigation context and navigate to the first page
                rootFrame = new Frame();

                rootFrame.NavigationFailed += OnNavigationFailed;

                // Mobile status bar customization
                if (ApiInformation.IsTypePresent("Windows.UI.ViewManagement.StatusBar"))
                {
                    var statusBar = StatusBar.GetForCurrentView();
                    if (statusBar != null)
                    {
                        statusBar.BackgroundOpacity = 1;
                        statusBar.BackgroundColor = Color.FromArgb(255, 0, 178, 238);
                        statusBar.ForegroundColor = Colors.White;
                    }
                }

                Xamarin.Forms.Forms.Init(e);

                MessagingCenter.Subscribe<SunnyDay.Client.Core.StartAlarmAlertTaskMessage>(this, "StartAlarmAlertTaskMessage", message =>
                {
                    Debug.WriteLine($"> Received message StartAlarmAlertTaskMessage");
                    SunnyDay.Client.UWP.Background.Tasks.StartAlarmAlertTask();
                });

                MessagingCenter.Subscribe<SunnyDay.Client.Core.BeginExtendedExecutionMessage>(this, "BeginExtendedExecutionMessage", message =>
                {
                    Debug.WriteLine($"> Received message BeginExtendedExecutionMessage");
                    ExtendedBandExecutionRequest();
                });

                /*MessagingCenter.Subscribe<SunnyDay.Client.Core.StartUvMonitorTaskMessage>(this, "StartUvMonitorTaskMessage", message =>
                {
                    Debug.WriteLine($"> Received message StartUvMonitorTaskMessage");
                    SunnyDay.Client.UWP.Background.Tasks.StartUvMonitorTask();
                });

                MessagingCenter.Subscribe<SunnyDay.Client.Core.StopAlarmAlertTaskMessage>(this, "StopAlarmAlertTaskMessage", message =>
                {
                    Debug.WriteLine($"> Received message StopAlarmAlertTaskMessage");
                    SunnyDay.Client.UWP.Background.Tasks.StopAlarmAlertTask();
                });

                MessagingCenter.Subscribe<SunnyDay.Client.Core.StopUvMonitorTaskMessage>(this, "StopUvMonitorTaskMessage", message =>
                {
                    Debug.WriteLine($"> Received message StopUvMonitorTaskMessage");
                    SunnyDay.Client.UWP.Background.Tasks.StopUvMonitorTask();
                });*/

                // ---------------------------------- ^ FORMS INIT


                if (e.PreviousExecutionState == ApplicationExecutionState.Terminated)
                {
                    //TODO: Load state from previously suspended application
                }

                Debug.WriteLine($"> (WIN) Checking last state - {e.PreviousExecutionState}");

                // Place the frame in the current Window
                Window.Current.Content = rootFrame;
            }

            if (rootFrame.Content == null)
            {
                // When the navigation stack isn't restored navigate to the first page,
                // configuring the new page by passing required information as a navigation
                // parameter
                rootFrame.Navigate(typeof(MainPage), e.Arguments);
            }
            // Ensure the current window is active
            Window.Current.Activate();
        }

        /// <summary>
        /// Invoked when Navigation to a certain page fails
        /// </summary>
        /// <param name="sender">The Frame which failed navigation</param>
        /// <param name="e">Details about the navigation failure</param>
        void OnNavigationFailed(object sender, NavigationFailedEventArgs e)
        {
            throw new Exception("Failed to load Page " + e.SourcePageType.FullName);
        }

        /// <summary>
        /// Invoked when application execution is being suspended.  Application state is saved
        /// without knowing whether the application will be terminated or resumed with the contents
        /// of memory still intact.
        /// </summary>
        /// <param name="sender">The source of the suspend request.</param>
        /// <param name="e">Details about the suspend request.</param>
        private async void OnSuspending(object sender, SuspendingEventArgs e)
        {
            var deferral = e.SuspendingOperation.GetDeferral();

            // TODO: Something when the app goes to sleep (suspending)

            deferral.Complete();
        }

        private ExtendedExecutionSession session = null;
        private Timer periodicTimer = null;

        void ClearExtendedExecution()
        {
            if (session != null)
            {
                session.Revoked -= SessionRevoked;
                session.Dispose();
                session = null;
            }

            if (periodicTimer != null)
            {
                periodicTimer.Dispose();
                periodicTimer = null;
            }
        }

        private async Task ExtendedBandExecutionRequest()
        {
            try
            {
                ClearExtendedExecution();

                var newSession = new ExtendedExecutionSession
                {
                    Reason = ExtendedExecutionReason.LocationTracking,
                    Description = "Band Sensor Monitoring"
                };
                newSession.Revoked += SessionRevoked;
                ExtendedExecutionResult result = await newSession.RequestExtensionAsync();

                switch (result)
                {
                    case ExtendedExecutionResult.Allowed:
                        Debug.WriteLine($"> Band tracking allowed");
                        //DisplayToast("Allowed");
                        session = newSession;
                        /*periodicTimer = new Timer(TimerCallback, DateTime.Now, TimeSpan.FromSeconds(5),
                            TimeSpan.FromSeconds(30));*/
                        await ServiceLocator.Instance.Resolve<IBandService>().Start();
                        break;

                    case ExtendedExecutionResult.Denied:
                        Debug.WriteLine($"> Band tracking denied");
                        newSession.Dispose();
                        break;
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine($"{e.Message} ({e.Source})");
                //DisplayToast($"{e.Message} ({e.Source})");
            }
        }

        private void TimerCallback(object state)
        {
            Debug.WriteLine("Tick~");
            try
            {
                var band = ServiceLocator.Instance.Resolve<IBandService>();
                DisplayToast($"Band Read: {band.CurrentUV}, ({band.CurrentDailyExposure}) - [{band.AverageAmbientLight}]");
            }
            catch (Exception e)
            {
                DisplayToast($"{e.Message} ({e.Source})");
            }
        }

        private async void SessionRevoked(object sender, ExtendedExecutionRevokedEventArgs args)
        {
            Debug.WriteLine($"> Session revoked!");

            ClearExtendedExecution();

            switch (args.Reason)
            {
                case ExtendedExecutionRevokedReason.Resumed:
                    //DisplayToast("Extended Execution Revokes! (Resumed)");
                    await ServiceLocator.Instance.Resolve<IBandService>().Stop();
                    await ExtendedBandExecutionRequest();
                    Debug.WriteLine("> Extended execution revoked due to returning to foreground.");
                    break;

                case ExtendedExecutionRevokedReason.SystemPolicy:
                    //DisplayToast("Extended Execution Revokes! (SystemPolicy)");
                    Debug.WriteLine("> Extended execution revoked due to system policy.");
                    break;
            }
        }

        public static ToastNotification DisplayToast(string content)
        {
            string xml = $@"<toast activationType='foreground'>
                                            <visual>
                                                <binding template='ToastGeneric'>
                                                    <text>Extended Execution</text>
                                                </binding>
                                            </visual>
                                        </toast>";

            XmlDocument doc = new XmlDocument();
            doc.LoadXml(xml);

            var binding = doc.SelectSingleNode("//binding");

            var el = doc.CreateElement("text");
            el.InnerText = content;
            binding.AppendChild(el); //Add content to notification

            var toast = new ToastNotification(doc);

            ToastNotificationManager.CreateToastNotifier().Show(toast); //Show the toast

            return toast;
        }
    }
}
