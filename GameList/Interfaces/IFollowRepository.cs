using GameList.Models;
using GameList.ViewModels;

namespace GameList.Interfaces
{
    public interface IFollowRepository
    {

        bool IsFollowing(string followerId, string followedId);
        bool AreBothFollowing(string followerId, string followedId);
        Task<IEnumerable<AvailableUser>> GetAvailableUsers(string userId);
        bool AddFollower(string followerId, string followedId);
        bool RemoveFollower(string followerId, string followedId);
        int CountFollowers(string userId);
        int CountFollowing(string userId);
        List<string> GetFollowingIds(string userId);
        List<string> GetFollowerIds(string userId);
        bool Add(UserFollow userFollow);
        bool Delete(UserFollow userFollow);
        bool Save();
        bool Update(UserFollow userFollow);
    }
}
