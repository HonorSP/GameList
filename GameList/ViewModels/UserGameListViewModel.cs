using GameList.Models;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace GameList.ViewModels
{
    public class UserGameListViewModel
    {
        public IEnumerable<UserGameList> UserGameLists { get; set; }
        public List<string> FinalStatuses { get; set; }
        public Dictionary<int, Tuple<string>> Statuses { get; set; }
        public string UserName { get; set; }
        public string? DisplayName{ get; set; }
        public string ReturnUrl { get; set; }
        public int? Status { get; set; }
        public string? BgColorBody { get; set; }
        public string? BgColorDisplay { get; set; }
        public string? FontColorDisplayName { get; set; }
        public string? BgColorGameListLogo        { get; set; }
        public string? BgColorStatusButtonActive  { get; set; }
        public string? BgColorStatusButton        { get; set; }
        public string? FontColorStatusButton      { get; set; }
        public string? BgColorStatus              { get; set; }
        public string? BgColorSearch              { get; set; }
        public string? FontColorSearch            { get; set; }
        public string? BgColorTable               { get; set; }
        public string? FontColorTable             { get; set; }
        public string? BgColorGameTile            { get; set; }
        public string? BgColorGamename            { get; set; }
        public string? FontColorGamename          { get; set; }
        public string? BgColorFooter { get; set; }
    }
}
