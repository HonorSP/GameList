using FluentAssertions;
using GameList.Data;
using GameList.Models;
using GameList.Repository;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace GameList.Tests.Repository
{
    public class MessageRepositoryTests
    {
        public MessageRepositoryTests()
        {
        }

        private async Task<ApplicationDbContext> GetDbContext()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
            var databaseContext = new ApplicationDbContext(options);
            databaseContext.Database.EnsureCreated();
            if (await databaseContext.UserMessages.CountAsync() < 0)
            {
                for (int i = 0; i < 10; i++)
                {
                    databaseContext.UserMessages.Add(
                      new UserMessage()
                      {
                          SenderId = "test-sender-id",
                          ReceiverId = "test-receiver-id",
                          Message = "test message"

                      });
                    await databaseContext.SaveChangesAsync();
                }
            }
            return databaseContext;
        }


        [Fact]
        public async void MessageRepository_Add_ReturnsBool()
        {
            //Arrange
            var userMessage = new UserMessage()
            {

                SenderId = "test-sender-id-2",
                ReceiverId = "test-receiver-id-2",
                Message = "test message 2"

            };
            var dbContext = await GetDbContext();
            var messageRepository = new MessageRepository(dbContext);

            //Act
            var result = messageRepository.Add(userMessage);

            //Assert
            result.Should().BeTrue();
        }

        [Fact]
        public async void MessageRepository_Delete_RemovesMessage()
        {
            //Arrange
            var userMessage = new UserMessage()
            {
                SenderId = "test-sender-id-2",
                ReceiverId = "test-receiver-id-2",
                Message = "test message 2"

            };
            var dbContext = await GetDbContext();
            var messageRepository = new MessageRepository(dbContext);
            var added = messageRepository.Add(userMessage);

            //Act
            var result = messageRepository.Delete(userMessage);

            //Assert
            result.Should().BeTrue();
        }

        [Fact]
        public async void MessageRepository_GetAllMessagesBetweenUsers_ReturnsCorrectMessages()
        {
            //Arrange
            var dbContext = await GetDbContext();
            var messageRepository = new MessageRepository(dbContext);
            var user1Id = "User1";
            var user2Id = "User2";
            var expectedMessages = new List<UserMessage>()
            {
                new UserMessage()
                {
                    SenderId = user1Id,
                    ReceiverId = user2Id,
                    Message = "test message 1",
                    SentTime = DateTime.Now
                },
                new UserMessage()
                {
                    SenderId = user2Id,
                    ReceiverId = user1Id,
                    Message = "test message 2",
                    SentTime = DateTime.Now.AddDays(-1)
                }
            };
            dbContext.UserMessages.AddRange(expectedMessages);
            dbContext.SaveChanges();

            //Act
            var result = messageRepository.GetAllMessagesBetweenUsers(user1Id, user2Id);

            //Assert
            result.Should().BeEquivalentTo(expectedMessages);
        }

    }
}
