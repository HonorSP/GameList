using GameList.Models;

namespace GameList.ViewModels
{
    public class AvailableUsersViewModel
    {
        public Task<IEnumerable<AvailableUser>> AvailableUsers { get; set; }
    }

    public class AvailableUser
    {
        public string UserId { get; set; }
        public string DisplayName { get; set; }
        public string UserName { get; set; }
        public string ProfilePictureUrl { get; set; }
    }
}
