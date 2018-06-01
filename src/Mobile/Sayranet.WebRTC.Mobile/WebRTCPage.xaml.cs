using Microsoft.AspNetCore.Http.Connections;
using Microsoft.AspNetCore.SignalR.Client;
using Sayranet.WebRTC.Common.Models;
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
    public partial class WebRTCPage : ContentPage
    {
        private readonly string _groupName;
        private readonly ObservableCollection<string> _listMessages;
        private HubConnection _hub;

        private bool _webRtcControlStarted = false;

        public WebRTCPage(string groupName)
        {
            InitializeComponent();

            _groupName = groupName;
            GroupNameLabel.Text = groupName;

            _listMessages = new ObservableCollection<string>();
            _listMessages.Add("Welcome in app");
            ChatConsole.ItemsSource = _listMessages;

            _webRtcControlStarted = false;
            WebRtcControl.OfferRecived += WebRtcControl_OfferRecived;
            WebRtcControl.AnswerRecived += WebRtcControl_AnswerRecived;
            WebRtcControl.Started += WebRtcControl_Started;
        }

        private void WebRtcControl_Started(object sender, EventArgs e)
        {
            _webRtcControlStarted = true;
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
            _hub.On<WebRTCSDP>("Answer", ReciveAnswer);
            _hub.On<WebRTCSDP>("Offer", ReciveOffer);
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

        private void WebRtcControl_ErrorOccurred(object sender, ErrorEventArgs e)
        {
            Device.BeginInvokeOnMainThread(() =>
            {
                _listMessages.Add("Error: " + e);
                ChatConsole.ScrollTo(e, ScrollToPosition.End, false);
            });
        }

        private void SendOfferWebRTCBtn_Clicked(object sender, EventArgs e)
        {
            // Start Call, create offer and send to SignalR
            WebRtcControl.SendOfferRequested();
        }

        private async void WebRtcControl_OfferRecived(object sender, SdpEventArgs e)
        {
            Device.BeginInvokeOnMainThread(() =>
            {
                _listMessages.Add("SDP: " + e.Sdp.Sdp);
            });

            // Send offer via SignalR
            await _hub.InvokeAsync("GroupOffer", new GroupWebRTCSdp()
            {
                GroupName = _groupName,
                Sdp = e.Sdp
            });
        }

        private void ReciveOffer(WebRTCSDP obj)
        {
            // If recive offer from other client then create answer
            WebRtcControl.SendAnswerRequested(obj);
        }

        private async void WebRtcControl_AnswerRecived(object sender, SdpEventArgs e)
        {
            // Created answer from control send to other client via SignalR
            await _hub.InvokeAsync("GroupAnswer", new GroupWebRTCSdp()
            {
                GroupName = _groupName,
                Sdp = e.Sdp
            });
        }

        private void ReciveAnswer(WebRTCSDP obj)
        {
            // If recive answer then set replay to control
            WebRtcControl.SendAnswerReplayed(obj);
        }

        private void StartWebRTCBtn_Clicked(object sender, EventArgs e)
        {
            WebRtcControl.SendStarting();

        }

        private void StopWebRTCBtn_Clicked(object sender, EventArgs e)
        {

        }
    }
}