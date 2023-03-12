using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyBot.Models
{
    public class LikeVideo:Video
    {
        public enum Fields
        {
            Id,
            UserId,
            UserName,
            ChannelId,
            ChannelTitle,
            VideoId,
            VideoTitle
        }
        public string Id { get; set; }
        public string UserId { get; set; }
        public string UserName { get; set; }
        public string ChannelId { get; set; }
    }
}
