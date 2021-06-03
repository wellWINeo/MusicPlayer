using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.ComponentModel;
using System.Windows.Data;
using System.Globalization;

using MusicPlayerApi;

namespace MusicPlayer
{
    public class TrackConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null) { return ""; }
            var track = (Track)value;
            var response = $"♫ {track.title}, {track.artist}, {track.genre}, {track.year}";
            return response;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
