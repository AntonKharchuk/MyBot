using MyBot.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyBot.DBUse
{
    public interface IDBConnection 
    {
        public Task<List<List<string>>> GetAllDataWithField(int Field, string value);
        public Task<List<List<string>>> GetAllData();
        public Task<bool> DelateDataById(string Id);
        public Task<bool> PostData(List<string> data);
    }
}
