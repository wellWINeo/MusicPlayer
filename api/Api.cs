using System;
using System.Text.Json;
using System.Net.Http;
using System.Net;
using System.IO;
using System.Collections.Generic;
using System.Diagnostics;

namespace MusicPlayerApi
{

    public struct Urls 
    {
        // authenication urls
        public static string SignIn = "/auth/sign-in";
        public static string SignUp = "/auth/sign-up";
        public static string Verify = "/auth/verify";

        // api urls
        public static string Users = "/api/users/";
        public static string Tracks = "/api/tracks/";
        public static string TracksAll = "/api/tracks/all";
        public static string DownloadTrack = "/api/tracks/download/";
        public static string UploadTrack = "/api/tracks/upload/";
        public static string Likes = "/api/like/";
        public static string History = "/api/history";
        public static string Playlist = "/api/playlists/";
        public static string PlaylistMod = "/api/playlists/mod/";
        public static string PlaylistsAll = "/api/playlists/all";
        public static string BuyPremim = "/api/buy";
        
    }

    public class Client 
    {
        public string login, password;
        // NOTE change to private
        public string token;
        private string host;
        private int userId;
        
        //
        // Constructors
        //
        public Client(string login, string password, string host = "localhost",
                int port = 8000)
        {
            this.login = login;
            this.password = password;
            
            this.host = String.Format("{0}:{1}", host, port); 
        }

        public Client(string token, string host="localhost", int port = 8000)
        {
            this.token = token;
            this.host = String.Format("{0}:{1}", host, port);
        }

        public Client()
        {
        }


        //
        // Shared methods
        //
        public string GenerateUrl(string url)
        {
            return string.Format("http://{0}{1}", this.host, url);
        }
        
        public string ReadError(HttpWebResponse resp)
        {
            using (var streamReader = new StreamReader(resp.GetResponseStream()))
            {
                var stringJson = streamReader.ReadToEnd();
                var json =
                    JsonSerializer.Deserialize<Dictionary<string, string>>(stringJson);
                return json["message"];
            } 
        }

        private HttpWebResponse SendRequest(string body, string url, string method)
        {
            HttpWebRequest request = (HttpWebRequest) WebRequest.Create(url);
            request.ContentType = "application/json; charset=utf-8";
            request.Headers.Add("Authorization", "Bearer " + this.token);
            request.Method = method;

            if (body != String.Empty)
            {
                using (var streamWriter = new StreamWriter(request.GetRequestStream()))
                {
                    streamWriter.Write(body);
                    streamWriter.Flush();
                }
            }

            var response = (HttpWebResponse) request.GetResponse();
            return response; 
        }
        
        //
        // Auth
        //
        public void Register(string userEmail)
        {
            
            HttpWebRequest request = 
                (HttpWebRequest) WebRequest.Create(this.GenerateUrl(Urls.SignUp));
            request.ContentType = "application/json; charset=utf-8";
            request.Method = "POST";

            var user = new User 
            {
                username = this.login,
                email = userEmail,
                password = this.password
            };
            
            using (var streamWriter = new StreamWriter(request.GetRequestStream()))
            {
                streamWriter.Write(JsonSerializer.Serialize(user));
                streamWriter.Flush();
            }

            HttpWebResponse response = (HttpWebResponse) request.GetResponse();
            using (var streamReader = new StreamReader(response.GetResponseStream()))
            {
                var jsonString = streamReader.ReadToEnd();
                var json =  
                    JsonSerializer.Deserialize<Dictionary<string, int>>(jsonString);
                this.userId = json["id"]; 
            }

        }

        public void Auth()
        {
            HttpWebRequest request = 
                (HttpWebRequest) WebRequest.Create(this.GenerateUrl(Urls.SignIn));
            request.ContentType = "application/json; charset=utf-8";
            request.Method = "POST";

            var user = new User
            {
                username = this.login,
                password = this.password
            };

            using (var streamWriter = new StreamWriter(request.GetRequestStream()))
            {
                streamWriter.Write(JsonSerializer.Serialize(user));
                streamWriter.Flush();
            }

            HttpWebResponse response = (HttpWebResponse) request.GetResponse();
            using (var streamReader = new StreamReader(response.GetResponseStream()))
            {
                var stringJson = streamReader.ReadToEnd();
                var json = 
                    JsonSerializer.Deserialize<Dictionary<string, string>>(stringJson);
                this.token = json["token"];
            }
        }

        public void Verify(int code)
        {
            HttpWebRequest request =
                (HttpWebRequest) WebRequest.Create(this.GenerateUrl(Urls.Verify));
            request.ContentType = "application/json; charset=utf-8";
            request.Method = "POST";
            
            Dictionary<string, string> dict = new Dictionary<string, string>();
            dict.Add("user_id", this.userId.ToString());
            dict.Add("code", code.ToString());

            using (var streamWriter = new StreamWriter(request.GetRequestStream()))
            {
                streamWriter.Write(JsonSerializer.Serialize(dict));
                streamWriter.Flush();
            }

            HttpWebResponse response = (HttpWebResponse) request.GetResponse();
            string msg = String.Empty;
            using (var streamReader = new StreamReader(response.GetResponseStream()))
            {
                var stringJson = streamReader.ReadToEnd();
                var json =
                    JsonSerializer.Deserialize<Dictionary<string, string>>(stringJson);
                msg = json["msg"];
            } 
            
            if (response.StatusCode != HttpStatusCode.OK)
                throw new Exception(msg);
        }
        
        //
        // Users
        //
        public User GetMe()
        {
            HttpWebRequest request =
                (HttpWebRequest) WebRequest.Create(this.GenerateUrl(Urls.Users));
            request.ContentType = "application/json; charset=utf-8";
            request.Method = "GET";
            request.Headers.Add("Authorization", "Bearer " + this.token);

            User user = new User();
            
            HttpWebResponse response = (HttpWebResponse) request.GetResponse();

            if (response.StatusCode != HttpStatusCode.OK)
                throw new Exception(this.ReadError(response));

            using (var streamReader = new StreamReader(response.GetResponseStream()))
            {
                var stringJson = streamReader.ReadToEnd();
                user = JsonSerializer.Deserialize<User>(stringJson);
            }

            return user;
        }


        public void DeleteUser()
        {
            HttpWebRequest request = 
                (HttpWebRequest) WebRequest.Create(this.GenerateUrl(Urls.Users));
            request.ContentType = "application/json; charset=utf-8";
            request.Method = "DELETE";
            request.Headers.Add("Authorization", "Bearer " + this.token);

            HttpWebResponse response = (HttpWebResponse) request.GetResponse();

            if (response.StatusCode != HttpStatusCode.OK)
                throw new Exception(this.ReadError(response));
        }

        public void UpdateUser(User user)
        {
            HttpWebRequest request = 
                (HttpWebRequest) WebRequest.Create(this.GenerateUrl(Urls.Users));
            request.ContentType = "application/json; charset=utf-8";
            request.Method = "PUT";
            request.Headers.Add("Authorization", "Bearer " + this.token);
            
            using (var streamWriter = new StreamWriter(request.GetRequestStream()))
            {
                streamWriter.Write(JsonSerializer.Serialize(user));
                streamWriter.Flush();
            }
            
            try 
            {
                HttpWebResponse response = (HttpWebResponse) request.GetResponse();
            } 
            catch (WebException err)
            {
                Debug.WriteLine(err.Message);
            }
        }

        //
        // Tracks
        //
        public void CreateTrack(Track track)
        {
            HttpWebRequest request = 
                (HttpWebRequest) WebRequest.Create(this.GenerateUrl(Urls.Tracks));
            request.ContentType = "application/json; charset=utf-8";
            request.Method = "POST";
            request.Headers.Add("Authorization", "Bearer " + this.token);

            using (var streamWriter = new StreamWriter(request.GetRequestStream()))
            {
                string jsonString = JsonSerializer.Serialize(track);
                streamWriter.Write(jsonString);
                streamWriter.Flush();
            }

            HttpWebResponse response = (HttpWebResponse) request.GetResponse();
            if (response.StatusCode != HttpStatusCode.OK)
                throw new Exception(this.ReadError(response));
        }

        public Track GetTrack(int id)
        {
            var resp = this.SendRequest(String.Empty, 
                    this.GenerateUrl(Urls.Tracks) + id.ToString(), "GET");
            if (resp.StatusCode != HttpStatusCode.OK)
                throw new Exception(resp.StatusCode.ToString());
            
            Track track = new Track();
            using (var streamReader = new StreamReader(resp.GetResponseStream()))
            {
                var stringJson = streamReader.ReadToEnd();
                track = JsonSerializer.Deserialize<Track>(stringJson);
            }

            return track;
        }
        
        // FIXME some crazy shit going here
        public void UpdateTrack(int id, Track track)
        {
            var body = JsonSerializer.Serialize(track);
            var resp = this.SendRequest(body, 
                    this.GenerateUrl(Urls.Tracks) + id.ToString(), "PUT");
            if (resp.StatusCode != HttpStatusCode.OK)
                throw new Exception(resp.StatusCode.ToString());
        }

        public void DeleteTrack(int id)
        {
            var resp = this.SendRequest(String.Empty,
                    this.GenerateUrl(Urls.Tracks) + id.ToString(), "DELETE");
            if (resp.StatusCode != HttpStatusCode.OK)
                throw new Exception(resp.StatusCode.ToString());
        }

        public List<Track> GetAllTracks()
        {
            var tracksList = new List<Track>();
            var resp = this.SendRequest(String.Empty, 
                    this.GenerateUrl(Urls.TracksAll), "GET");
            if (resp.StatusCode != HttpStatusCode.OK)
                throw new Exception(resp.StatusCode.ToString());

            using (var streamReader = new StreamReader(resp.GetResponseStream()))
            {
                var jsonString = streamReader.ReadToEnd();
                tracksList = JsonSerializer.Deserialize<List<Track>>(jsonString);
            }

            return tracksList;
        }

        //
        // Likes
        //
        public void SetLike(int id)
        {
            var resp = this.SendRequest(String.Empty,
                    this.GenerateUrl(Urls.Likes) + id.ToString(), "POST");
            if (resp.StatusCode !=HttpStatusCode.OK)
                throw new Exception(resp.StatusCode.ToString());
        }

        public List<int> GetAllLikes()
        {
            var likes = new List<int>();
            var resp = this.SendRequest(String.Empty,
                    this.GenerateUrl(Urls.Likes), "GET");
            if (resp.StatusCode !=HttpStatusCode.OK)
                throw new Exception(resp.StatusCode.ToString());
            using (var streamReader = new StreamReader(resp.GetResponseStream()))
            {
                var jsonString = streamReader.ReadToEnd();
                likes = JsonSerializer.Deserialize<List<int>>(jsonString);
            }

            if (likes == null)
                likes = new List<int>();
            return likes;
        }

        public List<History> GetHistory()
        {
            var history = new List<History>();
            var resp = this.SendRequest(String.Empty,
                    this.GenerateUrl(Urls.History), "GET");
            if (resp.StatusCode !=HttpStatusCode.OK)
                throw new Exception(resp.StatusCode.ToString());
            using (var streamReader = new StreamReader(resp.GetResponseStream()))
            {
                var jsonString = streamReader.ReadToEnd();
                history = JsonSerializer.Deserialize<List<History>>(jsonString);
            }
            if (history == null)
                history = new List<History>();
            return history;
        }

        public void CreatePlaylist(string title)
        {
            var dict = new Dictionary<string, string>();
            dict.Add("title", title);
            var jsonString = JsonSerializer.Serialize(dict);
            var resp = this.SendRequest(jsonString, 
                    this.GenerateUrl(Urls.Playlist), "POST");
            if (resp.StatusCode != HttpStatusCode.OK)
                throw new Exception(resp.StatusCode.ToString());
        }

        public List<Track> GetPlaylist(int id)
        {
            var content = new List<Track>();
            var resp = this.SendRequest(String.Empty,
                    this.GenerateUrl(Urls.Playlist) + id.ToString(), "GET");
            if (resp.StatusCode !=HttpStatusCode.OK)
                throw new Exception(resp.StatusCode.ToString());
            using (var streamReader = new StreamReader(resp.GetResponseStream()))
            {
                var jsonString = streamReader.ReadToEnd();
                content = JsonSerializer.Deserialize<List<Track>>(jsonString);
            }
            
            return content;
        }

        public void UpdatePlaylist(string title, int id)
        {
            var dict = new Dictionary<string, string>();
            dict.Add("title", title);
            var jsonString = JsonSerializer.Serialize(dict);
            var resp = this.SendRequest(jsonString, 
                    this.GenerateUrl(Urls.Playlist) + id.ToString(), "PUT");
            if (resp.StatusCode != HttpStatusCode.OK)
                throw new Exception(resp.StatusCode.ToString());
        }

        public void DeletePlaylist(int id)
        {
            var resp = this.SendRequest(String.Empty,
                    this.GenerateUrl(Urls.Playlist) + id.ToString(), "DELETE");
            if (resp.StatusCode != HttpStatusCode.OK)
                throw new Exception(resp.StatusCode.ToString());
        }

        public List<Playlist> GetUsersPlaylists()
        {
            var playlists = new List<Playlist>();
            var resp = this.SendRequest(String.Empty,
                    this.GenerateUrl(Urls.PlaylistsAll), "GET");
            if (resp.StatusCode != HttpStatusCode.OK)
                throw new Exception(resp.StatusCode.ToString());

            using (var streamReader = new StreamReader(resp.GetResponseStream()))
            {
                string jsonString = streamReader.ReadToEnd();
                playlists = JsonSerializer.Deserialize<List<Playlist>>(jsonString);
            }

            if (playlists == null)
                playlists = new List<Playlist>();

            return playlists;
        }

        public void AddToPlaylist(int playlistId, int trackId)
        {
            var dict = new Dictionary<string, int>();
            dict.Add("track_id", trackId);
            var jsonString = JsonSerializer.Serialize(dict);
            
            var resp = this.SendRequest(jsonString, 
                    this.GenerateUrl(Urls.PlaylistMod) +playlistId.ToString(), "POST");
            if (resp.StatusCode != HttpStatusCode.OK)
                throw new Exception(resp.StatusCode.ToString());
        }

        public void RemoveFromPlaylist(int playlistId, int trackId)
        {
            var dict = new Dictionary<string, int>();
            dict.Add("track_id", trackId);
            var jsonString = JsonSerializer.Serialize(dict);

            var resp = this.SendRequest(jsonString,
                    this.GenerateUrl(Urls.PlaylistMod) + playlistId.ToString(), "DELETE");
            if (resp.StatusCode != HttpStatusCode.OK)
                throw new Exception(resp.StatusCode.ToString());
        }

        //
        // uploading/downloading tracks
        //

        public void UploadFile(int id, string path)
        {
            var boundaryString = "----someBoundaryString";
            string url = this.GenerateUrl(Urls.UploadTrack) + id.ToString();
            Console.WriteLine(url);
            /* var request = (HttpWebRequest) WebRequest.Create( */
            /*         this.GenerateUrl(Urls.UploadTrack) + id.ToString()); */
            var request = (HttpWebRequest) WebRequest.Create(url);
            request.Headers.Add("Authorization", "Bearer " + this.token);
            request.Method = "POST";
            request.ContentType = "multipart/form-data; boundary=" + boundaryString;
            request.KeepAlive = true;

            var postStream = new MemoryStream();
            var postWriter = new StreamWriter(postStream);
            
            var fileStream = new FileStream(path, FileMode.Open, FileAccess.Read);
            byte[] buffer = new byte[1024];
            var bytesRead = 0;

            postWriter.Write("\r\n--" + boundaryString + "\r\n");
            postWriter.Write("Content-Disposition: form-data;"
                             + "name=\"{0}\";"
                             + "filename=\"{1}\""
                             + "\r\nContent-Type: {2}\r\n\r\n",
                            "file", Path.GetFileName(path),
                            Path.GetExtension(path));
            postWriter.Flush();

            while ((bytesRead = fileStream.Read(buffer, 0, buffer.Length)) != 0)
                postStream.Write(buffer, 0, bytesRead);
                
            fileStream.Close();

            postWriter.Write("\r\n--" + boundaryString + "--\r\n");
            postWriter.Flush();

            request.ContentLength = postStream.Length;

            using (var s = request.GetRequestStream())
            {
                postStream.WriteTo(s);
            }

            postStream.Close();

            var response = (HttpWebResponse) request.GetResponse();
            if (response.StatusCode != HttpStatusCode.OK)
                throw new Exception(response.StatusCode.ToString());
        }

        public void DownloadTrack(int id, string path)
        {
            using (var client = new WebClient())
            {
                client.Headers.Add("Authorization", "Bearer " + this.token);
                client.DownloadFile(this.GenerateUrl(Urls.DownloadTrack) +
                        id.ToString(), path);
            }
        
        }

        public void BuyPremium()
        {
            var resp = this.SendRequest(String.Empty,
               this.GenerateUrl(Urls.BuyPremim), "POST");
        }

    }
}
