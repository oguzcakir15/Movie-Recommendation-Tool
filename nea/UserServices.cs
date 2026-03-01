using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace nea
{
    internal class UserServices
    {
        

        public static void AddtoWatched()
        {
            List<Movie> finds = new List<Movie>();
            while (true)
            {
                Console.Clear();
                Console.WriteLine("Enter the name of the movie you want to add to your watched list:");
                string name = Console.ReadLine().Trim();

                Movie foundmovie = null;
                foreach (Movie m in Program.movies)
                {
                    if (!string.IsNullOrEmpty(m.Title) && m.Title.Trim().ToLower().Contains(name.Trim().ToLower()) == true)
                    {
                        foundmovie = m;
                        break;
                    }
                }
                if (foundmovie == null)
                {
                    Console.WriteLine("Movie not found, please check the title and try again.");
                    Program.Pause();
                    continue;
                }
                Console.WriteLine($"Do you mean {foundmovie.Title}({foundmovie.ReleaseYear}) y/n");
                string input = Console.ReadLine().Trim().ToLower();

                if (input == "y")
                {
                    if (!Program.currentUser.WatchedList.Contains(foundmovie))
                    {
                        Database.AddMovieToWatched(Program.currentUser.UserId, foundmovie.MovieId);
                        Program.currentUser.WatchedList.Add(foundmovie);
                        Console.WriteLine(foundmovie.Title + " has been successfully added to your watched list.");
                    }
                    else
                    {
                        Console.WriteLine(foundmovie.Title + " is already in your watched list.");
                    }
                    Program.Pause();
                    return;
                }
                else if (input == "n")
                {
                    Console.WriteLine("Please try again with a more specific title.");
                    Program.Pause();
                }
                else
                {
                    Console.WriteLine("Invalid input. Please only enter 'y' or 'n'.");
                    Program.Pause();
                }
            }
        }





        

        public static void RemoveFromWatched()
        {
            Database.LoadUserByUsername(Program.currentUser.username);
            Program.movies = Database.LoadAllMovies();

            while (true)
            {
                Console.Clear();
                Console.WriteLine("Enter the name of the movie you want to remove from your watched list:");
                string name = Console.ReadLine().Trim();

                Movie foundMovie = null;
                foreach (Movie m in Program.movies)
                {
                    if (!string.IsNullOrEmpty(m.Title) && m.Title.Trim().ToLower().Contains(name.ToLower()))
                    {
                        foundMovie = m;
                        break;
                    }
                }

                if (foundMovie == null)
                {
                    Console.WriteLine("Movie not found, please check the title and try again.");
                    Program.Pause();
                    continue;
                }

                Console.WriteLine($"Do you mean {foundMovie.Title} ({foundMovie.ReleaseYear})? y/n");
                string input = Console.ReadLine().Trim().ToLower();

                if (input == "y")
                {
                    // Find the movie in the watched list by MovieId (not reference)
                    Movie toRemove = Program.currentUser.WatchedList
                        .FirstOrDefault(m => m.MovieId == foundMovie.MovieId);

                    if (toRemove != null)
                    {
                        Database.RemoveMovieFromWatched(Program.currentUser.UserId, toRemove.MovieId);
                        Program.currentUser.WatchedList.Remove(toRemove);
                        Console.WriteLine($"{toRemove.Title} has been successfully removed from your watched list.");
                    }
                    else
                    {
                        Console.WriteLine($"{foundMovie.Title} is not in your watched list.");
                        Console.WriteLine("Would you like to add it to your watched list? (y/n)");
                        input = Console.ReadLine().Trim().ToLower();

                        if (input == "y")
                        {
                            Database.AddMovieToWatched(Program.currentUser.UserId, foundMovie.MovieId);
                            Program.currentUser.WatchedList.Add(foundMovie);
                            Console.WriteLine($"{foundMovie.Title} has been added to your watched list.");
                        }
                    }

                    Program.Pause();
                    return;
                }
                else if (input == "n")
                {
                    Console.WriteLine("Please try again with a more specific title.");
                    Program.Pause();
                }
                else
                {
                    Console.WriteLine("Invalid input. Please enter 'y' or 'n'.");
                    Program.Pause();
                }
            }
        }


        public static void ViewWatched() 
        {
            Console.Clear();
            Program.currentUser = Database.LoadUserByUsername(Program.currentUser.username);
            if (Program.currentUser.WatchedList.Count > 0)
            {
                Console.WriteLine("----- Your Watched List -----");
                foreach (Movie m in Program.currentUser.WatchedList)
                {
                    Console.WriteLine($"{m.Title} - {m.ReleaseYear} - {m.Duration}");
                }
               
            }
            else 
            {
                Console.WriteLine("Your watched list is empty");
            }
            Console.WriteLine();
            Program.Pause();
        }
        
        
        
        
        
        
        
        public static void AddtoWatchlist() 
        {
            List<Movie> finds = new List<Movie>();
            while (true)
            {
                Console.Clear();
                Console.WriteLine("Enter the name of the movie you want to add to your watchlist:");
                string name = Console.ReadLine().Trim();

                Movie foundmovie = null;
                foreach (Movie m in Program.movies)
                {
                    if (!string.IsNullOrEmpty(m.Title) && m.Title.Trim().ToLower().Contains(name.Trim().ToLower()) == true)
                    {
                        foundmovie = m;
                        break;
                    }
                }
                if (foundmovie == null)
                {
                    Console.WriteLine("Movie not found, please check the title and try again.");
                    Program.Pause();
                    continue;
                }
                Console.WriteLine($"Do you mean {foundmovie.Title}({foundmovie.ReleaseYear}) y/n");
                string input = Console.ReadLine().Trim().ToLower();

                if (input == "y")
                {
                    if (!Program.currentUser.WatchList.Contains(foundmovie))
                    {
                        Database.AddMovieToWatchList(Program.currentUser.UserId, foundmovie.MovieId);
                        Program.currentUser.WatchList.Add(foundmovie);
                        Console.WriteLine(foundmovie.Title + " has been successfully added to your watchlist.");
                    }
                    else
                    {
                        Console.WriteLine(foundmovie.Title + " is already in your watchlist.");
                    }
                    Program.Pause();
                    return;
                }
                else if (input == "n")
                {
                    Console.WriteLine("Please try again with a more specific title.");
                    Program.Pause();
                }
                else
                {
                    Console.WriteLine("Invalid input. Please only enter 'y' or 'n'.");
                    Program.Pause();
                }
            }
        }
        
        
        
        
        
        
        
        public static void RemoveFromWatchlist() 
        {
            List<Movie> finds = new List<Movie>();
            Database.LoadUserByUsername(Program.currentUser.username);
            while (true)
            {
                Console.Clear();
                Console.WriteLine("Enter the name of the movie you want to remove from your watchlist:");
                string name = Console.ReadLine().Trim();

                Movie foundmovie = null;
                foreach (Movie m in Program.movies)
                {
                    if (!string.IsNullOrEmpty(m.Title) && m.Title.Trim().ToLower().Contains(name.Trim().ToLower()) == true)
                    {
                        foundmovie = m;
                        break;
                    }
                }
                if (foundmovie == null)
                {
                    Console.WriteLine("Movie not found, please check the title and try again.");
                    Program.Pause();
                    continue;
                }
                Console.WriteLine($"Do you mean {foundmovie.Title}({foundmovie.ReleaseYear}) y/n");
                string input = Console.ReadLine().Trim().ToLower();

                if (input == "y")
                {
                    if (Program.currentUser.WatchList.Contains(foundmovie))
                    {
                        Database.RemoveMovieFromWatchlist(Program.currentUser.UserId, foundmovie.MovieId);
                        Program.currentUser.WatchList.Remove(foundmovie);
                        Console.WriteLine(foundmovie.Title + " has been successfully removed from your watchlist.");
                    }
                    else
                    {
                        Console.WriteLine(foundmovie.Title + " is not in your watchlist.");
                        while (true)
                        {
                            Console.WriteLine("Would you like to add " + foundmovie.Title + " to your watchlist (y/n)");
                            input = Console.ReadLine().Trim().ToLower();
                            if (input == "y")
                            {
                                Console.WriteLine(foundmovie.Title + " has been added to your watchlist");
                                Program.currentUser.WatchList.Add(foundmovie);
                                Database.AddMovieToWatchList(Program.currentUser.UserId, foundmovie.MovieId);
                            }
                            else if (input == "n")
                            {
                                break;
                            }
                            else
                            {
                                Console.WriteLine("Invalid input. Please only enter 'y' or 'n'.");
                            }
                        }
                    }
                    Program.Pause();
                    return;
                }
                else if (input == "n")
                {
                    Console.WriteLine("Please try again with a more specific title.");
                    Program.Pause();
                }
                else
                {
                    Console.WriteLine("Invalid input. Please only enter 'y' or 'n'.");
                    Program.Pause();
                }
            }
        }
        
        
        
        
        
        
        
        public static void ViewWatchlist() 
        {
            Console.Clear();
            Database.LoadUserByUsername(Program.currentUser.username);

            if (Program.currentUser.WatchList.Count > 0)
            {
                Console.WriteLine("----- Your Watchlist -----");
                foreach (Movie m in Program.currentUser.WatchList)
                {
                    Console.WriteLine($"{m.Title} - {m.ReleaseYear} - {m.Duration}");
                }
                
            }
            else
            {
                Console.WriteLine("Your watchlist is empty");
            }
            Console.WriteLine();
            Program.Pause();
        }
        
        
        
        
        
        
        public static void GiveRating() 
        {
            List<Movie> finds = new List<Movie>();
            while (true)
            {
                Console.Clear();
                Console.WriteLine("Enter the name of the movie you want to rate:");
                string name = Console.ReadLine().Trim();
                
                Movie foundmovie = null;
                foreach (Movie m in Program.movies)
                {
                    if (!string.IsNullOrEmpty(m.Title) && m.Title.Trim().ToLower().Contains(name.Trim().ToLower()) == true)
                    {
                        foundmovie = m;
                        break;
                    }
                }
                if (foundmovie == null)
                {
                    Console.WriteLine("Movie not found, please check the title and try again.");
                    Program.Pause();
                    continue;
                }
                while (true)
                {
                    Console.Clear();
                    Console.WriteLine($"Do you mean {foundmovie.Title}({foundmovie.ReleaseYear}) y/n");
                    string input = Console.ReadLine().Trim().ToLower();
                    string RatingInput;
                    double rating;
                    if (input == "y")
                    {
                        if (!Program.currentUser.Ratings.ContainsKey(foundmovie))
                        {
                            while (true)
                            {
                                Console.Clear();
                                Console.WriteLine($"Enter your rating for {foundmovie.Title}({foundmovie.ReleaseYear}) (0.0 - 10.0):");
                                RatingInput = Console.ReadLine().Trim();
                                if (double.TryParse(RatingInput, out rating) && rating <= 10 && rating >= 0)
                                {
                                    Database.AddOrUpdateRating(Program.currentUser.UserId, foundmovie.MovieId, rating);
                                    Program.currentUser.Ratings[foundmovie] = rating;
                                    Console.WriteLine($"Your rating of {rating} for {foundmovie.Title} has been saved");
                                    Program.Pause();
                                    return;
                                }
                                else
                                {
                                    Console.WriteLine("Please enter a valid rating between 0 and 10");
                                }
                            }
                        }
                        else
                        {
                            
                            while (true)
                            {
                                Console.Clear();
                                Console.WriteLine($"You have already rated {foundmovie.Title}. Your rating is: {Program.currentUser.Ratings[foundmovie]}");
                                Console.WriteLine("Would you like to change it (y/n)");
                                string choice = Console.ReadLine().Trim().ToLower();
                                if (choice == "y")
                                {
                                    while (true)
                                    {
                                        Console.Clear();
                                        Console.WriteLine($"Enter your rating for {foundmovie.Title}({foundmovie.ReleaseYear}) (0.0 - 10.0):");
                                        RatingInput = Console.ReadLine().Trim();
                                        if (double.TryParse(RatingInput, out rating) && rating <= 10 && rating >= 0)
                                        {
                                            Program.currentUser.Ratings[foundmovie] = rating;
                                            Database.AddOrUpdateRating(Program.currentUser.UserId, foundmovie.MovieId, rating);
                                            Console.WriteLine($"Your rating of {rating} for {foundmovie.Title} has been saved");
                                            Program.Pause();
                                            return;
                                        }
                                        else
                                        {
                                            Console.WriteLine("Please enter a valid rating between 0 and 10");
                                        }
                                    }
                                }
                                else if (choice == "n")
                                {
                                    Console.WriteLine("Returning...");
                                    return;
                                }
                                else
                                {
                                    Console.WriteLine("Invalid input. Please only enter 'y' or 'n'");
                                }
                            }
                        }

                    }
                    else if (input == "n")
                    {
                        Console.WriteLine("Please try again with a more specific title.");
                        Program.Pause();
                    }
                    else
                    {
                        Console.WriteLine("Invalid input. Please only enter 'y' or 'n'.");
                        Program.Pause();
                    }
                }
            }
        }
        
        
        
        
        
        
        
        public static void RemoveRating() 
        {
            List<Movie> finds = new List<Movie>();
            Database.LoadUserByUsername(Program.currentUser.username);

            while (true)
            {
                Console.Clear();
                Console.WriteLine("Enter the name of the movie you want to remove the rating of:");
                string name = Console.ReadLine().Trim();

                Movie foundmovie = null;
                foreach (Movie m in Program.movies)
                {
                    if (!string.IsNullOrEmpty(m.Title) && m.Title.Trim().ToLower().Contains(name.Trim().ToLower()) == true)
                    {
                        foundmovie = m;
                        break;
                    }
                }
                if (foundmovie == null)
                {
                    Console.WriteLine("Movie not found, please check the title and try again.");
                    Program.Pause();
                    continue;
                }
                Console.WriteLine($"Do you mean {foundmovie.Title}({foundmovie.ReleaseYear}) y/n");
                string input = Console.ReadLine().Trim().ToLower();
                string RatingInput;
                double rating;
                if (input == "y")
                {
                    if (Program.currentUser.Ratings.ContainsKey(foundmovie))
                    {
                        while (true)
                        {
                            Database.RemoveRating(Program.currentUser.UserId, foundmovie.MovieId);
                            Console.WriteLine($"Your rating of {Program.currentUser.Ratings[foundmovie]} for {foundmovie.Title} has been removed");
                            Program.currentUser.Ratings.Remove(foundmovie);
                            Program.Pause();
                            return;
                        }
                    }
                    else
                    {
                        Console.WriteLine($"You have not rated {foundmovie.Title}.");
                        Program.Pause();
                        return;                        
                    }

                }
                else if (input == "n")
                {
                    Console.WriteLine("Please try again with a more specific title.");
                    Program.Pause();
                }
                else
                {
                    Console.WriteLine("Invalid input. Please only enter 'y' or 'n'.");
                    Program.Pause();
                }
            }
        }
        
        
        
        
        
        
        
        
        public static void ViewRating() 
        {
            Console.Clear();
            Database.LoadUserByUsername(Program.currentUser.username);

            if (Program.currentUser.Ratings.Count > 0)
            {
                Console.WriteLine("----- Your Ratings -----");
                foreach (var entry in Program.currentUser.Ratings)
                {
                    Movie m = entry.Key;
                    double rating = entry.Value;
                    Console.WriteLine($"{m.Title} - {m.ReleaseYear} - {m.Duration} || Rating: {rating}");
                }
                
            }
            else
            {
                Console.WriteLine("You have no ratings");
            }
            Console.WriteLine();
            Program.Pause();
        }
        
        
        
        
        
        
        
        public static void AddComments() 
        {
            List<Movie> finds = new List<Movie>();
            while (true)
            {
                Console.Clear();
                Console.WriteLine("Enter the name of the movie you want to add a comment:");
                string name = Console.ReadLine().Trim();

                Movie foundmovie = null;
                foreach (Movie m in Program.movies)
                {
                    if (!string.IsNullOrEmpty(m.Title) && m.Title.Trim().ToLower().Contains(name.Trim().ToLower()) == true)
                    {
                        foundmovie = m;
                        break;
                    }
                }
                if (foundmovie == null)
                {
                    Console.WriteLine("Movie not found, please check the title and try again.");
                    Program.Pause();
                    continue;
                }
                while (true)
                {
                    Console.Clear();
                    Console.WriteLine($"Do you mean {foundmovie.Title}({foundmovie.ReleaseYear}) y/n");
                    string input = Console.ReadLine().Trim().ToLower();
                    string comment;
                    if (input == "y")
                    {
                        if (!Program.currentUser.Comments.ContainsKey(foundmovie))
                        {
                            while (true)
                            {
                                Console.Clear();
                                Console.WriteLine($"Enter your comment for {foundmovie.Title}({foundmovie.ReleaseYear}):");
                                comment = Console.ReadLine().Trim();

                                Database.AddOrUpdateComment(Program.currentUser.UserId, foundmovie.MovieId, comment);
                                Program.currentUser.Comments[foundmovie] = comment;
                                Console.WriteLine($"Your comment: '{comment}' for {foundmovie.Title} has been saved");
                                Program.Pause();
                                return;
                            }
                        }
                        else
                        {
                            
                            while (true)
                            { 
                                Console.Clear();
                                Console.WriteLine($"You have already commented {foundmovie.Title}. Your comment is: {Program.currentUser.Comments[foundmovie]}");
                                Console.WriteLine("Would you like to change it (y/n)");
                                string choice = Console.ReadLine().Trim().ToLower();
                                if (choice == "y")
                                {
                                    while (true)
                                    {
                                        Console.Clear();
                                        Console.WriteLine($"Enter your comment for {foundmovie.Title}({foundmovie.ReleaseYear}):");
                                        comment = Console.ReadLine().Trim();
                                       
                                        Database.AddOrUpdateComment(Program.currentUser.UserId, foundmovie.MovieId, comment);
                                        Program.currentUser.Comments[foundmovie] = comment;
                                        Console.WriteLine($"Your comment: '{comment}' for {foundmovie.Title} has been saved");
                                        Program.Pause();
                                        return;
                                    }
                                }
                                else if (choice == "n")
                                {
                                    Console.WriteLine("Returning...");
                                    return;
                                }
                                else
                                {
                                    Console.WriteLine("Invalid input. Please only enter 'y' or 'n'");
                                }
                            }
                        }

                    }
                    else if (input == "n")
                    {
                        Console.WriteLine("Please try again with a more specific title.");
                        Program.Pause();
                    }
                    else
                    {
                        Console.WriteLine("Invalid input. Please only enter 'y' or 'n'.");
                        Program.Pause();
                    }
                }
            }
        }
        
        
        
        
        
        
        
        public static void RemoveComments() 
        {
            List<Movie> finds = new List<Movie>();
            Database.LoadUserByUsername(Program.currentUser.username);

            while (true)
            {
                Console.Clear();
                Console.WriteLine("Enter the name of the movie you want to remove the comment of:");
                string name = Console.ReadLine().Trim();

                Movie foundmovie = null;
                foreach (Movie m in Program.movies)
                {
                    if (!string.IsNullOrEmpty(m.Title) && m.Title.Trim().ToLower().Contains(name.Trim().ToLower()) == true)
                    {
                        foundmovie = m;
                        break;
                    }
                }
                if (foundmovie == null)
                {
                    Console.WriteLine("Movie not found, please check the title and try again.");
                    Program.Pause();
                    continue;
                }
                Console.WriteLine($"Do you mean {foundmovie.Title}({foundmovie.ReleaseYear}) y/n");
                string input = Console.ReadLine().Trim().ToLower();
                string comment;
                if (input == "y")
                {
                    if (Program.currentUser.Comments.ContainsKey(foundmovie))
                    {
                        while (true)
                        {
                            Database.RemoveComment(Program.currentUser.UserId, foundmovie.MovieId);
                            Console.WriteLine($"Your comment: '{Program.currentUser.Comments[foundmovie]}' for {foundmovie.Title} has been removed");
                            Program.currentUser.Comments.Remove(foundmovie);
                            Program.Pause();
                            return;
                        }
                    }
                    else
                    {
                        Console.WriteLine($"You have not commented on {foundmovie.Title}.");
                        Program.Pause();
                        return;
                    }

                }
                else if (input == "n")
                {
                    Console.WriteLine("Please try again with a more specific title.");
                    Program.Pause();
                }
                else
                {
                    Console.WriteLine("Invalid input. Please only enter 'y' or 'n'.");
                    Program.Pause();
                }
            }
        }
        
        
        
        
        
        
        public static void ViewComments() 
        {
            Console.Clear();
            Database.LoadUserByUsername(Program.currentUser.username);

            if (Program.currentUser.Comments.Count > 0)
            {
                Console.WriteLine("----- Your Comments -----");
                foreach (var entry in Program.currentUser.Comments)
                {
                    Movie m = entry.Key;
                    string comment = entry.Value;
                    Console.WriteLine($"{m.Title} - {m.ReleaseYear} - {m.Duration} || Comment: {comment}");
                    Console.WriteLine("-------------------------------------");
                }
                
            }
            else
            {
                Console.WriteLine("You have no comments");
            }
            Console.WriteLine();
            Program.Pause();
        }
        
        
        
        
        
        
        public static void AddFavourite() 
        {
            List<Movie> finds = new List<Movie>();
            while (true)
            {
                Console.Clear();
                Console.WriteLine("Enter the name of the movie you want to add to your favourites list:");
                string name = Console.ReadLine().Trim();

                Movie foundmovie = null;
                foreach (Movie m in Program.movies)
                {
                    if (!string.IsNullOrEmpty(m.Title) && m.Title.Trim().ToLower().Contains(name.Trim().ToLower()) == true)
                    {
                        foundmovie = m;
                        break;
                    }
                }
                if (foundmovie == null)
                {
                    Console.WriteLine("Movie not found, please check the title and try again.");
                    Program.Pause();
                    continue;
                }
                Console.WriteLine($"Do you mean {foundmovie.Title}({foundmovie.ReleaseYear}) y/n");
                string input = Console.ReadLine().Trim().ToLower();

                if (input == "y")
                {
                    if (!Program.currentUser.Favourites.Contains(foundmovie))
                    {
                        Database.AddMovieToFavourites(Program.currentUser.UserId, foundmovie.MovieId);
                        Program.currentUser.Favourites.Add(foundmovie);
                        Console.WriteLine(foundmovie.Title + " has been successfully added to your favourites list.");
                    }
                    else
                    {
                        Console.WriteLine(foundmovie.Title + " is already in your favourites list.");
                    }
                    Program.Pause();
                    return;
                }
                else if (input == "n")
                {
                    Console.WriteLine("Please try again with a more specific title.");
                    Program.Pause();
                }
                else
                {
                    Console.WriteLine("Invalid input. Please only enter 'y' or 'n'.");
                    Program.Pause();
                }
            }
        }
        
        
        
        
        
        
        public static void RemoveFavourite() 
        {
            List<Movie> finds = new List<Movie>();
            Database.LoadUserByUsername(Program.currentUser.username);

            while (true)
            {
                Console.Clear();
                Console.WriteLine("Enter the name of the movie you want to remove from your favourites list:");
                string name = Console.ReadLine().Trim();

                Movie foundmovie = null;
                foreach (Movie m in Program.movies)
                {
                    if (!string.IsNullOrEmpty(m.Title) && m.Title.Trim().ToLower().Contains(name.Trim().ToLower()) == true)
                    {
                        foundmovie = m;
                        break;
                    }
                }
                if (foundmovie == null)
                {
                    Console.WriteLine("Movie not found, please check the title and try again.");
                    Program.Pause();
                    continue;
                }
                Console.WriteLine($"Do you mean {foundmovie.Title}({foundmovie.ReleaseYear}) y/n");
                string input = Console.ReadLine().Trim().ToLower();

                if (input == "y")
                {
                    if (Program.currentUser.Favourites.Contains(foundmovie))
                    {
                        Database.RemoveMovieFromFavourites(Program.currentUser.UserId, foundmovie.MovieId);
                        Program.currentUser.Favourites.Remove(foundmovie);
                        Console.WriteLine(foundmovie.Title + " has been successfully removed from your favourites list.");
                    }
                    else
                    {
                        Console.WriteLine(foundmovie.Title + " is not in your favourites list.");
                        while (true)
                        {
                            Console.WriteLine("Would you like to add " + foundmovie.Title + " to your favourites list (y/n)");
                            input = Console.ReadLine().Trim().ToLower();
                            if (input == "y")
                            {
                                Console.WriteLine(foundmovie.Title + " has been added to your favourites list");
                                Program.currentUser.Favourites.Add(foundmovie);
                                Database.AddMovieToFavourites(Program.currentUser.UserId, foundmovie.MovieId);
                            }
                            else if (input == "n")
                            {
                                Program.Pause();
                                break;
                            }
                            else
                            {
                                Console.WriteLine("Invalid input. Please only enter 'y' or 'n'.");
                            }
                        }
                    }
                    Program.Pause();
                    return;
                }
                else if (input == "n")
                {
                    Console.WriteLine("Please try again with a more specific title.");
                    Program.Pause();
                }
                else
                {
                    Console.WriteLine("Invalid input. Please only enter 'y' or 'n'.");
                    Program.Pause();
                }
            }
        }
        public static void ViewFavourites() 
        {
            Console.Clear();
            Database.LoadUserByUsername(Program.currentUser.username);

            if (Program.currentUser.Favourites.Count > 0)
            {
                Console.WriteLine("----- Your Favourites -----");
                foreach (Movie m in Program.currentUser.Favourites)
                {
                    Console.WriteLine($"{m.Title} - {m.ReleaseYear} - {m.Duration}");
                }
                
            }
            else
            {
                Console.WriteLine("Your favourites list is empty");
            }
            Console.WriteLine();
            Program.Pause();
        }
        
        
        
        
        
        public static void Remove() 
        {
            bool running = true;
            string userinput = "";
            while (running)
            {
                Console.Clear();
                Console.WriteLine("Would you want to...");
                Console.WriteLine("1. Remove a movie from your watched list");
                Console.WriteLine("2. Remove a movie from your watchlist");
                Console.WriteLine("3. Remove a comment");
                Console.WriteLine("4. Remove a rating");
                Console.WriteLine("5. Remove a movie from your favourites list");
                Console.WriteLine("6. Exit");
                userinput = Console.ReadLine();

                try
                {
                    int input = int.Parse(userinput);
                    if (input == 1)
                    {
                        Console.WriteLine("Remove a movie from your watched list selected");
                        RemoveFromWatched();
                    }
                    else if (input == 2)
                    {
                        Console.WriteLine("Remove a movie from your watchlist selected");
                        RemoveFromWatchlist();
                    }
                    else if (input == 3)
                    {
                        Console.WriteLine("Remove a comment selected");
                        RemoveComments();
                    }
                    else if (input == 4)
                    {
                        Console.WriteLine("Remove a rating selected");
                        RemoveRating();
                    }
                    else if (input == 5)
                    {
                        Console.WriteLine("Remove a movie from your favourites list selected");
                        RemoveFavourite();
                    }
                    else if (input == 6) 
                    {
                        
                        return;
                    }
                    else
                    {
                        Console.WriteLine("Invalid choice. Please enter a number between 1 and 5");
                        Program.Pause();
                    }
                }
                catch(FormatException)
                {
                    Console.WriteLine("Invalid choice. Please enter a number between 1 and 5");
                    Program.Pause();
                }
                catch(OverflowException) 
                {
                    Console.WriteLine("Invalid choice. Please enter a number between 1 and 5");
                    Program.Pause();
                }

            }
                
        }
    }
}
