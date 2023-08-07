using GameList.Models;
using System.Collections.Generic;

namespace GameList.ViewModels
{
    public class EditUserGameListViewModel
    {
        public IEnumerable<UserGameList> UserGameLists { get; set; }
        public List<string> FinalRatings { get; set; }
        public List<string> FinalStatuses { get; set; }
        public Dictionary<int, Tuple<string, string>> ratingsAndStatuses { get; set; }
        public IEnumerable<string> AvailableSelectRatings { get; set; }
        public IEnumerable<string> AvailableSelectStatus { get; set; }
        public string ReturnUrl { get; set; }
        public int? Status { get; set; }
    }
}
