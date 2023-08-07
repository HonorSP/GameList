
using GameList.Interfaces;
using GameList.Models;
using GameList.Repository;
using GameList.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace GameList.Controllers
{
    //The MessageController is a class in GameList that handles operations related to messaging between users.
    //It uses 3 services UserManager<AppUser>, IFollowRepository, IMessageRepository to perform its functions. 
    public class MessageController : Controller
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly IFollowRepository _followRepository;
        private readonly IMessageRepository _messageRepository;

        public MessageController(UserManager<AppUser> userManager, IFollowRepository followRepository, IMessageRepository messageRepository)
        {
            _userManager = userManager;
            _followRepository = followRepository;
            _messageRepository = messageRepository;
        }
        //The method "AllMessages" retrieves all messages between the current user and another user (followUserId).
        //It first checks if the user is logged in, then checks if both users are following each other. If both conditions are met,
        //the messages are retrieved and a view model is created to display the messages along with information about the receiver.
        //If the user is not logged in or if they are not following the receiver, they are redirected to the login page.
        public async Task<IActionResult> AllMessages(string followUserId)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return RedirectToAction("Login", "Account");
            }
            else
            {
                var checkAreBothFollowing = _followRepository.AreBothFollowing(user.Id, followUserId);
                if (checkAreBothFollowing == true)
                {
                    var messages = _messageRepository.GetAllMessagesBetweenUsers(user.Id, followUserId);
                    var receiver = _userManager.FindByIdAsync(followUserId);

                    var viewModel = new AllMessagesViewModel()
                    {
                        Messages = messages,
                        ReceiverDisplayName = receiver.Result.DisplayName ?? receiver.Result.UserName,
                        ReceiverProfilePictureUrl = receiver.Result.ProfilePictureUrl ?? "/img/avatar.jpg",
                        ReceiverId = followUserId,
                        ReceiverName = receiver.Result.UserName

                    };
                    return View(viewModel);
                }
                else
                {
                    return RedirectToAction("Login", "Account");
                }
            }
        }
        //This is a method for sending a message. It first checks if the user is logged in, if not it redirects to the login page. If the user is logged in,
        //it checks if both the sender and receiver are following each other. If both are following, it adds the message to the repository, and then redirects
        //to the "AllMessages" action, passing along the id of the user being messaged. If the check for mutual following fails, it redirects to the "AllMessages" action of the "Message" controller.
        public async Task<IActionResult> SendMessage(string followUserId, string message)
        {

            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return RedirectToAction("Login", "Account");
            }
            else
            {
                var checkAreBothFollowing = _followRepository.AreBothFollowing(user.Id, followUserId);
                if (checkAreBothFollowing == true)
                {
                    if (!string.IsNullOrEmpty(message))
                    {
                        var userMessage = new UserMessage()
                        {
                            SenderId = user.Id,
                            ReceiverId = followUserId,
                            Message = message,
                            SentTime = DateTime.UtcNow
                        };

                        _messageRepository.Add(userMessage);
                    }

                    return RedirectToAction("AllMessages", new { followUserId });
                }
                else
                {
                    return RedirectToAction("AllMessages", "Message");
                }
            }
        }
        //This is a method for retrieving available users for a user. It first checks if the user is logged in, if not it redirects to the login page.
        //If the user is logged in, it uses the _followRepository to get a list of available users and populates a view model with the list of available users.
        //The method then returns the view populated with the view model.
        public async Task<IActionResult> AvailableUsers()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return RedirectToAction("Login", "Account");
            }
            else
            {
                var availableUsers = _followRepository.GetAvailableUsers(user.Id);
                var viewModel = new AvailableUsersViewModel
                {
                    AvailableUsers = availableUsers
                };
                return View(viewModel);



            }
        }


    }
}
