
using GameList.Models;
using GameList.ViewModels;
using IGDB;
using IGDB.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Diagnostics;

namespace GameList.Controllers
{
    //The code is a HomeController class in a .NET Core MVC application, it has several actions for handling user requests for different pages,
    //it also utilizes the IGDBClient class to communicate with the IGDB API and retrieve game information to be displayed on the index page.
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IGDBClient igdb;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
            igdb = new IGDBClient("mglx6fger7kprdp4o25d74g9dg9ewf", "128zb34th4o2k2sdoeci8iqhq9x6wk");
        }
        //Index: retrieves random and upcoming games information from the IGDB API, creates an IndexGameViewModel instance and passes it to the view.
        public async Task<IActionResult> Index()
        {
            IEnumerable<IGDB.Models.Game> randomGames = await igdb.QueryAsync<Game> (IGDBClient.Endpoints.Games, query: "fields name, " +
                "cover.image_id, follows, first_release_date, aggregated_rating, genres.name ;where rating_count > 500; limit 32;");


            IEnumerable<IGDB.Models.Game> upcomingGames = await igdb.QueryAsync<Game>(IGDBClient.Endpoints.Games, query: "fields name, " +
                "cover.image_id, hypes, first_release_date; where first_release_date > 1674055471 & first_release_date < 1705591471 " +
                "& hypes != null; sort hypes desc; limit 10;");
            var games = new IndexGameViewModel
            {
                RandomGames = randomGames,
                UpcomingGames = upcomingGames
            };

            return View(games);
        }
        //Privacy: returns a view for the privacy policy page.
        public IActionResult Privacy()
        {
            return View();
        }
        //Cookies: returns a view for the cookies policy page.
        public IActionResult Cookies()
        {
            return View();
        }
        //FAQs: returns a view for the FAQs page.
        public IActionResult FAQs()
        {
            return View();
        }
        //ToggleTheme: sets a theme cookie in the user's browser and redirects the user back to the previous page.
        public IActionResult ToggleTheme(string theme)
        {

            Response.Cookies.Append("theme", theme, new CookieOptions
            {
                Expires = DateTime.Now.AddYears(1),
                Secure = true
        });
            var referer = Request.Headers["Referer"].ToString();
            return Redirect(referer);
        }


    }
}