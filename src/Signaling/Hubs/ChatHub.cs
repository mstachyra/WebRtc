using Microsoft.AspNetCore.SignalR;
using Sayranet.WebRTC.Common.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sayranet.WebRTC.Signaling.Hubs
{
    public class ChatHub : Hub
    {
        public void JoinGroup(string groupName)
        {
            Groups.AddToGroupAsync(Context.ConnectionId, groupName);
        }

        public void GroupMessage(GroupMessage msg)
        {
            // Send only to others
            Clients.OthersInGroup(msg.GroupName).SendAsync("Message", msg.Message);
        }

        public void GroupOffer(GroupWebRTCSdp webRTCSdp)
        {
            // Send only to others
            Clients.OthersInGroup(webRTCSdp.GroupName).SendAsync("Offer", webRTCSdp.Sdp);
        }

        public void GroupAnswer(GroupWebRTCSdp webRTCSdp)
        {
            // Send only to others
            Clients.OthersInGroup(webRTCSdp.GroupName).SendAsync("Answer", webRTCSdp.Sdp);
        }

        public void GroupSdp(GroupWebRTCSdp webRTCSdp)
        {
            if (webRTCSdp.Sdp == null)
            {
                return;
            }

            // Send only to others
            Clients.OthersInGroup(webRTCSdp.GroupName).SendAsync("Sdp", webRTCSdp.Sdp);
        }

        public void GroupIceCandidate(GroupWebRTCIceCandidate webRTCIceCandidate)
        {
            if (webRTCIceCandidate.IceCandidate == null)
            {
                return;
            }

            // Send only to others
            Clients.OthersInGroup(webRTCIceCandidate.GroupName).SendAsync("IceCandidate", webRTCIceCandidate.IceCandidate);
        }
    }
}
