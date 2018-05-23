using Microsoft.AspNetCore.SignalR;
using Sayranet.WebRTC.Signaling.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sayranet.WebRTC.Signaling.Hubs
{
    public class ChatHub : Hub
    {
        public void SendMessage(ChatMessage chatMessage)
        {
            chatMessage.CreateDate = DateTimeOffset.Now;
            Clients.All.SendAsync("send", chatMessage);
        }

        public void JoinGroup(string groupName)
        {
            Groups.AddToGroupAsync(Context.ConnectionId, groupName);
        }

        public void GroupMessage(GroupMessage msg)
        {
            // Send only to others
            Clients.OthersInGroup(msg.GroupName).SendAsync("Message", msg.Message);
        }
    }
}
