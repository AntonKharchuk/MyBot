using MyBot.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot.Types.ReplyMarkups;

namespace MyBot
{
    public class BotMethods
    {
        public static void DBReuestsAddingError(UserRequest userRequest)
        {
            Console.WriteLine("ERROR trying to add:");
            Console.WriteLine($"User {0} {1} {2}", userRequest.UserName, userRequest.Request, userRequest.Time);
        }
        public static InlineKeyboardMarkup GetSongsMarkup(List<Models.Video> videos)
        {

            List<List<InlineKeyboardButton>> buttons = new List<List<InlineKeyboardButton>> { };

            for (int i = 0; i < videos.Count; i++)
            {
                buttons.Add(new List<InlineKeyboardButton>
                        {
                            InlineKeyboardButton.WithUrl($"{videos[i].VideoTitle}", @"https://www.youtube.com/watch?v=" + videos[i].VideoId)
                        }
                );
                CallBackD likeCallBack = new CallBackD()
                {
                    Idv = videos[i].VideoId,
                    Do = "Like"
                };
                var likeJson = JsonConvert.SerializeObject(likeCallBack);
                CallBackD addToPCallBack = new CallBackD()
                {
                    Idv = videos[i].VideoId,
                    Do = "AddToP"
                };
                var addToPJson = JsonConvert.SerializeObject(addToPCallBack);

                buttons.Add(new List<InlineKeyboardButton>
                        {
                            InlineKeyboardButton.WithCallbackData("Like",likeJson),
                            InlineKeyboardButton.WithCallbackData("Add to playlist",addToPJson)
                        }
                );
                Console.WriteLine(videos[i].VideoTitle);

            }


            InlineKeyboardMarkup keyboardMarkup = new InlineKeyboardMarkup
                (
                buttons
                );
            return keyboardMarkup;
        }
        public static List<InlineKeyboardMarkup> GetLikeMatesMarkups(List<List<LikeVideo>> mateUsersLikes, out List<string> matesUserNames)
        {
            matesUserNames = new List<string> { };
            List<InlineKeyboardMarkup> result = new List<InlineKeyboardMarkup> { };
            foreach (var userDataInLikeVideo in mateUsersLikes)
            {
                matesUserNames.Add(userDataInLikeVideo[0].UserName);
                List<Video> userDataInVideo = new List<Video> { };
                foreach (var likeVideo in userDataInLikeVideo)
                {
                    userDataInVideo.Add(likeVideo);
                }
                result.Add(GetSongsMarkup(userDataInVideo));

            }
            return result;
        }

        public static InlineKeyboardMarkup GetPlaylistsMarkup(List<Models.Playlist> playlists)
        {
            List<List<InlineKeyboardButton>> buttons = new List<List<InlineKeyboardButton>> { };

            foreach (var playlist in playlists)
            {
                CallBackD playlistCallBack = new CallBackD()
                {
                    Idp = playlist.Id,
                    Do = "ShowPToUser"
                };
                var playlistJson = JsonConvert.SerializeObject(playlistCallBack);

                buttons.Add(new List<InlineKeyboardButton>
                        {
                            InlineKeyboardButton.WithCallbackData(playlist.PlaylistName,playlistJson),
                        }
                );
            }
            CallBackD addPlaylistCallBack = new CallBackD()
            {
                Do = "AddPlaylist"
            };
            var addPlaylistJson = JsonConvert.SerializeObject(addPlaylistCallBack);
            buttons.Add(new List<InlineKeyboardButton>
                        {
                            InlineKeyboardButton.WithCallbackData("Add playlist",addPlaylistJson),
                        }
               );

            InlineKeyboardMarkup keyboardMarkup = new InlineKeyboardMarkup
                          (
                          buttons
                          );
            return keyboardMarkup;

        }

    }

}

