using System;

namespace MusicPlayerApi
{
    public struct User
    {
        public string username { get; set; }
        public string email { get; set; }
        public string password { get; set; }
        public bool is_premium { get; set; }
        public string referal { get; set; }
    }

    public struct Track
    {
        public int track_id { get; set; }
        public string title { get; set; }
        public string genre { get; set; }
        public string artist { get; set; }
        public int year { get; set; }
        public bool has_video { get; set; }
        public bool is_liked { get; set; }
    }

    public struct History
    {
        public int track_id { get; set; }
        public int user_id { get; set; }
        public DateTime time { get; set; }
    }

    public struct Playlist
    {
        public int playlist_id { get; set; }
        public string title { get; set; }
    }
}
