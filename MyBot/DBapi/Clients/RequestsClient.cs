using MyBot.DBUse;
using MyBot.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyBot.DBapi.Clients
{
    internal class RequestsClient : IRequestsClient
    {
        private readonly string _dBPath;
        private DBConnection _dBConnection;
        public RequestsClient()
        {
            _dBPath = Constants.MyBotDirectory + @"DB\Requests.txt";
            _dBConnection = new DBConnection(_dBPath);
        }
        public async Task<List<UserRequest>> GetAll()
        {
            var content = await _dBConnection.GetAllData();
            var result = new List<UserRequest>();
            foreach (var userRequest in content)
            {
                result.Add(Convertor.FromStringToUserRequest(userRequest));
            }
            return result;
        }

        public async Task<string> GetLastUserMode(string userId)
        {
            string result = null;
            var allUserRaqestsString = await _dBConnection.GetAllDataWithField((int)UserRequest.Fields.UserId, userId);
            var allUserRaqests = new List<UserRequest> { };
            foreach (var line in allUserRaqestsString)
            {
                allUserRaqests.Add(Convertor.FromStringToUserRequest(line));
            }
            for (int i = allUserRaqests.Count-1; i >=0 ; i--)
            {
                if (allUserRaqests[i].Request.StartsWith("@"))
                {
                    result = allUserRaqests[i].Request;
                    break;
                }
            }
            return result;
        }

        public async Task<bool> PostRequest(UserRequest userRequest)
        {
            return await _dBConnection.PostData(Convertor.FromUserRequestToString(userRequest));
        }
    }
}
