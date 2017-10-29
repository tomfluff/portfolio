using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Acr.UserDialogs;
using SunnyDay.Client.Core.Models;
using SunnyDay.Client.ViewModels;
using Xamarin.Forms;

namespace SunnyDay.Client.Views
{
	public partial class MasterPageView : MasterDetailPage
	{
	    private MasterPageViewModel _model;
	    private Type _currenType;

		public MasterPageView ()
		{
			InitializeComponent ();
		    this.BindingContext = _model = new MasterPageViewModel();

            // Initial navigation, this can be used for our home page
            Detail = new NavigationPage((Page)Activator.CreateInstance(typeof(MainPageView)))
            {
                BarBackgroundColor = Color.FromHex("#00B2EE"),
                BarTextColor = Color.White,
                HeightRequest = 20
            };
            _currenType = typeof(MainPageView);
		}

	    private void Navigation_OnItemSelected(object sender, SelectedItemChangedEventArgs e)
	    {
            var item = (MasterPageNavigationItem)e.SelectedItem;
            Type page = item.TargetType;

	        if (page != _currenType)
	        {
	            Detail = new NavigationPage((Page)Activator.CreateInstance(page))
                {
                    BarBackgroundColor = Color.FromHex("#00B2EE"),
                    BarTextColor = Color.White,
                    HeightRequest = 20
                };
                _currenType = page;
	        }
	        IsPresented = false;
        }
	}
}
