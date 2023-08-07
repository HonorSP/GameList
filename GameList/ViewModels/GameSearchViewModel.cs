namespace GameList.ViewModels
{
    public class GameSearchViewModel
    {
        public IEnumerable<IGDB.Models.Game> Results { get; set; }
        public IEnumerable<IGDB.Models.Company> ResultsCompany { get; set; }
        public IEnumerable<IGDB.Models.Character> ResultsCharacter { get; set; }
        public string SelectedRating { get; set; }
        public string SelectedYear { get; set; }
        public string SelectedPlatform { get; set; }
        public string SearchQuery { get; set; }
        public string SelectedSort { get; set; }
        public string SelectedType{ get; set; }
        public IEnumerable<string> AvailableRatings { get; set; }
        public IEnumerable<string> AvailableYears { get; set; }
        public IEnumerable<string> AvailablePlatforms { get; set; }
        public IEnumerable<string> AvailableSorting { get; set; }
        public IEnumerable<string> AvailableTypes { get; set; }
        public int CurrentPage { get; set; }
        public int TotalPages { get; set; }
    }
}
