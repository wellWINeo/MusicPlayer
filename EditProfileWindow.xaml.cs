using MusicPlayerApi;
using System.Windows;

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
