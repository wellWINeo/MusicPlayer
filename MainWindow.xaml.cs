using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using System.Collections.ObjectModel;

namespace MusicPlayer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        bool isPause = false;
        public DispatcherTimer timer = new DispatcherTimer();
        public ObservableCollection<Node> nodes { get; set; }
        public string MusicFolder = Environment.GetFolderPath(Environment.SpecialFolder.MyMusic);
        public ObservableCollection<Node> TrackList = new ObservableCollection<Node>();
        public History trackHistory;
        public Node CurrentNode;
        int TrackListIndex;

        public MainWindow()
        {
            string historyDir = System.IO.Path.Combine(Directory.GetCurrentDirectory(), "history");
            if (!Directory.Exists(historyDir))
                Directory.CreateDirectory(historyDir);
            trackHistory = new History(historyDir);

            InitializeComponent();
            nextListView.ItemsSource = TrackList;
            historyListView.ItemsSource = trackHistory.HistoryList;

            timer.Interval = TimeSpan.FromSeconds(0.1);
            timer.Tick += new EventHandler(timer_Tick);
            timer.IsEnabled = true;

            playerElement.LoadedBehavior = MediaState.Manual;
            //playerElement.Source = new Uri(MusicFolder + "\\Запрещённые Барабанщики - Убили Негра.mp3");
            //playerElement.Play();

            TrackList.Add(new Node()
            {
                Name = "Запрещённые Барабанщики - Убили Негра.mp3",
            });
            TrackListIndex = 0;

            nodes = new ObservableCollection<Node>(){
                new Node()
                {
                    Icon = string.Empty,
                    FullName = MusicFolder,
                    Name = "Music",
                    Nodes = new ObservableCollection<Node>()
                }
            };
            
           
            foreach (var dir in Directory.GetDirectories(MusicFolder))
            {
                nodes[0].Nodes.Add(new Node()
                {
                    Icon = "📁",
                    FullName = dir,
                    Name = System.IO.Path.GetFileName(dir),
                    Nodes = new ObservableCollection<Node>()
                });

                foreach(var file in Directory.GetFiles(dir))
                {
                    nodes[0].Nodes[nodes.Count - 1].Nodes.Add(new Node()
                    {
                        Icon = "♫",
                        FullName = file,
                        Name = System.IO.Path.GetFileName(file)
                    });
                }
            }

            foreach(var file in Directory.GetFiles(MusicFolder))
            {
                nodes[0].Nodes.Add(new Node()
                {
                    Icon = "♫",
                    FullName = file,
                    Name = System.IO.Path.GetFileName(file)
                });
            }

            treeView.ItemsSource = nodes;


        }

        private void timer_Tick(object sender, EventArgs e)
        {
            playerSlider.Value = playerElement.Position.TotalSeconds;
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
            Node node = this.getSelectedNode();
            if (node != null)
            {
                trackHistory.Write(node);
                TrackList[TrackListIndex] = node;
                playerElement.Source = new Uri(node.FullName);
            }
        }

        private void ListViewDoubleClick(object sender, MouseButtonEventArgs e)
        {
            ListView listView = sender as ListView;
            if (listView.SelectedItem != null)
            {
                Node node = listView.SelectedItem as Node;
                trackHistory.Write(node);
                TrackList[TrackListIndex] = node;
                playerElement.Source = new Uri(node.FullName);
            }
        }

        private void MediaPlayer_MediaEnded(object sender, RoutedEventArgs e)
        {
            if (TrackListIndex + 1 >= TrackList.Count) return;
            playerElement.Source = new Uri(TrackList[++TrackListIndex].FullName);
            trackHistory.Write(TrackList[TrackListIndex]);
            nextListView.SelectedItem = TrackList[TrackListIndex];
        }

        private void ContextTrackPlayNow(object sender, RoutedEventArgs e)
        {
            TrackDoubleClick(sender, null);
        }

        private void ContextTrackPlayNext(object sender, RoutedEventArgs e)
        {
            Node node = this.getSelectedNode();
            if (node != null)
                TrackList.Insert(TrackListIndex + 1, node);

        }

        private void ContextAddToQueueClick(object sender, RoutedEventArgs e)
        {
            Node node = this.getSelectedNode();
            if (node != null)
                TrackList.Add(node);
        }

        private void ContextRemoveFromQueueclick(object sender, RoutedEventArgs e)
        {
            Node node = this.getSelectedNode();
            if (node != null)
            {
                if (TrackList[TrackListIndex].Equals(node))
                    TrackListIndex = TrackListIndex < TrackList.Count - 1 ?
                        ++TrackListIndex :
                        --TrackListIndex;
                TrackList.Remove(node);
            }
        }

        private Node getSelectedNode()
        {
            return myTabControl.SelectedIndex switch
            {
                0 => treeView.SelectedItem as Node,
                1 => nextListView.SelectedItem as Node,
                2 => historyListView.SelectedItem as Node,
                // here will be liked list view...
                _ => null,
            };
        }

        private void PlayNextButtonClick(object sender, RoutedEventArgs e)
        {
            if (TrackListIndex < TrackList.Count - 1)
            {
                ++TrackListIndex;
                playerElement.Source = new Uri(TrackList[TrackListIndex].FullName);
            } else {
                MessageBox.Show("Playing last track!");
            }
        }

        private void PlayPrevButtonClick(object sender, RoutedEventArgs e)
        {
            if (TrackListIndex > 0)
            {
                --TrackListIndex;
                playerElement.Source = new Uri(TrackList[TrackListIndex].FullName);
            } else
            {
                MessageBox.Show("Playing first track!");
            }
        }
    }
}
