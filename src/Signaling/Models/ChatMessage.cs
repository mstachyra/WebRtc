using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sayranet.WebRTC.Signaling.Models
{
    public class ChatMessage
    {
        public Guid Id { get; set; }
        public string Message { get; set; }
        public string Nick { get; set; }
        public DateTimeOffset? CreateDate { get; set; }
    }
}
