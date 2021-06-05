using System;
using System.Windows;

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
            }
            else
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
            }
            catch
            {
                return false;
            }
            return inputValue == price;
        }
    }
}
