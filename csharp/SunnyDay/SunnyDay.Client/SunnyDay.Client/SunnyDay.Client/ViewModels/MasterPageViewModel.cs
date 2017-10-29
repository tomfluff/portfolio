using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using SunnyDay.Client.Core.Models;
using SunnyDay.Client.Core.Services;
using SunnyDay.Client.Core.Utils;
using SunnyDay.Client.Views;
using Xamarin.Forms;

namespace SunnyDay.Client.ViewModels
{
    public class MasterPageViewModel : BaseViewModel
    {
        public MasterPageViewModel()
        {
            Title = "Navigation";

            PagesList = new List<MasterPageNavigationItem>()
            {
                new MasterPageNavigationItem() {Icon = "Images/protect_32x32.png", Title = "Real-Time Protection", TargetType = typeof(MainPageView)},
                new MasterPageNavigationItem() {Icon = "Images/sett_32x32.png", Title = "Settings", TargetType = typeof(SettingsView)},
                new MasterPageNavigationItem() {Icon = "Images/report_32x32.png", Title = "Detailed Report", TargetType = typeof(ReportPageView)}
            };
        }

        #region Property : PagesList

        private List<MasterPageNavigationItem> _pagesList = null;
        public const string PagesListPropertyName = "PagesList";
        /// <summary>
        /// Gets or sets the "PagesList" property
        /// </summary>
        /// <value>The property value.</value>
        public List<MasterPageNavigationItem> PagesList
        {
            get { return _pagesList; }
            set { SetProperty(ref _pagesList, value, PagesListPropertyName); }
        }

        #endregion

    }
}
