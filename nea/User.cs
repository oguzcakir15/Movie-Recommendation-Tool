using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace nea
{
    internal class User
    {
        public string username;
        public string PasswordHash;
        public int UserId;
        public List<Movie> WatchedList;
        public List<Movie> Favourites;
        public List<Movie> WatchList;
        public Dictionary<Movie, double> Ratings;
        public Dictionary <Movie, string> Comments;

        public User(string username1, string password1) 
        {
            username = username1;
            PasswordHash = Hashing.HashPassword(password1);
            WatchedList = new List<Movie>();
            Favourites = new List<Movie>();
            WatchList = new List<Movie>();
            Ratings = new Dictionary<Movie, double>();
            Comments = new Dictionary<Movie, string>();

        }

        public User(int id, string username1, string password1)
        {
            UserId = id;
            username = username1;
            PasswordHash = password1;
            WatchedList = new List<Movie>();
            Favourites = new List<Movie>();
            WatchList = new List<Movie>();
            Ratings = new Dictionary<Movie, double>();
            Comments = new Dictionary<Movie, string>();

        }

    }
}
