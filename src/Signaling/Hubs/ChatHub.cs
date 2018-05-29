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

        public void GroupOffer(GroupWebRTCSdp webRTCSDP)
        {
            // Send only to others
            Clients.OthersInGroup(webRTCSDP.GroupName).SendAsync("Answer", webRTCSDP);
        }
    }
}
