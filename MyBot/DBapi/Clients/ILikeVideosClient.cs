using MyBot.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyBot.DBapi.Clients
{
    public interface ILikeVideosClient
    {
       // public Task<LikeVideo> GetDataById(string Id);
        public Task<List<LikeVideo>> GetAll();


        public Task<bool> PostLikeVideo(LikeVideo likeVideo);
        public Task<bool> DelateLikeVideo(string userId, string VideoId);
       

        public Task<List<List<LikeVideo>>> GetLikeMates(string userId);
        public Task<List<Video>> GetUserLikesVideo(string userId);
        
    }
}
