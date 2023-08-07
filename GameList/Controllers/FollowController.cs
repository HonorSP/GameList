
using GameList.Interfaces;
using GameList.Models;
using GameList.Repository;
using GameList.Services;
using GameList.ViewModels;
using IGDB;
using IGDB.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GameList.Controllers
{
    //This is the implementation of a FollowController for GameList built using ASP.NET Core MVC.
    //It provides actions for following, unfollowing, viewing followers, and viewing following users.
    //Using the UserManager<AppUser> and IFollowRepository dependencies to retrieve information. 
    public class FollowController : Controller
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly IFollowRepository _followRepository;

        public FollowController(UserManager<AppUser> userManager, IFollowRepository followRepository)
        {
            _userManager = userManager;
            _followRepository = followRepository;
        }
        //Follows a user by adding the current user's ID to the followed user's followers list in the repository
        public async Task<IActionResult> FollowUser(string followUserId)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return RedirectToAction("Login", "Account");
            }
            else
            {
                var checkIsFollowing = _followRepository.IsFollowing(user.Id, followUserId);
                if (checkIsFollowing == false)
                {
                    _followRepository.AddFollower(user.Id, followUserId);
                    var returnUser = await _userManager.FindByIdAsync(followUserId);
                    return RedirectToAction("UserProfile", "User", new { userName = returnUser.UserName });
                }
                else
                {
                    return RedirectToAction("Login", "Account");
                }
            }
        }
        //UnfollowUser: unfollows a user by removing the current user's ID from the followed user's followers list in the repository
        public async Task<IActionResult> UnfollowUser(string followUserId)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return RedirectToAction("Login", "Account");
            }
            else
            {
                var checkIsFollowing = _followRepository.IsFollowing(user.Id, followUserId);
                if (checkIsFollowing == true)
                {
                    _followRepository.RemoveFollower(user.Id, followUserId);
                    var returnUser = await _userManager.FindByIdAsync(followUserId);
                    return RedirectToAction("UserProfile", "User", new { userName = returnUser.UserName });
                }
                else
                {
                    return RedirectToAction("Login", "Account");
                }
            }
        }
        //Following: returns a view of the list of users being followed by the specified user
        public async Task<IActionResult> Following(string followUserId, int page = 1, int pageSize = 18)
        {
            if (followUserId == null)
            {
                return RedirectToAction("Login", "Account");
            }
            else
            {
                List<string> followingIds = _followRepository.GetFollowingIds(followUserId);


                var followingUsers = new FollowingFollowersViewModel();
                followingUsers.Users = new List<UserViewModel>();
                int offset = (page - 1) * pageSize;

                var userCount = _userManager.Users.Where(u => followingIds.Contains(u.Id)).Count();
                var users = await _userManager.Users.Where(u => followingIds.Contains(u.Id)).Skip(offset).Take(pageSize).ToListAsync();
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
                    followingUsers.Users.Add(viewModel);

                }
                var totalUsers = userCount;
                followingUsers.FollowUserId = followUserId;
                followingUsers.TotalPages = (int)Math.Ceiling((double)totalUsers / pageSize);
                followingUsers.Page = page;
                followingUsers.PageSize = pageSize;


                return View(followingUsers);

                
            }
        }
        //Followers: returns a view of the list of followers for the specified user
        public async Task<IActionResult> Followers(string followUserId, int page = 1, int pageSize = 18)
        {
            if (followUserId == null)
            {
                return RedirectToAction("Login", "Account");
            }
            else
            {
                List<string> followersIds = _followRepository.GetFollowerIds(followUserId);


                var followerUsers = new FollowingFollowersViewModel();
                followerUsers.Users = new List<UserViewModel>();
                int offset = (page - 1) * pageSize;

                var userCount = _userManager.Users.Where(u => followersIds.Contains(u.Id)).Count();
                var users = await _userManager.Users.Where(u => followersIds.Contains(u.Id)).Skip(offset).Take(pageSize).ToListAsync();
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
                    followerUsers.Users.Add(viewModel);

                }
                var totalUsers = userCount;
                followerUsers.FollowUserId = followUserId;
                followerUsers.TotalPages = (int)Math.Ceiling((double)totalUsers / pageSize);
                followerUsers.Page = page;
                followerUsers.PageSize = pageSize;


                return View(followerUsers);
                
            }
        }


    }
}
