
using GameList.Interfaces;
using GameList.Models;
using GameList.ViewModels;
using IGDB.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace GameList.Controllers
{
    //The FeedbackController contains actions to manage feedbacks from the users of the GameList application
    public class FeedbackController : Controller
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly ISendGridEmail _sendGridEmail;

        public FeedbackController(UserManager<AppUser> userManager, ISendGridEmail sendGridEmail)
        {
            _userManager = userManager;
            _sendGridEmail = sendGridEmail;
        }
        public IActionResult Index()
        {
            return View();
        }


        //SendFeedback, first check if user is authenticated. If yes, returns the view to show feedback form with the user's name and email prefilled.
        [HttpGet]
        public async Task<IActionResult> SendFeedback()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return RedirectToAction("FeedbackDenial");
            }
                var model = new SendFeedbackViewModel
                {
                    UserName = user.UserName,
                    Email = user.Email

                };
                return View(model);
            

        }
        //SendFeedback [HttpPost], it validates the input and sends feedback email to a specified email (my personal email) using SendGridEmail service.
        [HttpPost]
        public async Task<IActionResult> SendFeedback(SendFeedbackViewModel model)
        {
            if (ModelState.IsValid)
            {
                await _sendGridEmail.SendEmailAsync("ahmadfasih@zoho.com", "Feedback for GameList", "UserName = " + model.UserName + " <br> Email = " + model.Email + " <br> Feedback = " + model.Feedback);

                return RedirectToAction("FeedbackConfirmation");
            }
            return View(model);
        }
        //FeedbackConfirmation returns the view to confirm that feedback email was sent successfully.
        [HttpGet]
        public IActionResult FeedbackConfirmation()
        {
            return View();
        }
        //FeedbackDenial returns the view to show error message when user is not authenticated.
        [HttpGet]
        public IActionResult FeedbackDenial()
        {
            return View();
        }
    }
}
