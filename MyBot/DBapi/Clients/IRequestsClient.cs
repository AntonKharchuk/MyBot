using MyBot.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyBot.DBapi.Clients
{
    internal interface IRequestsClient
    {
       // public Task<UserRequest> GetData(string Id);

        public Task<string> GetLastUserMode(string userId);


        public Task<bool> PostRequest(UserRequest userRequest);


        public Task<List<UserRequest>> GetAll();
    }
}
