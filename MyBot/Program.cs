using MyBot.Clients;
using MyBot.DBapi;
using MyBot.DBapi.Clients;
using MyBot.DBUse;
using MyBot.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using static System.Net.Mime.MediaTypeNames;

namespace MyBot
{
    internal class Program
    {
        static  void Main(string[] args)
        {

            Bot tGBot = new Bot();
            tGBot.Start();
            while (true)
            {

            }

            //Console.WriteLine("Hello World!");
            /*
            DBConnection connection = new DBConnection("D:\\Code\\СourseWork\\MyBot\\MyBot\\MyBot\\DB\\Requests.txt");

            for (int i = 0; i < 10; i++)
            {
                //var a = connection.PostData(new List<string> { "a", "b", "c" }).Result;
            }
            //var d = connection.DelateDataById("1").Result;

            //foreach (var line in connection.GetAllData().Result)
            //{
            //    foreach (var field in line)
            //    {
            //        Console.Write(field + "\t");
            //    }
            //    Console.WriteLine();
            //}

            //foreach (var field in connection.GetDataById("12").Result)
            //{
            //    Console.Write(field + "\t");
            //}
            //Console.WriteLine();


              UserRequest request = new UserRequest()
            {
                Id = "32",
                UserId = "H",
                UserName = "bSH",
                Request = "cSD",
                Time = "dVH",
            };

            string dir = System.AppContext.BaseDirectory;

            Console.WriteLine(dir);
            Console.WriteLine(Constants.MyBotDirectory);


            //var post = Convertor.FromUserRequestToString(request);

            ////for (int i = 0; i < 4; i++)
            ////{
            ////    var a = connection.PostData(post).Result;
            ////}

            //foreach (var item in connection.GetAllDataWithField((int)UserRequest.Fields.UserId, "a").Result) 
            //        { 
            //    foreach (var field in item)
            //    {
            //        Console.Write(field);
            //    }
            //    Console.WriteLine();
            //}  
            */

            //RequestsClient requestsClient = new RequestsClient();

            //UserRequest userRequest = new UserRequest()
            //{
            //    Request = "@sheeeesh",
            //    Time = DateTime.Now.ToString(),
            //    UserId = "1111",
            //    UserName = "theard"
            //};                                                                                   ЮНИТ ТЕСТЫ ДЛЯ ЮЗЕР РЕКВЕСТ КЛАЕНТ     

            ////var r = requestsClient.PostData(userRequest).Result;

            //var mode = requestsClient.GetLastUserMode("1111").Result;

            //Console.WriteLine(mode);



            //LikeVideosClient client = new LikeVideosClient();

            //LikeVideo like = new LikeVideo()
            //{
            //    UserId = "2222",
            //     UserName = "two",
            //     VideoId = "fasdfasdfgas",
            //     VideoTitle ="gqasg34g13",
            //     ChannelId ="asgasdbqw",
            //     ChannelTitle = "35h24h5rasr",
            //};

            ////var post = client.PostDate(like).Result;                                    ЮНИТ ТЕСТЫ ДЛЯ ЛАЙК ВИДЕО КЛАЕНТ

            //var userL = client.GetUserLikes("1111").Result;

            //var all = client.GetAll().Result;
            //foreach (var likeVideo in userL)
            //{
            //    Console.WriteLine(likeVideo.UserId+ "\t"+ likeVideo.VideoId);
            //}           

            //1111    asgasdgq

            //var d = client.DelateLikeVideo("1111", "asgasdgq").Result;

            //PlaylistsClient playlistsClient = new PlaylistsClient();

            //Playlist playlist = new Playlist()
            //{
            //    UserId = "2222",
            //    UserName = "two",
            //    PlaylistName = "foots",
            //    VideoIds = new List<string> { "asgasdbqw", "a;osigqwe",  "aghqwooirohui" },
            //    VideoTitles = new List<string> { "35h24h5rasr", "gqoiewgjqe[", "ohi[o[o[wort" },
            //};

            //Stopwatch stopwatch = new Stopwatch();                                                ЮНИТ ТЕСТЫ ПЛЕЙЛИСТ

            //stopwatch.Start();
            //for (int i = 0; i < 10; i++)
            //{
            //    var d = playlistsClient.DelateVideoFromList("2222", "foots", "asgasdbqw").Result;
            //}
            //stopwatch.Stop();

            //Console.WriteLine("it takes to: {0}", stopwatch.ElapsedMilliseconds);


            //YouTubeApiClient client = new YouTubeApiClient();
            //var request = client.GetSerchByVideoRequest("marmok").Result;
            //foreach (var item in request)
            //{
            //    Console.WriteLine($"{item.VideoId} - {item.VideoTitle}");   
            //}

            //Console.WriteLine();    
            //var r = client.GetSerchByArtist("marmok").Result;
            //Console.WriteLine("marmok");

            //foreach (var item in r)
            //{
            //    Console.WriteLine($"https://www.youtube.com/watch?v={item.VideoId} - {item.VideoTitle}");     
            //}

            //var res = client.GetLikeVideoInfo("pyjppt0gMII").Result;

            //Console.WriteLine($"{res.VideoId} - {res.VideoTitle}");

            //var t = client.GetTrendingMusic().Result;
            //var g = client.GetSerchByGenres().Result;
            //var corbat = client.GetSerchByArtist("корбен").Result;

            //foreach (var item in corbat)
            //{
            //    Console.WriteLine($"https://www.youtube.com/watch?v={item.VideoId} - {item.VideoTitle}");
            //}

            //LikeVideosClient likeVideosClient = new LikeVideosClient();

            //var f = likeVideosClient.GetLikeMates("1111").Result;
            //Console.WriteLine("dasfqweg q2wgqw34gbq3w4g");

            //foreach (var item in f)
            //{
            //    foreach (var video in item)
            //    {
            //        Console.WriteLine($"https://www.youtube.com/watch?v={video.VideoId} - {video.VideoTitle}");
            //    }

            //}


            //Console.WriteLine("done");

        }
    }
}
