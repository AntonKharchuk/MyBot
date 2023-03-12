using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyBot.Models
{
    public class Playlist
    {
        public enum Fields
        {
            Id,
            UserId,
            UserName,
            PlaylistName,
            VideoIds,
            VideoTitles
        }
        public string Id { get; set; }
        public string UserId { get; set; }
        public string UserName { get; set; }
        public string PlaylistName { get; set; }
        public List<string> VideoIds { get; set; }
        public List<string> VideoTitles { get; set; }

    }
}
