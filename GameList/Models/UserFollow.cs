using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GameList.Models
{
    public class UserFollow
    {
        [Key]
        public int Id { get; set; }
        public string FollowerId { get; set; }
        public string FollowedId { get; set; }
        public DateTime? FollowTime { get; set; }
        [ForeignKey("FollowerId")]
        public virtual AppUser Follower { get; set; }
        [ForeignKey("FollowedId")]
        public virtual AppUser Followed { get; set; }
    }
}
