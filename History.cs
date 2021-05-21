using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace MusicPlayer
{
    public class History
    {
        public ObservableCollection<Node> HistoryList = new ObservableCollection<Node>();
        private string pathCommited, pathUncommited;
        public bool isOnline;
        
        public History(string path, string nameCommited = "commited",
                       string nameUncommited = "uncommited", bool isOnline = false)
        {
            this.pathCommited = Path.Combine(path, nameCommited);
            this.pathUncommited = Path.Combine(path, nameUncommited);

            if (!File.Exists(this.pathCommited))
                File.Create(this.pathCommited);

            if (!File.Exists(this.pathUncommited))
                File.Create(this.pathUncommited);

            StreamReader commitedReader = new StreamReader(this.pathCommited);
            StreamReader uncommitedReader = new StreamReader(this.pathUncommited);

            while (!commitedReader.EndOfStream)
            {
                string jsonString = commitedReader.ReadLine();
                this.HistoryList.Add(JsonSerializer.Deserialize<Node>(jsonString));
            }

            while (!uncommitedReader.EndOfStream)
            {
                string jsonString = uncommitedReader.ReadLine();
                this.HistoryList.Add(JsonSerializer.Deserialize<Node>(jsonString));
            }

            commitedReader.Dispose();
            uncommitedReader.Dispose();

        }

        public void Write(Node track)
        {
            this.HistoryList.Add(track);
            string json = JsonSerializer.Serialize<Node>(track);
            File.AppendAllLines(this.pathUncommited, new string[]{ json });
        }

        public async void Flush()
        {
            List<string> JsonList = new List<string>();
            using (StreamReader reader = new StreamReader(this.pathUncommited))
            {
                while (!reader.EndOfStream);
                    JsonList.Add(await reader.ReadLineAsync());
            }
            await File.AppendAllLinesAsync(this.pathCommited, JsonList.ToArray());
        }
    }
}
