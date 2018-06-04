using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sayranet.WebRTC.Common.Models
{
    public class WebRTCIceCandidate
    {
        public string SdpMLineIndex { get; set; }
        public string SdpMid { get; set; }
        public string Candidate { get; set; }
    }
}
