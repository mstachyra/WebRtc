using Org.WebRtc;
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

        protected override void OnElementChanged(ElementChangedEventArgs<WebRtcView> e)
        {
            base.OnElementChanged(e);

            if (Control == null)
            {
                Debug.WriteLine("Setup WebRtcView Control null");

                e.NewElement.Started += NewElement_Started;
                e.NewElement.Starting += NewElement_Starting;

                _panel = new RelativePanel();
                _panel.Width = e.NewElement.WidthRequest;
                _panel.Height = e.NewElement.HeightRequest;
                _panel.Background = new SolidColorBrush(Colors.Aqua);

                _panel.Children.Add(new MediaElement());

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
                    //RunOnUiThread(async () =>
                    //{
                    //    var msgDialog = new MessageDialog(
                    //        );
                    //    await msgDialog.ShowAsync();
                    //});
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

            // Send event started
            Element.SendStarted();
        }

        private void NewElement_Started(object sender, EventArgs e)
        {

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
}
