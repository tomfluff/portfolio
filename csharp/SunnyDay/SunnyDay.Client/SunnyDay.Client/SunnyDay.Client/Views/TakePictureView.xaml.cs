using SunnyDay.Client.Core.Models;
using SunnyDay.Client.ViewModels;
using Xamarin.Forms;

namespace SunnyDay.Client.Views
{
    public sealed partial class TakePictureView : ContentPage
    {
        private readonly TakePictureViewModel _model;

        public TakePictureView()
        {
            InitializeComponent();
            BindingContext = _model = new TakePictureViewModel();
        }
    }
}
