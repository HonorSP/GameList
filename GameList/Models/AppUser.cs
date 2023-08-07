using Microsoft.AspNetCore.Identity;

namespace GameList.Models
{

    public class AppUser : IdentityUser
    {
        public string? DisplayName { get; set; }
        public string? ProfilePictureUrl { get; set; }
        public string? UserBio { get; set; }
        public DateTime? UserCreateTime { get; set; }
        public string? BgColorBody                     { get; set; }
        public string? BgColorDisplay                   { get; set; }
        public string? FontColorDisplayName             { get; set; }
        public string? BgColorGameListLogo              { get; set; }
        public string? BgColorStatusButtonActive        { get; set; }
        public string? BgColorStatusButton              { get; set; }
        public string? FontColorStatusButton            { get; set; }
        public string? BgColorStatus                    { get; set; }
        public string? BgColorSearch                    { get; set; }
        public string? FontColorSearch                  { get; set; }
        public string? BgColorTable                     { get; set; }
        public string? FontColorTable                   { get; set; }
        public string? BgColorGameTile                  { get; set; }
        public string? BgColorGamename                  { get; set; }
        public string? FontColorGamename                { get; set; }
        public string? BgColorFooter                    { get; set; }
        public ICollection<UserFollow> Following { get; set; }
        public ICollection<UserFollow> Followers { get; set; }
        public ICollection<UserMessage> SentMessages { get; set; }
        public ICollection<UserMessage> ReceivedMessages { get; set; }
    }
}
