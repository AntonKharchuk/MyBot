using MyBot.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace MyBot.Clients
{
    internal class YouTubeApiClient:IYouTubeApiClient
    {
        private HttpClient _client = new HttpClient();

        public YouTubeApiClient()
        {
            _client.BaseAddress = new Uri(Constants.LocalhostAddres + @"YouTubeApi/");
        }

        public async Task<LikeVideo> GetLikeVideoInfo(string videoId)
        {
            //videoinfo?VideoId=gebe

            var content = await _client.GetAsync($"videoinfo?VideoId={videoId}");

            var result = JsonConvert.DeserializeObject<LikeVideo>(content.Content.ReadAsStringAsync().Result);

            return result;
        }

        public async Task<List<Video>> GetSerchByArtist(string artist)
        {
            var content = await _client.GetAsync($"artistbyrequest?artist={artist}");

            var result = JsonConvert.DeserializeObject<List<Video>>(content.Content.ReadAsStringAsync().Result);

            return result;
        }

        public async Task<List<List<Video>>> GetSerchByGenres()
        {
            var content = await _client.GetAsync($"genresbyrequest");

            var result = JsonConvert.DeserializeObject<List<List<Video>>>(content.Content.ReadAsStringAsync().Result);

            return result;
        }

        public async Task<List<Video>> GetSerchByVideoRequest(string request)
        {
            var content = await _client.GetAsync($"videosbyrequest?request={request}");

            var result = JsonConvert.DeserializeObject<List<Video>>(content.Content.ReadAsStringAsync().Result);

            return result;
        }

        public async Task<List<Video>> GetTrendingMusic()
        {
            var content = await _client.GetAsync($"trendsbyrequest");

            var result = JsonConvert.DeserializeObject<List<Video>>(content.Content.ReadAsStringAsync().Result);

            return result;
        }

    }
}
