using CloudinaryDotNet.Actions;
using FakeItEasy;
using FluentAssertions;
using GameList.Controllers;
using GameList.Data;
using GameList.Interfaces;
using GameList.Models;
using GameList.Repository;
using GameList.Services;
using GameList.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace GameList.Tests.Controller
{
    public class UserControllerTests
    {

        private UserController _userController;
        private IFollowRepository _followRepository;
        private IUserRepository _userRepository;
        private UserManager<AppUser> _userManager;
        private IPhotoService _photoService;
        private IListRepository _listRepository;
        public UserControllerTests()
        {

            _userManager = A.Fake<UserManager<AppUser>>();
            _followRepository = A.Fake<IFollowRepository>();
            _userRepository = A.Fake<IUserRepository>();
            _photoService = A.Fake<IPhotoService>();
            _listRepository = A.Fake<IListRepository>();

            var contextOptions = new DbContextOptionsBuilder<ApplicationDbContext>()
                        .UseInMemoryDatabase(databaseName: "TestDB")
                        .Options;

            var applicationDbContext = new ApplicationDbContext(contextOptions);

            _userController = new UserController(_followRepository, _userRepository, _userManager, _photoService, _listRepository, applicationDbContext);
        }

        //[Fact]
        //public async Task SearchUsers_Should_Return_Correct_Result_For_Given_Search_Query()
        //{
        //    // Arrange
        //     var fakeUsers = new List<AppUser>
        //     {
        //         new AppUser { Id = "1", UserName = "User1", DisplayName = "Display1", ProfilePictureUrl = "/img/avatar1.jpg" },
        //         new AppUser { Id = "2", UserName = "User2", DisplayName = "Display2", ProfilePictureUrl = "/img/avatar2.jpg" },
        //         new AppUser { Id = "3", UserName = "User3", DisplayName = "Display3", ProfilePictureUrl = "/img/avatar3.jpg" },
        //     };

        //    A.CallTo(() => _userManager.Users).Returns(fakeUsers.AsQueryable());

        //    var searchQuery = "User1";
        //    var page = 1;
        //    var pageSize = 18;

        //    // Act
        //    var result = await _userController.SearchUsers(searchQuery, page, pageSize) as ViewResult;
        //    var model = result.Model as SearchUsersViewModel;

        //    // Assert
        //    model.Should().NotBeNull();
        //    model.SearchQuery.Should().Be(searchQuery);
        //    model.Users.Should().HaveCount(1);
        //    model.Users[0].Id.Should().Be("1");
        //    model.Users[0].UserName.Should().Be("User1");
        //    model.Users[0].DisplayName.Should().Be("Display1");
        //    model.Users[0].ProfilePictureUrl.Should().Be("/img/avatar1.jpg");
        //    model.Page.Should().Be(page);
        //    model.PageSize.Should().Be(pageSize);
        //    model.TotalPages.Should().Be(1);
        //}

        [Fact]
        public async Task EditProfile_GetAction_Should_Return_ViewResult_With_EditProfileViewModel()
        {
            // Arrange
            var user = new AppUser
            {
                UserBio = "This is a sample bio",
                DisplayName = "Test User",
                ProfilePictureUrl = "https://test.com/test.jpg"
            };
            A.CallTo(() => _userManager.GetUserAsync(A<ClaimsPrincipal>._)).Returns(user);

            // Act
            var result = await _userController.EditProfile();

            // Assert
            result.Should().BeOfType<ViewResult>();
            var viewResult = result as ViewResult;
            viewResult.Model.Should().BeOfType<EditProfileViewModel>();
            var editProfileViewModel = viewResult.Model as EditProfileViewModel;
            editProfileViewModel.Bio.Should().Be(user.UserBio);
            editProfileViewModel.DisplayName.Should().Be(user.DisplayName);
            editProfileViewModel.ProfilePictureUrl.Should().Be(user.ProfilePictureUrl);
        }

        [Fact]
        public async Task EditProfile_WithValidData_ShouldUpdatePicUserProfile()
        {
            // Arrange
            var currentUser = new AppUser { UserName = "testUser", ProfilePictureUrl = "testPictureUrl", DisplayName = "testDisplayName", UserBio = "testBio" };
            A.CallTo(() => _userManager.GetUserAsync(A<ClaimsPrincipal>._)).Returns(currentUser);
            A.CallTo(() => _userManager.UpdateAsync(A<AppUser>._)).Returns(IdentityResult.Success);

            var image = A.Dummy<IFormFile>();
            A.CallTo(() => _photoService.AddPhotoAsync(image)).Returns(new ImageUploadResult { Error = null, Url = new Uri("http://testurl") });
            A.CallTo(() => _photoService.DeletePhotoAsync(A<string>._)).Returns(new DeletionResult());

            var editVM = new EditProfileViewModel
            {
                DisplayName = "newDisplayName",
                Bio = "newBio",
                Image = image,
                ProfilePictureUrl = "testPictureUrl",
                UserName = "testUser"
            };

            // Act
            var result = await _userController.EditProfile(editVM) as ViewResult;

            // Assert
            A.CallTo(() => _userManager.GetUserAsync(A<ClaimsPrincipal>._)).MustHaveHappenedOnceExactly();
            A.CallTo(() => _userManager.UpdateAsync(A<AppUser>._)).MustHaveHappenedOnceExactly();
            A.CallTo(() => _photoService.AddPhotoAsync(image)).MustHaveHappenedOnceExactly();
            A.CallTo(() => _photoService.DeletePhotoAsync(A<string>._)).MustHaveHappenedOnceExactly();

            result.Model.Should().BeOfType<EditProfileViewModel>();

            result.Should().BeOfType<ViewResult>();
            var model = result.Model as EditProfileViewModel;
            model.DisplayName.Should().Be("testDisplayName");
            model.Bio.Should().Be("newBio");
            model.ProfilePictureUrl.Should().Be("http://testurl/");
        }

        [Fact]
        public async Task UserProfile_ReturnsCorrectViewModel()
        {
            // Arrange
            var user = new AppUser { UserName = "testuser", Id = "testid" };
            A.CallTo(() => _userManager.FindByNameAsync(user.UserName))
                .Returns(Task.FromResult(user));

            var currentUser = new AppUser { UserName = "currentuser", Id = "currentid" };
            A.CallTo(() => _userManager.GetUserAsync(A<ClaimsPrincipal>._))
                .Returns(Task.FromResult(currentUser));

            A.CallTo(() => _followRepository.IsFollowing(currentUser.Id, user.Id))
                .Returns(true);

            A.CallTo(() => _followRepository.AreBothFollowing(currentUser.Id, user.Id))
                .Returns(true);

            A.CallTo(() => _followRepository.CountFollowers(user.Id))
                .Returns(10);

            A.CallTo(() => _followRepository.CountFollowing(user.Id))
                .Returns(5);

            A.CallTo(() => _listRepository.GetCountByUserId(user.Id))
                .Returns(Task.FromResult(20));

            A.CallTo(() => _listRepository.GetCountByUserIdAndStatus(user.Id, A<int>._))
                .Returns(Task.FromResult(2));

            A.CallTo(() => _listRepository.GetCountByUserIdAndRating(user.Id, A<int>._))
                .Returns(Task.FromResult(2));

            _userController.ControllerContext = new ControllerContext
            {
                RouteData = new RouteData { Values = { { "UserName", "testuser" } } }
            };

            // Act
            var result = await _userController.UserProfile();

            // Assert
            result.Should().BeOfType<ViewResult>();
            var viewResult = result as ViewResult;
            viewResult.ViewData.Model.Should().BeOfType<UserProfileViewModel>();

            var model = viewResult.ViewData.Model as UserProfileViewModel;
            model.Followers.Should().Be(10);
            model.Following.Should().Be(5);
            model.CheckIsFollowing.Should().BeTrue();
            model.CheckBothAreFollowing.Should().BeTrue();
            model.CurrentUserName.Should().Be("currentuser");
            model.UserNameCheck.Should().Be("testuser");
            model.Id.Should().Be("testid");
            model.UserName.Should().Be("testuser");
            model.DisplayName.Should().Be("testuser");
            model.Bio.Should().Be("");
            model.ProfilePictureUrl.Should().Be("/img/avatar.jpg");
            model.CountStatusAll.Should().Be(20);
            model.CountStatusPlaying.Should().Be(2);
            model.CountStatusCompleted.Should().Be(2);
            model.CountStatusOnHold.Should().Be(2);
            model.CountStatusDropped.Should().Be(2);
            model.CountRating0.Should().Be(2);
            model.CountRating1.Should().Be(2);
            model.CountRating2.Should().Be(2);
            model.CountRating3.Should().Be(2);
            model.CountRating4.Should().Be(2);
            model.CountRating5.Should().Be(2);
        }






    }
}
