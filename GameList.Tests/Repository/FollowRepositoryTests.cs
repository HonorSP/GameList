using FluentAssertions;
using GameList.Data;
using GameList.Models;
using GameList.Repository;
using GameList.ViewModels;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameList.Tests.Repository
{
    public class FollowRepositoryTests
    {
        public FollowRepositoryTests()
        {
        }
        private async Task<ApplicationDbContext> GetDbContext()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
            var databaseContext = new ApplicationDbContext(options);
            databaseContext.Database.EnsureCreated();
            if (await databaseContext.UserFollows.CountAsync() == 0)
            {

                databaseContext.UserFollows.Add(
                     new UserFollow()
                     {
                         FollowerId = "userid1",
                         FollowedId = "userid2"

                     });
                await databaseContext.SaveChangesAsync();
                databaseContext.UserFollows.Add(
                     new UserFollow()
                     {
                         FollowerId = "userid2",
                         FollowedId = "userid1"

                     });

                await databaseContext.SaveChangesAsync();

                databaseContext.UserFollows.Add(
                     new UserFollow()
                     {
                         FollowerId = "userid1",
                         FollowedId = "userid3"

                     });
                await databaseContext.SaveChangesAsync();
                databaseContext.UserFollows.Add(
                     new UserFollow()
                     {
                         FollowerId = "userid3",
                         FollowedId = "userid1"

                     });

                await databaseContext.SaveChangesAsync();
            }
            return databaseContext;
        }

        [Fact]
        public async void FollowRepository_Add_ReturnsBool()
        {
            //Arrange
            var userFollow = new UserFollow()
            {

                FollowerId = "user3id",
                FollowedId = "user4id",
                FollowTime = DateTime.Now

            };
            var dbContext = await GetDbContext();
            var followRepository = new FollowRepository(dbContext);

            //Act
            var result = followRepository.Add(userFollow);

            //Assert
            result.Should().BeTrue();
        }

        [Fact]
        public async void FollowRepository_GetAvailableUsers_ReturnsAvailableUsers()
        {
            //Arrange
            var dbContext = await GetDbContext();
            var followRepository = new FollowRepository(dbContext);
            var userId = "userid1";
            var users = new List<AppUser>
                {
                    new AppUser
                    {
                        Id = "userid2",
                        UserName = "user2",
                        DisplayName = "User2",
                        ProfilePictureUrl = "/img/user2.jpg"
                    },
                    new AppUser
                    {
                        Id = "userid3",
                        UserName = "user3",
                        DisplayName = "User3",
                        ProfilePictureUrl = "/img/user3.jpg"
                    }
                };

            foreach (var user in users)
            {
                dbContext.Users.Add(user);
            }
            await dbContext.SaveChangesAsync();
            //Act
            var result = await followRepository.GetAvailableUsers(userId);


            //Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<List<AvailableUser>>();
            result.Count().Should().BeGreaterThan(0);
        }

        [Fact]
        public async void FollowRepository_IsFollowing_ReturnsTrueForFollowingUsers()
        {
            //Arrange
            var dbContext = await GetDbContext();
            var followRepository = new FollowRepository(dbContext);
            var followerId = "userid1";
            var followedId = "userid2";

            //Act
            var result = followRepository.IsFollowing(followerId, followedId);

            //Assert
            result.Should().BeTrue();
        }

        [Fact]
        public async void FollowRepository_IsFollowing_ReturnsFalseForNotFollowingUsers()
        {
            //Arrange
            var dbContext = await GetDbContext();
            var followRepository = new FollowRepository(dbContext);
            var followerId = "userid1";
            var followedId = "userid4";

            //Act
            var result = followRepository.IsFollowing(followerId, followedId);

            //Assert
            result.Should().BeFalse();
        }

        [Fact]
        public async void FollowRepository_AreBothFollowing_ReturnsTrueForFollowingUsers()
        {
            //Arrange
            var dbContext = await GetDbContext();
            var followRepository = new FollowRepository(dbContext);
            var followerId = "userid1";
            var followedId = "userid2";

            //Act
            var result = followRepository.AreBothFollowing(followerId, followedId);

            //Assert
            result.Should().BeTrue();
        }

        [Fact]
        public async void FollowRepository_AreBothFollowing_ReturnsFalseForNotFollowingUsers()
        {
            //Arrange
            var dbContext = await GetDbContext();
            var followRepository = new FollowRepository(dbContext);
            var followerId = "userid1";
            var followedId = "userid4";

            //Act
            var result = followRepository.AreBothFollowing(followerId, followedId);

            //Assert
            result.Should().BeFalse();
        }


    }
}
