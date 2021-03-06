using Xunit;
using System.Threading.Tasks;
using MicroCommunication.Api.Controllers;
using Microsoft.AspNetCore.Mvc;
using MicroCommunication.Api.Services;
using Microsoft.Extensions.Configuration;
using Moq;

namespace MicroCommunication.Api.Tests
{
    public class RandomControllerTests
    {
        private readonly HistoryService fakeHistoryService = new HistoryService("");
        private readonly IConfiguration fakeConfiguration = new Mock<IConfiguration>().Object;


        [Fact]
        public async Task Get_Dice_ReturnsOk()
        {
            // Arrange
            var randomController = new RandomController(fakeHistoryService, fakeConfiguration);

            // Act
            var result = await randomController.GetDice();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var randomNumber = Assert.IsType<int>(okResult.Value);
            Assert.True(randomNumber > 0);
            Assert.True(randomNumber <= 6);
        }

        [Fact]
        public async Task Get_WithoutParamenter_ReturnsOk()
        {
            // Arrange
            var randomController = new RandomController(fakeHistoryService, fakeConfiguration);

            // Act
            var result = await randomController.Get(0);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var randomNumber = Assert.IsType<int>(okResult.Value);
            Assert.True(randomNumber >= 0);
        }

        [Fact]
        public async Task Get_WithMax_ReturnsOk()
        {
            // Arrange
            var randomController = new RandomController(fakeHistoryService, fakeConfiguration);
            var max = 100;

            // Act
            var result = await randomController.Get(max);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var randomNumber = Assert.IsType<int>(okResult.Value);
            Assert.True(randomNumber >= 0);
            Assert.True(randomNumber <= max);
        }
    }
}
