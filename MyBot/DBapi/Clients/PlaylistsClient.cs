using MyBot.DBUse;
using MyBot.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyBot.DBapi.Clients
{
    internal class PlaylistsClient : IPlaylistsClient
    {
        private readonly string _dBPath;
        private DBConnection _dBConnection;
        public PlaylistsClient()
        {
            _dBPath = Constants.MyBotDirectory + @"DB\Playlists.txt";
            _dBConnection = new DBConnection(_dBPath);
        }
        public async Task<bool> DelatePlaylist(string playlistId)
        {
            bool result = false;

            var allUserPlaylists = await GetAll();

            foreach (var playlist in allUserPlaylists)
            {
                if (playlist.Id == playlistId)
                {
                    result = await _dBConnection.DelateDataById(playlist.Id);
                    break;
                }
            }
            return result;
        }

        public async Task<List<Playlist>> GetAll()
        {
            var content = await _dBConnection.GetAllData();
            var result = new List<Playlist>();
            foreach (var playlist in content)
            {
                result.Add(Convertor.FromStringToPlaylist(playlist));
            }
            return result;
        }

        public async Task<Playlist> GetPlaylistsById(string Id)
        {
            var result = new Playlist();
            foreach (var playlist in await GetAll())
            {
                if (playlist.Id == Id)
                {
                    result = playlist;
                    break;
                }
            } 
            return result;

        }

        public async Task<List<Playlist>> GetUserPlaylists(string userId)
        {
            var result = new List<Playlist>();
            var allUserPlaylists = await _dBConnection.GetAllDataWithField((int)Playlist.Fields.UserId, userId);
            foreach (var playlistStr in allUserPlaylists)
            {
                var playlist = Convertor.FromStringToPlaylist(playlistStr);
                result.Add(playlist);
            }
            return result;
        }

        public async Task<bool> PostPlaylist(Playlist playlist)
        {
            bool allreadyHasPlaylistWithThisName = false;
            foreach (var userPlaylist in await GetUserPlaylists(playlist.Id))
            {
                if (userPlaylist.PlaylistName == playlist.PlaylistName)
                {
                    allreadyHasPlaylistWithThisName = true;
                }
            }
            if (allreadyHasPlaylistWithThisName)
            {
                return false;
            }
            return await _dBConnection.PostData(Convertor.FromPlaylistToString(playlist));
        }
    }
}
