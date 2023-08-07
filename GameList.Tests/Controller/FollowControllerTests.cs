using FakeItEasy;
using FluentAssertions;
using GameList.Controllers;
using GameList.Data;
using GameList.Interfaces;
using GameList.Models;
using GameList.Services;
using GameList.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace GameList.Tests.Controller
{
    public class FollowControllerTests
    {


        private FollowController _followController;
        private UserManager<AppUser> _userManager;
        private IFollowRepository _followRepository;

        public FollowControllerTests()
        {

            _userManager = A.Fake<UserManager<AppUser>>();
            _followRepository = A.Fake<IFollowRepository>();

            _followController = new FollowController(_userManager, _followRepository);

        }


        [Fact]
        public async Task FollowUser_WithValidUserId_UserNotFollowingAlready_ReturnsRedirectToActionResult()
        {
            // Arrange
            var user = new AppUser { Id = "user1", UserName = "user1", ProfilePictureUrl = "someUrl" };
            var claimsPrincipal = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id),
                new Claim(ClaimTypes.Name, user.UserName),
            }, "mock"));
            A.CallTo(() => _userManager.GetUserAsync(A<ClaimsPrincipal>._)).Returns(user);

            A.CallTo(() => _followRepository.IsFollowing(user.Id, A<string>._)).Returns(false);

            _followController.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = claimsPrincipal }
            };

            // Act
            var result = await _followController.FollowUser("user2");

            // Assert
            result.Should().BeOfType<RedirectToActionResult>();
            result.As<RedirectToActionResult>().ActionName.Should().Be("UserProfile");
            result.As<RedirectToActionResult>().ControllerName.Should().Be("User");
            A.CallTo(() => _followRepository.AddFollower(user.Id, A<string>._)).MustHaveHappened();
        }


    //This method cant be tested as it countains a static method count.

    //    [Fact]
    //    public async Task Followers_WithValidFollowUserId_ReturnsViewResult()
    //    {
    //        // Arrange
    //        var followUserId = "user1";
    //        var followersIds = new List<string>() { "follower1", "follower2", "follower3" };
    //        var users = new List<AppUser>()
    //{
    //    new AppUser { Id = "follower1", UserName = "follower1", DisplayName = "follower1", ProfilePictureUrl = "someUrl1" },
    //    new AppUser { Id = "follower2", UserName = "follower2", DisplayName = "follower2", ProfilePictureUrl = "someUrl2" },
    //    new AppUser { Id = "follower3", UserName = "follower3", DisplayName = "follower3", ProfilePictureUrl = "someUrl3" },
    //};

    //        A.CallTo(() => _followRepository.GetFollowerIds(followUserId)).Returns(followersIds);
    //        A.CallTo(() => _userManager.Users.Where(u => followersIds.Contains(u.Id)).Count()).Returns(followersIds.Count);
    //        A.CallTo(() => _userManager.Users.Where(u => followersIds.Contains(u.Id)).Skip(A<int>._).Take(A<int>._).ToListAsync(A<CancellationToken>._)).Returns(users);

    //        // Act
    //        var result = await _followController.Followers(followUserId);

    //        // Assert
    //        result.Should().BeOfType<ViewResult>();
    //        result.As<ViewResult>().Model.Should().BeOfType<FollowingFollowersViewModel>();
    //        var model = result.As<ViewResult>().Model.As<FollowingFollowersViewModel>();
    //        model.FollowUserId.Should().Be(followUserId);
    //        model.Users.Should().HaveCount(followersIds.Count);
    //        for (int i = 0; i < followersIds.Count; i++)
    //        {
    //            model.Users[i].Id.Should().Be(users[i].Id);
    //            model.Users[i].UserName.Should().Be(users[i].UserName);
    //            model.Users[i].DisplayName.Should().Be(users[i].DisplayName);
    //            model.Users[i].ProfilePictureUrl.Should().Be(users[i].ProfilePictureUrl);
    //        }
    //    }



    }
}
