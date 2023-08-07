using FakeItEasy;
using FluentAssertions;
using GameList.Controllers;
using GameList.Interfaces;
using GameList.Models;
using GameList.Repository;
using GameList.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestPlatform.CommunicationUtilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace GameList.Tests.Controller
{
    public class MessageControllerTests
    {

        private MessageController _messageController;
        private UserManager<AppUser> _userManager;
        private IFollowRepository _followRepository;
        private IMessageRepository _messageRepository;

        public MessageControllerTests()
        {

            _userManager = A.Fake<UserManager<AppUser>>();
            _followRepository = A.Fake<IFollowRepository>();
            _messageRepository = A.Fake<IMessageRepository>();


            _messageController = new MessageController(_userManager, _followRepository, _messageRepository);
        }

        [Fact]
        public async Task AllMessages_ReturnsExpectedResult()
        {
            // Arrange
            var user = new AppUser { Id = "1", UserName = "user1" };
            A.CallTo(() => _userManager.GetUserAsync(A<ClaimsPrincipal>._))
             .Returns(Task.FromResult(user));

            A.CallTo(() => _followRepository.AreBothFollowing(user.Id, "2"))
             .Returns(true);

            var messages = new List<UserMessage> { new UserMessage { Id = 1, SenderId = "1", ReceiverId = "2", Message = "Test Message" } };
            A.CallTo(() => _messageRepository.GetAllMessagesBetweenUsers(user.Id, "2"))
             .Returns(messages);

            var receiver = new AppUser { Id = "2", UserName = "user2", DisplayName = "Display Name", ProfilePictureUrl = "ProfilePictureUrl" };
            A.CallTo(() => _userManager.FindByIdAsync("2"))
             .Returns(Task.FromResult(receiver));

            // Act
            var result = await _messageController.AllMessages("2") as ViewResult;

            // Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<ViewResult>();
            var model = result.Model as AllMessagesViewModel;
            model.Should().NotBeNull();
            model.ReceiverDisplayName.Should().Be(receiver.DisplayName);
            model.ReceiverProfilePictureUrl.Should().Be(receiver.ProfilePictureUrl);
            model.ReceiverId.Should().Be("2");
            model.ReceiverName.Should().Be(receiver.UserName);
            model.Messages.Should().BeEquivalentTo(messages);
        }

        [Fact]
        public async Task SendMessage_Returns_RedirectToAction_When_User_Is_Not_Null()
        {
            // Arrange
            var followUserId = "someUserId";
            var message = "hello";
            var user = new AppUser { Id = "someUserId" };
            A.CallTo(() => _userManager.GetUserAsync(A<ClaimsPrincipal>._)).Returns(user);
            A.CallTo(() => _followRepository.AreBothFollowing(user.Id, followUserId)).Returns(true);

            // Act
            var result = await _messageController.SendMessage(followUserId, message);

            // Assert
            result.Should().BeOfType<RedirectToActionResult>();
            var redirectToActionResult = result as RedirectToActionResult;
            redirectToActionResult.ActionName.Should().Be("AllMessages");
            redirectToActionResult.RouteValues.Keys.Should().Contain("followUserId");
            redirectToActionResult.RouteValues["followUserId"].Should().Be(followUserId);
        }

        [Fact]
        public async Task SendMessage_Returns_RedirectToAction_When_User_Is_Null()
        {
            // Arrange
            A.CallTo(() => _userManager.GetUserAsync(A<ClaimsPrincipal>._)).Returns((AppUser)null);

            // Act
            var result = await _messageController.SendMessage("someUserId", "hello");

            // Assert
            result.Should().BeOfType<RedirectToActionResult>();
            var redirectToActionResult = result as RedirectToActionResult;
            redirectToActionResult.ActionName.Should().Be("Login");
            redirectToActionResult.ControllerName.Should().Be("Account");
        }

        [Fact]
        public async Task AvailableUsers_Returns_RedirectToActionResult_When_User_Is_Not_Logged_In()
        {
            // Arrange
            A.CallTo(() => _userManager.GetUserAsync(A<ClaimsPrincipal>._)).Returns(Task.FromResult((AppUser)null));

            // Act
            var result = await _messageController.AvailableUsers();

            // Assert
            result.Should().BeOfType<RedirectToActionResult>();
            var redirectResult = result as RedirectToActionResult;
            redirectResult.ActionName.Should().Be("Login");
            redirectResult.ControllerName.Should().Be("Account");
        }


    }



}
