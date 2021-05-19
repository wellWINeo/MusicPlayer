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
        public MainWindow()
        {
            InitializeComponent();
            timer.Interval = TimeSpan.FromSeconds(0.1);
            timer.Tick += new EventHandler(timer_Tick);
            timer.IsEnabled = true;

            playerElement.LoadedBehavior = MediaState.Manual;
            playerElement.Source = new Uri(MusicFolder + "\\Запрещённые Барабанщики - Убили Негра.mp3");
            playerElement.Play();

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
                //MessageBox.Show($"{playerSlider.Value} - {playerElement.NaturalDuration.TimeSpan.TotalSeconds}");
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
                playButton.Content = "||";
            }
            else
            {
                playerElement.LoadedBehavior = MediaState.Play;
                playButton.Content = "▶️";
            }
        }
    }
}
