using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SunnyDay.Client.ViewModels;
using Xamarin.Forms;

namespace SunnyDay.Client.Views
{
	public partial class ReportPageView : ContentPage
	{
	    private ReportPageViewModel _model;

		public ReportPageView ()
		{
			InitializeComponent ();
		    this.BindingContext = _model = new ReportPageViewModel();
		}
	}
}
