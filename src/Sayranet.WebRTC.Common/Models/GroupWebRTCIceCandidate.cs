using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sayranet.WebRTC.Common.Models
{
    public class GroupWebRTCIceCandidate
    {
        public string GroupName { get; set; }

        public WebRTCIceCandidate IceCandidate { get; set; }
    }
}
