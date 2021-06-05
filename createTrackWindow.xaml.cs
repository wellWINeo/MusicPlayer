using System.Windows;

namespace MusicPlayer
{
    /// <summary>
    /// Логика взаимодействия для createTrackWindow.xaml
    /// </summary>
    public partial class createTrackWindow : Window
    {
        public createTrackWindow()
        {
            InitializeComponent();
        }

        public void createTrackClick(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
