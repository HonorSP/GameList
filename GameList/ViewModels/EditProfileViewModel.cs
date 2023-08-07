using System.ComponentModel.DataAnnotations;

namespace GameList.ViewModels
{
    public class EditProfileViewModel
    {
        [RegularExpression("^[a-zA-Z0-9]*$", ErrorMessage = "Only letters and numbers are allowed.")]
        [StringLength(25, MinimumLength = 5)]
        public string? DisplayName { get; set; }
        [RegularExpression("^[a-zA-Z0-9 .!?,]*$", ErrorMessage = "Only letters, numbers, spaces, commas, and full stops are allowed.")]
        [StringLength(100, MinimumLength = 0)]
        public string? Bio { get; set; }
        public string? ProfilePictureUrl { get; set; }
        public string? UserName { get; set; }
        public IFormFile? Image { get; set; }
    }
}
