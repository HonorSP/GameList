using System.ComponentModel.DataAnnotations;

namespace GameList.ViewModels
{
    public class Welcome2ViewModel
    {
        [RegularExpression("^[a-zA-Z0-9]*$", ErrorMessage = "Only letters and numbers are allowed.")]
        [StringLength(25, MinimumLength = 5)]
        public string? DisplayName { get; set; }
    }
}
