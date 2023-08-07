using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace GameList.Models
{
    public class UserGameList
    {
        public UserGameList()
        {
            GameRating = 11;
            GameStatus = 0;
        }
        [Key]
        public int Id { get; set; }
        public int GameId { get; set; }
        public string GameName { get; set; }
        public string? GameImageId { get; set; }
        public int? GameRating { get; set; }
        public int? GameStatus { get; set; }
        [ForeignKey("AppUser")]
        public string? AppUserId{ get; set; }
        public AppUser? AppUser { get; set; }
    }
}
