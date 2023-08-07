using GameList.Models;
using System.ComponentModel.DataAnnotations;

namespace GameList.ViewModels
{
    public class AllMessagesViewModel
    {
        public IEnumerable<UserMessage> Messages { get; set; }
        public string ReceiverId { get; set; }
        public string ReceiverName { get; set; }
        public string ReceiverDisplayName { get; set; }
        public string ReceiverProfilePictureUrl { get; set; }

        [Required(ErrorMessage = "Please enter a message.")]
        [RegularExpression("^[a-zA-Z0-9 .!?,]*$", ErrorMessage = "Only letters, numbers, spaces, commas, and full stops are allowed.")]
        [StringLength(500, MinimumLength = 0)]
        public string Message { get; set; }


    }
}
