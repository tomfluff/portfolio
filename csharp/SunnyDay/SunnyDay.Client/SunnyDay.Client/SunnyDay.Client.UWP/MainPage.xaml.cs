using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading;
using Windows.ApplicationModel.ExtendedExecution;
using Windows.Data.Xml.Dom;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Notifications;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using SunnyDay.Client.Core.Services;
using SunnyDay.Client.Core.Utils;
using Xamarin.Forms;

namespace SunnyDay.Client.UWP
{
    public sealed partial class MainPage
    {
       
        public MainPage()
        {
            this.InitializeComponent();

            LoadApplication(new SunnyDay.Client.App());
        }
        
    }
}
