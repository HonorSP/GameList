using CloudinaryDotNet.Actions;
using FakeItEasy;
using GameList.Controllers;
using GameList.Interfaces;
using GameList.Models;
using GameList.Repository;
using GameList.ViewModels;
using IGDB;
using IGDB.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

namespace GameList.Tests.Controller
{
    public class GameControllerTests
    {
        //private GameController _gameController;
        //private IListRepository _listRepository;
        //private UserManager<AppUser> _userManager;
        //private IGDBClient igdb;

        //public GameControllerTests()
        //{
        //    _listRepository = A.Fake<IListRepository>(); ;
        //    _userManager = A.Fake<UserManager<AppUser>>(); ;
        //    igdb = new IGDBClient("mglx6fger7kprdp4o25d74g9dg9ewf", "128zb34th4o2k2sdoeci8iqhq9x6wk");


        //    _gameController = new GameController(_listRepository, _userManager, igdb);
        //}

        //[Fact]
        //public async Task CharacterInfo_ReturnsCorrectResult()
        //{
        //    // Arrange
        //    var character = new IGDB.Models.Character
        //    {
        //        Name = "Test Character",
        //        MugShot = new Image { ImageId = "1" },
        //        Species = "Test Species",
        //        Gender = "Test Gender",
        //        Description = "Test Description",
        //        Games = new List<IGDB.Models.Game> { new IGDB.Models.Game { Name = "Test Game", Cover = new Image { ImageId = "2" } } }
        //    };

        //    A.CallTo(() => igdb.QueryAsync<Character>(IGDBClient.Endpoints.Characters, A<string>._)).Returns(new List<IGDB.Models.Character> { character });

        //    // Act
        //    var result = await _gameController.CharacterInfo(1);

        //    // Assert
        //    result.Should().BeOfType<ViewResult>();
        //    var viewResult = (ViewResult)result;
        //    var model = (IEnumerable<IGDB.Models.Character>)viewResult.Model;
        //    model.Should().NotBeNull();
        //    model.Count().Should().Be(1);
        //    model.First().Name.Should().Be("Test Character");
        //    model.First().MugShot.ImageId.Should().Be("1");
        //    model.First().Species.Should().Be("Test Species");
        //    model.First().Gender.Should().Be("Test Gender");
        //    model.First().Description.Should().Be("Test Description");
        //    model.First().Games.Count().Should().Be(1);
        //    model.First().Games.First().Name.Should().Be("Test Game");
        //    model.First().Games.First().Cover.ImageId.Should().Be("2");
        //}


    }
}
