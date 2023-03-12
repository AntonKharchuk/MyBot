using MyBot.DBUse;
using MyBot.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyBot.DBapi.Clients
{
    public class LikeVideosClient : ILikeVideosClient
    {
        private readonly string _dBPath;
        private DBConnection _dBConnection;
        public LikeVideosClient() 
        {
            _dBPath = Constants.MyBotDirectory + @"DB\LikeVideos.txt";
            _dBConnection = new DBConnection(_dBPath);
        }

        public async Task<bool> DelateLikeVideo(string userId, string VideoId)
        {
            bool result = false;
            var allUserLikes = await _dBConnection.GetAllDataWithField((int)LikeVideo.Fields.UserId, userId);
            foreach (var likeVideoStr in allUserLikes)
            {
                var likeVideo = Convertor.FromStringToLikeVideo(likeVideoStr);
                if (likeVideo.VideoId == VideoId)
                {
                    result=  await _dBConnection.DelateDataById(likeVideo.Id);
                    break;
                }
            }
            return result;
        }

        public async Task<List<LikeVideo>> GetAll()
        {
            var content = await _dBConnection.GetAllData(); 
            var result = new List<LikeVideo>();
            foreach (var likeVideo in content)
            {
                result.Add(Convertor.FromStringToLikeVideo(likeVideo));
            }
            return result;
        }
        //n0 check
        public async Task<List<List<LikeVideo>>> GetLikeMates(string userId)
        {

            var result = new List<List<LikeVideo>>();
            var allData = await GetAll();
            var allUserLikes = await GetUserLikesLikeVideo(userId);
            List<string> usersLikeChannels = new List<string>();
            foreach (var like in allUserLikes)
            {
                if (!usersLikeChannels.Contains(like.ChannelTitle))
                {
                    usersLikeChannels.Add(like.ChannelTitle);
                }
            }

            List<string> usersById = new List<string>();
            foreach (var like in allData)
            {
                if(like.UserId!= userId&&usersLikeChannels.Contains(like.ChannelTitle))
                if (!usersById.Contains(like.UserId))
                {
                    usersById.Add(like.UserId);
                }
            }
            foreach (var userid in usersById)
            {
                result.Add(await GetUserLikesLikeVideo(userid));
            }
            return result;

        }
        public async Task<List<Video>> GetUserLikesVideo(string userId)
        {
            var result = new List<Video>();
            var allUserLikes = await _dBConnection.GetAllDataWithField((int)LikeVideo.Fields.UserId, userId);
            foreach (var likeVideoStr in allUserLikes)
            {
                var likeVideo = Convertor.FromStringToLikeVideo(likeVideoStr);
                var video = new Video()
                {
                    VideoId = likeVideo.VideoId,
                    VideoTitle = likeVideo.VideoTitle,
                    ChannelTitle = likeVideo.ChannelTitle
                };
                result.Add(video);
            }
            return result;
        }
        private async Task<List<LikeVideo>> GetUserLikesLikeVideo(string userId)
        {
            var result  = new List<LikeVideo>();
            var allUserLikes = await _dBConnection.GetAllDataWithField((int)LikeVideo.Fields.UserId, userId);
            foreach (var likeVideoStr in allUserLikes)
            {
                var likeVideo = Convertor.FromStringToLikeVideo(likeVideoStr);
                result.Add(likeVideo);
            }
            return result;
        }

        public async Task<bool> PostLikeVideo(LikeVideo likeVideo)
        {
            return await _dBConnection.PostData(Convertor.FromLikeVideoToString(likeVideo));
        }
    }
}
