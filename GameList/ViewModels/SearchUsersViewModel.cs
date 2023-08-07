namespace GameList.ViewModels
{
    public class SearchUsersViewModel
    {
        public string? SearchQuery { get; set; }
        public List<UserViewModel> Users { get; set; }
        public int Page { get; set; }
        public int PageSize { get; set; }
        public int TotalPages { get; set; }
    }
}
