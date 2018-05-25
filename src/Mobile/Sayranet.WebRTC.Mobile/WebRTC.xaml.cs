using Microsoft.AspNetCore.Http.Connections;
using Microsoft.AspNetCore.SignalR.Client;
using Sayranet.WebRTC.Mobile.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
        private readonly ObservableCollection<string> _listMessages;
        private HubConnection _hub;

        public WebRTC(string groupName)
        {
            InitializeComponent();
            _groupName = groupName;
            GroupNameLabel.Text = groupName;
            _listMessages = new ObservableCollection<string>();
            _listMessages.Add("Welcome in app");
            ChatConsole.ItemsSource = _listMessages;
        }

        protected override async void OnAppearing()
        {
            await InitializeHub();
        }

        private async Task InitializeHub()
        {

            _hub = new HubConnectionBuilder()
                .WithUrl("http://test1.mstachyra.hostingasp.pl/chat", HttpTransportType.WebSockets)
                .Build();

            await _hub.StartAsync();
            await _hub.InvokeAsync("JoinGroup", _groupName);
            _hub.On<string>("Message", SetMessage);
        }

        private void SetMessage(string msg)
        {
            Device.BeginInvokeOnMainThread(() =>
            {
                _listMessages.Add("Recive: " + msg);
                ChatConsole.ScrollTo(msg, ScrollToPosition.End, false);
            });
        }

        private async void Entry_Completed(object sender, EventArgs e)
        {
            _listMessages.Add($"You: {ChatInput.Text}");
            await _hub.InvokeAsync("GroupMessage", new GroupMessage()
            {
                GroupName = _groupName,
                Message = ChatInput.Text
            });
            ChatInput.Text = string.Empty;
        }
    }
}