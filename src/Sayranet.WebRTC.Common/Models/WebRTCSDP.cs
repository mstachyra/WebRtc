using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sayranet.WebRTC.Common.Models
{
    public class WebRTCSDP
    {
        public SDPType? Type { get; set; }

        public string Sdp { get; set; }
    }

    public enum SDPType
    {
        Offer = 0,
        Pranswer = 1,
        Answer = 2
    }
}
