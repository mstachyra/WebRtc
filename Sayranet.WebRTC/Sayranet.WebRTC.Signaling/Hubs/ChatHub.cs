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
    }
}
