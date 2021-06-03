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

using MusicPlayerApi;

namespace MusicPlayer
{
    /// <summary>
    /// Логика взаимодействия для EditProfileWindow.xaml
    /// </summary>
    public partial class EditProfileWindow : Window
    {
        public User user;
        public EditProfileWindow(User user)
        {
            this.user = user;
            InitializeComponent();


            UsernameBox.Text = user.username;
            EmailBox.Text = user.email;
            PasswdBox.Text = user.password;

            isPremiumBox.Text = user.is_premium ? "True" : "False";
        }

        public void EditClick(object sender, RoutedEventArgs e)
        {
            UsernameBox.IsEnabled = UsernameBox.IsEnabled ? false : true;
            PasswdBox.IsEnabled = PasswdBox.IsEnabled ? false : true;
        }

        public void SubmitClick(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        public User getUser()
        {
            this.user.username = UsernameBox.Text;
            this.user.password = PasswdBox.Text;
            return this.user;
        }
    }
}
