
using System;
using System.IO;

using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;


using System.Threading;
using Telegram.Bot.Types.InputFiles;

using System.Reflection;
using MyBot.Models;
using Newtonsoft.Json;
using System.Net.Http;
using Telegram.Bot.Polling;
using MyBot.DBapi.Clients;
using MyBot.Clients;

namespace MyBot
{


    class Bot
    {
        private readonly LikeVideosClient _likeVideosClient;
        private readonly PlaylistsClient _playlistsClient;
        private readonly RequestsClient _requestsClient;
        private readonly YouTubeApiClient _youTubeApiClient;

        TelegramBotClient botClient;
        CancellationToken cancellationToken;
        ReceiverOptions receiverOptions;

        public Bot()
        {
            _likeVideosClient = new LikeVideosClient();
            _playlistsClient = new PlaylistsClient();
            _requestsClient = new RequestsClient();
            _youTubeApiClient = new YouTubeApiClient();
            botClient = new TelegramBotClient("5538327578:AAFlF5EVlJOsGxCicUBiweXj5E8PPt8NNQY");
            cancellationToken = new CancellationToken();
            receiverOptions = new ReceiverOptions { AllowedUpdates = { } };
        }

        public async Task Start()
        {

            botClient.StartReceiving(HandlerUpdateAsync, HandlerError, receiverOptions, cancellationToken);
            var botMe = await botClient.GetMeAsync();

            Console.WriteLine($"Bot {botMe.Username} start working");

        }

        private Task HandlerError(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
        {
            var ErrorMessage = exception switch
            {
                ApiRequestException apiRequestException => $"Error in API {apiRequestException.ErrorCode}\n" + $"{apiRequestException.Message}",
                _ => exception.ToString()
            };
            Console.WriteLine(ErrorMessage);
            return Task.CompletedTask;
        }

        private async Task HandlerUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
        {
            if (update.Type == UpdateType.Message && update?.Message?.Text != null)
            {
                await HandlerMessageAsync(botClient, update.Message);
            }

            if (update.Type == UpdateType.CallbackQuery)
            {
                await HandlerCallbackQueryAsync(botClient, update.CallbackQuery);
            }
        }

        private async Task HandlerCallbackQueryAsync(ITelegramBotClient botClient, CallbackQuery callbackQuery)
        {
            CallBackD callBack = JsonConvert.DeserializeObject<CallBackD>(callbackQuery.Data);
            switch (callBack.Do)
            {
                case "Like":
                    //    / YouTubeApi / videoinfo ? userId = qwerqw & userName = qwerqw & VideoId = CHekNnySAfM
                    try
                    {
                        var fullLikeVideoInfo = await _youTubeApiClient.GetLikeVideoInfo(callBack.Idv);

                        fullLikeVideoInfo.UserId = callbackQuery.From.Id.ToString();
                        fullLikeVideoInfo.UserName = callbackQuery.From.Username;

                        if (await _likeVideosClient.PostLikeVideo(fullLikeVideoInfo))
                        {
                            await botClient.SendTextMessageAsync(callbackQuery.Message.Chat.Id, "Added to Likes");
                        }
                        else
                        {
                            await botClient.SendTextMessageAsync(callbackQuery.Message.Chat.Id, "Not added to Likes");
                        }
                    }
                    catch
                    {
                        Console.WriteLine("Like");
                    }

                    return;
                case "AddToP":

                    try
                    {
                        var userPlaylists = await _playlistsClient.GetUserPlaylists(callbackQuery.From.Id.ToString());

                        var playlistsButtons = new List<List<InlineKeyboardButton>> { };

                        foreach (var playlists in userPlaylists)
                        {
                            CallBackD AddToUserPCallBack = new CallBackD()//dell Idv
                            {
                                Idp = playlists.Id,
                                Idv = callBack.Idv,
                                Do = "AddToUserP"
                            };
                            var AddToUserPJson = JsonConvert.SerializeObject(AddToUserPCallBack);

                            playlistsButtons.Add(new List<InlineKeyboardButton>
                        {
                            InlineKeyboardButton.WithCallbackData(playlists.PlaylistName,AddToUserPJson),
                        }
                            );
                        }
                        CallBackD AddNewPlaylistCallBack = new CallBackD()//dell Idv, Idp
                        {

                            Do = "AddPlaylist"
                        };
                        var AddNewPlaylistJson = JsonConvert.SerializeObject(AddNewPlaylistCallBack);
                        playlistsButtons.Add(new List<InlineKeyboardButton>
                        {
                            InlineKeyboardButton.WithCallbackData("Add playlist",AddNewPlaylistJson),
                        }
                        );

                        InlineKeyboardMarkup chosePlaylistMarkup = new InlineKeyboardMarkup
                            (
                            playlistsButtons
                            );

                        Console.WriteLine("Chose Playlist");

                        await botClient.SendTextMessageAsync(callbackQuery.Message.Chat.Id, "Chose Playlist", replyMarkup: chosePlaylistMarkup);

                    }
                    catch
                    {
                        Console.WriteLine("AddToP");
                    }
                    return;

                case "AddPlaylist":
                    try
                    {
                        await _requestsClient.PostRequest(new UserRequest()
                        {
                            UserId = callbackQuery.From.Id.ToString(),
                            UserName = callbackQuery.From.Username,
                            Request = "@AddPlaylist",
                            Time = callbackQuery.Message.Date.AddHours(3).ToString()
                        });

                        Console.WriteLine($"User {callbackQuery.From} AddPlaylist");
                    }
                    catch
                    {
                        BotMethods.DBReuestsAddingError(new UserRequest()
                        {
                            UserId = callbackQuery.From.Id.ToString(),
                            UserName = callbackQuery.From.Username,
                            Request = "@AddPlaylist",
                            Time = callbackQuery.Message.Date.AddHours(3).ToString()
                        });
                    }

                    //---
                    await botClient.SendTextMessageAsync(callbackQuery.Message.Chat.Id, "Type Playlist Name");

                    return;
                case "AddToUserP":

                    try
                    {
                        var userPlaylist = await _playlistsClient.GetPlaylistsById(callBack.Idp);

                        var fullLikeVideoInfo = await _youTubeApiClient.GetLikeVideoInfo(callBack.Idv);

                        await _playlistsClient.DelatePlaylist(userPlaylist.Id);

                        if (userPlaylist.VideoIds.Count<30)
                        {
                            userPlaylist.VideoIds.Add(fullLikeVideoInfo.VideoId);
                            userPlaylist.VideoTitles.Add(fullLikeVideoInfo.VideoTitle);
                        }
                        else
                        {
                            await botClient.SendTextMessageAsync(callbackQuery.Message.Chat.Id, "Can not add more than 30 videos");
                            return;
                        }

                        if (await _playlistsClient.PostPlaylist(userPlaylist))
                        {
                            await botClient.SendTextMessageAsync(callbackQuery.Message.Chat.Id, "Added to "+ userPlaylist.PlaylistName);
                        }
                        else
                        {
                            await botClient.SendTextMessageAsync(callbackQuery.Message.Chat.Id, "Not added to " + userPlaylist.PlaylistName);
                        }
                    }
                    catch
                    {
                        Console.WriteLine("AddToUserP");
                    }

                    return;
                case "ShowPToUser":
                    try
                    {
                        var userPlaylist = await _playlistsClient.GetPlaylistsById(callBack.Idp);

                        var userPlaylistButtons = new List<List<InlineKeyboardButton>> { };

                        CallBackD deletePlaylistCallBack = new CallBackD()
                        {
                            Idp = callBack.Idp,
                            Do = "DellP"
                        };
                        var deletePlaylistJson = JsonConvert.SerializeObject(deletePlaylistCallBack);

                        userPlaylistButtons.Add(new List<InlineKeyboardButton>
                        {
                            InlineKeyboardButton.WithCallbackData("Delete playlist",deletePlaylistJson)
                        }
                       );
                       


                        if (userPlaylist.VideoIds.Count>0)
                        {
                            for (int i = 0; i < userPlaylist.VideoIds.Count; i++)
                            {

                                userPlaylistButtons.Add(new List<InlineKeyboardButton>
                        {
                            InlineKeyboardButton.WithUrl($"{userPlaylist.VideoTitles[i]}", @"https://www.youtube.com/watch?v=" + userPlaylist.VideoIds[i])
                        }
                                );
                                var likeVideoFromPlaylistCallBack = new CallBackD()
                                {
                                    Idv = userPlaylist.VideoIds[i],
                                    Do = "Like"
                                };
                                var likeVideoFromPlaylistJson = JsonConvert.SerializeObject(likeVideoFromPlaylistCallBack);
                                CallBackD removeVideoFromPlaylistCallBack = new CallBackD()
                                {
                                    Idv = userPlaylist.VideoIds[i],
                                    Idp = userPlaylist.Id,
                                    Do = "Remove"
                                };
                                var removeVideoFromPlaylistJson = JsonConvert.SerializeObject(removeVideoFromPlaylistCallBack);

                                userPlaylistButtons.Add(new List<InlineKeyboardButton>
                        {
                            InlineKeyboardButton.WithCallbackData("Like",likeVideoFromPlaylistJson),
                            InlineKeyboardButton.WithCallbackData("Remove",removeVideoFromPlaylistJson)
                        }
                                );
                            }
                        }

                        var deletePlaylistMarkup = new InlineKeyboardMarkup
                            (
                            userPlaylistButtons
                            );
                        await botClient.SendTextMessageAsync(callbackQuery.Message.Chat.Id, userPlaylist.PlaylistName, replyMarkup: deletePlaylistMarkup);

                    }
                    catch
                    {
                        Console.WriteLine("ShowPToUser");
                    }


                    return;

                case "Remove":
                        try
                        {
                            var userPlaylist = await _playlistsClient.GetPlaylistsById(callBack.Idp);

                            await _playlistsClient.DelatePlaylist(userPlaylist.Id);

                            int removeVideoIndex = userPlaylist.VideoIds.IndexOf(callBack.Idv);

                            userPlaylist.VideoIds.RemoveAt(removeVideoIndex);
                            userPlaylist.VideoTitles.RemoveAt(removeVideoIndex);

                            if (await _playlistsClient.PostPlaylist(userPlaylist))
                            {
                                await botClient.SendTextMessageAsync(callbackQuery.Message.Chat.Id, "Video removed");
                            }
                            else
                            {
                                await botClient.SendTextMessageAsync(callbackQuery.Message.Chat.Id, "Video not removed");
                            }
                        }
                        catch
                        {
                            Console.WriteLine("Remove");
                        }
                    return;

                case "DellP":
                    try
                    {
                        if (await _playlistsClient.DelatePlaylist(callBack.Idp))
                        {
                            await botClient.SendTextMessageAsync(callbackQuery.Message.Chat.Id, "Playlist removed");
                        }
                        else
                        {
                            await botClient.SendTextMessageAsync(callbackQuery.Message.Chat.Id, "Playlist not removed");
                        }
                    }
                    catch
                    {
                        Console.WriteLine("DellP");
                    }

                    return;

                case "UnLike":
                    try
                    {
                        if (await _likeVideosClient.DelateLikeVideo(callbackQuery.From.Id.ToString(), callBack.Idv))
                        {
                            await botClient.SendTextMessageAsync(callbackQuery.Message.Chat.Id, "Removed from likes");
                        }
                        else
                        {
                            await botClient.SendTextMessageAsync(callbackQuery.Message.Chat.Id, "Not removed from likes");
                        }
                    }
                    catch
                    {
                        Console.WriteLine("UnLike");
                    }
                    
                    return;
            }

        }

        private async Task HandlerMessageAsync(ITelegramBotClient botClient, Message message)
        {
            switch (message.Text)
            {
                case "/start":

                    try
                    {
                        await _requestsClient.PostRequest(new UserRequest(message));
                        Console.WriteLine("User {0} {1}", message.From, message.Text);
                    }
                    catch
                    {
                        BotMethods.DBReuestsAddingError(new UserRequest(message));
                    }
                    //---
                    try
                    {
                        await botClient.SendTextMessageAsync(message.Chat.Id, "Chose command:");

                        await botClient.SendTextMessageAsync(message.Chat.Id, "/keyboard");

                        await botClient.SendTextMessageAsync(message.Chat.Id, "/help");
                    }
                    catch { Console.WriteLine("start"); }
                    return;

                case "/keyboard":
                    try
                    {
                        await _requestsClient.PostRequest(new UserRequest(message));
                        Console.WriteLine("User {0} {1}", message.From, message.Text);
                    }
                    catch
                    {
                        BotMethods.DBReuestsAddingError(new UserRequest(message));
                    }

                    //---
                    try
                    {
                        ReplyKeyboardMarkup replyKeyboardMarkup =
                            new
                            (
                            new[]
                            {
                                    new KeyboardButton[] { "Search", "Artist" },
                                    new KeyboardButton[] { "Trend", "Genres" },
                                    new KeyboardButton[] { "Likes", "Playlists", "Friends" }
                            }
                            )
                            {
                                ResizeKeyboard = true
                            };

                        await botClient.SendTextMessageAsync(message.Chat.Id, "Chose option", replyMarkup: replyKeyboardMarkup);
                    }
                    catch
                    {
                        Console.WriteLine("keyboard");
                    }

                    return;
                case "/help":
                    try
                    {
                        await _requestsClient.PostRequest(new UserRequest(message));
                        Console.WriteLine("User {0} {1}", message.From, message.Text);
                    }
                    catch
                    {
                        BotMethods.DBReuestsAddingError(new UserRequest(message));
                    }
                    //---
                    try
                    {
                        await botClient.SendTextMessageAsync(message.Chat.Id, $"Hi, {message.From.Username}!\n" +
                            $"I fetch tracks from YouTube!" +
                            $"\nUse:" +
                            $"\n/keyboard to open a keyboard ⌨️‍" +
                            $"\nSearch to receive an ordinary YouTube request 🔍" +
                            $"\nArtist to get all popular songs of the artist(start your request with * to get links) 🤵🏻" +
                            $"\nTrend to get today made music 📈" +
                            $"\nGenres to sort music by genres 🎸" +
                            $"\nLikes to view Liked songs ❤️" +
                            $"\nPlaylists to view your Playlists 🎧" +
                            $"\nFriends to see Likes of someone with similar musical taste 🙏🏻");
                    }
                    catch
                    {
                        Console.WriteLine("help");
                    }
                    return;

                case "Search":
                    try
                    {
                        await _requestsClient.PostRequest(new UserRequest(message, true));
                        Console.WriteLine("User {0} {1}", message.From, "@" + message.Text);
                    }
                    catch
                    {
                        BotMethods.DBReuestsAddingError(new UserRequest(message));
                    }
                    try
                    {
                        await botClient.SendTextMessageAsync(message.Chat.Id, "Type Request");
                    }
                    catch
                    {
                        Console.WriteLine("Search");
                    }
                    return;
                case "Trend":
                    try
                    {
                        await _requestsClient.PostRequest(new UserRequest(message));
                        Console.WriteLine("User {0} {1}", message.From, message.Text);
                    }
                    catch
                    {
                        BotMethods.DBReuestsAddingError(new UserRequest(message));
                    }
                    //---
                    try
                    {
                        var trendingMusic = await _youTubeApiClient.GetTrendingMusic();

                        if(trendingMusic.Count>0)
                        await botClient.SendTextMessageAsync(message.Chat.Id, "Today music", replyMarkup: BotMethods.GetSongsMarkup(trendingMusic));
                    }
                    catch
                    {
                        Console.WriteLine("Trend");
                    }

                    return;



                case "Artist":
                    try
                    {
                        await _requestsClient.PostRequest(new UserRequest(message, true));
                        Console.WriteLine("User {0} {1}", message.From, "@" + message.Text);
                    }
                    catch
                    {
                        BotMethods.DBReuestsAddingError(new UserRequest(message));
                    }
                    //---
                    try
                    {
                        await botClient.SendTextMessageAsync(message.Chat.Id, "Type Artist Name");
                    }
                    catch
                    {
                        Console.WriteLine("Artist");
                    }
                    return;

                case "Friends":
                    try
                    {
                        await _requestsClient.PostRequest(new UserRequest(message));
                        Console.WriteLine("User {0} {1}", message.From, message.Text);
                    }
                    catch
                    {
                        BotMethods.DBReuestsAddingError(new UserRequest(message));
                    }
                    //---

                    try
                    {
                        var likemates = await _likeVideosClient.GetLikeMates(message.From.Id.ToString());

                        var likematesUserNames = new List<string> { };

                        var likematesMarkups = BotMethods.GetLikeMatesMarkups(likemates, out likematesUserNames);

                        if (likematesMarkups.Count>0)
                        {
                            for (int i = 0; i < likematesMarkups.Count; i++)
                            {
                                await botClient.SendTextMessageAsync(message.Chat.Id, likematesUserNames[i], replyMarkup: likematesMarkups[i]);
                            }
                        }
                        else
                        {
                            await botClient.SendTextMessageAsync(message.Chat.Id, "You have no Likemates(");
                        }

                    }
                    catch
                    {
                        Console.WriteLine("Friends");
                    }


                    return;


                case "Likes":
                    try
                    {
                        await _requestsClient.PostRequest(new UserRequest(message));
                        Console.WriteLine("User {0} {1}", message.From, message.Text);
                    }
                    catch
                    {
                        BotMethods.DBReuestsAddingError(new UserRequest(message));
                    }
                    //---

                    try
                    {
                        var userLikes = await _likeVideosClient.GetUserLikesVideo(message.From.Id.ToString());

                        if (userLikes.Count > 0)
                        {

                            var buttonsWithUnlike = new List<List<InlineKeyboardButton>> { };

                            for (int i = 0; i < userLikes.Count; i++)
                            {
                                buttonsWithUnlike.Add(new List<InlineKeyboardButton>
                        {
                            InlineKeyboardButton.WithUrl($"{userLikes[i].VideoTitle}", @"https://www.youtube.com/watch?v=" + userLikes[i].VideoId)
                        }
                                );
                                CallBackD unlikeCallBack = new CallBackD()
                                {
                                    Idv = userLikes[i].VideoId,
                                    Do = "UnLike"
                                };
                                var unlikeJson = JsonConvert.SerializeObject(unlikeCallBack);
                                CallBackD AddToPCallBack = new CallBackD()
                                {
                                    Idv = userLikes[i].VideoId,
                                    Do = "AddToP"
                                };
                                var AddToPJson = JsonConvert.SerializeObject(AddToPCallBack);

                                buttonsWithUnlike.Add(new List<InlineKeyboardButton>
                        {
                            InlineKeyboardButton.WithCallbackData("DisLike",unlikeJson),
                            InlineKeyboardButton.WithCallbackData("Add to playlist",AddToPJson)
                        }
                                );
                                Console.WriteLine(userLikes[i].VideoTitle);

                            }


                            var likesMarkup = new InlineKeyboardMarkup
                                (
                                buttonsWithUnlike
                                );
                            await botClient.SendTextMessageAsync(message.Chat.Id, "Liked videos", replyMarkup: likesMarkup);

                        }
                        else
                        {
                            await botClient.SendTextMessageAsync(message.Chat.Id, "You have no Likes(");
                        }
                    }
                    catch
                    {
                        Console.WriteLine("Likes");
                    }

                    return;

                case "Playlists":
                    try
                    {
                        await _requestsClient.PostRequest(new UserRequest(message));
                        Console.WriteLine("User {0} {1}", message.From, message.Text);
                    }
                    catch
                    {
                        BotMethods.DBReuestsAddingError(new UserRequest(message));
                    }

                    //---

                    try
                    {
                        var userPlaylists = await _playlistsClient.GetUserPlaylists(message.From.Id.ToString());

                        if (userPlaylists.Count > 0)
                        {
                            await botClient.SendTextMessageAsync(message.Chat.Id, "Playlists", replyMarkup: BotMethods.GetPlaylistsMarkup(userPlaylists));
                        }
                        else
                        {
                            await botClient.SendTextMessageAsync(message.Chat.Id, "You have no Playlists(");
                        }
                    }
                    catch
                    {
                        Console.WriteLine("Playlists");
                    }
                   
                    return;

                case "Genres":
                    try
                    {
                        await _requestsClient.PostRequest(new UserRequest(message));
                        Console.WriteLine("User {0} {1}", message.From, message.Text);
                    }
                    catch
                    {
                        BotMethods.DBReuestsAddingError(new UserRequest(message));
                    }
                    //---

                    try
                    {
                        var genres = await _youTubeApiClient.GetSerchByGenres();
                       
                        if (genres.Count != 0)
                        {

                            for (int j = 0; j < genres.Count; j++)
                            {
                                var genresMarkup =  BotMethods.GetSongsMarkup(genres[j]);
                                switch (j)
                                {
                                    case 0:
                                        await botClient.SendTextMessageAsync(message.Chat.Id, "Classical", replyMarkup: genresMarkup);
                                        break;
                                    case 1:
                                        await botClient.SendTextMessageAsync(message.Chat.Id, "Pop", replyMarkup: genresMarkup);
                                        break;
                                    case 2:
                                        await botClient.SendTextMessageAsync(message.Chat.Id, "Country", replyMarkup: genresMarkup);
                                        break;
                                    case 3:
                                        await botClient.SendTextMessageAsync(message.Chat.Id, "Reggae", replyMarkup: genresMarkup);
                                        break;
                                    case 4:
                                        await botClient.SendTextMessageAsync(message.Chat.Id, "Rock", replyMarkup: genresMarkup);
                                        break;
                                    case 5:
                                        await botClient.SendTextMessageAsync(message.Chat.Id, "Jazz", replyMarkup: genresMarkup);
                                        break;
                                    case 6:
                                        await botClient.SendTextMessageAsync(message.Chat.Id, "Hip hop", replyMarkup: genresMarkup);
                                        break;
                                    case 7:
                                        await botClient.SendTextMessageAsync(message.Chat.Id, "Electronic", replyMarkup: genresMarkup);
                                        break;
                                }

                            }
                        }
                        else
                        {
                            await botClient.SendTextMessageAsync(message.Chat.Id, "Not today");
                            Console.WriteLine("Genres");
                        }

                    }
                    catch
                    {
                        Console.WriteLine("Genres");
                    }

                    return;

            }

            string Mode = await _requestsClient.GetLastUserMode(message.From.Id.ToString());

            switch (Mode)
            {
                case "@Search":
                    try
                    {
                        await _requestsClient.PostRequest(new UserRequest(message));
                        Console.WriteLine("User {0} {1}", message.From, message.Text);
                    }
                    catch
                    {
                        BotMethods.DBReuestsAddingError(new UserRequest(message));
                    }
                    //---
                    try
                    {
                        var serchResult = await _youTubeApiClient.GetSerchByVideoRequest(message.Text);

                        if (serchResult.Count != 0)
                        {
                            await botClient.SendTextMessageAsync(message.Chat.Id, "Music", replyMarkup: BotMethods.GetSongsMarkup(serchResult));
                        }
                        else
                        {
                            await botClient.SendTextMessageAsync(message.Chat.Id, "No result, say in ather words");
                        }
                    }
                    catch
                    {
                        Console.WriteLine("@Serch");
                    }
                    
                    return;


                case "@AddPlaylist":

                    try
                    {
                        Playlist playlist = new Playlist()
                        {

                            UserId = message.From.Id.ToString(),
                            UserName = message.From.Username,
                            PlaylistName = message.Text,
                            VideoIds = new List<string> { },
                            VideoTitles = new List<string> { }
                        };

                        bool playlistAdded = await _playlistsClient.PostPlaylist(playlist);
                        if (playlistAdded)
                        {
                            await botClient.SendTextMessageAsync(message.Chat.Id, "Playlist added");
                        }
                        else
                        {
                            await botClient.SendTextMessageAsync(message.Chat.Id, "Playlist not added, choose another name");
                        }
                    }
                    catch
                    {
                        Console.WriteLine("@AddPlaylist");
                    }
                    

                    return;

                case "@Artist":
                    try
                    {
                        await _requestsClient.PostRequest(new UserRequest(message));
                        Console.WriteLine("User {0} {1}", message.From, message.Text);
                    }
                    catch
                    {
                        BotMethods.DBReuestsAddingError(new UserRequest(message));
                    }

                    //---

                    try
                    {
                        string arrt = "";
                        if (message.Text[0] == '*')
                        {
                            for (int i = 1; i < message.Text.Length; i++)
                            {
                                arrt += message.Text[i];
                            }
                        }
                        else
                        {
                            arrt = message.Text;
                        }

                        var artistBestVideos = await _youTubeApiClient.GetSerchByArtist(arrt);

                        if (artistBestVideos.Count > 0)
                        {
                            var firstTen = new List<Models.Video> { };

                            for (int i = 0; i < 10; i++)
                            {
                                if (i< artistBestVideos.Count)
                                {
                                    firstTen.Add(artistBestVideos[i]);
                                }
                            }

                            await botClient.SendTextMessageAsync(message.Chat.Id, "Best artist music", replyMarkup: BotMethods.GetSongsMarkup(firstTen));

                            if (message.Text[0] == '*')
                            {
                                string WhatToCopy = "";
                                for (int i = 0; i < (artistBestVideos.Count > 25 ? 25 : artistBestVideos.Count); i++)
                                {
                                    WhatToCopy += $"https://www.youtube.com/watch?v={artistBestVideos[i].VideoId} \n";
                                    if (i % 5 == 4)
                                    {
                                        await botClient.SendTextMessageAsync(message.Chat.Id, WhatToCopy);
                                        WhatToCopy = "";
                                    }
                                }
                            }

                        }
                        else
                        {
                            await botClient.SendTextMessageAsync(message.Chat.Id, "No result, say in ather words");
                        }

                    }
                    catch
                    {
                        Console.WriteLine("@Artist");
                    }

                    return;
            }
        }
    }
}
