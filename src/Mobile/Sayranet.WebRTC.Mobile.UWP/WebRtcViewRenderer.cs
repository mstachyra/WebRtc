using Org.WebRtc;
using Sayranet.WebRTC.Common.Models;
using Sayranet.WebRTC.Mobile;
using Sayranet.WebRTC.Mobile.UWP;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Windows.ApplicationModel;
using Windows.Devices.Enumeration;
using Windows.Graphics.Display;
using Windows.Media.Capture;
using Windows.System.Display;
using Windows.UI;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Xamarin.Forms.Platform.UWP;

[assembly: ExportRenderer(typeof(WebRtcView), typeof(WebRtcViewRenderer))]
namespace Sayranet.WebRTC.Mobile.UWP
{
    public class WebRtcViewRenderer : ViewRenderer<WebRtcView, RelativePanel>
    {
        private RelativePanel _panel;
        private Media _media;

        private MediaStream _localStream;
        private MediaElement _localVideo;
        private RTCPeerConnection _peerConnection;

        private MediaElement _remoteVideo;

        protected override void OnElementChanged(ElementChangedEventArgs<WebRtcView> e)
        {
            base.OnElementChanged(e);

            if (Control == null)
            {
                Debug.WriteLine("Setup WebRtcView Control null");
                
                e.NewElement.Starting += NewElement_Starting;
                e.NewElement.OfferRequested += NewElement_OfferRequested;
                e.NewElement.AnswerRequested += NewElement_AnswerRequested;
                e.NewElement.AnswerReplayed += NewElement_AnswerReplayed;

                _panel = new RelativePanel();
                //_panel.Width = e.NewElement.WidthRequest;
                //_panel.Height = e.NewElement.HeightRequest;
                _panel.Background = new SolidColorBrush(Colors.Aqua);

                _localVideo = new MediaElement();
                _localVideo.Width = 320;
                _localVideo.Height = 240;

                _remoteVideo = new MediaElement();
                _remoteVideo.Width = 320;
                _remoteVideo.Height = 240;
                RelativePanel.SetAlignRightWithPanel(_remoteVideo, true);

                _panel.Children.Add(_localVideo);
                _panel.Children.Add(_remoteVideo);

                SetNativeControl(_panel);
            }
        }

        private void NewElement_Starting(object sender, EventArgs e)
        {
#if DEBUG
            //Enable logging (
            Org.WebRtc.WebRTC.EnableLogging(LogLevel.LOGLVL_INFO);
            Debug.WriteLine("WebRTC debug file: " + Org.WebRtc.WebRTC.LogFileName);
            Debug.WriteLine("WebRTC debug storage folder name :" + Org.WebRtc.WebRTC.LogFolder.DisplayName
                + "\nand path: " + Org.WebRtc.WebRTC.LogFolder.Path);
#endif
            // Display a permission dialog to request access to the microphone and camera
            Org.WebRtc.WebRTC.RequestAccessForMediaCapture().AsTask().ContinueWith(async t =>
            {
                if (t.Result)
                {
                    await Initialize();
                }
                else
                {
                    Element.SendError("Failed to obtain access to multimedia devices!");
                }
            });
        }

        private async Task Initialize()
        {
            //Initialization of WebRTC worker threads, etc
            Org.WebRtc.WebRTC.Initialize(Dispatcher);

            _media = Media.CreateMedia();

            //Selecting video device to use, setting preferred capabilities
            var videoDevices = _media.GetVideoCaptureDevices();
            var selectedVideoDevice = videoDevices.First();
            var videoCapabilites = await selectedVideoDevice.GetVideoCaptureCapabilities();
            var selectedVideoCapability = videoCapabilites.FirstOrDefault();

            //Needed for HoloLens camera, will not set compatible video capability automatically
            //Hololens Cam default capability: 1280x720x30
            Org.WebRtc.WebRTC.SetPreferredVideoCaptureFormat(
                (int)selectedVideoCapability.Width,
                (int)selectedVideoCapability.Height,
                (int)selectedVideoCapability.FrameRate);

            //Setting up local stream 
            RTCMediaStreamConstraints mediaStreamConstraints = new RTCMediaStreamConstraints
            {
                audioEnabled = false,
                videoEnabled = true
            };
            _localStream = await _media.GetUserMedia(mediaStreamConstraints);
            _media.SelectVideoDevice(selectedVideoDevice);
            
            // Get Video Tracks
            var videotrac = _localStream.GetVideoTracks();
            foreach (var videoTrack in videotrac) //This foreach may not be necessary 
            {
                videoTrack.Enabled = true;
            }
            var selectedVideoTrac = videotrac.FirstOrDefault();
            
            Debug.WriteLine("Creating RTCPeerConnection");
            var config = new RTCConfiguration()
            {
                BundlePolicy = RTCBundlePolicy.Balanced,
                IceTransportPolicy = RTCIceTransportPolicy.All,
                IceServers = GetDefaultList()
            };
            _peerConnection = new RTCPeerConnection(config);
            _peerConnection.OnIceCandidate += _localRtcPeerConnection_OnIceCandidate;
            _peerConnection.OnIceConnectionChange += _localRtcPeerConnection_OnIceConnectionChange;
            _peerConnection.OnAddStream += _peerConnection_OnAddStream;
           
            //_peerConnection.AddStream(_localStream);
            _media.AddVideoTrackMediaElementPair(selectedVideoTrac, _localVideo, _localStream.Id);

            // Send event started
            Element.SendStarted();

            //Debug.WriteLine("Creating 'remote' RTCPeerConnection");
            //_remoteRtcPeerConnection = new RTCPeerConnection(config);
            //_remoteRtcPeerConnection.OnIceCandidate += _remoteRtcPeerConnection_OnIceCandidate;
            //_remoteRtcPeerConnection.OnIceConnectionChange += _remoteRtcPeerConnection_OnIceConnectionChange;
            //// Wait for Stream
            //_remoteRtcPeerConnection.OnAddStream += _remoteRtcPeerConnection_OnAddStream;
        }

        private async void NewElement_OfferRequested(object sender, EventArgs e)
        {
            var offer = await _peerConnection.CreateOffer();
            Debug.WriteLine("OfferRequested CreateOffer, sdp\n" + offer.Sdp);

            await _peerConnection.SetLocalDescription(offer);
            Debug.WriteLine("OfferRequested SetLocalDescription complete offer");

            //await _remoteRtcPeerConnection.SetRemoteDescription(offer);
            //Debug.WriteLine("SetRemoteDescription complete offer");

            Element.SendOfferRecived(offer.ToWebRTCSDP());
        }

        private async void NewElement_AnswerRequested(object sender, SdpEventArgs e)
        {
            await _peerConnection.SetRemoteDescription(e.Sdp.ToRTCSessionDescription());
            Debug.WriteLine("AnswerRequested SetRemoteDescription complete");

            var answerDesc = await _peerConnection.CreateAnswer();
            Debug.WriteLine("AnswerRequested CreateAnswer complete");

            await _peerConnection.SetLocalDescription(answerDesc);
            Debug.WriteLine("AnswerRequested SetLocalDescription complete");

            Element.SendAnswerRecived(answerDesc.ToWebRTCSDP());
        }

        private async void NewElement_AnswerReplayed(object sender, SdpEventArgs e)
        {
            await _peerConnection.SetRemoteDescription(e.Sdp.ToRTCSessionDescription());
            Debug.WriteLine("AnswerReplayed SetRemoteDescription complete");
        }

        private void _localRtcPeerConnection_OnIceConnectionChange(RTCPeerConnectionIceStateChangeEvent __param0)
        {
            Debug.WriteLine($"Entered _localRtcPeerConnection_OnIceConnectionChange {__param0.State}");
        }

        private async void _localRtcPeerConnection_OnIceCandidate(RTCPeerConnectionIceEvent __param0)
        {
            Debug.WriteLine("Entered _localRtcPeerConnection_OnIceCandidate");
            await _peerConnection.AddIceCandidate(__param0.Candidate);
        }

        //private void _remoteRtcPeerConnection_OnIceConnectionChange(RTCPeerConnectionIceStateChangeEvent __param0)
        //{
        //    Debug.WriteLine($"Entered _remoteRtcPeerConnection_OnIceConnectionChange {__param0.State}");
        //}

        //private async void _remoteRtcPeerConnection_OnIceCandidate(RTCPeerConnectionIceEvent __param0)
        //{
        //    Debug.WriteLine("Entered _remoteRtcPeerConnection_OnIceCandidate");
        //    await _remoteRtcPeerConnection.AddIceCandidate(__param0.Candidate);
        //}

        //private void _remoteRtcPeerConnection_OnAddStream(MediaStreamEvent __param0)
        //{
        //    MediaStream remoteStream = __param0.Stream;
        //    var tracks = remoteStream.GetVideoTracks();
        //    _media.AddVideoTrackMediaElementPair(tracks.FirstOrDefault(), _remoteVideo, remoteStream.Id);
        //    Debug.WriteLine("Received a remote stream");
        //}

        private void _peerConnection_OnAddStream(MediaStreamEvent __param0)
        {
            MediaStream remoteStream = __param0.Stream;
            var tracks = remoteStream.GetVideoTracks();
            _media.AddVideoTrackMediaElementPair(tracks.FirstOrDefault(), _remoteVideo, remoteStream.Id);
            Debug.WriteLine("Received a remote stream");
        }

        //Stun and Turn servers borrowed from Chatterbox example
        private static List<RTCIceServer> GetDefaultList()
        {
            return new List<RTCIceServer>
            {
                new RTCIceServer
                {
                    Url = "stun:stun.l.google.com:19302",
                    Username = string.Empty,
                    Credential = string.Empty
                },
                new RTCIceServer
                {
                    Url = "stun:stun1.l.google.com:19302",
                    Username = string.Empty,
                    Credential = string.Empty
                },
                new RTCIceServer
                {
                    Url = "stun:stun2.l.google.com:19302",
                    Username = string.Empty,
                    Credential = string.Empty
                },
                new RTCIceServer
                {
                    Url = "stun:stun3.l.google.com:19302",
                    Username = string.Empty,
                    Credential = string.Empty
                },
                new RTCIceServer
                {
                    Url = "stun:stun4.l.google.com:19302",
                    Username = string.Empty,
                    Credential = string.Empty
                },
                new RTCIceServer
                {
                    Url = "turn:40.76.194.255:3478",
                    Username = "testrtc",
                    Credential = "rtc123"
                }
            };
        }
    }


    public static class RTCSessionDescriptionExtension
    {
        public static WebRTCSDP ToWebRTCSDP(this RTCSessionDescription sessionDescription)
        {
            return new WebRTCSDP
            {
                Type = sessionDescription.Type.HasValue ? (SDPType?)sessionDescription.Type.Value : null,
                Sdp = sessionDescription.Sdp
            };
        }

        public static RTCSessionDescription ToRTCSessionDescription(this WebRTCSDP sessionDescription)
        {
            if (!sessionDescription.Type.HasValue)
            {
                Debug.WriteLine("WebRTCSDP type is null");
            }
            return new RTCSessionDescription((RTCSdpType)sessionDescription.Type.GetValueOrDefault(), sessionDescription.Sdp);
        }
    }
}
