using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
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
using System.IO;

using MusicPlayerApi;

namespace MusicPlayer
{
    /// <summary>
    /// Логика взаимодействия для Login.xaml
    /// </summary>
    public partial class Login : Window
    {
        public Client client;
        public Login(Client client)
        {
            this.client = client;
            InitializeComponent();
        }

        private void RegisterClick(object sender, RoutedEventArgs e)
        {
            var registerWindow = new Register(client);
            this.Hide();
            registerWindow.ShowDialog();
            this.Show();
        }

        private void LoginClick(object sender, RoutedEventArgs e)
        {
            client.login = UsernameBox.Text;
            client.password = PasswordBox.Password;
            try
            {
                client.Auth();
                _ = client.GetMe();
                if (File.Exists(".token"))
                    File.Delete(".token");
                using (var fs = new FileStream(".token", FileMode.OpenOrCreate))
                {
                    using (var writer = new StreamWriter(fs))
                    {
                        writer.WriteLine(client.token);
                        writer.Flush();
                    }
                }
                this.Close();
            } catch (WebException err)
            {
                MessageBox.Show(err.Message);
            }
        }
    }
}
