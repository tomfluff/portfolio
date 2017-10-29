using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using SunnyDay.Client.Core.Helpers;
using SunnyDay.Client.Core.Services;
using SunnyDay.Client.Core.Utils;
using Xamarin.Forms;

namespace SunnyDay.Client.ViewModels
{
    public class ReportPageViewModel : BaseViewModel
    {
        private ICloudService _service;

        #region Property : UserEmail

        private string _userEmail = string.Empty;
        public const string UserEmailPropertyName = "UserEmail";
        /// <summary>
        /// Gets or sets the "UserEmail" property
        /// </summary>
        /// <value>The property value.</value>
        public string UserEmail
        {
            get { return _userEmail; }
            set { SetProperty(ref _userEmail, value, UserEmailPropertyName); }
        }

        #endregion

        #region Property : ReportSize

        private int _reportSize = 7;
        public const string ReportSizePropertyName = "ReportSize";
        /// <summary>
        /// Gets or sets the "ReportSize" property
        /// </summary>
        /// <value>The property value.</value>
        public int ReportSize
        {
            get { return _reportSize; }
            set { SetProperty(ref _reportSize, value, ReportSizePropertyName); }
        }

        #endregion

        public List<int> ReportSizeOptions { get; }

        public ReportPageViewModel()
        {
            _service = ServiceLocator.Instance.Resolve<ICloudService>();

            ReportSizeOptions = new List<int>() {1,3,7,14,21,30};

            Task.Run(async () =>
            {
                UserEmail = (await _service.GetUser(Settings.UserId)).Email;
            });
        }

        #region Command : SendReport

        /// <summary>
        /// A command property representing the "SendReport" operation
        /// </summary>
        ICommand _sendReportCommand;
        public ICommand SendReportCommand =>
            _sendReportCommand ?? (_sendReportCommand = new Command(async () => await ExecuteSendReportCommandAsync()));

        async Task ExecuteSendReportCommandAsync()
        {
            if (IsBusy)
                return;

            IsBusy = true;
            try
            {
                await _service.SendReportForUser(UserEmail, ReportSize);
            }
            catch (Exception e)
            {
                Debug.WriteLine($"> {e.GetType()} : {e.Message} ({e.Source})");
            }
            finally
            {
                IsBusy = false;
            }
        }

        #endregion
    }
}
