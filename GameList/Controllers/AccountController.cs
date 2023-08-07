using GameList.Interfaces;
using GameList.Models;
using GameList.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SendGrid.Helpers.Mail;
using SendGrid;
using GameList.Services;
using Microsoft.EntityFrameworkCore;
using GameList.Data;
using CloudinaryDotNet;
using Microsoft.Data.SqlClient.Server;
using Newtonsoft.Json.Linq;
using static SendGrid.BaseClient;
using static System.Net.Mime.MediaTypeNames;
using System.Reflection.Metadata;
namespace GameList.Controllers
{
    //This is an implementation of a controller in an ASP.NET Core MVC application for account management. It has several action methods that
    //handle different account management functions such as forgot password, reset password, login and registration.
    //The controller takes in several services as dependencies via constructor injection:
    //UserManager<AppUser> and SignInManager<AppUser> are used for managing user accounts, such as generating password reset tokens, resetting passwords and signing in users.
    //ISendGridEmail is used to send emails.
    //IPhotoService is used for photo services (not mentioned in the code snippet).
    //ApplicationDbContext is the database context used to interact with the database.
    public class AccountController : Controller
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signManager;
        private readonly ISendGridEmail _sendGridEmail;
        private readonly IPhotoService _photoService;
        private readonly ApplicationDbContext _context;
        public AccountController(UserManager<AppUser> userManager,
        SignInManager<AppUser> signManager,
        ISendGridEmail sendGridEmail,
        IPhotoService photoService,
            ApplicationDbContext context)
        {
            _userManager = userManager;
            _signManager = signManager;
            _sendGridEmail = sendGridEmail;
            _photoService = photoService;
            _context = context;
        }
        
        [HttpGet]
        public IActionResult ForgotPassword()
        {
            return View();
        }
        //The code sends a password reset link to a user's email after validating their input in the "ForgotPassword" view and generating a password reset token for the user.
        [HttpPost]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByEmailAsync(model.Email);
                if (user == null)
                {
                    return RedirectToAction("ForgotPasswordConfirmation");
                }
                var code = await _userManager.GeneratePasswordResetTokenAsync(user);
                var callbackurl = Url.Action("ResetPassword", "Account", new { userId = user.Id, code = code }, protocol: HttpContext.Request.Scheme);

                await _sendGridEmail.SendEmailAsync(model.Email, "GameList Reset Password", "Please reset GameList password by going to this " +
                    "<a href=\"" + callbackurl + "\">link</a>");
                return RedirectToAction("ForgotPasswordConfirmation");
            }
            return View(model);
        }
        //Return if password was changed
        [HttpGet]
        public IActionResult ForgotPasswordConfirmation()
        {
            return View();
        }
        [HttpGet]
        public IActionResult ResetPassword(string code=null)
        {
            return code == null ? View("Error") : View();
        }
        //This code is a reset password action method that updates a user's password after validating the reset password information and reset code.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ResetPassword(ResetPasswordViewModel resetPasswordViewModel)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByEmailAsync(resetPasswordViewModel.Email);
                if(user == null)
                {
                    ModelState.AddModelError("Email", "User not found");
                    return View(resetPasswordViewModel);
                }
                var result = await _userManager.ResetPasswordAsync(user, resetPasswordViewModel.Code, resetPasswordViewModel.Password);
                if (result.Succeeded)
                {
                    return RedirectToAction("ResetPasswordConfirmation");
                }
            }
            return View(resetPasswordViewModel);
        }

        [HttpGet]
        public IActionResult ResetPasswordConfirmation()
        {
            return View();
        }


        //Login Action method to display Login View and accept return URL
        [HttpGet]
        public IActionResult Login(string? returnUrl = null)
        {
            LoginViewModel loginViewModel = new LoginViewModel();
            loginViewModel.ReturnUrl = returnUrl ?? Url.Content("~/");

            return View(loginViewModel);
        }
        //Login POST Action method to verify user credentials and return appropriate response.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel loginViewModel, string? returnUrl = null)
        {
            if (ModelState.IsValid)
            {
                var result = await _signManager.PasswordSignInAsync(loginViewModel.UserName, loginViewModel.Password, loginViewModel.RememberMe, lockoutOnFailure: true);
                if (result.Succeeded)
                {
                    return RedirectToAction("Index", "Home");
                }
                if (result.IsLockedOut)
                {
                    return View("Lockout");
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Invalid user name or password");
                    return View(loginViewModel);
                }
            }
            return View(loginViewModel);
        }




        //Get request for Register: Shows the Register View.
        [HttpGet]
        public IActionResult Register(string? returnUrl = null)
        {
            RegisterViewModel registerViewModel = new RegisterViewModel();
            registerViewModel.ReturnUrl = returnUrl;
            return View(registerViewModel);
        }
        //Post request for Register: Attempts to register a user. If successful, the user is signed in and redirected to the Welcome page in the Account controller.
        //If there are any errors, they are added to the ModelState and the Register view is shown again.
        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel registerViewModel, string? returnUrl = null)
        {
            registerViewModel.ReturnUrl = returnUrl;
            returnUrl = returnUrl ?? Url.Content("~/");
            if (ModelState.IsValid)
            {
                var user = new AppUser { Email = registerViewModel.EmailAddress, UserName = registerViewModel.UserName, UserCreateTime = DateTime.UtcNow };
                var result = await _userManager.CreateAsync(user, registerViewModel.Password);
                if (result.Succeeded)
                {
                    await _signManager.SignInAsync(user, isPersistent: false);
                    return RedirectToAction("Welcome", "Account");
                }
                else
                {
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }
                }
            }
            return View(registerViewModel);

        }
        //LogOff logs out the user and redirects to home page.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> LogOff()
        {
            await _signManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }

        //Welcome displays the welcome view with data the user might have put.
        [HttpGet]
        public  IActionResult Welcome()
        {
            WelcomeViewModel welcomeViewModel = new WelcomeViewModel();
            return View(welcomeViewModel);
        }

        //Welcome updates profile with image or returns view with validation errors.
        [HttpPost]
        public async Task<IActionResult> Welcome(WelcomeViewModel editVM)
        {
            if (!ModelState.IsValid)
            {
                ModelState.AddModelError("", "Failed to edit profile");
                return View("Welcome", editVM);
            }

            var user = await _userManager.GetUserAsync(User);

            if (user == null)
            {
                return View("Error");
            }

            if (editVM.Image != null) // only update profile image
            {
                var photoResult = await _photoService.AddPhotoAsync(editVM.Image);

                if (photoResult.Error != null)
                {
                    ModelState.AddModelError("Image", "Failed to upload image");
                    return View("Welcome", editVM);
                }

                if (!string.IsNullOrEmpty(user.ProfilePictureUrl))
                {
                    _ = _photoService.DeletePhotoAsync(user.ProfilePictureUrl);
                }

                user.ProfilePictureUrl = photoResult.Url.ToString();
                editVM.ProfilePictureUrl = user.ProfilePictureUrl;

                await _userManager.UpdateAsync(user);

                return View(editVM);
            }

            return View(editVM);
        }

        [HttpGet]
        public IActionResult Welcome2()
        {
            Welcome2ViewModel welcome2ViewModel = new Welcome2ViewModel();
            return View(welcome2ViewModel);
        }
        // POST action updates display name and redirects to Index action of Home controller if update is successful
        [HttpPost]
        public async Task<IActionResult> Welcome2(Welcome2ViewModel editVM)
        {
            if (!ModelState.IsValid)
            {
                ModelState.AddModelError("", "Failed to edit profile");
                return View("Welcome2", editVM);
            }

            var user = await _userManager.GetUserAsync(User);

            if (user == null)
            {
                return View("Error");
            }

            user.DisplayName = editVM.DisplayName;

            await _userManager.UpdateAsync(user);

            return RedirectToAction("Index", "Home");
        }


        //DeleteUserAccount(): shows the view for deleting user account if user is authenticated
        [HttpGet]
        public IActionResult DeleteUserAccount()
        {
            if (!User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Home");
            }
            return View();
        }
        //Deletes the user account if provided password is correct, deletes related data (game lists, follows, messages),
        //deletes profile picture, signs out the user, and redirects to confirmation or error page.
        [HttpPost]
        public async Task<IActionResult> DeleteUserAccount(string Password)
        {

            var user = await _userManager.GetUserAsync(User);
            if (user == null || !User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Home");
            }
            if (!await _userManager.CheckPasswordAsync(user, Password))
            {
                // Return an error message if the password is incorrect
                ModelState.AddModelError("password", "Incorrect password");
                return View();
            }


            var userGameList = _context.UserGameLists.Where(u => u.AppUserId == user.Id);
            var userFollow = _context.UserFollows.Where(u => u.FollowerId == user.Id || u.FollowedId == user.Id);
            var userMessages = _context.UserMessages.Where(u => u.ReceiverId == user.Id || u.SenderId == user.Id);

            _context.UserGameLists.RemoveRange(userGameList);
            _context.UserFollows.RemoveRange(userFollow);
            _context.UserMessages.RemoveRange(userMessages);
            await _context.SaveChangesAsync();

            if (!string.IsNullOrEmpty(user.ProfilePictureUrl))
            {
                _ = _photoService.DeletePhotoAsync(user.ProfilePictureUrl);
            }
            var deleteUser = await _userManager.DeleteAsync(user);
            await _signManager.SignOutAsync();
            if (deleteUser.Succeeded)
            {
                // redirect or return a message to let the user know their account has been deleted
                
                return RedirectToAction("ConfirmDeleteAccount", "Account");
            }
            else
            {
                // return an error message
                return RedirectToAction("ErrorDeleteAccount", "Account");
            }
        }
        //Return if account deleted
        [HttpGet]
        public IActionResult ConfirmDeleteAccount()
        {
            return View();
        }
        //Return if account couldn't be deleted
        [HttpGet]
        public IActionResult ErrorDeleteAccount()
        {
            return View();
        }
    }
}
