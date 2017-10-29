using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SunnyDay.Client.ViewModels;
using Xamarin.Forms;

namespace SunnyDay.Client.Views
{
	public partial class LoginView : ContentPage
	{
	    private LoginViewModel _model;

		public LoginView ()
		{
			InitializeComponent ();
            BindingContext = _model = new LoginViewModel();
		}
	}
}
