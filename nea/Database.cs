using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SQLite;
using System.Dynamic;
namespace nea
{
    internal class Database
    {
        public static SQLiteConnection GetConnection() 
        {
            return new SQLiteConnection("Data Source=database.db;Version=3;");
        }


        public static void CreateTables()
        {
            using (var connection = GetConnection())
            {
                connection.Open();

                using (var pragma = new SQLiteCommand("PRAGMA foreign_keys = ON;", connection))
                {
                    pragma.ExecuteNonQuery();
                }

                string CreateUsersTable = "CREATE TABLE IF NOT EXISTS Users (" +
                    "UserID INTEGER PRIMARY KEY AUTOINCREMENT, " +
                    "Username TEXT NOT NULL UNIQUE, " +
                    "PasswordHash TEXT NOT NULL" +
                    ");";
                using (var command = new SQLiteCommand(CreateUsersTable, connection))
                {
                    command.ExecuteNonQuery();
                }

                string CreateMoviesTable = "CREATE TABLE IF NOT EXISTS Movies (" +
                    "MovieID INTEGER PRIMARY KEY AUTOINCREMENT, " +
                    "Title TEXT NOT NULL, " +
                    "Genre TEXT, " +
                    "AgeRating TEXT, " +
                    "Director TEXT, " +
                    "LeadActors TEXT, " +
                    "Rating REAL, " +
                    "ReleaseYear INTEGER, " +
                    "Duration REAL, " +
                    "UNIQUE(Title, ReleaseYear)" +
                    ");";
                using (var command = new SQLiteCommand(CreateMoviesTable, connection))
                {
                    command.ExecuteNonQuery();
                }

                string CreateWatchList = "CREATE TABLE IF NOT EXISTS UserWatchList (" +
                    "UserID INTEGER NOT NULL, " +
                    "MovieID INTEGER NOT NULL, " +
                    "PRIMARY KEY(UserID, MovieID), " +
                    "FOREIGN KEY(UserID) REFERENCES Users(UserID) ON DELETE CASCADE, " +
                    "FOREIGN KEY(MovieID) REFERENCES Movies(MovieID) " +
                    ");";
                using (var command = new SQLiteCommand(CreateWatchList, connection))
                {
                    command.ExecuteNonQuery();
                }

                string CreateFavourites = "CREATE TABLE IF NOT EXISTS UserFavourites (" +
                    "UserID INTEGER NOT NULL, " +
                    "MovieID INTEGER NOT NULL, " +
                    "PRIMARY KEY(UserID, MovieID), " +
                    "FOREIGN KEY(UserID) REFERENCES Users(UserID) ON DELETE CASCADE, " +
                    "FOREIGN KEY(MovieID) REFERENCES Movies(MovieID) " +
                    ");";
                using (var command = new SQLiteCommand(CreateFavourites, connection))
                {
                    command.ExecuteNonQuery();
                }

                string CreateWatched = "CREATE TABLE IF NOT EXISTS UserWatched (" +
                    "UserID INTEGER NOT NULL, " +
                    "MovieID INTEGER NOT NULL, " +
                    "PRIMARY KEY(UserID, MovieID), " +
                    "FOREIGN KEY(UserID) REFERENCES Users(UserID) ON DELETE CASCADE, " +
                    "FOREIGN KEY(MovieID) REFERENCES Movies(MovieID) " +
                    ");";
                using (var command = new SQLiteCommand(CreateWatched, connection))
                {
                    command.ExecuteNonQuery();
                }

                string CreateRatings = "CREATE TABLE IF NOT EXISTS UserRatings (" +
                    "UserID INTEGER NOT NULL, " +
                    "MovieID INTEGER NOT NULL, " +
                    "Rating REAL NOT NULL, " +
                    "PRIMARY KEY(UserID, MovieID), " +
                    "FOREIGN KEY(UserID) REFERENCES Users(UserID) ON DELETE CASCADE, " +
                    "FOREIGN KEY(MovieID) REFERENCES Movies(MovieID) " +
                    ");";
                using (var command = new SQLiteCommand(CreateRatings, connection))
                {
                    command.ExecuteNonQuery();
                }

                string CreateComments = "CREATE TABLE IF NOT EXISTS UserComments (" +
                    "UserID INTEGER NOT NULL, " +
                    "MovieID INTEGER NOT NULL, " +
                    "Comment TEXT, " +
                    "PRIMARY KEY(UserID, MovieID), " +
                    "FOREIGN KEY(UserID) REFERENCES Users(UserID) ON DELETE CASCADE, " +
                    "FOREIGN KEY(MovieID) REFERENCES Movies(MovieID) " +
                    ");";
                using (var command = new SQLiteCommand(CreateComments, connection))
                {
                    command.ExecuteNonQuery();
                }

                Console.WriteLine("Database tables created successfully.");
            }
        }

        


        public static List<User> LoadAllUsers()
        {
            List<User> users = new List<User>();
            var userInfos = new List<(int userId, string username, string passwordHash)>();
            using (var connection = GetConnection())
            {
                connection.Open();


                string sql = "SELECT UserID, Username, PasswordHash FROM Users;";
                using (var command = new SQLiteCommand(sql, connection))
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        int userId = reader.GetInt32(0);
                        string username = reader.GetString(1);
                        string passwordHash = reader.GetString(2);
                        userInfos.Add((userId, username, passwordHash));
                    }
                }


                foreach (var info in userInfos)
                {
                    var user = new User(info.userId, info.username, info.passwordHash);

                    user.Favourites = LoadMoviesForUser(info.userId, "Favourites");
                    user.WatchedList = LoadMoviesForUser(info.userId, "Watched");
                    user.WatchList = LoadMoviesForUser(info.userId, "WatchList");

                    user.Ratings = LoadRatingsForUser(info.userId);
                    user.Comments = LoadCommentsForUser(info.userId);

                    users.Add(user);
                }

                    
                
            }
            return users;
        }

        public static Movie LoadMovieByID(int movieId)
        {
            using (var connection = Database.GetConnection())
            {
                connection.Open();

                string sql =
                    "SELECT Title, Genre, AgeRating, Director, LeadActors, Rating, ReleaseYear, Duration " +
                    "FROM Movies WHERE MovieID = @mid;";

                using (var command = new SQLiteCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@mid", movieId);

                    using (var reader = command.ExecuteReader())
                    {
                        if (!reader.Read())
                        {
                            return null; 
                        }

                        Movie movie = new Movie();

                        movie.Title = reader.GetString(0);

                        movie.Genre = reader.GetString(1);

                        if (reader.IsDBNull(2))
                        {
                            movie.AgeRating = null;
                        }
                        else
                        {
                            movie.AgeRating = reader.GetString(2);
                        }

                        if (reader.IsDBNull(3))
                        {
                            movie.Director = null;
                        }
                        else
                        {
                            movie.Director = reader.GetString(3);
                        }

                        string actorsString;
                        if (reader.IsDBNull(4))
                        {
                            actorsString = null;
                        }
                        else
                        {
                            actorsString = reader.GetString(4);
                        }

                        if (actorsString == null)
                        {
                            movie.LeadActors = new string[0];
                        }
                        else
                        {
                            movie.LeadActors = actorsString.Split(',');
                        }

                        if (reader.IsDBNull(5))
                        {
                            movie.Rating = 0;
                        }
                        else
                        {
                            movie.Rating = reader.GetDouble(5);
                        }


                        if (reader.IsDBNull(6))
                        {
                            movie.ReleaseYear = 0;
                        }
                        else
                        {
                            movie.ReleaseYear = reader.GetInt32(6);
                        }

                        if (reader.IsDBNull(7))
                        {
                            movie.Duration = 0;
                        }
                        else
                        {
                            movie.Duration = (int)reader.GetDouble(7);
                        }

                        return movie;
                    }
                }
            }
        }

        public static List<Movie> LoadAllMovies()
        {
            List<Movie> list = new List<Movie>();
            
            using (var connection = Database.GetConnection())
            {
                connection.Open();

                string sql =
                    "SELECT MovieId, Title, Genre, AgeRating, Director, LeadActors, Rating, ReleaseYear, Duration " +
                    "FROM Movies;";

                using (var command = new SQLiteCommand(sql, connection))
                {
                    

                    using (var reader = command.ExecuteReader())
                    {
                        
                        while (reader.Read()) 
                        {
                            Movie movie = new Movie();

                            movie.MovieId = reader.GetInt32(0);

                            movie.Title = reader.GetString(1);

                            movie.Genre = reader.GetString(2);

                            if (reader.IsDBNull(3))
                            {
                                movie.AgeRating = null;
                            }
                            else
                            {
                                movie.AgeRating = reader.GetString(3);
                            }

                            if (reader.IsDBNull(4))
                            {
                                movie.Director = null;
                            }
                            else
                            {
                                movie.Director = reader.GetString(4);
                            }

                            string actorsString;
                            if (reader.IsDBNull(5))
                            {
                                actorsString = null;
                            }
                            else
                            {
                                actorsString = reader.GetString(5);
                            }

                            if (actorsString == null)
                            {
                                movie.LeadActors = new string[0];
                            }
                            else
                            {
                                movie.LeadActors = actorsString.Split(',');
                            }

                            if (reader.IsDBNull(6))
                            {
                                movie.Rating = 0;
                            }
                            else
                            {
                                movie.Rating = reader.GetDouble(6);
                            }


                            if (reader.IsDBNull(7))
                            {
                                movie.ReleaseYear = 0;
                            }
                            else
                            {
                                movie.ReleaseYear = reader.GetInt32(7);
                            }

                            if (reader.IsDBNull(8))
                            {
                                movie.Duration = 0;
                            }
                            else
                            {
                                movie.Duration = (int)reader.GetDouble(8);
                            }

                            list.Add(movie);
                        }
                    }
                }

                return list;
            }
        }

        private static List<Movie> LoadMoviesForUser(int userId, string table)
        {
            List<Movie> movies = new List<Movie>();
            using (var connection = GetConnection())
            {
                connection.Open();
                string sql = $"SELECT MovieID FROM User{table} WHERE UserID = @uid;";
                using (var command = new SQLiteCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@uid", userId);

                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            int movieId = reader.GetInt32(0);
                            Movie movie = null;
                            foreach (Movie m in Program.movies)
                            {
                                if (m.MovieId == movieId)
                                {
                                    movie = m;
                                    break;
                                }
                            }

                            if (movie != null)
                            {
                                movies.Add(movie);
                            }
                        }
                    }
                }
            }
            return movies;
        }

        private static Dictionary<Movie, double> LoadRatingsForUser(int userId)
        {
            var ratings = new Dictionary<Movie, double>();
            using (var connection = GetConnection())
            {
                connection.Open();
                string sql = "SELECT MovieID, Rating FROM UserRatings WHERE UserID = @uid;";
                using (var command = new SQLiteCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@uid", userId);

                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            int movieId = reader.GetInt32(0);
                            double rating = reader.GetDouble(1);

                            Movie movie = LoadMovieByID(movieId);
                            if (movie != null)
                                ratings[movie] = rating;
                        }
                    }
                }
            }
            return ratings;
        }

        private static Dictionary<Movie, string> LoadCommentsForUser(int userId)
        {
            var comments = new Dictionary<Movie, string>();
            using (var connection = GetConnection())
            {
                connection.Open();
                string sql = "SELECT MovieID, Comment FROM UserComments WHERE UserID = @uid;";
                using (var command = new SQLiteCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@uid", userId);

                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            int movieId = reader.GetInt32(0);
                            string text = reader.GetString(1);

                            Movie movie = LoadMovieByID(movieId);
                            if (movie != null)
                                comments[movie] = text;
                        }
                    }
                }
            }
            return comments;
        }


        public static void AddMovieToFavourites(int userId, int movieId)
        {
            if (userId <= 0) throw new ArgumentException("Invalid user ID.", "userId");
            if (movieId <= 0) throw new ArgumentException("Invalid movie ID.", "movieId");

            using (var connection = GetConnection())
            {
                connection.Open();

                string sql =
                    "INSERT OR IGNORE INTO UserFavourites (UserID, MovieID) " +
                    "VALUES (@uid, @mid);";

                using (var command = new SQLiteCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@uid", userId);
                    command.Parameters.AddWithValue("@mid", movieId);
                    command.ExecuteNonQuery();
                }
            }
        }


        public static void AddMovieToWatched(int userId, int movieId)
        {
            if (userId <= 0) throw new ArgumentException("Invalid user ID.", "userId");
            if (movieId <= 0) throw new ArgumentException("Invalid movie ID.", "movieId");

            using (var connection = GetConnection())
            {
                connection.Open();

                string sql =
                    "INSERT OR IGNORE INTO UserWatched (UserID, MovieID) " +
                    "VALUES (@uid, @mid);";

                using (var command = new SQLiteCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@uid", userId);
                    command.Parameters.AddWithValue("@mid", movieId);
                    command.ExecuteNonQuery();
                }
            }
        }

        public static void AddMovieToWatchList(int userId, int movieId)
        {
            if (userId <= 0) throw new ArgumentException("Invalid user ID.", "userId");
            if (movieId <= 0) throw new ArgumentException("Invalid movie ID.", "movieId");

            using (var connection = GetConnection())
            {
                connection.Open();

                string sql =
                    "INSERT OR IGNORE INTO UserWatchlist (UserID, MovieID) " +
                    "VALUES (@uid, @mid);";

                using (var command = new SQLiteCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@uid", userId);
                    command.Parameters.AddWithValue("@mid", movieId);
                    command.ExecuteNonQuery();
                }
            }
        }

        public static void AddOrUpdateRating(int userId, int movieId, double ratingValue)
        {
            if (userId <= 0) throw new ArgumentException("Invalid user ID.", "userId");
            if (movieId <= 0) throw new ArgumentException("Invalid movie ID.", "movieId");
            if (ratingValue < 0 || ratingValue > 10)
                throw new ArgumentOutOfRangeException(nameof(ratingValue), "Rating must be between 0 and 10.");

            using (var connection = GetConnection())
            {
                connection.Open();

                string sql =
                    "INSERT OR REPLACE INTO UserRatings (UserID, MovieID, Rating) " +
                    "VALUES (@uid, @mid, @rating);";

                using (var command = new SQLiteCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@uid", userId);
                    command.Parameters.AddWithValue("@mid", movieId);
                    command.Parameters.AddWithValue("@rating", ratingValue);

                    command.ExecuteNonQuery();
                }
            }
        }

        public static void RemoveRating(int userId, int movieId)
        {
            if (userId <= 0) throw new ArgumentException("Invalid user ID.", "userId");
            if (movieId <= 0) throw new ArgumentException("Invalid movie ID.", "movieId");
            

            using (var connection = GetConnection())
            {
                connection.Open();

                string sql =
                    "DELETE FROM UserRatings WHERE UserID = @uid AND MovieID = @mid;";

                using (var command = new SQLiteCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@uid", userId);
                    command.Parameters.AddWithValue("@mid", movieId);
                    command.ExecuteNonQuery();
                }
            }
        }

        public static void AddOrUpdateComment(int userId, int movieId, string commentText)
        {
            if (userId <= 0) throw new ArgumentException("Invalid user ID.", "userId");
            if (movieId <= 0) throw new ArgumentException("Invalid movie ID.", "movieId");
            if (string.IsNullOrWhiteSpace(commentText)) throw new ArgumentException("Comment cannot be empty.", "commentText");

            using (var connection = GetConnection())
            {
                connection.Open();

                string sql =
                    "INSERT OR REPLACE INTO UserComments (UserID, MovieID, Comment) " +
                    "VALUES (@uid, @mid, @comment);";

                using (var command = new SQLiteCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@uid", userId);
                    command.Parameters.AddWithValue("@mid", movieId);
                    command.Parameters.AddWithValue("@comment", commentText);

                    command.ExecuteNonQuery();
                }
            }
        }

        public static void RemoveComment(int userId, int movieId)
        {
            if (userId <= 0) throw new ArgumentException("Invalid user ID.", "userId");
            if (movieId <= 0) throw new ArgumentException("Invalid movie ID.", "movieId");
            

            using (var connection = GetConnection())
            {
                connection.Open();

                string sql =
                    "DELETE FROM UserComments WHERE UserID = @uid AND MovieID = @mid;";

                using (var command = new SQLiteCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@uid", userId);
                    command.Parameters.AddWithValue("@mid", movieId);
                    command.ExecuteNonQuery();
                }
            }
        }

        public static void RemoveMovieFromFavourites(int userId, int movieId)
        {
            if (userId <= 0) throw new ArgumentException("Invalid user ID.", "userId");
            if (movieId <= 0) throw new ArgumentException("Invalid movie ID.", "movieId");

            using (var connection = GetConnection())
            {
                connection.Open();

                string sql =
                    "DELETE FROM UserFavourites WHERE UserID = @uid AND MovieID = @mid;";

                using (var command = new SQLiteCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@uid", userId);
                    command.Parameters.AddWithValue("@mid", movieId);
                    command.ExecuteNonQuery();
                }
            }
        }


        public static void RemoveMovieFromWatched(int userId, int movieId)
        {
            if (userId <= 0) throw new ArgumentException("Invalid user ID.", "userId");
            if (movieId <= 0) throw new ArgumentException("Invalid movie ID.", "movieId");

            using (var connection = GetConnection())
            {
                connection.Open();

                string sql =
                    "DELETE FROM UserWatched WHERE UserID = @uid AND MovieID = @mid;";

                using (var command = new SQLiteCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@uid", userId);
                    command.Parameters.AddWithValue("@mid", movieId);
                    command.ExecuteNonQuery();
                }
            }
        }



        public static void RemoveMovieFromWatchlist(int userId, int movieId)
        {
            if (userId <= 0) throw new ArgumentException("Invalid user ID.", "userId");
            if (movieId <= 0) throw new ArgumentException("Invalid movie ID.", "movieId");

            using (var connection = GetConnection())
            {
                connection.Open();

                string sql =
                    "DELETE FROM UserWatchlist WHERE UserID = @uid AND MovieID = @mid;";

                using (var command = new SQLiteCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@uid", userId);
                    command.Parameters.AddWithValue("@mid", movieId);
                    command.ExecuteNonQuery();
                }
            }
        }


        public static void AddMovieToDB(Movie movie)
        {
            using (var connection = new SQLiteConnection("Data Source=database.db;Version=3;"))
            {
                connection.Open();

                using (var pragma = new SQLiteCommand("PRAGMA foreign_keys = ON;", connection))
                {
                    pragma.ExecuteNonQuery();
                }

                string actors = string.Join(",", movie.LeadActors);

                string sql = "INSERT OR IGNORE INTO Movies (Title, Genre, AgeRating, Director, LeadActors, Rating, ReleaseYear, Duration) VALUES (@Title, @Genre, @AgeRating, @Director, @LeadActors, @Rating, @ReleaseYear, @Duration);";

                using (var command = new SQLiteCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@Title", movie.Title);
                    command.Parameters.AddWithValue("@Genre", movie.Genre);
                    command.Parameters.AddWithValue("@AgeRating", movie.AgeRating);
                    command.Parameters.AddWithValue("@Director", movie.Director);
                    command.Parameters.AddWithValue("@LeadActors", actors);
                    command.Parameters.AddWithValue("@Rating", movie.Rating);
                    command.Parameters.AddWithValue("@ReleaseYear", movie.ReleaseYear);
                    command.Parameters.AddWithValue("@Duration", movie.Duration);

                    command.ExecuteNonQuery();
                }
            }
        }

        public static void AddUser(User user)
        {
            if (user == null) throw new ArgumentNullException("user");

            using (var connection = GetConnection())
            {
                connection.Open();
                string sql = "INSERT INTO Users (Username, PasswordHash) VALUES (@Username, @PasswordHash);";
                using (var cmd = new SQLiteCommand(sql, connection))
                {
                    cmd.Parameters.AddWithValue("@Username", user.username);
                    cmd.Parameters.AddWithValue("@PasswordHash", user.PasswordHash);
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public static int GetUserIdByUsername(string username)
        {
            if (string.IsNullOrWhiteSpace(username))
                throw new ArgumentException("Username cannot be empty.", "username");

            using (var connection = GetConnection())
            {
                connection.Open();

                string sql = "SELECT UserID FROM Users WHERE Username = @username LIMIT 1;";

                using (var command = new SQLiteCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@username", username);

                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read()) 
                        {
                            return reader.GetInt32(0); 
                        }
                        else
                        {
                            return -1; 
                        }
                    }
                }
            }
        }


        public static User LoadUserByUsername(string username)
        {
            if (string.IsNullOrWhiteSpace(username)) throw new ArgumentException("Username cannot be empty.", nameof(username));

            using (var connection = GetConnection())
            {
                connection.Open();

                string sql = "SELECT UserID, Username, PasswordHash FROM Users WHERE Username = @username LIMIT 1;";

                using (var command = new SQLiteCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@username", username);

                    using (var reader = command.ExecuteReader())
                    {
                        if (!reader.Read())
                        {
                            return null; 
                        }
                        int userId = reader.GetInt32(0);
                        string userNameFromDb = reader.GetString(1);
                        string passwordHash = reader.GetString(2);
                        User user = new User(userId, userNameFromDb, passwordHash);

                        

                        

                        user.WatchedList = LoadMoviesForUser(userId, "Watched");
                        user.WatchList = LoadMoviesForUser(userId, "WatchList");
                        user.Favourites = LoadMoviesForUser(userId, "Favourites");

                        user.Ratings = LoadRatingsForUser(userId);
                        user.Comments = LoadCommentsForUser(userId);

                        return user;
                    }
                }
            }
        }


    }
}
