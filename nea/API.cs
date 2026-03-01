using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Net.Http;
using System.Diagnostics;
namespace nea
{
    internal class API
    {
        public static string GetApiResponse(string url)
        {
            WebRequest request = WebRequest.Create(url);
            request.Method = WebRequestMethods.Http.Get;
            request.ContentType = "application/json; charset=utf-8";

            string text;
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            using (StreamReader sr = new StreamReader(response.GetResponseStream()))
            {
                text = sr.ReadToEnd();
            }
            return text;
        }

        



        public static void GetMoviesApi()
        {
            string ApiKey = "4d032c951b0aac664fb96e5c24f335c1";

            string genreURL = $"https://api.themoviedb.org/3/genre/movie/list?api_key={ApiKey}";
            string genreJson = GetApiResponse(genreURL);
            TmdbGenreList genreList = JsonSerializer.Deserialize<TmdbGenreList>(genreJson, new JsonSerializerOptions{PropertyNameCaseInsensitive = true});
            //https://api.themoviedb.org/3/genre/movie/list?api_key=4d032c951b0aac664fb96e5c24f335c1
            //https://api.themoviedb.org/3/discover/movie?api_key=4d032c951b0aac664fb96e5c24f335c1&page=1
            //https://api.themoviedb.org/3/movie/755898?api_key=4d032c951b0aac664fb96e5c24f335c1&append_to_response=credits,release_dates
            //https://api.themoviedb.org/3/movie/1319895?api_key=4d032c951b0aac664fb96e5c24f335c1&append_to_response=credits,release_dates --- these lines are for testing and seeing the output of the API 
            for (int page = 1; page <= 500; page++)
            {
                string discoverURL = $"https://api.themoviedb.org/3/discover/movie?api_key={ApiKey}&sort_by=popularity.desc&include_adult=false&primary_release_date.gte=1965-01-01&primary_release_date.lte=2025-12-31&with_original_language=en&page={page}";
                string discoverJson = GetApiResponse(discoverURL);
                TmdbMovieResult discoverResult = JsonSerializer.Deserialize<TmdbMovieResult>(discoverJson, new JsonSerializerOptions {PropertyNameCaseInsensitive = true});

                if (discoverResult?.Results == null)
                {
                    Console.WriteLine($"No results for page {page}. JSON returned:");
                    Console.WriteLine(discoverJson);
                    continue; // Skip this page

                }
                
                
                foreach (var m in discoverResult.Results)
                {
                    Movie movie = new Movie();

                    if (!string.IsNullOrEmpty(m.ReleaseDate) && m.ReleaseDate.Length >= 4)
                    {
                        movie.ReleaseYear = int.Parse(m.ReleaseDate.Substring(0, 4));
                    }
                    else
                    {
                        movie.ReleaseYear = 0;
                    }
                    movie.Title = m.Title;
                    movie.Rating = m.VoteAverage;
      
                    string detailsURL = $"https://api.themoviedb.org/3/movie/{m.Id}?api_key={ApiKey}&append_to_response=credits,release_dates";
                    string detailsJson = GetApiResponse(detailsURL);
                    TmdbMovieDetails details = JsonSerializer.Deserialize<TmdbMovieDetails>(detailsJson, new JsonSerializerOptions {PropertyNameCaseInsensitive = true});
                    if (details == null)
                    {
                        Console.WriteLine($"No details for movie ID {m.Id}: {m.Title}");
                        continue;
                    }
                    movie.Duration = details.runtime;


                    if (m.GenreIds != null && m.GenreIds.Count > 0)
                    {
                        for (int i = 0; i < m.GenreIds.Count; i++)
                        {
                            movie.Genre = details.genres[0].name;
                        }
                    }
                    else
                    {
                        movie.Genre = "";
                    }

                    movie.Director = "";

                    if (details?.credits?.crew != null)
                    {
                        foreach (var crew in details.credits.crew)
                        {
                            if (crew.job == "Director")
                            {
                                movie.Director = crew.name;
                                break;
                            }
                        }
                    }

                    List<string> leadActors = new List<string>();
                    int maxActors = 3;
                    if (details?.credits?.cast.Count < maxActors)
                    {
                        maxActors = details.credits.cast.Count;
                    }

                    for (int i = 0; i < maxActors; i++)
                    {
                        leadActors.Add(details?.credits?.cast[i].name);
                    }
                    movie.LeadActors = leadActors.ToArray();


                    movie.AgeRating = "Not Rated in the UK";
                    if (details.release_dates.results != null)
                    {
                        foreach (var release in details.release_dates.results)
                        {
                            if (release.iso_3166_1 == "GB" && release.release_dates.Count > 0)
                            {
                                if (release.release_dates.Count > 0)
                                {
                                    if (!string.IsNullOrEmpty(release.release_dates[0].certification.Trim())) 
                                    {
                                        movie.AgeRating = release.release_dates[0].certification;
                                        break;
                                    }                                    
                                }
                            }
                        }
                    }

                    Program.movies.Add(movie);
                    Database.AddMovieToDB(movie);
                }

            }
            Console.WriteLine($"Total movies fetched: {Program.movies.Count}");

        }





        public class TmdbGenreList
        {
            [JsonPropertyName("genres")]
            public List<TmdbGenreProperty> Genres;
        }

        public class TmdbGenreProperty 
        {
            [JsonPropertyName("id")]
            public int Id;
            [JsonPropertyName("name")]
            public string Name; 
        }





        public class TmdbMovieResult
        {
            [JsonPropertyName("page")]
            public int Page { get; set; }

            [JsonPropertyName("results")]
            public List<TmdbMovieBasic> Results { get; set; }
        }
        public class TmdbMovieBasic
        {
            [JsonPropertyName("id")]
            public int Id { get; set; }

            [JsonPropertyName("title")]
            public string Title { get; set; }

            [JsonPropertyName("release_date")]
            public string ReleaseDate { get; set; }

            [JsonPropertyName("vote_average")]
            public double VoteAverage { get; set; }

            [JsonPropertyName("genre_ids")]
            public List<int> GenreIds { get; set; }
        }

        public class TmdbMovieDetails
        {
            [JsonPropertyName("runtime")]
            public int runtime { get; set; }
            [JsonPropertyName("genres")]
            public List<TmdbGenre> genres { get; set; }
            [JsonPropertyName("credits")]
            public TmdbCredits credits { get; set; }
            [JsonPropertyName("release_dates")]
            public TmdbReleaseDates release_dates { get; set; }
        }
        public class TmdbGenre
        {
            [JsonPropertyName("name")]
            public string name { get; set; }
        }

        public class TmdbCredits
        {
            [JsonPropertyName("cast")]
            public List<TmdbCast> cast { get; set; }
            [JsonPropertyName("crew")]
            public List<TmdbCrew> crew { get; set; }
        }
        public class TmdbCast
        {
            [JsonPropertyName("name")]
            public string name { get; set; }
        }
        public class TmdbCrew
        {
            [JsonPropertyName("name")]
            public string name { get; set; }
            [JsonPropertyName("job")]
            public string job { get; set; }
        }

        public class TmdbReleaseDates
        {
            [JsonPropertyName("results")]
            public List<TmdbReleaseCountry> results { get; set; }
        }
        public class TmdbReleaseCountry
        {
            [JsonPropertyName("iso_3166_1")]
            public string iso_3166_1 { get; set; }
            [JsonPropertyName("release_dates")]
            public List<TmdbReleaseInfo> release_dates { get; set; }
        }
        public class TmdbReleaseInfo
        {
            [JsonPropertyName("certification")]
            public string certification { get; set; }
        }


    }
}
