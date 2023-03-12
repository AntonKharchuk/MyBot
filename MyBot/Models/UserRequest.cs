using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot.Types;

namespace MyBot.Models
{
    public class UserRequest
    {
        public enum Fields
        {
            Id,
            UserId,
            UserName,
            Request,
            Time
        }

        public UserRequest() { }
        public UserRequest(Message message, bool IsMode=false)
        {
            UserId = message.From.Id.ToString();
            UserName = message.From.Username;
            
            Request = IsMode? "@"+message.Text:message.Text;

            Time = message.Date.AddHours(3).ToString();
        }

        public string Id { get; set; }
        public string UserId { get; set; }
        public string UserName { get; set; }
        public string Request { get; set; }
        public string Time { get; set; }
    }
}
