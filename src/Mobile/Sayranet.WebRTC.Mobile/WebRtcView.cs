using Sayranet.WebRTC.Mobile.Abstraction;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace Sayranet.WebRTC.Mobile
{
    public class WebRtcView : View, IWebRtcControl
    {
        public event EventHandler Starting;
        public event EventHandler Started;
        public event EventHandler<ErrorEventArgs> ErrorOccurred;

        public void SendStarting()
        {
            EventHandler eventHandler = this.Starting;
            eventHandler?.Invoke(this, EventArgs.Empty);
        }

        public void SendStarted()
        {
            EventHandler eventHandler = this.Started;
            eventHandler?.Invoke(this, EventArgs.Empty);
        }

        public void SendError(string message)
        {
            var eventHandler = this.ErrorOccurred;
            eventHandler?.Invoke(this, new ErrorEventArgs(message));
        }
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
