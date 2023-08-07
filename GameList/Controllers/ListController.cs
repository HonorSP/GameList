
using GameList.Interfaces;
using GameList.Models;
using GameList.ViewModels;
using IGDB.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using System.Linq;
using System.Net;

namespace GameList.Controllers
{
    //This is a controller in GameList that handles editing a user's game list. It uses the UserManager to get the current user and retrieves their game list from the IListRepository. 
    public class ListController : Controller
    {

        //private readonly IUserRepository _userRepository;
        private readonly IListRepository _listRepository;
        private readonly UserManager<AppUser> _userManager;
        private readonly IUserRepository _userRepository;

        public ListController(IListRepository listRepository, UserManager<AppUser> userManager, IUserRepository userRepository)
        {
            _listRepository = listRepository;
            _userManager = userManager;
            _userRepository = userRepository;
        }
        //This method retrieves a user's game list, creates display strings for each game's status and rating, and passes that information to a view for display.
        //If the user is not logged in, the method redirects them to the login page.
        public async Task<IActionResult> EditUserGameList()
        {


            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return RedirectToAction("Login", "Account");
            }
            else
            {
            string appUserId = user.Id;
            IEnumerable<UserGameList> userGameLists = await _listRepository.GetListByUserId(appUserId);

            List<string> finalStatuses = new List<string>();
            List<string> finalRatings = new List<string>();
            foreach (var userGameList in userGameLists)
            {

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

                    int gameRating = userGameList.GameRating ?? 11;
                    string finalRating = ratingLookup[gameRating];
                    finalRatings.Add(finalRating);


                    Dictionary<int, string> statusLookup = new Dictionary<int, string>
                           {
                               { 1, "Playing"         },
                               { 2, "Completed"     },
                               { 3, "On-Hold"        },
                               { 4, "Dropped"         },
                               { 5, "Plan to Play"    },
                               { 0, "Select Status" }

                           };

                    int gameStatus = userGameList.GameStatus ?? 0;
                    string finalStatus = statusLookup[gameStatus];
                    finalStatuses.Add(finalStatus);

                }
                Dictionary<int, Tuple<string, string>> ratingsAndStatuses = new Dictionary<int, Tuple<string, string>>();

                for (int i = 0; i < userGameLists.Count(); i++)
                {
                    ratingsAndStatuses.Add(userGameLists.ElementAt(i).GameId, new Tuple<string, string>(finalRatings[i], finalStatuses[i]));
                }

                string returnUrl = Request.GetDisplayUrl();
                var viewModel = new EditUserGameListViewModel
                {
                    UserGameLists = userGameLists,
                    FinalRatings = finalRatings,
                    FinalStatuses = finalStatuses,
                    ReturnUrl = returnUrl,
                    ratingsAndStatuses = ratingsAndStatuses,
                    AvailableSelectRatings = new List<string> { "Select Rating", "0 (Awful)", "1 (Appalling)", "3 (Very Bad)", "4 (Bad)", "5 (Average)", "6 (Fine)", "7 (Good)", "8 (Very Good)", "9 (Great)", "10 (GOTY)" },
                    AvailableSelectStatus = new List<string> { "Select Status", "Playing", "Completed", "On-Hold", "Dropped", "Plan to Play" }
                };
                return View(viewModel);
            }
        }
        //The action retrieves a list of games and their ratings and statuses for a specific status for a user.
        //If the user is not logged in, they will be redirected to the login page. The retrieved game data is transformed into a view model and passed to a view to be displayed to the user.
        public async Task<IActionResult> EditSpecificStatus(int status)
        {


            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return RedirectToAction("Login", "Account");
            }
            else
            {
                string appUserId = user.Id;
                IEnumerable<UserGameList> userGameLists = await _listRepository.GetListByUserIdAndStatus(appUserId, status);

                List<string> finalStatuses = new List<string>();
                List<string> finalRatings = new List<string>();
                foreach (var userGameList in userGameLists)
                {

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

                    int gameRating = userGameList.GameRating ?? 11;
                    string finalRating = ratingLookup[gameRating];
                    finalRatings.Add(finalRating);


                    Dictionary<int, string> statusLookup = new Dictionary<int, string>
                           {
                               { 1, "Playing"         },
                               { 2, "Completed"     },
                               { 3, "On-Hold"        },
                               { 4, "Dropped"         },
                               { 5, "Plan to Play"    },
                               { 0, "Select Status" }

                           };

                    int gameStatus = userGameList.GameStatus ?? 0;
                    string finalStatus = statusLookup[gameStatus];
                    finalStatuses.Add(finalStatus);

                }
                Dictionary<int, Tuple<string, string>> ratingsAndStatuses = new Dictionary<int, Tuple<string, string>>();

                for (int i = 0; i < userGameLists.Count(); i++)
                {
                    ratingsAndStatuses.Add(userGameLists.ElementAt(i).GameId, new Tuple<string, string>(finalRatings[i], finalStatuses[i]));
                }

                string returnUrl = Request.GetDisplayUrl();
                var viewModel = new EditUserGameListViewModel
                {
                    UserGameLists = userGameLists,
                    FinalRatings = finalRatings,
                    FinalStatuses = finalStatuses,
                    ReturnUrl = returnUrl,
                    Status = status,
                    ratingsAndStatuses = ratingsAndStatuses,
                    AvailableSelectRatings = new List<string> { "Select Rating", "0 (Awful)", "1 (Appalling)", "3 (Very Bad)", "4 (Bad)", "5 (Average)", "6 (Fine)", "7 (Good)", "8 (Very Good)", "9 (Great)", "10 (GOTY)" },
                    AvailableSelectStatus = new List<string> { "Select Status", "Playing", "Completed", "On-Hold", "Dropped", "Plan to Play" }
                };
                return View(viewModel);
            }
        }
        //This is an HttpPost method that adds a game to a user's game list. It first checks if the user is authenticated, and if not, it redirects to the login page.
        //If the user is authenticated, it checks if the game already exists in the user's game list. If the game already exists, it redirects to the game info page.
        //If the game doesn't exist, it converts the rating and status from string to int and creates a new instance of the UserGameList class with the provided information.
        //Then, it adds the new UserGameList to the repository and redirects to the game info page.
        [HttpPost]
        public async Task<IActionResult> AddGameList(int gameId, string gameName, string? gameImageId, string gameStatus, string gameRating)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return RedirectToAction("Login", "Account");
            }
            else
            {
                string appUserId = user.Id;
                var gameCheck = _listRepository.CheckGameDuplicateWithUser(gameId, appUserId);

                if (gameCheck)
                    {
                         return RedirectToAction("GameInfo", "Game", new { id = gameId});
                    }
                else
                {

                    Dictionary<string, int> ratingLookup = new Dictionary<string, int>
                    {
                        {"0 (Awful)",       0},
                        {"1 (Appalling)",   1},
                        {"2 (Horrible)",    2},
                        {"3 (Very Bad)",    3},
                        {"4 (Bad)",         4},
                        {"5 (Average)",     5},
                        {"6 (Fine)",        6},
                        {"7 (Good)",        7},
                        {"8 (Very Good)",   8},
                        {"9 (Great)",       9},
                        {"10 (GOTY)",        10},
                        {"Select Rating",   11},
                        {"", 11}

                    };

                    string ratingValue = gameRating ?? "";

                    int finalRating = ratingLookup[ratingValue];

                   Dictionary<string, int> statusLookup = new Dictionary<string, int>
                       {
                           {"Playing",          1},
                           {"Completed",        2},
                           {"On-Hold",          3},
                           {"Dropped",          4},
                           {"Plan to Play",     5},
                           {"Select Status",    0},
                           {"", 0}

                       };

                string statusValue = gameStatus ?? "";

                int finalStatus = statusLookup[statusValue];

                var userGameList = new UserGameList
                    {
                        
                        GameId = gameId,
                        GameName = gameName,
                        GameStatus = finalStatus,
                        GameRating = finalRating,
                        GameImageId = gameImageId,
                        AppUserId = user.Id
                    };
                    _listRepository.Add(userGameList);

                 return RedirectToAction("GameInfo", "Game", new { id = gameId });
                }
            }
        }
        //This action method updates the game status and rating for a user's game list. It first retrieves the current user information, checks if the user is logged in, and if so,
        //converts the status and rating values to integers based on dictionaries, updates the game information in the database, and redirects the user to one of several
        //different pages based on the return URL.
        [HttpPost]
        public async Task<IActionResult> UpdateAddGameList(int gameId, string gameStatus, string gameRating, int? status)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return RedirectToAction("Login", "Account");
            }
            else
            {
                string appUserId = user.Id;

                    Dictionary<string, int> ratingLookup = new Dictionary<string, int>
                        {
                            {"0 (Awful)",       0},
                            {"1 (Appalling)",   1},
                            {"2 (Horrible)",    2},
                            {"3 (Very Bad)",    3},
                            {"4 (Bad)",         4},
                            {"5 (Average)",     5},
                            {"6 (Fine)",        6},
                            {"7 (Good)",        7},
                            {"8 (Very Good)",   8},
                            {"9 (Great)",       9},
                            {"10 (GOTY)",        10},
                            {"Select Rating",   11},
                            {"", 11}

                        };

                    string ratingValue = gameRating ?? "";

                    int finalRating = ratingLookup[ratingValue];

                    Dictionary<string, int> statusLookup = new Dictionary<string, int>
                           {
                               {"Playing",          1},
                               {"Completed",        2},
                               {"On-Hold",          3},
                               {"Dropped",          4},
                               {"Plan to Play",     5},
                               {"Select Status",    0},
                               {"", 0}

                           };

                    string statusValue = gameStatus ?? "";

                    int finalStatus = statusLookup[statusValue];


                    _listRepository.UpdateGameRatingAndStatus(gameId, appUserId, finalRating, finalStatus);
                    
                    string returnUrl = Request.Form["returnUrl"];
                    if (returnUrl.Contains("GameInfo"))
                    {
                        return RedirectToAction("GameInfo", "Game", new { id = gameId });
                    }
                    else if (returnUrl.Contains("EditUserGameList"))
                    {
                        return RedirectToAction("EditUserGameList", "List");
                    }
                    else if (returnUrl.Contains("EditSpecificStatus"))
                    {
                        return RedirectToAction("EditSpecificStatus", "List", new { status = status });
                    }
                    else
                    {
                        return RedirectToAction("Index", "Home");
                    }
                
            }
        }

        //This is a HttpPost method in a List controller that removes a game from a user's list. It first checks if the user is logged in and redirects to the login page if not.
        //If the user is logged in, it gets the user's Id and calls the RemoveGame method in _listRepository passing the gameId and appUserId as parameters.
        //It then redirects to a specific action based on the return URL (stored in a form variable) which can be either "EditUserGameList", "EditSpecificStatus", or "Index".
        [HttpPost]
        public async Task<IActionResult> RemoveGame(int gameId, int?status)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return RedirectToAction("Login", "Account");
            }
            else
            {

                string appUserId = user.Id;
                _listRepository.RemoveGame(gameId, appUserId);

                string returnUrl = Request.Form["returnUrl"];
                if (returnUrl.Contains("EditUserGameList"))
                {
                    return RedirectToAction("EditUserGameList", "List");
                }
                else if (returnUrl.Contains("EditSpecificStatus"))
                {
                    return RedirectToAction("EditSpecificStatus", "List", new { status = status });
                }
                else
                {
                    return RedirectToAction("Index", "Home");
                }

            }
        }

        //This action method retrieves a list of games for a user based on their username.
        //The action method is annotated with a [Route] attribute with the parameter "GameList/{UserName}" which means it can be accessed using a URL like "/GameList/[UserName]".
        //The action method takes an integer "status" as a parameter, which is used to filter the list of games returned.If "status" is 0, then all games are returned, otherwise only games with the specified status are returned.        
        //First, the action method retrieves the "UserName" from the RouteData and checks if it is null. If it is null, the method redirects to the "Login" action on the "Account" controller.        
        //Next, the method uses the UserManager class to retrieve the user by their username.If the user is found, it gets their "Id" and uses the _listRepository object to retrieve the list of games for that user either with or without the specified status.       
        //For each game in the list, the game status is converted from an integer to a string using a dictionary of integers and statuses.The final statuses are added to a list of strings.       
        //Next, the action method creates a dictionary of integers (game ids) and tuples of strings (final statuses). The view model is then created and populated with the list of games, final statuses, the return URL, the statuses dictionary, and various UI customization options for the user.        
        //Finally, the view model is passed to the view and the method returns the view.
        [Route("GameList/{UserName}")]
        public async Task<IActionResult> UserGameList(int status)
        {


            string userName = RouteData.Values["UserName"].ToString();
            if (userName == null)
            {

                return RedirectToAction("Login", "Account");
            }
            else
            {
                if(status == 0)
                {

                     var user = await _userManager.FindByNameAsync(userName);
                     if (user != null)
                     {
                         string appUserId = user.Id;
                         IEnumerable<UserGameList> userGameLists = await _listRepository.GetListByUserId(appUserId);



                         List<string> finalStatuses = new List<string>();
                         foreach (var userGameList in userGameLists)
                         {
                             Dictionary<int, string> statusLookup = new Dictionary<int, string>
                                {
                                    { 1, "Playing"         },
                                    { 2, "Completed"     },
                                    { 3, "On-Hold"        },
                                    { 4, "Dropped"         },
                                    { 5, "Plan to Play"    },
                                    { 0, "Select Status" }

                                };

                             int gameStatus = userGameList.GameStatus ?? 0;
                             string finalStatus = statusLookup[gameStatus];
                             finalStatuses.Add(finalStatus);

                         }
                         Dictionary<int, Tuple<string>> statuses = new Dictionary<int, Tuple<string>>();

                         for (int i = 0; i < userGameLists.Count(); i++)
                         {
                             statuses.Add(userGameLists.ElementAt(i).GameId, new Tuple<string>(finalStatuses[i]));
                         }

                         string returnUrl = Request.GetDisplayUrl();
                        var viewModel = new UserGameListViewModel
                        {
                            UserGameLists = userGameLists,
                            FinalStatuses = finalStatuses,
                            ReturnUrl = returnUrl,
                            Statuses = statuses,
                            Status = status,
                            UserName = userName,
                            DisplayName = user.DisplayName,
                            BgColorBody = user.BgColorBody ?? "#0c0c0c",
                            BgColorDisplay = user.BgColorDisplay ?? "#1d1d1d",
                            FontColorDisplayName = user.FontColorDisplayName ?? "#b5b5b5",
                            BgColorGameListLogo =        user.BgColorGameListLogo ?? "#272727",
                            BgColorStatusButtonActive =  user.BgColorStatusButtonActive ?? "#4a4a4a",
                            BgColorStatusButton =        user.BgColorStatusButton ?? "#313131",
                            FontColorStatusButton =      user.FontColorStatusButton ?? "#b5b5b5",
                            BgColorStatus =              user.BgColorStatus ?? "#272727",
                            BgColorSearch =              user.BgColorSearch ?? "#1d1d1d",
                            FontColorSearch =            user.FontColorSearch ?? "#b5b5b5",
                            BgColorTable =               user.BgColorTable ?? "#272727",
                            FontColorTable =             user.FontColorTable ?? "#b5b5b5",
                            BgColorGameTile =            user.BgColorGameTile ?? "#262626",
                            BgColorGamename =            user.BgColorGamename ?? "#313131",
                            FontColorGamename =          user.FontColorGamename ?? "#b5b5b5",
                            BgColorFooter =             user.BgColorFooter ?? "#272727"

                    };
                         return View(viewModel);



                     }
                     else
                     {
                         return RedirectToAction("Login", "Account");
                     }
                }

                else
                {


                    var user = await _userManager.FindByNameAsync(userName);
                    if (user != null)
                    {
                        string appUserId = user.Id;
                        IEnumerable<UserGameList> userGameLists = await _listRepository.GetListByUserIdAndStatus(appUserId, status);



                        List<string> finalStatuses = new List<string>();
                        foreach (var userGameList in userGameLists)
                        {
                            Dictionary<int, string> statusLookup = new Dictionary<int, string>
                                {
                                    { 1, "Playing"         },
                                    { 2, "Completed"     },
                                    { 3, "On-Hold"        },
                                    { 4, "Dropped"         },
                                    { 5, "Plan to Play"    },
                                    { 0, "Select Status" }

                                };

                            int gameStatus = userGameList.GameStatus ?? 0;
                            string finalStatus = statusLookup[gameStatus];
                            finalStatuses.Add(finalStatus);

                        }
                        Dictionary<int, Tuple<string>> statuses = new Dictionary<int, Tuple<string>>();

                        for (int i = 0; i < userGameLists.Count(); i++)
                        {
                            statuses.Add(userGameLists.ElementAt(i).GameId, new Tuple<string>(finalStatuses[i]));
                        }

                        string returnUrl = Request.GetDisplayUrl();
                        var viewModel = new UserGameListViewModel
                        {
                            UserGameLists = userGameLists,
                            FinalStatuses = finalStatuses,
                            ReturnUrl = returnUrl,
                            Statuses = statuses,
                            Status = status,
                            UserName = userName,
                            DisplayName = user.DisplayName,
                            BgColorBody = user.BgColorBody ?? "#0c0c0c",
                            BgColorDisplay = user.BgColorDisplay ?? "#1d1d1d",
                            FontColorDisplayName = user.FontColorDisplayName ?? "#b5b5b5",
                            BgColorGameListLogo = user.BgColorGameListLogo ?? "#272727",
                            BgColorStatusButtonActive = user.BgColorStatusButtonActive ?? "#4a4a4a",
                            BgColorStatusButton = user.BgColorStatusButton ?? "#313131",
                            FontColorStatusButton = user.FontColorStatusButton ?? "#b5b5b5",
                            BgColorStatus = user.BgColorStatus ?? "#272727",
                            BgColorSearch = user.BgColorSearch ?? "#1d1d1d",
                            FontColorSearch = user.FontColorSearch ?? "#b5b5b5",
                            BgColorTable = user.BgColorTable ?? "#272727",
                            FontColorTable = user.FontColorTable ?? "#b5b5b5",
                            BgColorGameTile = user.BgColorGameTile ?? "#262626",
                            BgColorGamename = user.BgColorGamename ?? "#313131",
                            FontColorGamename = user.FontColorGamename ?? "#b5b5b5",
                            BgColorFooter = user.BgColorFooter ?? "#272727"
                        };
                        return View(viewModel);
                    }
                    else
                    {
                        return RedirectToAction("Login", "Account");
                    }
                }
            }
        }
        //CustomiseList (GET): Retrieves the current user and returns the CustomiseList view with a CustomiseListViewModel object populated with user details,
        //including the user's background colors for body, display, and game tiles. If the user is not found, it redirects to the Login action of the Account controller.
        [HttpGet]
        public async Task<IActionResult> CustomiseList()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return RedirectToAction("Login", "Account");
            }
            else
            {
                var customList = new CustomiseListViewModel()
                {
                    UserName = user.UserName,
                    BgColorBody = user.BgColorBody ?? "#0c0c0c",
                    BgColorDisplay = user.BgColorDisplay ?? "#1d1d1d",
                    BgColorGameTile = user.BgColorGameTile ?? "#262626"
                };
                return View(customList);
            }
        }
        //CustomiseList (POST): Retrieves the current user and updates the user's background colors for body, display, and game tiles, using values from the posted CustomiseListViewModel object.
        //If the user is not found, it redirects to the Login action of the Account controller. The updated view model with only the user name is then returned to the view.
        [HttpPost]
        public async Task<IActionResult> CustomiseList(CustomiseListViewModel customList)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return RedirectToAction("Login", "Account");
            }
            else
            {
                if (!string.IsNullOrEmpty(customList.BgColorBody))
                {
                    user.BgColorBody = customList.BgColorBody;
                }

                if (!string.IsNullOrEmpty(customList.BgColorDisplay))
                {
                    user.BgColorDisplay = customList.BgColorDisplay;
                }

                if (!string.IsNullOrEmpty(customList.BgColorGameTile))
                {
                    user.BgColorGameTile = customList.BgColorGameTile;
                }

                await _userManager.UpdateAsync(user);

                var userName = new CustomiseListViewModel()
                {
                    UserName = user.UserName
                };
                return View(userName);
            }
        }
        //AdvancedCustomiseList: A GET method that retrieves the color customization settings for the current user. If the user is not authenticated, it redirects the user to the login page.
        //If the user is authenticated, it retrieves the user's color customization settings from the database and returns it to the view. If a setting does not exist, a default color value is used.
        [HttpGet]
        public async Task<IActionResult> AdvancedCustomiseList()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return RedirectToAction("Login", "Account");
            }
            else
            {
                var customList = new AdvancedCustomiseListViewModel()
                {
                    UserName = user.UserName,
                    BgColorBody = user.BgColorBody ?? "#0c0c0c",
                    BgColorDisplay = user.BgColorDisplay ?? "#1d1d1d",
                    FontColorDisplayName = user.FontColorDisplayName ?? "#b5b5b5",
                    BgColorGameListLogo = user.BgColorGameListLogo ?? "#272727",
                    BgColorStatusButtonActive = user.BgColorStatusButtonActive ?? "#313131",
                    BgColorStatusButton = user.BgColorStatusButton ?? "#262626",
                    FontColorStatusButton = user.FontColorStatusButton ?? "#b5b5b5",
                    BgColorStatus = user.BgColorStatus ?? "#272727",
                    BgColorSearch = user.BgColorSearch ?? "#1d1d1d",
                    FontColorSearch = user.FontColorSearch ?? "#b5b5b5",
                    BgColorTable = user.BgColorTable ?? "#272727",
                    FontColorTable = user.FontColorTable ?? "#b5b5b5",
                    BgColorGameTile = user.BgColorGameTile ?? "#262626",
                    BgColorGamename = user.BgColorGamename ?? "#313131",
                    FontColorGamename = user.FontColorGamename ?? "#b5b5b5",
                    BgColorFooter = user.BgColorFooter ?? "#272727"
                };
                return View(customList);
            }
        }
        //AdvancedCustomiseList: A POST method that saves the updated color customization settings to the database. If the user is not authenticated, it redirects the user to the login page.
        //If the user is authenticated, it updates the user's color customization settings in the database. The updated values are saved to the user's profile if the corresponding properties
        //are not null or empty.
        [HttpPost]
        public async Task<IActionResult> AdvancedCustomiseList(AdvancedCustomiseListViewModel customList)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return RedirectToAction("Login", "Account");
            }
            else
            {
                if (!string.IsNullOrEmpty(customList.BgColorBody))
                {
                    user.BgColorBody = customList.BgColorBody;
                }

                if (!string.IsNullOrEmpty(customList.BgColorDisplay))
                {
                    user.BgColorDisplay = customList.BgColorDisplay;
                }

                if (!string.IsNullOrEmpty(customList.FontColorDisplayName))
                {
                    user.FontColorDisplayName = customList.FontColorDisplayName;
                }

                if (!string.IsNullOrEmpty(customList.BgColorGameListLogo))
                {
                    user.BgColorGameListLogo = customList.BgColorGameListLogo;
                }

                if (!string.IsNullOrEmpty(customList.BgColorStatusButtonActive))
                {
                    user.BgColorStatusButtonActive = customList.BgColorStatusButtonActive;
                }

                if (!string.IsNullOrEmpty(customList.BgColorStatusButton))
                {
                    user.BgColorStatusButton = customList.BgColorStatusButton;
                }

                if (!string.IsNullOrEmpty(customList.FontColorStatusButton))
                {
                    user.FontColorStatusButton = customList.FontColorStatusButton;
                }

                if (!string.IsNullOrEmpty(customList.BgColorStatus))
                {
                    user.BgColorStatus = customList.BgColorStatus;
                }

                if (!string.IsNullOrEmpty(customList.BgColorSearch))
                {
                    user.BgColorSearch = customList.BgColorSearch;
                }

                if (!string.IsNullOrEmpty(customList.FontColorSearch))
                {
                    user.FontColorSearch = customList.FontColorSearch;
                }

                if (!string.IsNullOrEmpty(customList.BgColorTable))
                {
                    user.BgColorTable = customList.BgColorTable;
                }

                if (!string.IsNullOrEmpty(customList.FontColorTable))
                {
                    user.FontColorTable = customList.FontColorTable;
                }

                if (!string.IsNullOrEmpty(customList.BgColorGameTile))
                {
                    user.BgColorGameTile = customList.BgColorGameTile;
                }

                if (!string.IsNullOrEmpty(customList.BgColorGamename))
                {
                    user.BgColorGamename = customList.BgColorGamename;
                }

                if (!string.IsNullOrEmpty(customList.FontColorGamename))
                {
                    user.FontColorGamename = customList.FontColorGamename;
                }
                if (!string.IsNullOrEmpty(customList.BgColorFooter))
                {
                    user.BgColorFooter = customList.BgColorFooter;
                }
                await _userManager.UpdateAsync(user);

                var userName = new AdvancedCustomiseListViewModel()
                {
                    UserName = user.UserName
                };
                return View(userName);
            }
        }
        //The ResetDefault and AdvancedResetDefault methods are used to reset the customization settings for a user in a web application. The methods retrieve the current user,
        //and if the user exists, the customization settings for the user are reset to their default values and the user is updated in the database. If the user does not exist,
        //the user is redirected to the login page. The ResetDefault method resets a limited set of customization settings, while the AdvancedResetDefault method resets all
        //customization settings for the user.
        public async Task<IActionResult> ResetDefault()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return RedirectToAction("Login", "Account");
            }
            else
            {
                user.BgColorBody = null;
                user.BgColorDisplay = null;
                user.BgColorGameTile = null;

                await _userManager.UpdateAsync(user);

                return RedirectToAction("CustomiseList", "List");
            }
        }

        public async Task<IActionResult> AdvancedResetDefault()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return RedirectToAction("Login", "Account");
            }
            else
            {
                user.BgColorBody                       = null;
                user.BgColorDisplay                    = null;
                user.FontColorDisplayName              = null;
                user.BgColorGameListLogo               = null;
                user.BgColorStatusButtonActive         = null;
                user.BgColorStatusButton               = null;
                user.FontColorStatusButton             = null;
                user.BgColorStatus                     = null;
                user.BgColorSearch                     = null;
                user.FontColorSearch                   = null;
                user.BgColorTable                      = null;
                user.FontColorTable                    = null;
                user.BgColorGameTile                   = null;
                user.BgColorGamename                   = null;
                user.FontColorGamename                 = null;
                user.BgColorFooter = null;
                await _userManager.UpdateAsync(user);

                return RedirectToAction("AdvancedCustomiseList", "List");
            }
        }
    }
}
