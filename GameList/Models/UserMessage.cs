using System.ComponentModel.DataAnnotations.Schema;

namespace GameList.Models
{
    public class UserMessage
    {
        public int Id { get; set; }
        public string SenderId { get; set; }
        public string ReceiverId { get; set; }
        public string Message { get; set; }
        public DateTime SentTime { get; set; }

        [ForeignKey("SenderId")]
        public virtual AppUser Sender { get; set; }
        [ForeignKey("ReceiverId")]
        public virtual AppUser Receiver { get; set; }
    }
}