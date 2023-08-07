using IGDB.Models;

namespace GameList.ViewModels
{
    public class GameInfoViewModel
    {

        public IEnumerable<IGDB.Models.Game> GameDataInfo { get; set; }
        public bool? GameCheck { get; set; }
        public string GameRating { get; set; }
        public string GameStatus { get; set; }
        public string ReturnUrl { get; set; }
        public IEnumerable<string> AvailableSelectRatings { get; set; }
        public IEnumerable<string> AvailableSelectStatus { get; set; }

        public double AverageRating { get; set; }

        public int NumberRatings { get; set; }


        

    }
}
