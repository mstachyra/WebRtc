using Sayranet.WebRTC.Mobile.Abstraction;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace Sayranet.WebRTC.Mobile
{
	public partial class MainPage : ContentPage
	{
		public MainPage()
		{
			InitializeComponent();
		}

        private async void Button_Clicked(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(groupNameInput.Text))
            {
                await DisplayAlert("Hey", "Fill group name", "Close");
            }
            else
            {
                await Navigation.PushAsync(new WebRTCPage(groupNameInput.Text));
            }
        }

        private void Button_Clicked2(object sender, EventArgs e)
        {
            WebRtcControl.SendStarting();
        }

        private async void WebRtcControl_ErrorOccurred(object sender, ErrorEventArgs e)
        {
            await DisplayAlert("Error", e.Message, "Ok");
        }
    }
}
