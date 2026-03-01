using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SQLite;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.Remoting.Services;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Net.Http;
using System.Diagnostics.Eventing.Reader;
using System.Security.Cryptography;


namespace nea
{
    internal class Program
    {


        public static void Pause()
        {
            Console.WriteLine("Please press anything to continue...");
            Console.ReadKey();
        }








        public static void menu()
        {
            bool running = true;
            string userinput;

            while (running)
            {
                Console.Clear();
                Console.WriteLine("Type in the number of the option you want to choose");
                Console.WriteLine("1 - Login");
                Console.WriteLine("2 - Register");
                Console.WriteLine("3 - Get recommendations without logging in");
                Console.WriteLine("4 - Exit");

                userinput = Console.ReadLine(); ;

                try
                {
                    int input = int.Parse(userinput);

                    if (input == 1)
                    {
                        Console.WriteLine("Login selected");
                        LoginUser();

                    }
                    else if (input == 2)
                    {
                        Console.WriteLine("Register selected");
                        RegisterUser();
                        Pause();
                    }
                    else if (input == 3)
                    {
                        Console.WriteLine("Movie recommendation selected");
                        GetFilteredMovie();
                    }
                    else if (input == 4)
                    {
                        Console.WriteLine("Exit selected");
                        running = false;
                        Pause();
                    }
                    else
                    {
                        Console.WriteLine("Invalid choice. Please enter 1, 2, 3 or 4");
                        Pause();
                    }

                }
                catch (OverflowException)
                {
                    Console.WriteLine("Number is out of range. Please choose 1, 2, 3 or 4.");
                    Pause();
                }
                catch (FormatException)
                {
                    Console.WriteLine("Invalid input. Please enter a number between 1 and 4.");
                    Pause();
                }

            }
        }












        public static void PostLoginMenu()
        {
            bool running = true;
            string userinput;

            while (running)
            {
                Console.Clear();
                Console.WriteLine("Welcome " + currentUser.username);
                Console.WriteLine("Type in the number of the option you want to choose");
                Console.WriteLine(" 1 - Get a recommendation");
                Console.WriteLine(" 2 - Mark a movie as watched");
                Console.WriteLine(" 3 - Add a movie to your watchlist");
                Console.WriteLine(" 4 - Add a comment to a movie");
                Console.WriteLine(" 5 - Add a rating to a movie");
                Console.WriteLine(" 6 - Add a movie to your favourites");
                Console.WriteLine(" 7 - View your watched list");
                Console.WriteLine(" 8 - View your watchlist");
                Console.WriteLine(" 9 - View your comments");
                Console.WriteLine("10 - View your ratings");
                Console.WriteLine("11 - View your favourites");
                Console.WriteLine("12 - Remove; whether its a comment,rating or a movie in your watchlist");
                Console.WriteLine("13 - Log out");
                userinput = Console.ReadLine(); ;

                try
                {
                    int input = int.Parse(userinput);

                    if (input == 1)
                    {
                        Console.WriteLine("Movie recommendation selected");
                        GetFilteredMovie();
                        Pause();
                    }
                    else if (input == 2)
                    {
                        Console.WriteLine("Mark a movie as watched selected");
                        UserServices.AddtoWatched();
                    }
                    else if (input == 3)
                    {
                        Console.WriteLine("Add a movie to your watchlist selected");
                        UserServices.AddtoWatchlist();
                    }
                    else if (input == 4)
                    {
                        Console.WriteLine("Add a comment to a movie selected");
                        UserServices.AddComments();
                    }
                    else if (input == 5)
                    {
                        Console.WriteLine("Add a rating to a movie selected");
                        UserServices.GiveRating();
                    }
                    else if (input == 6)
                    {
                        Console.WriteLine("Add a movie to your favourites");
                        UserServices.AddFavourite();
                    }
                    else if (input == 7)
                    {
                        Console.WriteLine("View your watched list selected");
                        UserServices.ViewWatched();
                    }
                    else if (input == 8)
                    {
                        Console.WriteLine("View your watchlist selected");
                        UserServices.ViewWatchlist();
                    }
                    else if (input == 9)
                    {
                        Console.WriteLine("View your comments selected");
                        UserServices.ViewComments();
                    }
                    else if (input == 10)
                    {
                        Console.WriteLine("View your ratings selected");
                        UserServices.ViewRating();
                    }
                    else if (input == 11)
                    {
                        Console.WriteLine("View your favourites");
                        UserServices.ViewFavourites();
                    }
                    else if (input == 12)
                    {
                        Console.WriteLine("Remove selected");
                        UserServices.Remove();
                    }
                    else if (input == 13)
                    {
                        Console.WriteLine("Log out selected");
                        running = false;
                        currentUser = null;
                        Console.WriteLine("Returning to the main menu...");
                        Pause();
                        //menu();
                    }
                    else
                    {
                        Console.WriteLine("Invalid choice. Please enter a number between 1 and 13");
                        Pause();
                    }

                }
                catch (OverflowException)
                {
                    Console.WriteLine("Number is out of range. Please choose a number between 1 and 13");
                    Pause();
                }
                catch (FormatException)
                {
                    Console.WriteLine("Invalid input. Please enter a number between 1 and 13.");
                    Pause();
                }

            }

        }












        public static void RegisterUser()
        {
            string username = "", password;
            bool exists = true;
            while (exists)
            {
                Console.Clear();
                Console.WriteLine("--- REGISTER ---");
                Console.WriteLine("Enter your username:");
                username = Console.ReadLine();

                if (Database.GetUserIdByUsername(username) != -1)
                {
                    Console.WriteLine("Username already exists. Try a different one.");
                    Pause();
                }
                else
                {
                    exists = false;
                }
            }

            Console.WriteLine("Enter your password:");
            password = Console.ReadLine();

            string hash = Hashing.HashPassword(password);

            Database.AddUser(new User(username, password));

            users = Database.LoadAllUsers();

            Console.WriteLine("Registered successfully");

        }









        public static void LoginUser()
        {
            string username = "", password;
            bool found = false;
            while (found == false)
            {

                Console.Clear();
                Console.WriteLine("--- LOGIN ---");

                Console.WriteLine("enter your username");
                username = Console.ReadLine().Trim();
                Console.WriteLine("Enter your password");
                password = Console.ReadLine();

                User user = Database.LoadUserByUsername(username);

                if (user == null)
                {
                    Console.WriteLine("Invalid username or password.");
                    Pause();
                    return;
                }
                else if (!Hashing.PasswordValidation(password, user.PasswordHash))
                {
                    Console.WriteLine("Invalid username or password.");
                    Pause();
                    return;
                }
                else 
                {
                    currentUser = user;
                    Console.WriteLine("Login successful!");
                    found = true;
                    Pause();
                    PostLoginMenu();
                }
            }
        }








        public static void GetFilteredMovie()
        {

            List<Movie> filtered = new List<Movie>();
            string genre = "", actor = "", director = "";
            double minrating = 0;
            int earliest = 0, latest = 0, duration = 1000;
            bool running = true, exists = false;
            bool directorinput = true, actorinput = true, durationinput = true;
            Console.Clear();
            while (running)
            {
                exists = false;
                bool empty = false;
                Console.WriteLine("Enter preferred genre: ");
                genre = Console.ReadLine().Trim().ToLower();
                if (string.IsNullOrWhiteSpace(genre))
                {
                    Console.WriteLine("Genre cannot be empty. Please enter a valid genre.");
                    empty = true;
                }
                if (!empty)
                {
                    foreach (Movie m in movies)
                    {
                        if (m.Genre.ToLower().Contains(genre))
                        {
                            exists = true;
                            break;
                        }
                    } 
                }
                if (exists)
                {
                    running = false;
                }
                else
                {
                    Console.WriteLine("Please enter a valid genre");

                }
            }


            running = true;
            while (running)
            {
                Console.WriteLine("Do you want duration restrictions? (y/n)");
                string inputduration = Console.ReadLine().Trim().ToLower();

                if (inputduration == "y")
                {
                    Console.WriteLine("What is your preferred duration in minutes? (Enter the maximum duration)");
                    while (true)
                    {
                        if (int.TryParse(Console.ReadLine().Trim(), out duration) && duration > 0)
                        {
                            running = false;
                            durationinput = true;
                            break;
                        }
                        else
                        {
                            Console.WriteLine("Please enter a valid value");
                        }
                    }
                }
                else if (inputduration == "n")
                {
                    durationinput = false;
                    running = false;
                }
                else
                {
                    Console.WriteLine("Invalid input. Please only enter 'y' or 'n'.");
                    running = true;
                    Program.Pause();
                }
            }
            running = true;
            while (running)
            {

                Console.WriteLine("What is the minimum rating of the movie?");
                string tempminrating = Console.ReadLine().ToLower().Trim();
                try
                {
                    minrating = int.Parse(tempminrating);
                    running = false;
                }
                catch (OverflowException)
                {
                    Console.WriteLine("Please enter a valid rating between 1 and 10");
                    Pause();
                }
                catch (FormatException)
                {
                    Console.WriteLine("Please enter a valid rating between 1 and 10");
                    Pause();
                }
                if (minrating > 10 || minrating < 0) 
                {
                    Console.WriteLine("Please enter a valid rating between 1 and 10");
                    running = true;
                }
            }
            running = true;
            while (running)
            {
                while (running)
                {
                    Console.WriteLine("What is the earliest release date?");
                    string tempearliest = Console.ReadLine();
                    try
                    {
                        earliest = int.Parse(tempearliest);
                        running = false;
                    }
                    catch (OverflowException)
                    {
                        Console.WriteLine("Please enter a valid year for movies between 1950 and 2025");

                    }
                    catch (FormatException)
                    {
                        Console.WriteLine("Please enter a valid year for movies between 1950 and 2025");

                    }
                    if (earliest > 2025 || earliest < 1950)
                    {
                        running = true;
                        Console.WriteLine("Please enter a valid year for movies between 1950 and 2025");
                    }

                }
                running = true;
                while (running)
                {
                    Console.WriteLine("What is the latest release date?");
                    string templatest = Console.ReadLine();
                    try
                    {
                        latest = int.Parse(templatest);
                        running = false;
                    }
                    catch (OverflowException)
                    {
                        Console.WriteLine("Please enter a valid year for movies between 1950 and 2025");

                    }
                    catch (FormatException)
                    {
                        Console.WriteLine("Please enter a valid year for movies between 1950 and 2025");

                    }
                    if (latest > 2025 || latest < 1950)
                    {
                        running = true;
                        Console.WriteLine("Please enter a valid year for movies between 1950 and 2025");
                    }
                }
                
                if (earliest > latest)
                {
                    running = true;
                    Console.WriteLine("Earliest year you have entered is higher than the latest year, please enter them again");
                }

            }

            running = true;
            while (running)
            {
                Console.WriteLine("Do you have an actor that you want to be in the movie? (y/n)");
                string inputactor = Console.ReadLine().Trim().ToLower();
                actor = "";
                if (inputactor == "y")
                {
                    bool valid = false;
                    while (!valid)
                    {
                        Console.WriteLine("Enter the name of the actor:");
                        actor = Console.ReadLine().ToLower().Trim();
                        exists = false;
                        foreach (Movie m in movies)
                        {
                            foreach (string a in m.LeadActors)
                            {
                                if (a.IndexOf(actor, StringComparison.OrdinalIgnoreCase) >= 0)
                                {
                                    exists = true;
                                    actorinput = true;
                                    running = false;
                                    valid = true;
                                    break;
                                }
                            }
                        }
                        if (!exists)
                        {
                            Console.WriteLine("No match for the actor you entered, please press enter to retry or enter 'cancel' ");
                            string retry = Console.ReadLine().Trim().ToLower();
                            if (retry == "cancel")
                            {
                                actorinput = false;
                                break;
                            }
                        }
                    }
                }
                else if (inputactor == "n")
                {
                    actorinput = false;
                    running = false;
                }
                else
                {
                    Console.WriteLine("Invalid input. Please only enter 'y' or 'n'.");
                    Program.Pause();
                }
            }
            running = true;
            while (running)
            {
                Console.WriteLine("Do you have a director that you want to be the director of the movie? (y/n)");
                string inputdirector = Console.ReadLine().Trim().ToLower();
                director = "";
                if (inputdirector == "y")
                {
                    exists = false;
                    while (!exists)
                    {
                        Console.WriteLine("Enter the name of the director:");
                        director = Console.ReadLine().ToLower().Trim();
                        
                        foreach (Movie m in movies)
                        {
                            if (m.Director.ToLower().Contains(director))
                            {
                                exists = true;
                                directorinput = true;
                                running = false;
                                break;
                            }
                        }
                        if (!exists)
                        {
                            Console.WriteLine("No match for the director you entered, please try another name");
                        }
                    }
                }
                else if (inputdirector == "n")
                {
                    directorinput = false;
                    running = false;
                }
                else
                {
                    Console.WriteLine("Invalid input. Please only enter 'y' or 'n'.");
                    Program.Pause();
                }
            }
            foreach (Movie m in Program.movies) // this foreach loop checks the entered inputs by the user and filters the movies accordingly 
            {
                bool passes = true;

                if (directorinput)
                {
                    if (m.Director == null || !m.Director.ToLower().Contains(director))
                    {
                        passes = false;
                    }
                }

                if (passes && actorinput)
                {
                    bool foundActor = false;

                    foreach (string a in m.LeadActors)
                    {
                        if (a != null && a.ToLower().Contains(actor))
                        {
                            foundActor = true;
                            break;
                        }
                    }

                    if (!foundActor)
                    {
                        passes = false;
                    }
                }

                if (passes && durationinput)
                {
                    if (m.Duration > duration)
                    {
                        passes = false;
                    }
                }

                if (passes)
                {
                    if (m.ReleaseYear < earliest || m.ReleaseYear > latest || m.Rating < minrating || m.Genre.ToLower().Trim() != genre)
                    {
                        passes = false;
                    }
                }

                if (passes)
                {
                    filtered.Add(m);
                }
            }



            if (filtered.Count > 0)
            {
                foreach (Movie m in filtered)
                {
                    Console.WriteLine($"{m.Title} ({m.ReleaseYear})");
                    Console.WriteLine($"Genre: {m.Genre} | Rating: {m.Rating} | Age Rating: {m.AgeRating} | Duration: {m.Duration} | Director: {m.Director}");
                    Console.WriteLine($"Actors: {string.Join(", ", m.LeadActors)}");
                    Console.WriteLine("------------------------------------");
                }
                Pause();
                if (currentUser == null)
                {
                    menu();
                }
                else
                {
                    PostLoginMenu();
                }
            }
            else
            {
                running = true;
                while (running)
                {
                    Console.WriteLine("There is no match");
                    Console.WriteLine("Would you like to...");
                    Console.WriteLine("1. Get a partial match");
                    Console.WriteLine("2. Filter only by the genre");
                    Console.WriteLine("3. Retry with different filters");
                    Console.WriteLine("4. Exit");
                    Console.WriteLine("Enter the option you want to choose");
                    string userinput = Console.ReadLine();

                    try
                    {
                        int input = int.Parse(userinput);

                        if (input == 1)
                        {
                            running = true;
                            bool testedAtLeastOne = false;
                            filtered.Clear();
                            Console.WriteLine("Partial match selected");
                            while (running)
                            {
                                Console.Clear();
                                Console.WriteLine("What features do you want to keep?");
                                Console.WriteLine("1. Genre");
                                Console.WriteLine("2. Duration");
                                Console.WriteLine("3. Minimum rating");
                                Console.WriteLine("4. Earliest release year");
                                Console.WriteLine("5. Latest release year");
                                Console.WriteLine("6. Actor");
                                Console.WriteLine("7. Director");
                                userinput = Console.ReadLine().ToLower().Trim();
                                string[] parts = userinput.Split(' ');
                                List<int> selectedfeatures = new List<int>();
                                foreach (string part in parts)
                                {
                                    try
                                    {
                                        int number = int.Parse(part);
                                        selectedfeatures.Add(number);
                                    }
                                    catch (FormatException)
                                    {
                                        Console.WriteLine("Invalid input please try again");
                                        Pause();
                                    }
                                    catch (OverflowException)
                                    {
                                        Console.WriteLine("Invalid input please try again");
                                        Pause();
                                    }
                                }
                                foreach (Movie m in movies)
                                {
                                    bool matches = true;
                                    if (selectedfeatures.Contains(1) && !m.Genre.ToLower().Contains(genre))
                                    {
                                        testedAtLeastOne = true;
                                        matches = false;
                                    }
                                    if (selectedfeatures.Contains(2) && m.Duration > duration)
                                    {
                                        matches = false;
                                        testedAtLeastOne = true;
                                    }
                                    if (selectedfeatures.Contains(3) && m.Rating < minrating)
                                    {
                                        matches = false;
                                        testedAtLeastOne = true;
                                    }
                                    if (selectedfeatures.Contains(4) && m.ReleaseYear < earliest)
                                    {
                                        matches = false;
                                        testedAtLeastOne = true;
                                    }
                                    if (selectedfeatures.Contains(5) && m.ReleaseYear > latest)
                                    {
                                        matches = false;
                                        testedAtLeastOne = true;
                                    }
                                    if (selectedfeatures.Contains(6))
                                    {
                                        bool actormatch = false;
                                        foreach (string s in m.LeadActors)
                                        {
                                            if (!string.IsNullOrEmpty(s) && s.ToLower().Contains(actor.ToLower()))
                                            {
                                                actormatch = true;
                                                break;
                                            }
                                        }
                                        if (!actormatch)
                                        {
                                            matches = false;
                                        }
                                        testedAtLeastOne = true;
                                    }
                                    if (selectedfeatures.Contains(7) && !m.Director.ToLower().Trim().Contains(director.ToLower().Trim()))
                                    {
                                        matches = false;
                                        testedAtLeastOne = true;
                                    }
                                    if (matches && testedAtLeastOne)
                                    {
                                        filtered.Add(m);
                                    }

                                }
                                if (filtered.Count > 0)
                                {
                                    foreach (Movie m in filtered)
                                    {
                                        Console.WriteLine($"{m.Title} ({m.ReleaseYear})");
                                        Console.WriteLine($"Genre: {m.Genre} | Rating: {m.Rating} | Age Rating: {m.AgeRating} | Duration: {m.Duration} | Director: {m.Director}");
                                        Console.WriteLine($"Actors: {string.Join(", ", m.LeadActors)}");
                                        Console.WriteLine("------------------------------------");
                                    }
                                    running = false;
                                    Pause();
                                }
                                else
                                {
                                    bool running1 = true;
                                    while (running1)
                                    {
                                        Console.Clear();
                                        Console.WriteLine("No matches again");
                                        Console.WriteLine("Would you like to...");
                                        Console.WriteLine("1. Retry with different features");
                                        Console.WriteLine("2. Filter only by the genre");
                                        Console.WriteLine("3. Exit");
                                        userinput = Console.ReadLine();
                                        try
                                        {
                                            int number = int.Parse(userinput);


                                            if (number == 1)
                                            {
                                                running = true;
                                                running1 = false;                                            
                                            }
                                            else if (number == 2)
                                            {
                                                foreach (Movie m in movies)
                                                {
                                                    if (m.Genre.ToLower().Trim() == genre)
                                                    {
                                                        filtered.Add(m);
                                                    }
                                                }

                                                foreach (Movie m in filtered)
                                                {
                                                    Console.WriteLine($"{m.Title} ({m.ReleaseYear})");
                                                    Console.WriteLine($"Genre: {m.Genre} | Rating: {m.Rating} | Age Rating: {m.AgeRating} | Duration: {m.Duration} | Director: {m.Director}");
                                                    Console.WriteLine($"Actors: {string.Join(", ", m.LeadActors)}");
                                                    Console.WriteLine("------------------------------------");
                                                }
                                                running1 = false;
                                                running = false;

                                            }
                                            else if (number == 3)
                                            {
                                                running = false;
                                                running1 = false;
                                                Console.WriteLine("Press a key to return to the previous menu...");
                                                Console.ReadKey();
                                                if (currentUser == null)
                                                {
                                                    menu();
                                                }
                                                else
                                                {
                                                    PostLoginMenu();
                                                }

                                            }
                                            else
                                            {
                                                Console.WriteLine("Invalid input, please enter a number between 1 and 3");
                                                Pause();
                                            }

                                        }
                                        catch (FormatException)
                                        {
                                            Console.WriteLine("Please enter a number between 1 and 3");
                                        }
                                        catch (OverflowException)
                                        {
                                            Console.WriteLine("Please enter a number between 1 and 3");
                                        }
                                        
                                    }
                                }

                            }
                        }
                        else if (input == 2)
                        {
                            Console.WriteLine("Filtering only by the genre selected");
                            foreach (Movie m in movies)
                            {
                                if (m.Genre.ToLower().Trim() == genre)
                                {
                                    filtered.Add(m);
                                }
                            }
                            if (filtered.Count > 0)
                            {
                                foreach (Movie m in filtered)
                                {
                                    Console.WriteLine($"{m.Title} ({m.ReleaseYear})");
                                    Console.WriteLine($"Genre: {m.Genre} | Rating: {m.Rating} | Age Rating: {m.AgeRating} | Duration: {m.Duration} | Director: {m.Director}");
                                    Console.WriteLine($"Actors: {string.Join(", ", m.LeadActors)}");
                                    Console.WriteLine("------------------------------------");
                                }
                                running = false;
                                Pause();
                                if (currentUser == null)
                                {
                                    menu();
                                }
                                else
                                {
                                    PostLoginMenu();
                                }
                            }
                            
                            Pause();
                            if (currentUser == null)
                            {
                                menu();
                            }
                            else
                            {
                                PostLoginMenu();
                            }
                        }
                        else if (input == 3)
                        {
                            Console.WriteLine("Movie recommendation selected");
                            running = false;
                            GetFilteredMovie();
                            Pause();
                        }
                        else if (input == 4)
                        {
                            Console.WriteLine("Exit selected");
                            running = false;
                            Pause();
                            if (currentUser == null)
                            {
                                menu();
                            }
                            else
                            {
                                PostLoginMenu();
                            }
                        }
                        else
                        {
                            Console.WriteLine("Invalid choice. Please enter 1, 2, 3 or 4");
                            Pause();
                        }

                    }
                    catch (OverflowException)
                    {
                        Console.WriteLine("Number is out of range. Please choose 1, 2, 3 or 4.");
                        Pause();
                    }
                    catch (FormatException)
                    {
                        Console.WriteLine("Invalid input. Please enter a number between 1 and 4.");
                        Pause();
                    }
                }
            }
        }









        public static UserServices UserAction = null;
        public static List<Movie> movies = new List<Movie>();
        public static List<User> users = new List<User>();
        public static User currentUser = null;
        public static void Main(string[] args)
        {
            Console.WriteLine("Welcome");

            Database.CreateTables();
            users = Database.LoadAllUsers();
            movies = Database.LoadAllMovies();

            if (movies.Count == 0)
            {
                Console.WriteLine("Loading movies...");    
                API.GetMoviesApi();
            }
            else if (movies.Count < 10000)
            {
                while (true)
                {
                    Console.WriteLine("There are less than 10,000 (" + movies.Count + ") movies in the database, do you want to load more movies? (y/n)");
                    string choice = Console.ReadLine().Trim().ToLower();
                    if (choice == "y")
                    {
                        Console.WriteLine("Loading movies...");
                        API.GetMoviesApi();
                        break;
                    }
                    else if (choice == "n")
                    {
                        Console.WriteLine("Continuing...");
                        break;
                    }
                    else
                    {
                        Console.WriteLine("Invalid input. Please only enter 'y' or 'n'");
                    }
                }
            }
            else if (movies.Count >= 10000) 
            {                       
                while (true)
                {

                    Console.WriteLine("There are more than 10,000(" + movies.Count + ") movies in the database, do you want to load more movies? (y/n)"); string choice = Console.ReadLine().Trim().ToLower();
                    if (choice == "y")
                    {
                        Console.WriteLine("Loading movies...");
                        API.GetMoviesApi();
                        break;
                    }
                    else if (choice == "n")
                    {
                        Console.WriteLine("Continuing...");
                        break;
                    }
                    else
                    {
                        Console.WriteLine("Invalid input. Please only enter 'y' or 'n'");
                    }
                }
            }
            movies = Database.LoadAllMovies();
            menu();

            









            Console.ReadLine();
        }
    }
}








