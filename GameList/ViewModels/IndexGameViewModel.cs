namespace GameList.ViewModels
{
    public class IndexGameViewModel
    {

        public IEnumerable<IGDB.Models.Game> RandomGames { get; set; }
        public IEnumerable<IGDB.Models.Game> UpcomingGames { get; set; }
    }
}
