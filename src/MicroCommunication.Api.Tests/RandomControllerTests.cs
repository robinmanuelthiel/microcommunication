using System;
using Xunit;
using MicroCommunication.Api.Controllers;
using Microsoft.AspNetCore.Mvc;

namespace MicroCommunication.Api.Tests
{
    public class RandomControllerTests
    {
        [Fact]
        public void Get_Dice_ReturnsOk()
        {
            // Arrange
            var randomController = new RandomController();

            // Act
            var result = randomController.GetDice();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var randomNumber = Assert.IsType<int>(okResult.Value);
            Assert.True(randomNumber > 0);
            Assert.True(randomNumber <= 6);
        }

        [Fact]
        public void Get_WithoutParamenter_ReturnsOk()
        {
            // Arrange
            var randomController = new RandomController();

            // Act
            var result = randomController.Get(0);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var randomNumber = Assert.IsType<int>(okResult.Value);
            Assert.True(randomNumber >= 0);
        }

        [Fact]
        public void Get_WithMax_ReturnsOk()
        {
            // Arrange
            var randomController = new RandomController();
            var max = 100;

            // Act
            var result = randomController.Get(max);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var randomNumber = Assert.IsType<int>(okResult.Value);
            Assert.True(randomNumber >= 0);
            Assert.True(randomNumber <= max);
        }
    }
}
