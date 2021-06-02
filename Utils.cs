using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

using MusicPlayerApi;
using System.Net;
using System.Windows;

namespace MusicPlayer
{
    class Utils
    {
        public static void Init(Client client)
        {
            if (File.Exists(".token"))
            {
                using var fs = new FileStream(".token", FileMode.Open);
                using var reader = new StreamReader(fs);
                client.token = reader.ReadLine();

                try
                {
                    _ = client.GetMe();
                    return;
                } catch (WebException err )
                {
                    MessageBox.Show(err.Message);
                }
            }

            var loginWindow = new Login(client);
            loginWindow.ShowDialog();
        }
    }
}
