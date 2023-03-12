using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;


namespace MyBot.DBUse
{
    public class DBConnection : IDBConnection
    {
        private  FileInfo _dB { get; set; }
        private  int  _nextId { get; set; }

        public DBConnection(string dBPath) 
        { 
            _dB = new FileInfo(dBPath);

            var allData = GetAllData().Result;
            if (allData == null||allData.Count==0) { _nextId = 0; }
            else
            {
                _nextId = int.Parse(allData.Last().First())+1;
            }
        }
        public async Task<bool> DelateDataById(string Id)
        {
            var allLines = await GetAllData();
            for (int i = 0; i < allLines.Count; i++)
            {
                if (allLines[i][0] == Id)
                {
                    allLines.Remove(allLines[i]);
                    
                }
            }
            _dB.Delete();
            _dB.Create().Close();
            foreach (var line in allLines)
            {
                await PostDataInternal(line);
            }
            return true;
        }

        public async Task<List<List<string>>> GetAllData()
        {
            List<List<string>> result = new List<List<string>>();

            using (StreamReader sr = _dB.OpenText())
            {
                string line;
                while(true) {  
                    line = await sr.ReadLineAsync();
                    if (line == null)
                    {
                        break;
                    }
                    List<string> lineData = new List<string>( line.Split(new string[] {"\t", "\n" }, StringSplitOptions.RemoveEmptyEntries));

                    result.Add(lineData);
                } 
            }
            return result;

        }

        

        public async Task<List<List<string>>> GetAllDataWithField(int Field, string value)
        {
            List<List<string>> result = new List<List<string>> { };

            using (StreamReader sr = _dB.OpenText())
            {

                string line = string.Empty;
                while (true)
                {
                    line = await sr.ReadLineAsync();
                    if (line == null)
                        break;
                    var content = new List<string>(line.Split(new string[] { "\t", "\n" }, StringSplitOptions.RemoveEmptyEntries));
                    if (content[Field] == value)
                    {
                        result.Add(content);
                    }

                }
            }
            return result;
        }

        private async Task<bool> PostDataInternal(List<string> data)
        {
            if (data == null|| data.Count==0)
            {
               return false;
            }
            using (StreamWriter sw =  new StreamWriter(_dB.FullName,true))
            {
                string line = string.Empty;
                foreach (var field in data)
                {
                    line+=(field+"\t");
                }
                await sw.WriteLineAsync(line);
            }
            return true;
        }

        public async Task<bool> PostData(List<string> data)
        {
            if (data == null || data.Count == 0)
            {
                return false;
            }
            using (StreamWriter sw = new StreamWriter(_dB.FullName, true))
            {
                string line = _nextId.ToString() + "\t";
                _nextId++;
                for (int i = 1; i < data.Count; i++)
                {
                    if (data[i] == null)
                        line += (" " + "\t");
                    else line += data[i]+ "\t";    
                }
             
                await sw.WriteLineAsync(line);
            }
            return true;
        }

    }
}
