using MusicPlayerApi;
using System.Collections.Generic;
using System.Windows;

namespace MusicPlayer
{
    /// <summary>
    /// Interaction logic for ChoosePlaylistWindow.xaml
    /// </summary>
    public partial class ChoosePlaylistWindow : Window
    {
        public List<Playlist> playlists;
        public string selectedTitle;
        public ChoosePlaylistWindow(List<Playlist> playlists)
        {
            this.playlists = playlists;
            InitializeComponent();
            foreach (var elem in playlists)
            {
                cmbBox.Items.Add(elem.title);
            }

        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            this.selectedTitle = (string)cmbBox.SelectedItem;
            this.Close();
        }

        public int GetPlaylistId()
        {
            foreach (var elem in this.playlists)
            {
                if (elem.title == this.selectedTitle)
                    return elem.playlist_id;
            }
            return 0;
        }
    }
}
