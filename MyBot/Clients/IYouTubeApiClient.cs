using MyBot.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyBot.Clients
{
    public interface IYouTubeApiClient
    {

        public Task<List<Video>> GetSerchByVideoRequest(string request);


        public Task<LikeVideo> GetLikeVideoInfo(string videoId);//dell user


        public Task<List<Video>> GetSerchByArtist(string artist);

        public Task<List<List<Video>>> GetSerchByGenres();

        public Task<List<Video>> GetTrendingMusic();
    }
}
