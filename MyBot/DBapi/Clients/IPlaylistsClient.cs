using MyBot.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyBot.DBapi.Clients
{
    public interface IPlaylistsClient
    {
        //public Task<Playlist> GetData(string Id);
        public Task<bool> PostPlaylist(Playlist playlist);
        //public Task<bool> PostVideos(Playlist playlist);

        public Task<bool> DelatePlaylist(string playlistId);
        public Task<List<Playlist>> GetAll();
        public Task<List<Playlist>> GetUserPlaylists(string userId);
        public Task<Playlist> GetPlaylistsById(string Id);



    }
}
