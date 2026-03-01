//using System;
//using System.Collections.Generic;
//using System.Data.SQLite;
//using System.Linq;
//using System.Text;

//namespace nea
//{
//    internal class Database1
//    {
//        public static SQLiteConnection GetConnection1()
//        {
//            return new SQLiteConnection("Data Source=movies.db;Version=3;");
//        }

//        public static void CreateTables()
//        {
//            using (var connection = GetConnection1())
//            {
//                connection.Open();

//                // Turn on foreign keys (good practice)
//                using (var pragma = new SQLiteCommand("PRAGMA foreign_keys = ON;", connection))
//                {
//                    pragma.ExecuteNonQuery();
//                }

//                // USERS TABLE
//                string CreateUsersTable = "CREATE TABLE IF NOT EXISTS Users (" +
//                    "UserID INTEGER PRIMARY KEY AUTOINCREMENT, " +
//                    "Username TEXT NOT NULL UNIQUE, " +
//                    "PasswordHash TEXT NOT NULL" +
//                    ");";
//                using (var command = new SQLiteCommand(CreateUsersTable, connection))
//                {
//                    command.ExecuteNonQuery();
//                }

//                // MOVIES TABLE
//                // LeadActors stored as 'actor1|actor2|actor3' (pipe-separated)
//                string CreateMoviesTable = "CREATE TABLE IF NOT EXISTS Movies (" +
//                    "MovieID INTEGER PRIMARY KEY AUTOINCREMENT, " +
//                    "Title TEXT NOT NULL, " +
//                    "Genre TEXT, " +
//                    "AgeRating TEXT, " +
//                    "Director TEXT, " +
//                    "LeadActors TEXT, " +
//                    "Rating REAL, " +
//                    "ReleaseYear INTEGER, " +
//                    "Duration REAL" +
//                    ");";
//                using (var command = new SQLiteCommand(CreateMoviesTable, connection))
//                {
//                    command.ExecuteNonQuery();
//                }

//                // USER WATCHLIST
//                string CreateWatchList = "CREATE TABLE IF NOT EXISTS UserWatchList (" +
//                    "UserID INTEGER NOT NULL, " +
//                    "MovieID INTEGER NOT NULL, " +
//                    "PRIMARY KEY(UserID, MovieID), " +
//                    "FOREIGN KEY(UserID) REFERENCES Users(UserID) ON DELETE CASCADE, " +
//                    "FOREIGN KEY(MovieID) REFERENCES Movies(MovieID) ON DELETE CASCADE" +
//                    ");";
//                using (var command = new SQLiteCommand(CreateWatchList, connection))
//                {
//                    command.ExecuteNonQuery();
//                }

//                // USER FAVOURITES
//                string CreateFavourites = "CREATE TABLE IF NOT EXISTS UserFavourites (" +
//                    "UserID INTEGER NOT NULL, " +
//                    "MovieID INTEGER NOT NULL, " +
//                    "PRIMARY KEY(UserID, MovieID), " +
//                    "FOREIGN KEY(UserID) REFERENCES Users(UserID) ON DELETE CASCADE, " +
//                    "FOREIGN KEY(MovieID) REFERENCES Movies(MovieID) ON DELETE CASCADE" +
//                    ");";
//                using (var command = new SQLiteCommand(CreateFavourites, connection))
//                {
//                    command.ExecuteNonQuery();
//                }

//                // USER WATCHED MOVIES
//                string CreateWatched = "CREATE TABLE IF NOT EXISTS UserWatched (" +
//                    "UserID INTEGER NOT NULL, " +
//                    "MovieID INTEGER NOT NULL, " +
//                    "PRIMARY KEY(UserID, MovieID), " +
//                    "FOREIGN KEY(UserID) REFERENCES Users(UserID) ON DELETE CASCADE, " +
//                    "FOREIGN KEY(MovieID) REFERENCES Movies(MovieID) ON DELETE CASCADE" +
//                    ");";
//                using (var command = new SQLiteCommand(CreateWatched, connection))
//                {
//                    command.ExecuteNonQuery();
//                }

//                // USER RATINGS (one rating per user per movie)
//                string CreateRatings = "CREATE TABLE IF NOT EXISTS UserRatings (" +
//                    "UserID INTEGER NOT NULL, " +
//                    "MovieID INTEGER NOT NULL, " +
//                    "Rating REAL NOT NULL, " +
//                    "PRIMARY KEY(UserID, MovieID), " +
//                    "FOREIGN KEY(UserID) REFERENCES Users(UserID) ON DELETE CASCADE, " +
//                    "FOREIGN KEY(MovieID) REFERENCES Movies(MovieID) ON DELETE CASCADE" +
//                    ");";
//                using (var command = new SQLiteCommand(CreateRatings, connection))
//                {
//                    command.ExecuteNonQuery();
//                }

//                // USER COMMENTS (one comment per user per movie)
//                string CreateComments = "CREATE TABLE IF NOT EXISTS UserComments (" +
//                    "UserID INTEGER NOT NULL, " +
//                    "MovieID INTEGER NOT NULL, " +
//                    "Comment TEXT, " +
//                    "PRIMARY KEY(UserID, MovieID), " +
//                    "FOREIGN KEY(UserID) REFERENCES Users(UserID) ON DELETE CASCADE, " +
//                    "FOREIGN KEY(MovieID) REFERENCES Movies(MovieID) ON DELETE CASCADE" +
//                    ");";
//                using (var command = new SQLiteCommand(CreateComments, connection))
//                {
//                    command.ExecuteNonQuery();
//                }

//                Console.WriteLine("Database tables created successfully.");
//            }
//        }

//        // ----------------------------
//        // A: AddMovie(Movie movie)
//        // - ensures no duplicate by Title + ReleaseYear
//        // - if exists, sets movie.MovieID to existing id and returns it
//        // - if not, inserts and sets movie.MovieID
//        // ----------------------------
//        public static int AddMovie(Movie movie)
//        {
//            if (movie == null) throw new ArgumentNullException(nameof(movie));

//            using (var connection = GetConnection())
//            {
//                connection.Open();

//                // 1) Check if movie exists by Title + ReleaseYear (basic dedupe)
//                string checkSql = "SELECT MovieID FROM Movies WHERE Title = @Title AND ReleaseYear = @ReleaseYear LIMIT 1;";
//                using (var cmd = new SQLiteCommand(checkSql, connection))
//                {
//                    cmd.Parameters.AddWithValue("@Title", movie.Title ?? "");
//                    cmd.Parameters.AddWithValue("@ReleaseYear", movie.ReleaseYear);
//                    var obj = cmd.ExecuteScalar();
//                    if (obj != null && obj != DBNull.Value)
//                    {
//                        int existingId = Convert.ToInt32(obj);
//                        // set MovieID on the object if you want (we don't have a MovieID field in your Movie class yet)
//                        // If you add MovieID to class later, you can set it here.
//                        return existingId;
//                    }
//                }

//                // 2) Insert movie
//                string leadActorsText = "";
//                if (movie.LeadActors != null && movie.LeadActors.Length > 0)
//                {
//                    // join with pipe to avoid issues with commas inside actor names
//                    leadActorsText = string.Join("|", movie.LeadActors);
//                }

//                string insertSql = "INSERT INTO Movies (Title, Genre, AgeRating, Director, LeadActors, Rating, ReleaseYear, Duration) " +
//                    "VALUES (@Title, @Genre, @AgeRating, @Director, @LeadActors, @Rating, @ReleaseYear, @Duration);";

//                using (var cmd = new SQLiteCommand(insertSql, connection))
//                {
//                    cmd.Parameters.AddWithValue("@Title", movie.Title ?? "");
//                    cmd.Parameters.AddWithValue("@Genre", movie.Genre ?? "");
//                    cmd.Parameters.AddWithValue("@AgeRating", movie.AgeRating ?? "");
//                    cmd.Parameters.AddWithValue("@Director", movie.Director ?? "");
//                    cmd.Parameters.AddWithValue("@LeadActors", leadActorsText);
//                    cmd.Parameters.AddWithValue("@Rating", movie.Rating);
//                    cmd.Parameters.AddWithValue("@ReleaseYear", movie.ReleaseYear);
//                    cmd.Parameters.AddWithValue("@Duration", movie.Duration);

//                    cmd.ExecuteNonQuery();
//                }

//                // 3) Get last inserted id
//                using (var cmd = new SQLiteCommand("SELECT last_insert_rowid();", connection))
//                {
//                    long lastId = (long)cmd.ExecuteScalar();
//                    return (int)lastId;
//                }
//            }
//        }

//        // ----------------------------
//        // Helper: GetMovieById(int id)
//        // ----------------------------
//        public static Movie GetMovieById(int id)
//        {
//            using (var connection = GetConnection())
//            {
//                connection.Open();
//                string sql = "SELECT MovieID, Title, Genre, AgeRating, Director, LeadActors, Rating, ReleaseYear, Duration FROM Movies WHERE MovieID = @MovieID LIMIT 1;";
//                using (var cmd = new SQLiteCommand(sql, connection))
//                {
//                    cmd.Parameters.AddWithValue("@MovieID", id);
//                    using (var reader = cmd.ExecuteReader())
//                    {
//                        if (reader.Read())
//                        {
//                            Movie m = new Movie();
//                            // If you later add MovieID field to Movie class, set it here
//                            m.Title = reader["Title"]?.ToString();
//                            m.Genre = reader["Genre"]?.ToString();
//                            m.AgeRating = reader["AgeRating"]?.ToString();
//                            m.Director = reader["Director"]?.ToString();
//                            string leadActorsText = reader["LeadActors"]?.ToString() ?? "";
//                            if (!string.IsNullOrEmpty(leadActorsText))
//                                m.LeadActors = leadActorsText.Split('|');
//                            else
//                                m.LeadActors = new string[0];
//                            m.Rating = reader["Rating"] == DBNull.Value ? 0.0 : Convert.ToDouble(reader["Rating"]);
//                            m.ReleaseYear = reader["ReleaseYear"] == DBNull.Value ? 0 : Convert.ToInt32(reader["ReleaseYear"]);
//                            m.Duration = reader["Duration"] == DBNull.Value ? 0.0 : Convert.ToDouble(reader["Duration"]);
//                            return m;
//                        }
//                    }
//                }
//            }
//            return null;
//        }

//        // ----------------------------
//        // B: LoadUserByUsername(string username)
//        //    returns User with Username & PasswordHash filled (lists empty)
//        // ----------------------------
//        public static User LoadUserByUsername(string username)
//        {
//            using (var connection = GetConnection())
//            {
//                connection.Open();
//                string sql = "SELECT UserID, Username, PasswordHash FROM Users WHERE Username = @Username LIMIT 1;";
//                using (var cmd = new SQLiteCommand(sql, connection))
//                {
//                    cmd.Parameters.AddWithValue("@Username", username);
//                    using (var reader = cmd.ExecuteReader())
//                    {
//                        if (reader.Read())
//                        {
//                            string uname = reader["Username"].ToString();
//                            string pHash = reader["PasswordHash"].ToString();
//                            // Create user object with dummy password; replace PasswordHash directly
//                            User u = new User(uname, "temporaryPasswordThatWillBeReplaced");
//                            u.PasswordHash = pHash;
//                            // Clear collections (constructor already created them)
//                            u.WatchedList = new List<Movie>();
//                            u.WatchList = new List<Movie>();
//                            u.Favourites = new List<Movie>();
//                            u.Ratings = new Dictionary<Movie, double>();
//                            u.Comments = new Dictionary<Movie, string>();
//                            return u;
//                        }
//                    }
//                }
//            }
//            return null;
//        }

//        // Helper: get user id from username (returns -1 if not found)
//        private static int GetUserIdByUsername(string username)
//        {
//            using (var connection = GetConnection())
//            {
//                connection.Open();
//                string sql = "SELECT UserID FROM Users WHERE Username = @Username LIMIT 1;";
//                using (var cmd = new SQLiteCommand(sql, connection))
//                {
//                    cmd.Parameters.AddWithValue("@Username", username);
//                    var obj = cmd.ExecuteScalar();
//                    if (obj != null && obj != DBNull.Value)
//                        return Convert.ToInt32(obj);
//                }
//            }
//            return -1;
//        }

//        // ----------------------------
//        // AddUser(User user)
//        // ----------------------------
//        public static void AddUser(User user)
//        {
//            if (user == null) throw new ArgumentNullException("user");

//            using (var connection = GetConnection())
//            {
//                connection.Open();
//                string sql = "INSERT INTO Users (Username, PasswordHash) VALUES (@Username, @PasswordHash);";
//                using (var cmd = new SQLiteCommand(sql, connection))
//                {
//                    cmd.Parameters.AddWithValue("@Username", user.username);
//                    cmd.Parameters.AddWithValue("@PasswordHash", user.PasswordHash);
//                    cmd.ExecuteNonQuery();
//                }
//            }
//        }

//        // ----------------------------
//        // C: Add movie to user's watchlist
//        // ----------------------------
//        public static void AddToWatchlist(User user, Movie movie)
//        {
//            if (user == null || movie == null) throw new ArgumentNullException();

//            int userId = GetUserIdByUsername(user.username);
//            if (userId == -1) throw new Exception("User not found in database.");

//            // Ensure movie exists in DB and get movieId
//            int movieId = AddMovie(movie);

//            using (var connection = GetConnection())
//            {
//                connection.Open();
//                string sql = "INSERT OR IGNORE INTO UserWatchList (UserID, MovieID) VALUES (@UserID, @MovieID);";
//                using (var cmd = new SQLiteCommand(sql, connection))
//                {
//                    cmd.Parameters.AddWithValue("@UserID", userId);
//                    cmd.Parameters.AddWithValue("@MovieID", movieId);
//                    cmd.ExecuteNonQuery();
//                }
//            }
//        }

//        // ----------------------------
//        // D: Add movie to user's favourites
//        // ----------------------------
//        public static void AddToFavourites(User user, Movie movie)
//        {
//            if (user == null || movie == null) throw new ArgumentNullException();

//            int userId = GetUserIdByUsername(user.username);
//            if (userId == -1) throw new Exception("User not found in database.");

//            int movieId = AddMovie(movie);

//            using (var connection = GetConnection())
//            {
//                connection.Open();
//                string sql = "INSERT OR IGNORE INTO UserFavourites (UserID, MovieID) VALUES (@UserID, @MovieID);";
//                using (var cmd = new SQLiteCommand(sql, connection))
//                {
//                    cmd.Parameters.AddWithValue("@UserID", userId);
//                    cmd.Parameters.AddWithValue("@MovieID", movieId);
//                    cmd.ExecuteNonQuery();
//                }
//            }
//        }

//        // ----------------------------
//        // E: Add movie to user's watched list
//        // ----------------------------
//        public static void AddToWatched(User user, Movie movie)
//        {
//            if (user == null || movie == null) throw new ArgumentNullException();

//            int userId = GetUserIdByUsername(user.username);
//            if (userId == -1) throw new Exception("User not found in database.");

//            int movieId = AddMovie(movie);

//            using (var connection = GetConnection())
//            {
//                connection.Open();
//                string sql = "INSERT OR IGNORE INTO UserWatched (UserID, MovieID) VALUES (@UserID, @MovieID);";
//                using (var cmd = new SQLiteCommand(sql, connection))
//                {
//                    cmd.Parameters.AddWithValue("@UserID", userId);
//                    cmd.Parameters.AddWithValue("@MovieID", movieId);
//                    cmd.ExecuteNonQuery();
//                }
//            }
//        }

//        // ----------------------------
//        // F: Save/Update rating for a user
//        // ----------------------------
//        public static void SaveRating(User user, Movie movie, double rating)
//        {
//            if (user == null || movie == null) throw new ArgumentNullException();

//            int userId = GetUserIdByUsername(user.username);
//            if (userId == -1) throw new Exception("User not found in database.");

//            int movieId = AddMovie(movie);

//            using (var connection = GetConnection())
//            {
//                connection.Open();
//                // INSERT or REPLACE to update rating if it already exists
//                string sql = "INSERT OR REPLACE INTO UserRatings (UserID, MovieID, Rating) VALUES (@UserID, @MovieID, @Rating);";
//                using (var cmd = new SQLiteCommand(sql, connection))
//                {
//                    cmd.Parameters.AddWithValue("@UserID", userId);
//                    cmd.Parameters.AddWithValue("@MovieID", movieId);
//                    cmd.Parameters.AddWithValue("@Rating", rating);
//                    cmd.ExecuteNonQuery();
//                }
//            }
//        }

//        // ----------------------------
//        // G: Save/Update comment for a user
//        // ----------------------------
//        public static void SaveComment(User user, Movie movie, string comment)
//        {
//            if (user == null || movie == null) throw new ArgumentNullException();

//            int userId = GetUserIdByUsername(user.username);
//            if (userId == -1) throw new Exception("User not found in database.");

//            int movieId = AddMovie(movie);

//            using (var connection = GetConnection())
//            {
//                connection.Open();
//                // INSERT or REPLACE to update comment if it exists
//                string sql = "INSERT OR REPLACE INTO UserComments (UserID, MovieID, Comment) VALUES (@UserID, @MovieID, @Comment);";
//                using (var cmd = new SQLiteCommand(sql, connection))
//                {
//                    cmd.Parameters.AddWithValue("@UserID", userId);
//                    cmd.Parameters.AddWithValue("@MovieID", movieId);
//                    cmd.Parameters.AddWithValue("@Comment", comment ?? "");
//                    cmd.ExecuteNonQuery();
//                }
//            }
//        }

//        // ----------------------------
//        // H: Load full user profile into the passed User object
//        //    (fills WatchedList, WatchList, Favourites, Ratings, Comments)
//        // ----------------------------
//        public static void LoadUserProfile(User user)
//        {
//            if (user == null) throw new ArgumentNullException(nameof(user));

//            int userId = GetUserIdByUsername(user.username);
//            if (userId == -1) throw new Exception("User not found in database.");

//            user.WatchedList = new List<Movie>();
//            user.WatchList = new List<Movie>();
//            user.Favourites = new List<Movie>();
//            user.Ratings = new Dictionary<Movie, double>();
//            user.Comments = new Dictionary<Movie, string>();

//            using (var connection = GetConnection())
//            {
//                connection.Open();

//                // Load WatchList
//                string watchSql = "SELECT m.MovieID, m.Title, m.Genre, m.AgeRating, m.Director, m.LeadActors, m.Rating, m.ReleaseYear, m.Duration " +
//                    "FROM Movies m JOIN UserWatchList uw ON m.MovieID = uw.MovieID WHERE uw.UserID = @UserID;";
//                using (var cmd = new SQLiteCommand(watchSql, connection))
//                {
//                    cmd.Parameters.AddWithValue("@UserID", userId);
//                    using (var reader = cmd.ExecuteReader())
//                    {
//                        while (reader.Read())
//                        {
//                            Movie m = new Movie();
//                            m.Title = reader["Title"]?.ToString();
//                            m.Genre = reader["Genre"]?.ToString();
//                            m.AgeRating = reader["AgeRating"]?.ToString();
//                            m.Director = reader["Director"]?.ToString();
//                            string leadActorsText = reader["LeadActors"]?.ToString() ?? "";
//                            m.LeadActors = string.IsNullOrEmpty(leadActorsText) ? new string[0] : leadActorsText.Split('|');
//                            m.Rating = reader["Rating"] == DBNull.Value ? 0.0 : Convert.ToDouble(reader["Rating"]);
//                            m.ReleaseYear = reader["ReleaseYear"] == DBNull.Value ? 0 : Convert.ToInt32(reader["ReleaseYear"]);
//                            m.Duration = reader["Duration"] == DBNull.Value ? 0.0 : Convert.ToDouble(reader["Duration"]);
//                            user.WatchList.Add(m);
//                        }
//                    }
//                }

//                // Load WatchedList
//                string watchedSql = "SELECT m.MovieID, m.Title, m.Genre, m.AgeRating, m.Director, m.LeadActors, m.Rating, m.ReleaseYear, m.Duration " +
//                    "FROM Movies m JOIN UserWatched uw ON m.MovieID = uw.MovieID WHERE uw.UserID = @UserID;";
//                using (var cmd = new SQLiteCommand(watchedSql, connection))
//                {
//                    cmd.Parameters.AddWithValue("@UserID", userId);
//                    using (var reader = cmd.ExecuteReader())
//                    {
//                        while (reader.Read())
//                        {
//                            Movie m = new Movie();
//                            m.Title = reader["Title"]?.ToString();
//                            m.Genre = reader["Genre"]?.ToString();
//                            m.AgeRating = reader["AgeRating"]?.ToString();
//                            m.Director = reader["Director"]?.ToString();
//                            string leadActorsText = reader["LeadActors"]?.ToString() ?? "";
//                            m.LeadActors = string.IsNullOrEmpty(leadActorsText) ? new string[0] : leadActorsText.Split('|');
//                            m.Rating = reader["Rating"] == DBNull.Value ? 0.0 : Convert.ToDouble(reader["Rating"]);
//                            m.ReleaseYear = reader["ReleaseYear"] == DBNull.Value ? 0 : Convert.ToInt32(reader["ReleaseYear"]);
//                            m.Duration = reader["Duration"] == DBNull.Value ? 0.0 : Convert.ToDouble(reader["Duration"]);
//                            user.WatchedList.Add(m);
//                        }
//                    }
//                }

//                // Load Favourites
//                string favSql = "SELECT m.MovieID, m.Title, m.Genre, m.AgeRating, m.Director, m.LeadActors, m.Rating, m.ReleaseYear, m.Duration " +
//                    "FROM Movies m JOIN UserFavourites uf ON m.MovieID = uf.MovieID WHERE uf.UserID = @UserID;";
//                using (var cmd = new SQLiteCommand(favSql, connection))
//                {
//                    cmd.Parameters.AddWithValue("@UserID", userId);
//                    using (var reader = cmd.ExecuteReader())
//                    {
//                        while (reader.Read())
//                        {
//                            Movie m = new Movie();
//                            m.Title = reader["Title"]?.ToString();
//                            m.Genre = reader["Genre"]?.ToString();
//                            m.AgeRating = reader["AgeRating"]?.ToString();
//                            m.Director = reader["Director"]?.ToString();
//                            string leadActorsText = reader["LeadActors"]?.ToString() ?? "";
//                            m.LeadActors = string.IsNullOrEmpty(leadActorsText) ? new string[0] : leadActorsText.Split('|');
//                            m.Rating = reader["Rating"] == DBNull.Value ? 0.0 : Convert.ToDouble(reader["Rating"]);
//                            m.ReleaseYear = reader["ReleaseYear"] == DBNull.Value ? 0 : Convert.ToInt32(reader["ReleaseYear"]);
//                            m.Duration = reader["Duration"] == DBNull.Value ? 0.0 : Convert.ToDouble(reader["Duration"]);
//                            user.Favourites.Add(m);
//                        }
//                    }
//                }

//                // Load Ratings
//                string ratingSql = "SELECT r.MovieID, r.Rating, m.Title, m.Genre, m.AgeRating, m.Director, m.LeadActors, m.ReleaseYear, m.Duration " +
//                    "FROM UserRatings r JOIN Movies m ON r.MovieID = m.MovieID WHERE r.UserID = @UserID;";
//                using (var cmd = new SQLiteCommand(ratingSql, connection))
//                {
//                    cmd.Parameters.AddWithValue("@UserID", userId);
//                    using (var reader = cmd.ExecuteReader())
//                    {
//                        while (reader.Read())
//                        {
//                            Movie m = new Movie();
//                            m.Title = reader["Title"]?.ToString();
//                            m.Genre = reader["Genre"]?.ToString();
//                            m.AgeRating = reader["AgeRating"]?.ToString();
//                            m.Director = reader["Director"]?.ToString();
//                            string leadActorsText = reader["LeadActors"]?.ToString() ?? "";
//                            m.LeadActors = string.IsNullOrEmpty(leadActorsText) ? new string[0] : leadActorsText.Split('|');
//                            m.Rating = reader["Rating"] == DBNull.Value ? 0.0 : Convert.ToDouble(reader["Rating"]); // movie global rating
//                            m.ReleaseYear = reader["ReleaseYear"] == DBNull.Value ? 0 : Convert.ToInt32(reader["ReleaseYear"]);
//                            m.Duration = reader["Duration"] == DBNull.Value ? 0.0 : Convert.ToDouble(reader["Duration"]);

//                            double userRating = reader["Rating"] == DBNull.Value ? 0.0 : Convert.ToDouble(reader["Rating"]);
//                            // Note: to avoid confusion between movie.Rating and user's rating, we'll use user's rating from the r.Rating column
//                            user.Ratings[m] = userRating;
//                        }
//                    }
//                }

//                // Load Comments
//                string commentSql = "SELECT c.MovieID, c.Comment, m.Title, m.Genre, m.AgeRating, m.Director, m.LeadActors, m.ReleaseYear, m.Duration " +
//                    "FROM UserComments c JOIN Movies m ON c.MovieID = m.MovieID WHERE c.UserID = @UserID;";
//                using (var cmd = new SQLiteCommand(commentSql, connection))
//                {
//                    cmd.Parameters.AddWithValue("@UserID", userId);
//                    using (var reader = cmd.ExecuteReader())
//                    {
//                        while (reader.Read())
//                        {
//                            Movie m = new Movie();
//                            m.Title = reader["Title"]?.ToString();
//                            m.Genre = reader["Genre"]?.ToString();
//                            m.AgeRating = reader["AgeRating"]?.ToString();
//                            m.Director = reader["Director"]?.ToString();
//                            string leadActorsText = reader["LeadActors"]?.ToString() ?? "";
//                            m.LeadActors = string.IsNullOrEmpty(leadActorsText) ? new string[0] : leadActorsText.Split('|');
//                            m.Rating = 0.0;
//                            m.ReleaseYear = reader["ReleaseYear"] == DBNull.Value ? 0 : Convert.ToInt32(reader["ReleaseYear"]);
//                            m.Duration = reader["Duration"] == DBNull.Value ? 0.0 : Convert.ToDouble(reader["Duration"]);

//                            string comment = reader["Comment"]?.ToString() ?? "";
//                            user.Comments[m] = comment;
//                        }
//                    }
//                }
//            }
//        }
//    }
//}
