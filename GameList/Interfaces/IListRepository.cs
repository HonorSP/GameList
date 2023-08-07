using GameList.Models;

namespace GameList.Interfaces
{
    public interface IListRepository
    {
        Task<IEnumerable<UserGameList>> GetAll();
        Task<IEnumerable<UserGameList>> GetListByUserId(string appUserId);
        Task<IEnumerable<UserGameList>> GetAllByGameStatus(int gameStatus);
        Task<int> GetCountByUserIdAndStatus(string appUserId, int status);
        Task<int> GetCountByUserIdAndRating(string appUserId, int rating);
        Task<int> GetCountByUserId(string appUserId);
        bool CheckGameDuplicate(int gameId);
        bool CheckGameDuplicateWithUser(int gameId, string appUserId);
        public (bool isDuplicate, int? GameRating, int? GameStatus) GetGameCheckRatingStatus(int gameId, string appUserId);
        public bool UpdateGameRatingAndStatus(int gameId, string appUserId, int finalRating, int finalStatus);
        Task<UserGameList> GetIdByAsync(int id);
        Task<IEnumerable<UserGameList>> GetListByUserIdAndStatus(string appUserId, int status);
        bool Add(UserGameList userGameList);
        bool Update(UserGameList userGameList);
        bool Delete(UserGameList userGameList);
        bool Save();
        public bool RemoveGame(int gameId, string appUserId);
        Task<double> GetAverageRatingByGameId(int gameId);
        Task<int> GetCountRatingByGameId(int gameId);

    }
}
