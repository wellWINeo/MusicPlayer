using Microsoft.Win32;
using MusicPlayerApi;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;

namespace MusicPlayer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        #region vars
        bool isPause = false;
        public bool isPremium = false;
        public Client client;
        public DispatcherTimer timer = new DispatcherTimer();
        public ObservableCollection<Track> TracksCollection = new ObservableCollection<Track>();
        public string MusicFolder = Environment.GetFolderPath(Environment.SpecialFolder.MyMusic);
        public ObservableCollection<Track> QueueList = new ObservableCollection<Track>();
        public ObservableCollection<Playlist> playlists = new ObservableCollection<Playlist>();
        public ObservableCollection<string> trackHistory = new ObservableCollection<string>();
        public ObservableCollection<Track> likes = new ObservableCollection<Track>();
        public int ChoosedPlaylist = 0;
        int TrackListIndex;

        #endregion

        public MainWindow()
        {

            this.client = new Client("", "192.168.122.1", 8000);

            Utils.Init(client);

            string historyDir = System.IO.Path.Combine(Directory.GetCurrentDirectory(), "history");
            if (!Directory.Exists(historyDir))
                Directory.CreateDirectory(historyDir);

            InitializeComponent();

            this.Updater();

            try
            {
                isPremium = client.GetMe().is_premium;
            }
            catch (WebException err)
            {
                MessageBox.Show(err.Message);
            }

            if (isPremium)
            {
                playerElement.Visibility = Visibility.Visible;
                BuyPremium1.Visibility = Visibility.Collapsed;
                BuyPremium2.Visibility = Visibility.Collapsed;
            }


            timer.Interval = TimeSpan.FromSeconds(0.1);
            timer.Tick += new EventHandler(timer_Tick);
            timer.IsEnabled = true;

            playerElement.LoadedBehavior = MediaState.Manual;

            var tracks = client.GetAllTracks();
            foreach (var track in tracks)
            {
                TracksCollection.Add(track);
            }
            TrackListIndex = 0;

        }

        private void timer_Tick(object sender, EventArgs e)
        {
            playerSlider.Value = playerElement.Position.TotalSeconds;
        }

        private void Logout(object sender, RoutedEventArgs e)
        {
            File.Delete(".token");
        }

        private void Updater()
        {
            try
            {
                TracksCollection = new ObservableCollection<Track>(client.GetAllTracks());
                var tmp = client.GetHistory();
                trackHistory = new ObservableCollection<string>();
                playlists = new ObservableCollection<Playlist>(client.GetUsersPlaylists());

                foreach (var t in tmp)
                {
                    var track = client.GetTrack(t.track_id);
                    var tmp_string = $"🕑 {track.title}, {track.artist} ({t.time})";
                    trackHistory.Add(tmp_string);
                }

                var likesList = new List<Track>();
                var likesId = new List<int>();
                try
                {
                    likesId = client.GetAllLikes();
                }
                catch (WebException err)
                {
                    MessageBox.Show(err.Message);
                }

                foreach (var id in likesId)
                {
                    try
                    {
                        likesList.Add(client.GetTrack(id));
                    }
                    catch (WebException err)
                    {
                        MessageBox.Show(err.Message);
                    }
                }

                likes = new ObservableCollection<Track>(likesList);
            }
            catch (WebException err)
            {
                MessageBox.Show(err.Message);
            }

            tracksView.ItemsSource = TracksCollection;
            nextListView.ItemsSource = QueueList;
            playlistsView.ItemsSource = playlists;
            historyListView.ItemsSource = trackHistory;
            likesView.ItemsSource = likes;
        }

        private void PlaylistBackClick(object sender, RoutedEventArgs e)
        {
            BackButton.Visibility = Visibility.Collapsed;
            playlistsView.ItemsSource = playlists;
        }

        private void PlaylistsDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (playlistsView.SelectedItem != null)
            {
                if (BackButton.Visibility == Visibility.Collapsed)
                {
                    var pl = (Playlist)playlistsView.SelectedItem;
                    try
                    {
                        playlistsView.ItemsSource = client.GetPlaylist(pl.playlist_id);
                    }
                    catch (WebException err)
                    {
                        MessageBox.Show(err.Message);
                    }
                    BackButton.Visibility = Visibility.Visible;
                    ChoosedPlaylist = pl.playlist_id;
                }
                else
                {
                    var track = (Track)playlistsView.SelectedItem;
                    if (QueueList.Count == 0)
                        QueueList.Add(track);
                    else
                        QueueList[TrackListIndex] = (Track)playlistsView.SelectedItem;
                    string path;
                    if (track.has_video)
                        path = System.IO.Path.Combine(MusicFolder, track.track_id.ToString() + ".mp3");
                    else
                        path = System.IO.Path.Combine(MusicFolder, track.track_id.ToString() + ".mp4");
                    if (track.track_id == 0) { return; }
                    try
                    {
                        this.client.DownloadTrack(track.track_id, path);
                    }
                    catch (WebException err)
                    {
                        MessageBox.Show(err.Message);
                    }
                    playerElement.Source = new Uri(path);
                }
            }
        }

        private void PlayerElementSourceUpdated(object sender, RoutedEventArgs e)
        {
            if (playerElement.HasAudio)
            {
                playerSlider.Maximum = playerElement.NaturalDuration.TimeSpan.TotalSeconds;
                playerSlider.Value = playerElement.Position.TotalSeconds;
                playerElement.LoadedBehavior = MediaState.Manual;
                playerElement.Play();
                playButton.Content = "||";
                if (!isPremium)
                {
                    var rand = new Random(DateTime.Now.Second);
                    var chance = rand.Next(5);
                    if (chance == 1)
                    {
                        MessageBox.Show("Yoe are still not premium user! Buy premium version ☹");
                    }
                }
            }
        }

        private void SliderValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (e.NewValue != e.OldValue && e.NewValue != playerElement.Position.TotalSeconds)
                playerElement.Position = TimeSpan.FromSeconds(e.NewValue);


        }

        private void PlayPauseClick(object sender, RoutedEventArgs e)
        {
            isPause = !isPause;
            if (isPause)
            {
                playerElement.LoadedBehavior = MediaState.Pause;
                playButton.Content = "▶️";
            }
            else
            {
                playerElement.LoadedBehavior = MediaState.Play;
                playButton.Content = "||";
            }
        }

        private void TrackDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (tracksView.SelectedItem != null)
            {
                var track = (Track)tracksView.SelectedItem;
                if (QueueList.Count == 0)
                    QueueList.Add(track);
                else
                    QueueList[TrackListIndex] = (Track)tracksView.SelectedItem;
                string path;
                if (track.has_video)
                    path = System.IO.Path.Combine(MusicFolder, track.track_id.ToString() + ".mp3");
                else
                    path = System.IO.Path.Combine(MusicFolder, track.track_id.ToString() + ".mp4");
                if (track.track_id == 0) { return; }
                try
                {
                    this.client.DownloadTrack(track.track_id, path);
                }
                catch (WebException err)
                {
                    MessageBox.Show(err.Message);
                }
                playerElement.Source = new Uri(path);
            }
        }

        private void MediaPlayer_MediaEnded(object sender, RoutedEventArgs e)
        {
            if (TrackListIndex + 1 >= QueueList.Count) return;
            // TODO switch source player for next
            nextListView.SelectedItem = QueueList[TrackListIndex];
        }


        #region tracks
        private void ContextTrackPlayNow(object sender, RoutedEventArgs e)
        {
            TrackDoubleClick(sender, null);
        }

        private void ContextTrackPlayNext(object sender, RoutedEventArgs e)
        {
            if (tracksView.SelectedItem != null)
            {
                if (QueueList.Count <= TrackListIndex)
                    QueueList.Add((Track)tracksView.SelectedItem);
                else
                    QueueList.Insert(TrackListIndex + 1, (Track)tracksView.SelectedItem);
            }
        }

        private void ContextAddToQueueClick(object sender, RoutedEventArgs e)
        {
            if (tracksView.SelectedItem != null)
            {
                QueueList.Add((Track)tracksView.SelectedItem);
            }
        }

        private void ContextDeleteClick(object sender, RoutedEventArgs e)
        {
            if (tracksView.SelectedItem != null)
            {
                var id = ((Track)tracksView.SelectedItem).track_id;
                try
                {
                    client.DeleteTrack(id);
                }
                catch (WebException err)
                {
                    MessageBox.Show(err.Message);
                }
            }
        }

        private void ContextRemoveFromQueueclick(object sender, RoutedEventArgs e)
        {
            if (tracksView.SelectedItem != null)
            {
                var track = (Track)tracksView.SelectedItem;
                if (QueueList[TrackListIndex].Equals(track))
                    TrackListIndex = TrackListIndex < QueueList.Count - 1 ?
                        ++TrackListIndex :
                        --TrackListIndex;
                QueueList.Remove(track);
            }
        }

        private void ContextLikeClick(object sender, RoutedEventArgs e)
        {
            if (tracksView.SelectedItem != null)
            {
                try
                {
                    client.SetLike(((Track)tracksView.SelectedItem).track_id);
                }
                catch (WebException err)
                {
                    MessageBox.Show(err.Message);
                }
            }
        }

        private void ContextEditTrackClick(object sender, RoutedEventArgs e)
        {
            if (tracksView.SelectedItem != null)
            {
                var editWin = new createTrackWindow();
                var track = (Track)tracksView.SelectedItem;
                editWin.titleBox.Text = track.title;
                editWin.genreBox.Text = track.genre;
                editWin.artistBox.Text = track.artist;
                editWin.yearBox.Text = track.year.ToString();

                editWin.ShowDialog();

                track.title = editWin.titleBox.Text;
                track.artist = editWin.artistBox.Text;
                track.genre = editWin.genreBox.Text;
                try
                {
                    track.year = Convert.ToInt32(editWin.yearBox.Text);
                }
                catch
                {
                    track.year = 0;
                }

                try
                {
                    client.UpdateTrack(track.track_id, track);
                }
                catch (WebException err)
                {
                    MessageBox.Show(err.Message);
                }
            }
        }

        private void ContextAddToPlaylist(object sender, RoutedEventArgs e)
        {
            if (tracksView.SelectedItem != null)
            {
                var track = (Track)tracksView.SelectedItem;
                var playlists = new List<Playlist>();
                try
                {
                    playlists = client.GetUsersPlaylists();
                }
                catch (WebException err)
                {
                    MessageBox.Show(err.Message);
                    return;
                }
                var add = new ChoosePlaylistWindow(playlists);
                add.ShowDialog();
                var id = add.GetPlaylistId();
                if (id != 0)
                {
                    try
                    {
                        client.AddToPlaylist(id, track.track_id);
                    }
                    catch (WebException err)
                    {
                        MessageBox.Show(err.Message);
                        return;
                    }
                }
                else
                {
                    MessageBox.Show("Wrong value");
                }
            }
        }

        #endregion

        private void PlayNextButtonClick(object sender, RoutedEventArgs e)
        {
            if (TrackListIndex < QueueList.Count - 1)
            {
                ++TrackListIndex;
                var trackId = QueueList[TrackListIndex].track_id;
                var path = System.IO.Path.Combine(MusicFolder, trackId.ToString() + ".mp3");
                try
                {
                    client.DownloadTrack(trackId, path);
                }
                catch (WebException err)
                {
                    MessageBox.Show(err.Message);
                }
                playerElement.Source = new Uri(path);
            }
            else
            {
                MessageBox.Show("Playing last track!");
            }
        }

        private void PlayPrevButtonClick(object sender, RoutedEventArgs e)
        {
            if (TrackListIndex > 0)
            {
                --TrackListIndex;
                var trackId = QueueList[TrackListIndex].track_id;
                var path = System.IO.Path.Combine(MusicFolder, trackId.ToString() + ".mp3");
                try
                {
                    client.DownloadTrack(trackId, path);
                }
                catch (WebException err)
                {
                    MessageBox.Show(err.Message);
                }
                playerElement.Source = new Uri(path);
            }
            else
            {
                MessageBox.Show("Playing first track!");
            }
        }

        private void ListViewDoubleClick(object sender, MouseButtonEventArgs e)
        {

        }

        private void UploadTrackClick(object sender, RoutedEventArgs e)
        {
            var track = new Track();
            if (tracksView.SelectedItem != null)
                track = (Track)tracksView.SelectedItem;

            var dialog = new OpenFileDialog();
            if (dialog.ShowDialog() == true)
            {
                var path = dialog.FileName;
                try
                {
                    client.UploadFile(track.track_id, path);
                }
                catch (WebException err)
                {
                    MessageBox.Show(err.Message);
                }
            }
        }

        #region toolbar

        public void BuyClick(object sender, RoutedEventArgs e)
        {
            bool is_ref = false;
            try
            {
                var refer = client.GetMe().referal;
                is_ref = (refer != "0");
            }
            catch (WebException err)
            {
                MessageBox.Show(err.Message);
                return;
            }
            var buyWin = new BuyWindow(is_ref);
            buyWin.ShowDialog();
            try
            {
                client.BuyPremium();
            }
            catch (WebException err)
            {
                MessageBox.Show(err.Message);
            }

        }

        public void CreateTrackClick(object sender, RoutedEventArgs e)
        {
            var createTrackWindow = new createTrackWindow();
            createTrackWindow.ShowDialog();
            int year = 0;
            try
            {
                year = Convert.ToInt32(createTrackWindow.yearBox.Text);
            }
            catch
            {
                MessageBox.Show("Wrong year!");
                return;
            }
            var track = new Track()
            {
                title = createTrackWindow.titleBox.Text,
                artist = createTrackWindow.artistBox.Text,
                genre = createTrackWindow.genreBox.Text,
                year = year,
                has_video = (bool)createTrackWindow.HasVideoBox.IsChecked
            };

            try
            {
                client.CreateTrack(track);
            }
            catch (WebException err)
            {
                MessageBox.Show(err.Message);
            }
        }

        private void RefreshToolbarClick(object sender, RoutedEventArgs e)
        {
            this.Updater();
        }

        private void EditProfileClick(object sender, RoutedEventArgs e)
        {
            var user = new User();
            try
            {
                user = client.GetMe();
            }
            catch (WebException err)
            {
                MessageBox.Show(err.Message);
            }
            var editWin = new EditProfileWindow(user);
            editWin.ShowDialog();
            try
            {
                client.UpdateUser(editWin.getUser());
            }
            catch (WebException err)
            {
                MessageBox.Show(err.Message);
            }
        }

        private void ExitButtonClick(object sender, RoutedEventArgs e)
        {
            Environment.Exit(0);
        }

        #endregion


        #region playlist

        private void CreatePlaylistClick(object sender, RoutedEventArgs e)
        {
            var win = new PlaylistWindow();
            win.ShowDialog();
            try
            {
                client.CreatePlaylist(win.playlistBox.Text);
            }
            catch (WebException err)
            {
                MessageBox.Show(err.Message);
            }
        }

        private void ContextPlaylistPlayNext(object sender, RoutedEventArgs e)
        {
            if (playlistsView.SelectedItem != null)
            {
                var playlist = (Playlist)playlistsView.SelectedItem;
                var tracks = new List<Track>();

                try
                {
                    tracks = client.GetPlaylist(playlist.playlist_id);
                }
                catch (WebException err)
                {
                    MessageBox.Show(err.Message);
                }

                if (QueueList.Count <= TrackListIndex)
                {
                    foreach (var track in tracks)
                    {
                        QueueList.Add(track);
                    }
                    return;
                }
                foreach (var track in tracks)
                {
                    QueueList.Insert(TrackListIndex, track);
                }
            }
        }

        private void ContextPlaylistToQueueClick(object sender, RoutedEventArgs e)
        {
            if (playlistsView.SelectedItem != null)
            {
                var playlist = (Playlist)playlistsView.SelectedItem;
                var tracks = new List<Track>();

                try
                {
                    tracks = client.GetPlaylist(playlist.playlist_id);
                }
                catch (WebException err)
                {
                    MessageBox.Show(err.Message);
                }

                QueueList.Concat(tracks);
            }
        }

        private void ContextEditPlaylistClick(object sender, RoutedEventArgs e)
        {
            if (playlistsView.SelectedItem != null)
            {
                var playlist = (Playlist)playlistsView.SelectedItem;
                var win = new PlaylistWindow();
                win.ShowDialog();
                try
                {
                    client.UpdatePlaylist(win.playlistBox.Text, playlist.playlist_id);
                }
                catch (WebException err)
                {
                    MessageBox.Show(err.Message);
                }
            }
        }

        private void ContextDeletePlaylistClick(object sender, RoutedEventArgs e)
        {
            if (playlistsView.SelectedItem != null)
            {
                if (BackButton.Visibility == Visibility.Collapsed)
                {
                    try
                    {
                        client.DeletePlaylist(((Playlist)playlistsView.SelectedItem).playlist_id);
                    }
                    catch (WebException err)
                    {
                        MessageBox.Show(err.Message);
                    }
                }
                else
                {
                    try
                    {
                        client.RemoveFromPlaylist(ChoosedPlaylist, ((Track)playlistsView.SelectedItem).track_id);
                    }
                    catch (WebException err)
                    {
                        MessageBox.Show(err.Message);
                    }
                }
            }
        }


        #endregion
    }
}
