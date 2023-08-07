using GameList.Data;
using GameList.Interfaces;
using GameList.Models;
using GameList.ViewModels;

namespace GameList.Repository
{
    public class MessageRepository : IMessageRepository
    {

        private readonly ApplicationDbContext _context;

        public MessageRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public IEnumerable<UserMessage> GetAllMessagesBetweenUsers(string user1Id, string user2Id)
        {
            var messages = _context.UserMessages
                .Where(m => (m.SenderId == user1Id && m.ReceiverId == user2Id)
                        || (m.SenderId == user2Id && m.ReceiverId == user1Id))
                .OrderByDescending(m => m.SentTime);

            return messages;
        }


        public bool Add(UserMessage userMessage)
        {
            _context.Add(userMessage);
            return Save();
        }

        public bool Delete(UserMessage userMessage)
        {
            _context.Remove(userMessage);
            return Save();
        }

        public bool Save()
        {
            var saved = _context.SaveChanges();
            return saved > 0;
        }

        public bool Update(UserMessage userMessage)
        {
            throw new NotImplementedException();
        }
    }
}
