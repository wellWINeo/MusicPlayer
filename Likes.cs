using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Threading;

using MusicPlayerApi;

namespace MusicPlayer
{
    public class Likes
    {
        public ObservableCollection<Track> likes = new ObservableCollection<Track>();
        public Client client;
        
        public Likes(Client client)
        {
            this.client = client;
            Task.Factory.StartNew(() => LikesUpdater());
        }


        public void LikesUpdater()
        {
            var likesList = new List<Track>();
            var likesId = new List<int>();
            try
            {
                likesId = client.GetAllLikes();
            } catch (WebException err)
            {
                MessageBox.Show(err.Message);
            }

            foreach(var id in likesId)
            {
                try
                {
                    likesList.Add(client.GetTrack(id));
                } catch (WebException err)
                {
                    MessageBox.Show(err.Message);
                }
            }

            foreach (Track track in likesList)
            {
                this.likes.Add(track);
            }

            Thread.Sleep(10000);
        }
    }
}
