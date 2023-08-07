using FakeItEasy;
using FluentAssertions;
using GameList.Controllers;
using GameList.Interfaces;
using GameList.Models;
using GameList.Repository;
using GameList.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace GameList.Tests.Controller
{
    public class ListControllerTests
    {

        private ListController _listController;
        private IListRepository _listRepository;
        private UserManager<AppUser> _userManager;
        private IUserRepository _userRepository;
        public ListControllerTests()
        {
            _userManager = A.Fake<UserManager<AppUser>>();
            _listRepository = A.Fake<IListRepository>();
            _userRepository = A.Fake<IUserRepository>();


            _listController = new ListController(_listRepository, _userManager,  _userRepository);
        }

        //unable to test removegame as it uses request from http context

        //[Fact]
        //public async Task RemoveGame_ShouldReturnCorrectRedirectAction()
        //{
        //    // Arrange
        //    int gameId = 1;
        //    int? status = 2;
        //    var user = new AppUser { Id = "test_user_id" };
        //    A.CallTo(() => _userManager.GetUserAsync(A<ClaimsPrincipal>.Ignored))
        //        .Returns(Task.FromResult(user));


        //    var formCollection = new FormCollection(new Dictionary<string, StringValues>
        // {
        //     { "returnUrl", "EditUserGameList" }
        // });
        //    var request = new DefaultHttpContext().Request;
        //    request.Form = formCollection;
        //    _listController.ControllerContext = new ControllerContext
        //    {
        //        HttpContext = new DefaultHttpContext { Request = request }
        //    };




        //    // Act
        //    var result = await _listController.RemoveGame(gameId, status);

        //    // Assert
        //    result.Should().BeOfType<RedirectToActionResult>();
        //    A.CallTo(() => _listRepository.RemoveGame(gameId, user.Id))
        //        .MustHaveHappenedOnceExactly();
        //    var redirectResult = result as RedirectToActionResult;
        //    redirectResult.ActionName.Should().Be("EditUserGameList");
        //    redirectResult.ControllerName.Should().Be("List");
        //}

        [Fact]
        public async Task CustomiseList_GetAction_ShouldReturnCorrectViewResult()
        {
            // Arrange
            var user = new AppUser
            {
                Id = "test_user_id",
                UserName = "test_user_name",
                BgColorBody = "#0c0c0c",
                BgColorDisplay = "#1d1d1d",
                BgColorGameTile = "#262626"
            };
            A.CallTo(() => _userManager.GetUserAsync(A<ClaimsPrincipal>.Ignored))
                .Returns(Task.FromResult(user));

            // Act
            var result = await _listController.CustomiseList();

            // Assert
            result.Should().BeOfType<ViewResult>();
            A.CallTo(() => _userManager.GetUserAsync(A<ClaimsPrincipal>.Ignored))
                .MustHaveHappenedOnceExactly();
            var viewResult = result as ViewResult;
            viewResult.ViewName.Should().BeNull();
            viewResult.Model.Should().BeOfType<CustomiseListViewModel>();
            var customList = viewResult.Model as CustomiseListViewModel;
            customList.UserName.Should().Be(user.UserName);
            customList.BgColorBody.Should().Be(user.BgColorBody ?? "#0c0c0c");
            customList.BgColorDisplay.Should().Be(user.BgColorDisplay ?? "#1d1d1d");
            customList.BgColorGameTile.Should().Be(user.BgColorGameTile ?? "#262626");
        }

        [Fact]
        public async Task CustomiseList_PostAction_ShouldReturnCorrectViewResult()
        {
            // Arrange
            var user = new AppUser { Id = "test_user_id", UserName = "TestUser" };
            A.CallTo(() => _userManager.GetUserAsync(A<ClaimsPrincipal>.Ignored))
                .Returns(Task.FromResult(user));
            var customList = new CustomiseListViewModel()
            {
                UserName = "TestUser",
                BgColorBody = "#0c0c0c",
                BgColorDisplay = "#1d1d1d",
                BgColorGameTile = "#262626"
            };

            // Act
            var result = await _listController.CustomiseList(customList);

            // Assert
            result.Should().BeOfType<ViewResult>();
            A.CallTo(() => _userManager.UpdateAsync(user))
                .MustHaveHappenedOnceExactly();
            var viewResult = result as ViewResult;
            viewResult.Model.Should().BeOfType<CustomiseListViewModel>();
            var model = viewResult.Model as CustomiseListViewModel;
            model.UserName.Should().Be(user.UserName);
            user.BgColorBody.Should().Be(customList.BgColorBody);
            user.BgColorDisplay.Should().Be(customList.BgColorDisplay);
            user.BgColorGameTile.Should().Be(customList.BgColorGameTile);
        }
         [Fact]
        public async Task ResetDefault_ShouldReturnCorrectViewResult()
        {
            // Arrange
            var user = new AppUser { Id = "test_user_id", UserName = "TestUser" };
            A.CallTo(() => _userManager.GetUserAsync(A<ClaimsPrincipal>.Ignored))
                .Returns(Task.FromResult(user));

            user.BgColorBody = "#0c0c0c";
            user.BgColorDisplay = "#0c0c0c";
            user.BgColorGameTile = "#0c0c0c";
            // Act
            var result = await _listController.ResetDefault();

            // Assert
            result.Should().BeOfType<RedirectToActionResult>();
            A.CallTo(() => _userManager.UpdateAsync(user))
                .MustHaveHappenedOnceExactly();

            result.As<RedirectToActionResult>().ActionName.Should().Be("CustomiseList");
            result.As<RedirectToActionResult>().ControllerName.Should().Be("List");
            user.BgColorBody.Should().Be(null);
            user.BgColorDisplay.Should().Be(null);
            user.BgColorGameTile.Should().Be(null);
        }




    }
}
