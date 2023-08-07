using GameList.Models;
using GameList.ViewModels;

namespace GameList.Interfaces
{
    public interface IMessageRepository
    {
        IEnumerable<UserMessage> GetAllMessagesBetweenUsers(string user1Id, string user2Id);
        bool Add(UserMessage userMessage);
        bool Delete(UserMessage userMessage);
        bool Save();
        bool Update(UserMessage userMessage);
    }   
}
