using FluentAssertions;
using GameList.Data;
using GameList.Models;
using GameList.Repository;
using GameList.ViewModels;
using IGDB.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameList.Tests.Repository
{
    public class ListRepositoryTests
    {
        public ListRepositoryTests()
        {

        }

        private async Task<ApplicationDbContext> GetDbContext()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
            var databaseContext = new ApplicationDbContext(options);
            databaseContext.Database.EnsureCreated();
            if (await databaseContext.UserGameLists.CountAsync() == 0)
            {
                for (int i = 0; i < 10; i++)
                {
                    databaseContext.UserGameLists.Add(
                      new UserGameList()
                      {
                          GameId = i,
                          GameName = "game" + i,
                          GameImageId = "test message",
                          GameRating = i,
                          GameStatus = 3,
                          AppUserId = "userid1"

                      });
                    await databaseContext.SaveChangesAsync();
                }

                databaseContext.UserGameLists.Add(
                  new UserGameList()
                  {
                      GameId = 1,
                      GameName = "game1",
                      GameImageId = "test message",
                      GameRating = 7,
                      GameStatus = 3,
                      AppUserId = "userid1"

                  });
                await databaseContext.SaveChangesAsync();
                databaseContext.UserGameLists.Add(
                  new UserGameList()
                  {
                      GameId = 1,
                      GameName = "game1",
                      GameImageId = "test message",
                      GameRating = 6,
                      GameStatus = 3,
                      AppUserId = "userid1"

                  });
                await databaseContext.SaveChangesAsync();
            }
            return databaseContext;
        }

        [Fact]
        public async Task GetListByUserId_Returns_UserGameList()
        {
            // Arrange
            var context = await GetDbContext();
            var listRepository = new ListRepository(context);
            string appUserId = "userid1";

            // Act
            var result = await listRepository.GetListByUserId(appUserId);

            // Assert
            result.Should().NotBeNull();
            result.Should().BeOfType <List<UserGameList>> ();
            result.Count().Should().BeGreaterThan(8);
        }
        [Fact]
        public async Task GetCountByUserIdAndStatus_Returns_Count()
        {
            // Arrange
            var context = await GetDbContext();
            var listRepository = new ListRepository(context);
            string appUserId = "userid1";
            int status = 3;

            // Act
            var result = await listRepository.GetCountByUserIdAndStatus(appUserId, status);

            // Assert
            result.Should().BeGreaterThan(0);
        }
        [Fact]
        public async Task GetAverageRatingByGameId_Returns_AverageRating()
        {
            // Arrange
            var context = await GetDbContext();
            var listRepository = new ListRepository(context);
            int gameId = 1;

            // Act
            var result = await listRepository.GetAverageRatingByGameId(gameId);

            // Assert
            result.Should().BeLessThan(11);
            result.Should().BeGreaterOrEqualTo(0);
        }
        [Fact]
        public async Task GetGameCheckRatingStatus_Returns_CorrectData()
        {
            // Arrange
            var context = await GetDbContext();
            var listRepository = new ListRepository(context);
            int gameId = 1;
            string appUserId = "userid1";

            // Act
            var result = listRepository.GetGameCheckRatingStatus(gameId, appUserId);

            // Assert
            result.isDuplicate.Should().BeTrue();
            result.GameRating.Should().Be(1);
            result.GameStatus.Should().Be(3);
        }
        [Fact]
        public async Task UpdateGameRatingAndStatus_Updates_Record()
        {
            // Arrange
            var context = await GetDbContext();
            var listRepository = new ListRepository(context);
            int gameId = 1;
            string appUserId = "userid1";
            int finalRating = 5;
            int finalStatus = 4;

            // Act
            var result = listRepository.UpdateGameRatingAndStatus(gameId, appUserId, finalRating, finalStatus);

            // Assert
            result.Should().BeTrue();
            var updatedRecord = context.UserGameLists.FirstOrDefault(x => x.AppUserId == appUserId && x.GameId == gameId);
            updatedRecord.Should().NotBeNull();
            updatedRecord.GameRating.Should().Be(finalRating);
            updatedRecord.GameStatus.Should().Be(finalStatus);
        }

    }
}
