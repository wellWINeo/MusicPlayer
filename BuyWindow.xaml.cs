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
    /// Логика взаимодействия для BuyWindow.xaml
    /// </summary>
    public partial class BuyWindow : Window
    {
        public double price = 100;
        public BuyWindow(bool is_ref)
        {
            InitializeComponent();
            if (is_ref)
            {
                price *= 1 - 0.15;
                offerLabel.Content = $"You have referal code. Your sale is 15%. ({price}$ insted of 100$";
            } else
            {
                offerLabel.Content = $"You need to pay - {price}$";
            }
        }

        public void BuyClick(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        public bool IsPayed()
        {
            double inputValue = 0;
            try
            {
                inputValue = Convert.ToDouble(paymentBox.Text);
            } catch
            {
                return false;
            }
            return inputValue == price;
        }
    }
}
