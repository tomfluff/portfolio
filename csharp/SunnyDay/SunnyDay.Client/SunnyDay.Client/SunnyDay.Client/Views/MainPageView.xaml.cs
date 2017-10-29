using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SunnyDay.Client.Core.Services;
using SunnyDay.Client.Core.Utils;
using SunnyDay.Client.ViewModels;
using Xamarin.Forms;

namespace SunnyDay.Client.Views
{
	public partial class MainPageView : ContentPage
	{
	    private MainPageViewModel _model;

		public MainPageView ()
		{
			InitializeComponent ();
		    BindingContext = _model = new MainPageViewModel();
		}

	    protected override void OnAppearing()
	    {
	        base.OnAppearing();
	        Debug.WriteLine("> MainPage Appeared!!");
	    }

	    protected override void OnDisappearing()
	    {
	        base.OnDisappearing();
            Debug.WriteLine("> MainPage Disappeared!!");
        }
        
	}
}
