using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace SunnyDay.Client.ViewModels
{
    public class BaseViewModel : MvvmHelpers.BaseViewModel
    {
        /// <summary>
        /// This implementation adds the Info area properties like:
        /// InfoMessage - The text to display,
        /// InfoColor - The background color of the area
        /// InfoDisplay - True if the area should be displayed
        /// </summary>
        #region Property : InfoMessage

        private string _infoMessage = string.Empty;
        public const string InfoMessagePropertyName = "InfoMessage";
        /// <summary>
        /// Gets or sets the "InfoMessage" property
        /// </summary>
        /// <value>The property value.</value>
        public string InfoMessage
        {
            get { return _infoMessage; }
            set { SetProperty(ref _infoMessage, value, InfoMessagePropertyName); }
        }

        #endregion

        #region Property : InfoColor

        private Color _infoColor = Color.Transparent;
        public const string InfoColorPropertyName = "InfoColor";
        /// <summary>
        /// Gets or sets the "InfoColor" property
        /// </summary>
        /// <value>The property value.</value>
        public Color InfoColor
        {
            get { return _infoColor; }
            set { SetProperty(ref _infoColor, value, InfoColorPropertyName); }
        }

        #endregion

        #region Property : InfoDisplay

        private bool _infoDisplay = false;
        public const string InfoDisplayPropertyName = "InfoDisplay";
        /// <summary>
        /// Gets or sets the "InfoDisplay" property
        /// </summary>
        /// <value>The property value.</value>
        public bool InfoDisplay
        {
            get { return _infoDisplay; }
            set { SetProperty(ref _infoDisplay, value, InfoDisplayPropertyName); }
        }

        #endregion

        #region Command : CloseInfo

        /// <summary>
        /// A command property representing the "CloseInfo" operation
        /// </summary>
        ICommand _closeInfoCommand;
        public ICommand CloseInfoCommand =>
            _closeInfoCommand ?? (_closeInfoCommand = new Command(async () => await ExecuteCloseInfoCommandAsync()));

        async Task ExecuteCloseInfoCommandAsync()
        {
            InfoDisplay = false;
        }

        #endregion
    }
}
