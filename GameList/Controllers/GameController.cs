using IGDB;
using IGDB.Models;
using Microsoft.AspNetCore.Mvc;
using GameList.ViewModels;
using System.Collections.Generic;
using System.Globalization;
using System;
using Microsoft.Extensions.Hosting;
using CloudinaryDotNet;
using Microsoft.AspNetCore.Http;
using GameList.Interfaces;
using GameList.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Http.Extensions;
using CloudinaryDotNet.Actions;
using Newtonsoft.Json.Linq;
using RestEase.Implementation;
using System.Collections;

namespace GameList.Controllers
{
    //GameController contains all actions related to getting information from IGDB api to display in the view.
    //The class also has constructor dependency injection for IListRepository, UserManager<AppUser>, and IGDBClient.
    public class GameController : Controller
    {

        private readonly IGDBClient igdb;
        private readonly IListRepository _listRepository;
        private readonly UserManager<AppUser> _userManager;

        public GameController(IListRepository listRepository, UserManager<AppUser> userManager)
        {
            igdb = new IGDBClient("mglx6fger7kprdp4o25d74g9dg9ewf", "128zb34th4o2k2sdoeci8iqhq9x6wk");
            _listRepository = listRepository;
            _userManager = userManager;
        }
        public IActionResult Index()
        {
            return View();
        }
        //This code is a GameInfo action method that retrieves game information from an API (IGDB) and returns a view with a GameInfoViewModel.
        //If a user is logged in, their rating and status of the game is included in the view model, otherwise just the game information is included.
        public async Task<IActionResult> GameInfo(int Id, string selectedRating, string selectedStatus)
        {

            IEnumerable<IGDB.Models.Game> gameinfo = await igdb.QueryAsync<Game>(IGDBClient.Endpoints.Games, query: "fields name, cover.image_id," +
                " screenshots.image_id, follows, first_release_date, involved_companies, genres.name, summary, storyline, " +
                "platforms.name,  involved_companies.company.name, involved_companies.publisher, involved_companies.developer, category, age_ratings, " +
                "age_ratings.category, age_ratings.rating, dlcs.cover.image_id, dlcs.name, expansions.cover.image_id," +
                " expansions.name, standalone_expansions.cover.image_id, standalone_expansions.name, similar_games.cover.image_id, similar_games.name,videos.video_id," +
                " aggregated_rating, rating; where id = " + Id + "; ");


            int gameId = Id;

            var user = await _userManager.GetUserAsync(User);

            double averageRating2 = await _listRepository.GetAverageRatingByGameId(gameId);
            double averageRating = Math.Round(averageRating2, 2);
            int numberRatings = await _listRepository.GetCountRatingByGameId(gameId);
            if (user != null)
            {


                string appUserId = user.Id;
                var gameCheck = _listRepository.GetGameCheckRatingStatus(gameId, appUserId).isDuplicate;
                int? gameStatus = _listRepository.GetGameCheckRatingStatus(gameId, appUserId).GameStatus;
                int? gameRating = _listRepository.GetGameCheckRatingStatus(gameId, appUserId).GameRating;



                Dictionary<int, string> ratingLookup = new Dictionary<int, string>
                        {
                            {0,"0 (Awful)"   },
                            {1,"1 (Appalling)"},
                            {2,"2 (Horrible)"},
                            {3,"3 (Very Bad)"},
                            {4,"4 (Bad)"     },
                            {5,"5 (Average)"},
                            {6,"6 (Fine)"   },
                            {7,"7 (Good)"    },
                            {8,"8 (Very Good)"},
                            {9,"9 (Great)"   },
                            {10,"10 (GOTY)"},
                            {11,"Select Rating"}

                        };

                int ratingValue = gameRating ?? 11;

                string finalRating = ratingLookup[ratingValue];

                Dictionary<int, string> statusLookup = new Dictionary<int, string>
                           {
                               { 1, "Playing"         },
                               { 2, "Completed"     },
                               { 3, "On-Hold"        },
                               { 4, "Dropped"         },
                               { 5, "Plan to Play"    },
                               { 0, "Select Status" }

                           };

                int statusValue = gameStatus ?? 0;

                string finalStatus = statusLookup[statusValue];
                string returnUrl = Request.GetDisplayUrl();
                var gameInfoViewModel2 = new GameInfoViewModel
                {
                    NumberRatings = numberRatings,
                    AverageRating = averageRating,
                    GameDataInfo = gameinfo,
                    GameCheck = gameCheck,
                    GameRating = finalRating,
                    GameStatus = finalStatus,
                    ReturnUrl = returnUrl,
                    AvailableSelectRatings = new List<string> { "Select Rating", "0 (Awful)", "1 (Appalling)", "2 (Horrible)",   "3 (Very Bad)", "4 (Bad)", "5 (Average)", "6 (Fine)", "7 (Good)", "8 (Very Good)", "9 (Great)", "10 (GOTY)" },
                    AvailableSelectStatus = new List<string> { "Select Status", "Playing", "Completed", "On-Hold", "Dropped", "Plan to Play" }
                    //CompanyDataInfo = companyinfo2,
                    //CompanyDataInfo2 = companyinfo3
                };
                return View(gameInfoViewModel2);
            }
            else
            {
                var gameInfoViewModel2 = new GameInfoViewModel
                {
                    NumberRatings = numberRatings,
                    AverageRating = averageRating,
                    GameDataInfo = gameinfo
                };
                return View(gameInfoViewModel2);
            }
        }

        //CharacterInfo: Async method that fetches information about a character from IGDB API and returns the result in a view.
        public async Task<IActionResult> CharacterInfo(int Id)
        {
            IEnumerable<IGDB.Models.Character> characterinfo = await igdb.QueryAsync<Character>(IGDBClient.Endpoints.Characters, query: "fields name," +
                " mug_shot.image_id, species, gender, description, games.name, games.cover.image_id; where id = " + Id + "; ");

            return View(characterinfo);
        }
        //CompanyInfo: Async method that fetches information about a company from IGDB API and returns the result in a view.
        public async Task<IActionResult> CompanyInfo(int Id)
        {
            IEnumerable<IGDB.Models.Company> companyinfo = await igdb.QueryAsync<Company>(IGDBClient.Endpoints.Companies, query: "fields name, logo.image_id, " +
                "description, published.name, published.cover.image_id, developed.name, developed.cover.image_id; where id = " + Id + "; ");

            return View(companyinfo);
        }
        //GameSearch is a method that retrieves video game information based on a set of filters (searchquery, rating, platform, year, type, sort) and a page number. It's using the IGDB API to retrieve the data.
        //The method first maps the string representation of each filter to its corresponding API value in a dictionary.For example, the platform filter is mapped from "PlayStation 4" to " & platforms = {48}".
        //If the filter value is not found in the dictionary, the default value is used.
        //Next, the method checks the type filter and performs a different set of mappings based on the type.If the type is "Game", the platform, year, and sort filters are mapped to their
        //corresponding API values. The platform and year filters use the same approach as the type filter, using dictionaries to map the filter values to their API representation.
        //The sort filter is not currently used in the code.
        //Finally, the method uses the mapped values to build the API request URL and sends the request to retrieve the video game data and then the data is sent to the view through a model.
        public async Task<IActionResult> GameSearch(string? searchquery, string? rating, string? platform, string? year, string? type, string? sort, int page = 1)
        {
            Dictionary<string, string> typeLookup = new Dictionary<string, string>
                 {
                        {"Game",  "Game"},
                        {"Character", "Character"},
                        {"Users", "Users"},
                        {"", "Game"}

                        // Add more platform-to-integer mappings here
                };

            string typeValue = type ?? "";

            string typeApi = typeLookup[typeValue]; 


            if (typeApi == "Game")
            {

                Dictionary<string, string> platformLookup = new Dictionary<string, string>
                    {
                        {"PlayStation 4", " & platforms = {48}"},
                        {"PlayStation 5", " & platforms = {167}"},
                        {"Xbox One", " & platforms = {49}"},
                        {"PC", " & platforms = {6}"},
                        {"Nintendo Switch", " &  platforms = {130}"},
                        {"Any", ""},
                        {"", ""}

                        // Add more platform-to-integer mappings here
                };

                string platformValue = platform ?? "";

                string platformApi = platformLookup[platformValue];  // 48

                Dictionary<string, string> yearLookup = new Dictionary<string, string>
                    {
                        {"2023", " first_release_date > 1672531200 & first_release_date < 1704067200 "},
                        {"2022", " first_release_date > 1640995200 & first_release_date < 1672531200 "},
                        {"2021", " first_release_date > 1609459200 & first_release_date < 1640995200 "},
                        {"2020", " first_release_date > 1577836800 & first_release_date < 1609459200 "},
                        {"2019", " first_release_date > 1546300800 & first_release_date < 1577836800 "},
                        {"2018", " first_release_date > 1514764800 & first_release_date < 1546300800 "},
                        {"2017", " first_release_date > 1483228800 & first_release_date < 1514764800 "},
                        {"2016", " first_release_date > 1451606400 & first_release_date < 1483228800 "},
                        {"2015", " first_release_date > 1420070400 & first_release_date < 1451606400 "},
                        {"2014", " first_release_date > 1388534400 & first_release_date < 1420070400 "},
                        {"2013", " first_release_date > 1356998400 & first_release_date < 1388534400 "},
                        {"2012", " first_release_date > 1325376000 & first_release_date < 1356998400 "},
                        {"2011", " first_release_date > 1293840000 & first_release_date < 1325376000 "},
                        {"2010", " first_release_date > 1262304000 & first_release_date < 1293840000 "},
                        {"2009", " first_release_date > 1230768000 & first_release_date < 1262304000 "},
                        {"2008", " first_release_date > 1199145600 & first_release_date < 1230768000 "},
                        {"2007", " first_release_date > 1167609600 & first_release_date < 1199145600 "},
                        {"2006", " first_release_date > 1167609600 & first_release_date < 1167609600 "},
                        {"2005", " first_release_date > 1104537600 & first_release_date < 1167609600 "},
                        {"2004", " first_release_date > 1072915200 & first_release_date < 1104537600 "},
                        {"2003", " first_release_date > 1041379200 & first_release_date < 1072915200 "},
                        {"2002", " first_release_date > 1009843200 & first_release_date < 1041379200 "},
                        {"2001", " first_release_date > 978307200 & first_release_date < 1009843200 "},
                        {"2000", " first_release_date > 946684800 & first_release_date < 978307200 "},
                        {"Any year", " first_release_date > -631152000 & first_release_date < 2524608000 "},
                        {"", " first_release_date > -62167219125 & first_release_date < 32503680000 "}
    
                        // Add more platform-to-integer mappings here
                    };
    
                string yearValue = year ?? "";
    
                string yearApi = yearLookup[yearValue];  // 48
    
    
    
                Dictionary<string, string> ratingLookup = new Dictionary<string, string>
                    {
                        {"0",  " & rating >= 0"},
                        {"10", " & rating >= 10"},
                        {"20", " & rating >= 20"},
                        {"30", " & rating >= 30"},
                        {"40", " & rating >= 40"},
                        {"50", " & rating >= 50"},
                        {"60", " & rating >= 60"},
                        {"70", " & rating >= 70"},
                        {"80", " & rating >= 80"},
                        {"90", " & rating >= 90"},
                        {"Any", ""},
                        {"", ""}
    
                        // Add more platform-to-integer mappings here
                    };
    
                string ratingValue = rating ?? "";
    
                string ratingApi = ratingLookup[ratingValue];  // 48
    
    
    
                Dictionary<string, string> sortLookup = new Dictionary<string, string>
                        {
                            {"A-Z",  ";sort name asc"},
                            {"Z-A",  ";sort name desc"},
                            {"Rating",  " & rating != null; sort rating desc"},
                            {"Relevance", ""},
                            {"", "& rating != null; sort rating desc"}
    
                            // Add more platform-to-integer mappings here
                        };
    
                string sortValue = sort ?? "";
    
                string sortApi = sortLookup[sortValue];  // 48
    
    
                string filterValue ="where" + yearApi + platformApi + ratingApi;
    
    
                int offset = (page - 1) * 32;
    
                IEnumerable<IGDB.Models.Game> gameinfo;
    
                if (searchquery == null)
                {
                       string filterValueWithSort = filterValue + sortApi + ";";
                       gameinfo = await igdb.QueryAsync<Game>(IGDBClient.Endpoints.Games, query: "fields name, cover.image_id, summary, rating;limit 320;offset " + offset + ";" + filterValueWithSort);
    
    
                }
                else
                {
    
                     string filterValueWithoutSort = filterValue + ";";
                     string searchApi = "search \"" + searchquery + "\"";
                     gameinfo = await igdb.QueryAsync<Game>(IGDBClient.Endpoints.Games, query: "fields name, cover.image_id, summary, rating;limit 320; offset " + offset + ";" + searchApi + "; " + filterValueWithoutSort);
    
                }
                int itemsPerPage = 32;
                int totalItems = gameinfo.Count();
                int totalPages = (int)Math.Ceiling((double)totalItems / itemsPerPage);
                if (totalPages < 10)
                {
                    totalPages = totalPages + (page - 1);
                }

                var modelGame = new GameSearchViewModel
                {
                    Results = gameinfo,
                    SearchQuery = searchquery,
                    SelectedRating = rating,
                    SelectedYear = year,    
                    SelectedPlatform = platform,
                    SelectedSort = sort,
                    SelectedType = type,
                    CurrentPage = page,
                    TotalPages = totalPages,
                    AvailableRatings = new List<string> { "Any", "0", "10", "20", "30", "40", "50", "60", "70", "80", "90" },
                    AvailablePlatforms = new List<string> { "Any", "PlayStation 4", "PlayStation 5", "Nintendo Switch", "Xbox One", "PC" },
                    AvailableSorting = new List<string> { "Relevance", "A-Z", "Z-A", "Rating" },
                    AvailableYears = new List<string> { "Any year", "2023", "2022", "2021", "2020", "2019", "2018", "2017", "2016", "2015", "2014", "2013", "2012", "2011", "2010", "2009", "2008", "2007", "2006", "2005", "2004", "2003", "2002", "2001", "2000" },
                    AvailableTypes = new List<string> { "Game", "Character", "Users" }
                };
                return View(modelGame);
    


            }

            //No search endpoint for company in IGDB

            //if (typeApi == "Company")
            //{


            //    int offset = (page - 1) * 32;

            //    IEnumerable<IGDB.Models.Company> companyinfo;
            //    if (searchquery == null)
            //    {
            //        companyinfo = await igdb.QueryAsync<Company>(IGDBClient.Endpoints.Companies, query: "fields name, logo.image_id;limit 320;offset " + offset + ";");

            //    }
            //    else
            //    {

            //        string searchApi = "search \"" + searchquery + "\"";
            //        companyinfo = await igdb.QueryAsync<Company>(IGDBClient.Endpoints.Companies, query: "fields name,  logo.image_id;limit 320; offset " + offset + ";" + searchApi + ";");

            //    }

            //    int itemsPerPage = 32;
            //    int totalItems = companyinfo.Count();
            //    int totalPages = (int)Math.Ceiling((double)totalItems / itemsPerPage);
            //    if (totalPages < 10)
            //    {
            //        totalPages = totalPages + (page - 1);
            //    }

            //    var modelCharacter = new GameSearchViewModel
            //    {

            //        ResultsCompany = companyinfo,
            //        SearchQuery = searchquery,
            //        SelectedRating = rating,
            //        SelectedYear = year,
            //        SelectedPlatform = platform,
            //        SelectedSort = sort,
            //        SelectedType = type,
            //        CurrentPage = page,
            //        TotalPages = totalPages,
            //        AvailableRatings = new List<string> { "Any", "0", "10", "20", "30", "40", "50", "60", "70", "80", "90" },
            //        AvailablePlatforms = new List<string> { "Any", "PlayStation 4", "PlayStation 5", "Nintendo Switch", "Xbox One", "PC" },
            //        AvailableSorting = new List<string> { "Relevance", "A-Z", "Z-A", "Rating" },
            //        AvailableYears = new List<string> { "Any year", "2023", "2022", "2021", "2020", "2019", "2018", "2017", "2016", "2015", "2014", "2013", "2012", "2011", "2010", "2009", "2008", "2007", "2006", "2005", "2004", "2003", "2002", "2001", "2000" },
            //        AvailableTypes = new List<string> { "Game", "Character" }
            //    };

            //    return View(modelCharacter);
            //}

            if (typeApi == "Character")
            {

                int offset = (page - 1) * 32;

                IEnumerable<IGDB.Models.Character> characterinfo;
                if (searchquery == null)
                {
                    characterinfo = await igdb.QueryAsync<Character>(IGDBClient.Endpoints.Characters, query: "fields name, mug_shot.image_id;limit 320;offset " + offset + ";");

                }
                else
                {

                    string searchApi = "search \"" + searchquery + "\"";
                    characterinfo = await igdb.QueryAsync<Character>(IGDBClient.Endpoints.Characters, query: "fields name,  mug_shot.image_id;limit 320; offset " + offset + ";" + searchApi + ";");

                }

                int itemsPerPage = 32;
                int totalItems = characterinfo.Count();
                int totalPages = (int)Math.Ceiling((double)totalItems / itemsPerPage);
                if (totalPages < 10)
                {
                    totalPages = totalPages + (page - 1);
                }

                var modelCharacter = new GameSearchViewModel
                {

                    ResultsCharacter = characterinfo,
                    SearchQuery = searchquery,
                    SelectedRating = rating,
                    SelectedYear = year,
                    SelectedPlatform = platform,
                    SelectedSort = sort,
                    SelectedType = type,
                    CurrentPage = page,
                    TotalPages = totalPages,
                    AvailableRatings = new List<string> { "Any", "0", "10", "20", "30", "40", "50", "60", "70", "80", "90" },
                    AvailablePlatforms = new List<string> { "Any", "PlayStation 4", "PlayStation 5", "Nintendo Switch", "Xbox One", "PC" },
                    AvailableSorting = new List<string> { "Relevance", "A-Z", "Z-A", "Rating" },
                    AvailableYears = new List<string> { "Any year", "2023", "2022", "2021", "2020", "2019", "2018", "2017", "2016", "2015", "2014", "2013", "2012", "2011", "2010", "2009", "2008", "2007", "2006", "2005", "2004", "2003", "2002", "2001", "2000" },
                    AvailableTypes = new List<string> { "Game", "Character", "Users" }
                };

                return View(modelCharacter);
            }
            if (typeApi == "Users")
            {

                    return RedirectToAction("SearchUsers", "User", new { searchquery = searchquery });

            }


                var model = new GameSearchViewModel
            {
                SearchQuery = searchquery,
                SelectedRating = rating,
                SelectedYear = year,
                SelectedPlatform = platform,
                SelectedSort = sort,
                SelectedType = type,
                AvailableRatings = new List<string> { "Any", "0", "10", "20", "30", "40", "50", "60", "70", "80", "90" },
                AvailablePlatforms = new List<string> { "Any", "PlayStation 4", "PlayStation 5", "Nintendo Switch", "Xbox One", "PC" },
                AvailableSorting = new List<string> { "Relevance", "A-Z", "Z-A", "Rating" },
                AvailableYears = new List<string> { "Any year", "2023", "2022", "2021", "2020", "2019", "2018", "2017", "2016", "2015", "2014", "2013", "2012", "2011", "2010", "2009", "2008", "2007", "2006", "2005", "2004", "2003", "2002", "2001", "2000" },
                AvailableTypes = new List<string> { "Game", "Character", "Users" }
            };
            return View(model);


        }
    }
}
