using MyBot.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyBot.DBapi
{
    public static class Convertor
    {

        public static LikeVideo FromStringToLikeVideo(List<string> data)
        {
            var result = new LikeVideo()
            {
                Id = data[0],
                UserId = data[1],
                UserName = data[2],
                ChannelId = data[3],
                ChannelTitle = data[4],
                VideoId = data[5],
                VideoTitle = data[6]
            };
            return result;
        }
        public static List<string> FromLikeVideoToString(LikeVideo data)
        {
            var result = new List<string>();
            result.Add(data.Id);
            result.Add(data.UserId);
            result.Add(data.UserName);
            result.Add(data.ChannelId);
            result.Add(data.ChannelTitle);
            result.Add(data.VideoId);
            result.Add(data.VideoTitle);
            return result;
        }
        public static UserRequest FromStringToUserRequest(List<string> data)
        {
            var result = new UserRequest()
            {
                Id = data[0],
                UserId = data[1],
                UserName = data[2],
                Request = data[3],
                Time = data[4]
            };
            return result;

        }
        public static List<string> FromUserRequestToString(UserRequest data)
        {
            var result = new List<string>();
            result.Add(data.Id);
            result.Add(data.UserId);
            result.Add(data.UserName);
            result.Add(data.Request);
            result.Add(data.Time);
            return result;
        }
        public static Playlist FromStringToPlaylist(List<string> data)
        {
            var result = new Playlist()
            {
                Id = data[0],
                UserId = data[1],
                UserName = data[2],
                PlaylistName = data[3],
                VideoIds = JsonConvert.DeserializeObject<List<string>>(data[4]),
                VideoTitles = JsonConvert.DeserializeObject<List<string>>(data[5])
            };
       
            return result;
        }
        public static List<string> FromPlaylistToString(Playlist data)
        {
            
            var result = new List<string>();
            result.Add(data.Id);
            result.Add(data.UserId);
            result.Add(data.UserName);
            result.Add(data.PlaylistName);
            result.Add(JsonConvert.SerializeObject(data.VideoIds));
            result.Add(JsonConvert.SerializeObject(data.VideoTitles));
            return result;
        }
    }
}
