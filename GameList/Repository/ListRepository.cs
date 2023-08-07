using GameList.Data;
using GameList.Interfaces;
using GameList.Models;
using IGDB.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace GameList.Repository
{
    public class ListRepository : IListRepository
    {
        private readonly ApplicationDbContext _context;

        public ListRepository(ApplicationDbContext context)
        {
            _context = context;
        }
        public bool Add(UserGameList userGameList)
        {
           _context .Add(userGameList);
            return Save();
        }
        public bool Delete(UserGameList userGameList)
        {
            _context.Remove(userGameList);
            return Save();
        }
        public async Task<IEnumerable<UserGameList>> GetAll()
        {
            return await _context.UserGameLists.ToListAsync();
        }
        public async Task<IEnumerable<UserGameList>> GetListByUserId(string appUserId)
        {
            return await _context.UserGameLists.Where(c => c.AppUserId.Contains(appUserId)).ToListAsync();
        }
        public async Task<IEnumerable<UserGameList>> GetListByUserIdAndStatus(string appUserId, int status)
        {
            return await _context.UserGameLists.Where(c => c.AppUserId.Contains(appUserId) && c.GameStatus.Value.Equals(status)).ToListAsync();
        }
        public async Task<IEnumerable<UserGameList>> GetAllByGameStatus(int gameStatus)
        {
            return await _context.UserGameLists.Where(c => c.GameStatus == gameStatus).ToListAsync();
        }
        public async Task<int> GetCountByUserIdAndStatus(string appUserId, int status)
        {
            return await _context.UserGameLists
                .Where(c => c.AppUserId == appUserId && c.GameStatus == status)
                .CountAsync();
        }
        public async Task<int> GetCountByUserIdAndRating(string appUserId, int rating)
        {
            return await _context.UserGameLists
                .Where(c => c.AppUserId == appUserId && c.GameRating == rating)
                .CountAsync();
        }
        public async Task<int> GetCountByUserId(string appUserId)
        {
            return await _context.UserGameLists.Where(c => c.AppUserId == appUserId).CountAsync();
        }

        public bool CheckGameDuplicate(int gameId)
        {
            return _context.UserGameLists.Any(g => g.GameId == gameId);
        }
        public bool CheckGameDuplicateWithUser(int gameId, string appUserId)
        {
            return _context.UserGameLists.Where(c => c.AppUserId == appUserId).Any(g => g.GameId == gameId);
        }

        public async Task<UserGameList> GetIdByAsync(int id)
        {
            return await _context.UserGameLists.FirstOrDefaultAsync(i => i.Id == id);
        }

        public (bool isDuplicate, int? GameRating, int? GameStatus) GetGameCheckRatingStatus(int gameId, string appUserId)
        {
            var result = _context.UserGameLists
                .Where(c => c.AppUserId == appUserId && c.GameId == gameId)
                .Select(x => new { IsDuplicate = true, x.GameRating, x.GameStatus });

            return result.Any() ? (result.First().IsDuplicate, result.First().GameRating, result.First().GameStatus) : (false, null, null);
        }
        public bool UpdateGameRatingAndStatus(int gameId, string appUserId, int finalRating, int finalStatus)
        {
            var existingRecord = _context.UserGameLists.FirstOrDefault(x => x.AppUserId == appUserId && x.GameId == gameId);
            if (existingRecord != null)
            {
                existingRecord.GameRating = finalRating;
                existingRecord.GameStatus = finalStatus;
                _context.SaveChanges();
                return true;
            }
            return false;
        }
        public bool Save()
        {
            var saved = _context.SaveChanges();
            return saved > 0;
        }

        public bool Update(UserGameList userGameList)
        {
            throw new NotImplementedException();
        }
        public bool RemoveGame(int gameId, string appUserId)
        {
            var userGameList = _context.UserGameLists.FirstOrDefault(u => u.GameId == gameId && u.AppUserId == appUserId);
            if (userGameList != null)
            {
                _context.Remove(userGameList);
                return Save();
            }
            return false;
        }


        public async Task<double> GetAverageRatingByGameId(int gameId)
        {
            var gameRatings = await _context.UserGameLists
                .Where(c => c.GameId == gameId && c.GameRating >= 0 && c.GameRating <= 10)
                .ToListAsync();

            if (gameRatings.Count < 3)
            {
                return 11;
            }

            double totalRating = 0;
            foreach (var rating in gameRatings)
            {
                totalRating += rating.GameRating ?? 1000;
            }

            return totalRating / gameRatings.Count;
        }
        public async Task<int> GetCountRatingByGameId(int gameId)
        {
            var gameRatings = await _context.UserGameLists
                .Where(c => c.GameId == gameId && c.GameRating >= 0 && c.GameRating <= 10)
                .ToListAsync();

            return  gameRatings.Count;
        }


    }
}
