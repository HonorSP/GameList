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
    public class AccountControllerTests
    {
        private AccountController _accountController;
        private UserManager<AppUser> _userManager;
        private SignInManager<AppUser> _signManager;
        private ISendGridEmail _sendGridEmail;
        private IPhotoService _photoService;
        private DbContextOptions<ApplicationDbContext> _context;

        public AccountControllerTests()
        {
            //Dependencies
            _userManager = A.Fake<UserManager<AppUser>>();
            _signManager = A.Fake<SignInManager<AppUser>>();
            _sendGridEmail= A.Fake<ISendGridEmail>();
            _photoService = A.Fake<IPhotoService>();
            var contextOptions = new DbContextOptionsBuilder<ApplicationDbContext>()
                        .UseInMemoryDatabase(databaseName: "TestDB")
                        .Options;

            var applicationDbContext = new ApplicationDbContext(contextOptions);

            _accountController = new AccountController(_userManager, _signManager, _sendGridEmail, _photoService, applicationDbContext);
        }

        //[Fact]
        //public async Task ResetPassword_ValidInput_ReturnsResetPasswordConfirmation()
        //{
        //    // Arrange
        //    var resetPasswordViewModel = new ResetPasswordViewModel
        //    {
        //        Email = "test@email.com",
        //        Code = "code",
        //        Password = "password"
        //    };

        //    A.CallTo(() => _userManager.FindByEmailAsync(A<string>._))
        //        .Returns(new AppUser());

        //    A.CallTo(() => _userManager.ResetPasswordAsync(A<AppUser>._, A<string>._, A<string>._))
        //        .Returns(IdentityResult.Success);

        //    // Act
        //    var result = await _accountController.ResetPassword(resetPasswordViewModel);

        //    // Assert
        //    result.Should().BeOfType<RedirectToActionResult>();
        //    result.As<RedirectToActionResult>().ActionName.Should().Be("ResetPasswordConfirmation");
        //}

        [Fact]
        public async Task ResetPassword_ReturnsViewResult_WithModel_WhenModelStateIsInvalid()
        {
            // Arrange
            _accountController.ModelState.AddModelError("Error", "Error");

            // Act
            var result = await _accountController.ResetPassword(new ResetPasswordViewModel());

            // Assert
            result.Should().BeOfType<ViewResult>();
            var viewResult = result as ViewResult;
            viewResult.Model.Should().BeOfType<ResetPasswordViewModel>();
        }

        [Fact]
        public async Task ResetPassword_ReturnsRedirectToActionResult_WhenUserNotFound()
        {
            // Arrange
            A.CallTo(() => _userManager.FindByEmailAsync(A<string>._))
                .Returns(Task.FromResult((AppUser)null));

            // Act
            var result = await _accountController.ResetPassword(new ResetPasswordViewModel { Email = "test@test.com" });

            // Assert
            result.Should().BeOfType<ViewResult>();
            var viewResult = result as ViewResult;
            viewResult.Model.Should().BeOfType<ResetPasswordViewModel>();
            _accountController.ModelState["Email"].Errors.Count.Should().Be(1);
        }

        [Fact]
        public async Task ResetPassword_ReturnsRedirectToActionResult_WhenResetPasswordSucceeded()
        {
            // Arrange
            A.CallTo(() => _userManager.FindByEmailAsync(A<string>._))
                .Returns(Task.FromResult(new AppUser()));
            A.CallTo(() => _userManager.ResetPasswordAsync(A<AppUser>._, A<string>._, A<string>._))
                .Returns(Task.FromResult(IdentityResult.Success));

            // Act
            var result = await _accountController.ResetPassword(new ResetPasswordViewModel { Email = "test@test.com", Code = "code", Password = "password" });

            // Assert
            result.Should().BeOfType<RedirectToActionResult>();
            var redirectToActionResult = result as RedirectToActionResult;
            redirectToActionResult.ActionName.Should().Be("ResetPasswordConfirmation");
        }
        [Fact]
        public async Task Login_PostAction_WithValidCredentials_ReturnsRedirectToActionResult()
        {
            // Arrange
            A.CallTo(() => _signManager.PasswordSignInAsync(A<string>._, A<string>._, A<bool>._, A<bool>._))
                .Returns(Microsoft.AspNetCore.Identity.SignInResult.Success);

            // Act
            var result = await _accountController.Login(new LoginViewModel { UserName = "test", Password = "password", RememberMe = true});

            // Assert
            result.Should().BeOfType<RedirectToActionResult>();
            result.As<RedirectToActionResult>().ActionName.Should().Be("Index");
            result.As<RedirectToActionResult>().ControllerName.Should().Be("Home");
        }
        [Fact]
        public async Task DeleteUserAccount_PostAction_WithValidPassword_ReturnsRedirectToActionResult()
        {
            // Arrange
            var user = new AppUser { Id = "user1", UserName = "user1", ProfilePictureUrl = "someUrl" };
            var claimsPrincipal = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id),
                new Claim(ClaimTypes.Name, user.UserName),
            }, "mock"));
            A.CallTo(() => _userManager.GetUserAsync(A<ClaimsPrincipal>._)).Returns(user);
            A.CallTo(() => _userManager.CheckPasswordAsync(user, A<string>._)).Returns(true);
            A.CallTo(() => _userManager.DeleteAsync(user)).Returns(IdentityResult.Success);
            _accountController.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = claimsPrincipal }
            };

            // Act
            var result = await _accountController.DeleteUserAccount("password");

            // Assert
            result.Should().BeOfType<RedirectToActionResult>();
            result.As<RedirectToActionResult>().ActionName.Should().Be("ConfirmDeleteAccount");
            result.As<RedirectToActionResult>().ControllerName.Should().Be("Account");
            A.CallTo(() => _photoService.DeletePhotoAsync("someUrl")).MustHaveHappened();
            A.CallTo(() => _signManager.SignOutAsync()).MustHaveHappened();
        }

    }
}
