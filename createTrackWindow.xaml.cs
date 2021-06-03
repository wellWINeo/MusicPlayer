using System;
using System.Collections.Generic;
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
using System.Windows.Shapes;

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
