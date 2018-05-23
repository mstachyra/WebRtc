using Microsoft.AspNet.SignalR.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Sayranet.WebRTC.Mobile
{
    //[XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class WebRTC : ContentPage
    {
        private readonly string _groupName;

        public WebRTC(string groupName)
        {
            InitializeComponent();
            _groupName = groupName;
        }

        protected override void OnAppearing()
        {
            InitializeHub();
        }

        private void InitializeHub()
        {
            var hubConnection = new HubConnection("http://test1.mstachyra.hostingasp.pl");
            var hubProxy = hubConnection.CreateHubProxy("chat");

            hubProxy.On<string>("Message", GetMessage);

            hubConnection.Start().ContinueWith((o) =>
            {
                // Register group
                hubProxy.Invoke("JoinGroup", _groupName);
                GetMessage("Welcome");
            });
        }

        private void GetMessage(string msg)
        {
            Device.BeginInvokeOnMainThread(() =>
            {
                MessageLabel.Text = msg;
            });
        }
    }
}