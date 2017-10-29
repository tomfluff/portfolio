using SunnyDay.Client.ViewModels;
using System;
using Xamarin.Forms;

namespace SunnyDay.Client.Views
{
    public partial class SettingsView : ContentPage
    {
        private readonly SettingsViewModel _model;

        public SettingsView()
        {
            InitializeComponent();
            BindingContext = _model = new SettingsViewModel();
        }
    }
}
