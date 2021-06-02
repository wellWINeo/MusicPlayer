using System;
using System.Collections.Generic;
using System.ComponentModel;
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

using MusicPlayerApi;

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
            } catch (WebException err)
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
            } catch (WebException err)
            {
                MessageBox.Show(err.Message);
            }
            this.Close();
        }
    }
}
