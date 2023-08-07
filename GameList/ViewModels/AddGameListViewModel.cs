namespace GameList.ViewModels
{
    public class AddGameListViewModel
    {
        public AddGameListViewModel()
        {
            GameRating = 11;
            GameStatus = 0;
        }
        public int GameId { get; set; }
        public string GameName { get; set; }
        public int GameStatus { get; set; }
        public int GameRating { get; set; }
        public string AppUserId { get; set; }
    }
}
