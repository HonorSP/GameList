namespace GameList.ViewModels
{
    public class FollowingFollowersViewModel
    {
        public string? FollowUserId { get; set; }
        public List<UserViewModel> Users { get; set; }
        public int Page { get; set; }
        public int PageSize { get; set; }
        public int TotalPages { get; set; }
    }
}
