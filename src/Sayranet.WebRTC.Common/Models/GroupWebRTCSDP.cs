using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sayranet.WebRTC.Common.Models
{
    public class GroupWebRTCSdp : WebRTCSDP
    {
        public string GroupName { get; set; }

        public WebRTCSDP Sdp { get; set; }
    }
}
