using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Collections.ObjectModel;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;

using MusicPlayerApi;
using System.Net;
using System.Windows;

namespace MusicPlayer
{
    public class History
    {
        public ObservableCollection<MusicPlayerApi.History> HistoryList = 
            new ObservableCollection<MusicPlayerApi.History>();
        public Client client;
        
        public History(Client client)
        {
            this.client = client;
            Task.Factory.StartNew(() => HistoryUpdater());
        }

        public void Write(MusicPlayerApi.History track)
        {
            this.HistoryList.Add(track);

        }

        public void HistoryUpdater()
        {
            List<MusicPlayerApi.History> history = new List<MusicPlayerApi.History>();
            try
            {
                history = client.GetHistory();
            } catch (WebException e)
            {
                MessageBox.Show(e.Message);
            }
            
            foreach (MusicPlayerApi.History record in history)
            {
                this.HistoryList.Add(record);
            }
            Thread.Sleep(10000);
        }

    }
}
