using GameList.Data;
using GameList.Interfaces;
using GameList.Models;
using GameList.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace GameList.Repository
{
    public class FollowRepository : IFollowRepository
    {
        private readonly ApplicationDbContext _context;

        public FollowRepository(ApplicationDbContext context)
        {
            _context = context;
        }
        public bool IsFollowing(string followerId, string followedId)
        {
            var userFollow = _context.UserFollows.FirstOrDefault(uf => uf.FollowerId == followerId && uf.FollowedId == followedId);
            return userFollow != null;
        }
        public bool AreBothFollowing(string followerId, string followedId)
        {
            var follower = _context.UserFollows.FirstOrDefault(uf => uf.FollowerId == followerId && uf.FollowedId == followedId);
            var followed = _context.UserFollows.FirstOrDefault(uf => uf.FollowerId == followedId && uf.FollowedId == followerId);
            return (follower != null && followed != null);
        }


        public async Task<IEnumerable<AvailableUser>> GetAvailableUsers(string userId)
        {
            var followerIds = _context.UserFollows
                .Where(uf => uf.FollowedId == userId)
                .Select(uf => uf.FollowerId)
                .ToList();

            var followedIds = _context.UserFollows
                .Where(uf => uf.FollowerId == userId)
                .Select(uf => uf.FollowedId)
                .ToList();

            var availableUsers = _context.Users
                .Where(u => followerIds.Contains(u.Id) && followedIds.Contains(u.Id))
                .Select(u => new AvailableUser
                {
                    UserId = u.Id,
                    DisplayName = u.DisplayName ?? u.UserName,
                    UserName = u.UserName,
                    ProfilePictureUrl = u.ProfilePictureUrl ?? "/img/avatar.jpg"
                })
                .ToList();

            return availableUsers;
        }



        public bool AddFollower(string followerId, string followedId)
        {
            var userFollow = new UserFollow()
            {
                FollowerId = followerId,
                FollowedId = followedId,
                FollowTime = DateTime.UtcNow
            };
            _context.Add(userFollow);
            return Save();
        }
        public bool RemoveFollower(string followerId, string followedId)
        {
            var userFollow = _context.UserFollows.SingleOrDefault(uf => uf.FollowerId == followerId && uf.FollowedId == followedId);
            _context.Remove(userFollow);
            return Save();
        }
        public int CountFollowers(string userId)
        {
            return _context.UserFollows.Count(uf => uf.FollowedId == userId);
        }

        public int CountFollowing(string userId)
        {
            return _context.UserFollows.Count(uf => uf.FollowerId == userId);
        }


        public List<string> GetFollowingIds(string userId)
        {
            var followingIds = _context.UserFollows
                .Where(uf => uf.FollowerId == userId)
                .Select(uf => uf.FollowedId)
                .ToList();
            return followingIds;
        }

        public List<string> GetFollowerIds(string userId)
        {
            var followerIds = _context.UserFollows
                .Where(uf => uf.FollowedId == userId)
                .Select(uf => uf.FollowerId)
                .ToList();

            return followerIds;
        }

        public bool Add(UserFollow userFollow)
        {
            _context.Add(userFollow);
            return Save();
        }

        public bool Delete(UserFollow userFollow)
        {
            _context.Remove(userFollow);
            return Save();
        }

        public bool Save()
        {
            var saved = _context.SaveChanges();
            return saved > 0;
        }

        public bool Update(UserFollow userFollow)
        {
            throw new NotImplementedException();
        }
    }
}
