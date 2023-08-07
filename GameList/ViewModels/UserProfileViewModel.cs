namespace GameList.ViewModels
{
    public class FeedItem
    {
        public string? UserId { get; set; }
        public string? UserName { get; set; }
        public string? Action { get; set; }
        public DateTime? Timestamp { get; set; }
    }
    public class UserProfileViewModel
    {
        public string Id { get; set; }
        public string UserNameCheck { get; set; }
        public string UserName { get; set; }
        public string? DisplayName { get; set; }
        public string? CurrentUserName { get; set; }
        public bool CheckIsFollowing { get; set; }
        public bool CheckBothAreFollowing { get; set; }
        public int Followers { get; set; }
        public int Following { get; set; }
        public string? Bio { get; set; }
        public string ProfilePictureUrl { get; set; }
        public int CountStatusAll { get; set; }
        public int CountStatusPlaying { get; set; }
        public int CountStatusCompleted { get; set; }
        public int CountStatusOnHold { get; set; }
        public int CountStatusDropped { get; set; }
        public int CountStatusPlanToPlay { get; set; }
        public int CountRating0   { get; set; }
        public int CountRating1   { get; set; }
        public int CountRating2   { get; set; }
        public int CountRating3   { get; set; }
        public int CountRating4   { get; set; }
        public int CountRating5   { get; set; }
        public int CountRating6   { get; set; }
        public int CountRating7   { get; set; }
        public int CountRating8   { get; set; }
        public int CountRating9   { get; set; }
        public int CountRating10 { get; set; }
        public List<FeedItem>? Feed { get; set; }
    }
}
