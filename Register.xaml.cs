using MusicPlayerApi;
using System;
using System.Net;
using System.Windows;

namespace MusicPlayer
{
    /// <summary>
    /// Логика взаимодействия для Register.xaml
    /// </summary>
    public partial class Register : Window
    {
        public Client client;
        public Register(Client client)
        {
            this.client = client;
            InitializeComponent();
        }

        private void SubmitClick(object sender, RoutedEventArgs e)
        {
            client.login = UsernameBox.Text;
            client.password = PasswdBox.Password;
            try
            {
                client.Register(EmailBox.Text);
            }
            catch (WebException err)
            {
                MessageBox.Show(err.Message);
            }

            registerPanel.Visibility = Visibility.Collapsed;
            verifyPanel.Visibility = Visibility.Visible;
        }

        private void VerifyClick(object sender, RoutedEventArgs e)
        {
            try
            {
                client.Verify(Convert.ToInt32(verifyBox.Text));
            }
            catch (WebException err)
            {
                MessageBox.Show(err.Message);
            }
            this.Close();
        }
    }
}
