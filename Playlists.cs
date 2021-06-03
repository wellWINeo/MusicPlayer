using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using MusicPlayerApi;

namespace MusicPlayer
{
    public class Playlists
    {
        public Client client;
        public ObservableCollection<Playlist> playlists = new ObservableCollection<Playlist>();
        public Playlists(Client client)
        {
            this.client = client;
            Task.Factory.StartNew(() => PlaylistsUpdater());
        }

        public void PlaylistsUpdater()
        {
            var playlists = new List<Playlist>();
            try
            {
                playlists = client.GetUsersPlaylists();
            } catch (WebException err)
            {
                MessageBox.Show(err.Message);
            }

            foreach(Playlist pl in playlists)
            {
                this.playlists.Add(pl);
            }
        }
    }
}
