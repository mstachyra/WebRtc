using Sayranet.WebRTC.Common.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace Sayranet.WebRTC.Mobile
{
    public class WebRtcView : View
    {
        public event EventHandler Starting;
        public event EventHandler Started;
        public event EventHandler<ErrorEventArgs> ErrorOccurred;
        public event EventHandler OfferRequested;
        public event EventHandler<SdpEventArgs> OfferRecived;
        public event EventHandler<SdpEventArgs> AnswerRequested;
        public event EventHandler<SdpEventArgs> AnswerRecived;
        public event EventHandler<SdpEventArgs> AnswerReplayed;

        public event EventHandler Stoping;
        public event EventHandler Stopped;

        public WebRTCSDP MyProperty { get; set; }

        public void SendStarting()
        {
            Starting?.Invoke(this, EventArgs.Empty);
        }

        public void SendStarted()
        {
            Started?.Invoke(this, EventArgs.Empty);
        }

        public void SendError(string message)
        {
            ErrorOccurred?.Invoke(this, new ErrorEventArgs(message));
        }

        /// <summary>
        /// Create offer and set to local Peer
        /// </summary>
        public void SendOfferRequested()
        {
            OfferRequested?.Invoke(this, EventArgs.Empty);
        }

        /// <summary>
        /// Return local peer offer to send via signaling
        /// </summary>
        /// <param name="sdp"></param>
        public void SendOfferRecived(WebRTCSDP sdp)
        {
            OfferRecived?.Invoke(this, new SdpEventArgs(sdp));
        }

        /// <summary>
        /// Set SDP recived via signaling to remote for Local peer and create answer
        /// </summary>
        /// <param name="sdp"></param>
        public void SendAnswerRequested(WebRTCSDP sdp)
        {
            AnswerRequested?.Invoke(this, new SdpEventArgs(sdp));
        }

        /// <summary>
        /// Return answer to send via signaling
        /// </summary>
        /// <param name="sdp"></param>
        public void SendAnswerRecived(WebRTCSDP sdp)
        {
            AnswerRecived?.Invoke(this, new SdpEventArgs(sdp));
        }

        /// <summary>
        /// Set answer SDP to local peer
        /// </summary>
        /// <param name="sdp"></param>
        public void SendAnswerReplayed(WebRTCSDP sdp)
        {
            AnswerReplayed?.Invoke(this, new SdpEventArgs(sdp));
        }
    }

    public class SdpEventArgs : EventArgs
    {
        public SdpEventArgs(WebRTCSDP sdp)
        {
            Sdp = sdp;
        }

        public WebRTCSDP Sdp { get; private set; }
    }

    public class ErrorEventArgs : EventArgs
    {
        public ErrorEventArgs(string message)
        {
            Message = message;
        }

        public string Message { get; private set; }
    }
}
