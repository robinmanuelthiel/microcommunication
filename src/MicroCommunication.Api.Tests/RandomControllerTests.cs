using System;
using Xunit;
using MicroCommunication.Api.Controllers;

namespace MicroCommunication.Api.Tests
{
    public class RandomControllerTests
    {
        [Fact]
        public void Get_Dice_ReturnsOk()
        {
            var randomController = new RandomController();

            var result = randomController.GetDice();
            var randomNumber = result.Value;

            Assert.True(randomNumber > 0);
            Assert.True(randomNumber <= 6);
        }

        [Fact]
        public void Get_WithoutParamenter_ReturnsOk()
        {
            var randomController = new RandomController();

            var result = randomController.Get(0);
            var randomNumber = result.Value;

            Assert.True(randomNumber >= 0);
        }

        [Fact]
        public void Get_WithMax_ReturnsOk()
        {
            var randomController = new RandomController();
            var max = 100;

            var result = randomController.Get(max);
            var randomNumber = result.Value;

            Assert.True(randomNumber >= 0);
            Assert.True(randomNumber <= max);
        }
    }
}
