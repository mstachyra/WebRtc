using Sayranet.WebRTC.Mobile;
using Sayranet.WebRTC.Mobile.UWP;
using System;
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

        protected override void OnElementChanged(ElementChangedEventArgs<WebRtcView> e)
        {
            base.OnElementChanged(e);

            if (Control == null)
            {
                //_panel = new RelativePanel();
                //_panel.Width = e.NewElement.WidthRequest;
                //_panel.Height = e.NewElement.HeightRequest;

                //_panel.Background = new SolidColorBrush(Colors.Aqua);
                //SetNativeControl(_panel);
            }

            if (Control != null)
            {
                Control.Background = new SolidColorBrush(Colors.Aqua);
            }
        }
    }
}
