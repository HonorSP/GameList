
using CloudinaryDotNet.Actions;
using GameList.Interfaces;
using GameList.Models;
using GameList.Services;
using GameList.ViewModels;
using IGDB;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.Text;
using System.Text.Encodings.Web;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;
using System.Threading.Channels;
using GameList.Repository;
using GameList.Data;

namespace GameList.Controllers
{
    //This is a UserController class that provides actions for handling user information.
    //The class has dependencies on IFollowRepository, IUserRepository, UserManager, IPhotoService, and IListRepository.
    public class UserController : Controller
    {

        private readonly IFollowRepository _followRepository;
        private readonly IUserRepository _userRepository;
        private readonly UserManager<AppUser> _userManager;
        private readonly IPhotoService _photoService;
        private readonly IListRepository _listRepository;
        private readonly ApplicationDbContext _context;

        public UserController(IFollowRepository followRepository, IUserRepository userRepository, UserManager<AppUser> userManager, IPhotoService photoService, IListRepository listRepository, ApplicationDbContext context)
        { 
            _followRepository = followRepository;
            _userRepository = userRepository;
            _userManager = userManager;
            _photoService = photoService;
            _listRepository = listRepository;
            _context = context;
        }
        //The "SearchUsers" method searches for users and returns the results in a list of UserViewModel objects. If a search query is provided, the method filters the list
        //of users by the search query and returns the filtered list. If no search query is provided, the method returns a list of all users. The results are paginated with
        //a page size specified by the "pageSize" parameter, and the current page is specified by the "page" parameter. The method populates a SearchUsersViewModel with the
        //list of UserViewModel objects, the search query (if provided), the current page, the page size, and the total number of pages. Finally, the method returns
        //the SearchUsersViewModel as a view.
        public async Task<IActionResult> SearchUsers(string? searchquery, int page = 1, int pageSize = 18)
        {
            var searchUsersViewModel = new SearchUsersViewModel();
            searchUsersViewModel.Users = new List<UserViewModel>();
            int offset = (page - 1) * pageSize;
            if (searchquery == null)
            {

                var userCount = _userManager.Users.Count();
                var users = await _userManager.Users.Skip(offset).Take(pageSize).ToListAsync();

                // populate the userViewModels list with data from the users
                foreach (var user in users)
                {
                    var viewModel = new UserViewModel()
                    {
                        Id = user.Id,
                        UserName = user.UserName,
                        DisplayName = user.DisplayName ?? user.UserName,
                        ProfilePictureUrl = user.ProfilePictureUrl ?? "/img/avatar.jpg"
                    };
                    searchUsersViewModel.Users.Add(viewModel);

                }
                var totalUsers = userCount;
                searchUsersViewModel.TotalPages = (int)Math.Ceiling((double)totalUsers / pageSize);
            }
            else
            {

                var userCount = _userManager.Users.Where(u => u.UserName.Contains(searchquery)).Count();
                var users = await _userManager.Users.Where(u => u.UserName.Contains(searchquery)).Skip(offset).Take(pageSize).ToListAsync();

                searchUsersViewModel.SearchQuery = searchquery;
                // populate the userViewModels list with data from the users
                foreach (var user in users)
                {
                    var viewModel = new UserViewModel()
                    {
                        Id = user.Id,
                        UserName = user.UserName,
                        DisplayName = user.DisplayName ?? user.UserName,
                        ProfilePictureUrl = user.ProfilePictureUrl ?? "/img/avatar.jpg"
                    };

                    searchUsersViewModel.Users.Add(viewModel);
                }
                var totalUsers = userCount;
                searchUsersViewModel.TotalPages = (int)Math.Ceiling((double)totalUsers / pageSize);
            }

            searchUsersViewModel.Page = page; 
            searchUsersViewModel.PageSize = pageSize;
            return View(searchUsersViewModel);
        }



        //EditProfile retrieves the currently logged in user from the user manager and maps the user's information to a EditProfileViewModel object.
        //If the user is null, the method returns an error view.
        [HttpGet]
        public async Task<IActionResult> EditProfile()
        {
            var user = await _userManager.GetUserAsync(User);

            if (user == null)
            {
                return View("Error");
            }

            var editMV = new EditProfileViewModel()
            {
                Bio = user.UserBio,
                DisplayName = user.DisplayName,
                ProfilePictureUrl = user.ProfilePictureUrl,
            };
            return View(editMV);
        }
        //EditProfile, takes the EditProfileViewModel as an argument and performs the following operations:
        //Validates the model, if it is not valid the method adds an error to the model state and returns the EditProfile view with the editVM object.
        //If the model is valid, the method updates the user's information based on the provided data in the editVM object.
        //If the image property of editVM is not null, the method updates the profile picture. If the display name is not null, the method updates the display name.
        //If the bio is not null, the method updates the bio.
        //Finally, the method saves the changes to the user and returns the EditProfile view with the updated editVM object.
        [HttpPost]
        public async Task<IActionResult> EditProfile(EditProfileViewModel editVM)
        {
            var user = await _userManager.GetUserAsync(User);
            editVM.UserName = user.UserName;
            if (!ModelState.IsValid)
            {

                editVM.Bio = user.UserBio;
                editVM.DisplayName = user.DisplayName;
                editVM.ProfilePictureUrl = user.ProfilePictureUrl;
                ModelState.AddModelError("", "Failed to edit profile");
                return View("EditProfile", editVM);
            }


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
                    return View("EditProfile", editVM);
                }

                if (!string.IsNullOrEmpty(user.ProfilePictureUrl))
                {
                    _ = _photoService.DeletePhotoAsync(user.ProfilePictureUrl);
                }

                user.ProfilePictureUrl = photoResult.Url.ToString();
                editVM.ProfilePictureUrl = user.ProfilePictureUrl;
                editVM.DisplayName = user.DisplayName;

                await _userManager.UpdateAsync(user);

                return View(editVM);
            }
            if(editVM.DisplayName != null)
            {
                user.DisplayName = HtmlEncoder.Default.Encode(editVM.DisplayName);
            }
            else
            {
                editVM.DisplayName = user.DisplayName;
            }
            if (editVM.Bio != null)
            {
                user.UserBio = editVM.Bio;
            }
            else
            {

                editVM.Bio = user.UserBio;
            }
            editVM.ProfilePictureUrl = user.ProfilePictureUrl;
            await _userManager.UpdateAsync(user);

            //return RedirectToAction("Detail", "User", new { user.Id });
            return View("EditProfile", editVM);
        }

        //UserProfile handles a request to display the user profile page. The method takes the username from the URL and retrieves the user data from the UserManager.
        //It also retrieves the data on the user's follow status, followers count, and following count, as well as data on the user's anime lists (counts of anime with
        //different statuses and ratings). Finally, it creates an instance of the UserProfileViewModel with all the gathered data and returns it to the view for display.
        [Route("UserProfile/{UserName}")]
        public async Task<IActionResult> UserProfile()
        {

            string userName = RouteData.Values["UserName"]?.ToString();
            var user = await _userManager.FindByNameAsync(userName);

            if (user == null)
            {
                return View("Index", "Home");
            }



            var currentUser = await _userManager.GetUserAsync(User);
            string currentUserName = "";
            bool checkIsFollowing = false;
            bool checkBothAreFollowing = false;
            if (currentUser != null)
            {
                currentUserName = currentUser.UserName;
                checkIsFollowing = _followRepository.IsFollowing(currentUser.Id, user.Id);
                checkBothAreFollowing = _followRepository.AreBothFollowing(currentUser.Id, user.Id);
            }

            int followers = _followRepository.CountFollowers(user.Id);
            int following = _followRepository.CountFollowing(user.Id);


            int countStatusAll = await _listRepository.GetCountByUserId(user.Id);
            int countStatusPlaying = await _listRepository.GetCountByUserIdAndStatus(user.Id, 1);
            int countStatusCompleted = await _listRepository.GetCountByUserIdAndStatus(user.Id, 2);
            int countStatusOnHold = await _listRepository.GetCountByUserIdAndStatus(user.Id, 3);
            int countStatusDropped = await _listRepository.GetCountByUserIdAndStatus(user.Id, 4);
            int countStatusPlanToPlay = await _listRepository.GetCountByUserIdAndStatus(user.Id, 5);
            int countRating0 = await _listRepository.GetCountByUserIdAndRating(user.Id, 0);
            int countRating1 = await _listRepository.GetCountByUserIdAndRating(user.Id, 1);
            int countRating2 = await _listRepository.GetCountByUserIdAndRating(user.Id, 2);
            int countRating3 = await _listRepository.GetCountByUserIdAndRating(user.Id, 3);
            int countRating4 = await _listRepository.GetCountByUserIdAndRating(user.Id, 4);
            int countRating5 = await _listRepository.GetCountByUserIdAndRating(user.Id, 5);
            int countRating6 = await _listRepository.GetCountByUserIdAndRating(user.Id, 6);
            int countRating7 = await _listRepository.GetCountByUserIdAndRating(user.Id, 7);
            int countRating8 = await _listRepository.GetCountByUserIdAndRating(user.Id, 8);
            int countRating9 = await _listRepository.GetCountByUserIdAndRating(user.Id, 9);
            int countRating10 = await _listRepository.GetCountByUserIdAndRating(user.Id, 10);


            var userProfileViewModel = new UserProfileViewModel()
            {
                Followers = followers,
                Following = following,
                CheckIsFollowing = checkIsFollowing,
                CheckBothAreFollowing = checkBothAreFollowing,
                CurrentUserName = currentUserName,
                UserNameCheck = userName,
                Id = user.Id,
                UserName = user.UserName,
                DisplayName = user.DisplayName ?? user.UserName,
                Bio = user.UserBio ?? "",
                ProfilePictureUrl = user.ProfilePictureUrl ?? "/img/avatar.jpg",
                CountStatusAll        = countStatusAll ,
                CountStatusPlaying    = countStatusPlaying  ,
                CountStatusCompleted  = countStatusCompleted   ,
                CountStatusOnHold     = countStatusOnHold  ,
                CountStatusDropped    = countStatusDropped   ,
                CountStatusPlanToPlay = countStatusPlanToPlay,
                CountRating0   = countRating0 ,
                CountRating1   = countRating1 ,
                CountRating2   = countRating2 ,
                CountRating3   = countRating3 ,
                CountRating4   = countRating4 ,
                CountRating5   = countRating5 ,
                CountRating6   = countRating6 ,
                CountRating7   = countRating7 ,
                CountRating8   = countRating8 ,
                CountRating9   = countRating9 ,
                CountRating10 =  countRating10,
                Feed = new List<FeedItem>()
            };

            var feedFollowers = _context.UserFollows.Where(f => f.FollowedId == user.Id).ToList();
            var feedMessages = _context.UserMessages.Where(m => m.ReceiverId == user.Id).ToList();
            foreach (var follow in feedFollowers)
            {
                var FollowerUsername = _userManager.FindByIdAsync(follow.FollowerId).Result;
                if (follow.FollowTime.HasValue)
                {
                    userProfileViewModel.Feed.Add(new FeedItem
                    {
                        UserId = FollowerUsername.Id,
                        UserName = FollowerUsername.UserName,
                        Action = "followed you",
                        Timestamp = follow.FollowTime.Value
                    });
                }
            }
            foreach (var message in feedMessages)
            {

                var MessageUsername = _userManager.FindByIdAsync(message.SenderId).Result;
                userProfileViewModel.Feed.Add(new FeedItem
                {
                    UserId = MessageUsername.Id,
                    UserName = MessageUsername.UserName,
                    Action = "messaged you",
                    Timestamp = message.SentTime
                });
            }
            userProfileViewModel.Feed = userProfileViewModel.Feed.OrderByDescending(f => f.Timestamp).Take(10).ToList();

            return View(userProfileViewModel);
        }



        //[Route("UserProfile/{UserName}")]
        //public async Task<IActionResult> VisitorProfile()
        //{

        //    string userName = RouteData.Values["UserName"].ToString();

        //    if (userName == null)
        //    {
        //        return RedirectToAction("Login", "Account");
        //    }
        //    else
        //    {

        //        var user = await _userManager.FindByNameAsync(userName);

        //        if (user == null)
        //        {
        //            return View("Index", "Home");
        //        }

        //        int countStatusAll = await _listRepository.GetCountByUserId(user.Id);
        //        int countStatusPlaying = await _listRepository.GetCountByUserIdAndStatus(user.Id, 1);
        //        int countStatusCompleted = await _listRepository.GetCountByUserIdAndStatus(user.Id, 2);
        //        int countStatusOnHold = await _listRepository.GetCountByUserIdAndStatus(user.Id, 3);
        //        int countStatusDropped = await _listRepository.GetCountByUserIdAndStatus(user.Id, 4);
        //        int countStatusPlanToPlay = await _listRepository.GetCountByUserIdAndStatus(user.Id, 5);
        //        int countRating0 = await _listRepository.GetCountByUserIdAndRating(user.Id, 0);
        //        int countRating1 = await _listRepository.GetCountByUserIdAndRating(user.Id, 1);
        //        int countRating2 = await _listRepository.GetCountByUserIdAndRating(user.Id, 2);
        //        int countRating3 = await _listRepository.GetCountByUserIdAndRating(user.Id, 3);
        //        int countRating4 = await _listRepository.GetCountByUserIdAndRating(user.Id, 4);
        //        int countRating5 = await _listRepository.GetCountByUserIdAndRating(user.Id, 5);
        //        int countRating6 = await _listRepository.GetCountByUserIdAndRating(user.Id, 6);
        //        int countRating7 = await _listRepository.GetCountByUserIdAndRating(user.Id, 7);
        //        int countRating8 = await _listRepository.GetCountByUserIdAndRating(user.Id, 8);
        //        int countRating9 = await _listRepository.GetCountByUserIdAndRating(user.Id, 9);
        //        int countRating10 = await _listRepository.GetCountByUserIdAndRating(user.Id, 10);

        //        var userProfileViewModel = new UserProfileViewModel()
        //        {
        //            UserName = user.UserName,
        //            DisplayName = user.DisplayName ?? user.UserName,
        //            Bio = user.UserBio ?? "",
        //            ProfilePictureUrl = user.ProfilePictureUrl ?? "/img/avatar.jpg",
        //            CountStatusAll = countStatusAll,
        //            CountStatusPlaying = countStatusPlaying,
        //            CountStatusCompleted = countStatusCompleted,
        //            CountStatusOnHold = countStatusOnHold,
        //            CountStatusDropped = countStatusDropped,
        //            CountStatusPlanToPlay = countStatusPlanToPlay,
        //            CountRating0 = countRating0,
        //            CountRating1 = countRating1,
        //            CountRating2 = countRating2,
        //            CountRating3 = countRating3,
        //            CountRating4 = countRating4,
        //            CountRating5 = countRating5,
        //            CountRating6 = countRating6,
        //            CountRating7 = countRating7,
        //            CountRating8 = countRating8,
        //            CountRating9 = countRating9,
        //            CountRating10 = countRating10
        //        };
        //        return View(userProfileViewModel);
        //    }
               
        //}



    }
}
