using CoffeeMachineApi.Controllers;
using Microsoft.AspNetCore.Mvc;
using System;
using Xunit;

namespace CoffeeMachineApi.Tests
{
    public class CoffeeControllerUnitTests()
    {
        [Fact]
        public void BrewCoffee_Returns200_WhenNotApril1st_AndNotFifthCall()
        {
            // Arrange
            var controller = new CoffeeController();
            var field = typeof(CoffeeController).GetField("_callCount", System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.NonPublic);
            if (field != null)
            {
                field.SetValue(null, 0);
            }

            // Act
            var result = controller.BrewCoffee() as OkObjectResult;

            // Assert
            Assert.NotNull(result);
            Assert.Equal(200, result.StatusCode ?? 200);
            var obj = result.Value;
            Assert.NotNull(obj);
            var messageProp = obj.GetType().GetProperty("message");
            var preparedProp = obj.GetType().GetProperty("prepared");
            Assert.NotNull(messageProp);
            Assert.NotNull(preparedProp);
            var messageValue = messageProp.GetValue(obj) as string;
            var preparedValue = preparedProp.GetValue(obj) as string;
            Assert.NotNull(messageValue);
            Assert.NotNull(preparedValue);
            Assert.Equal("Your piping hot coffee is ready", messageValue);
            Assert.True(DateTime.TryParse(preparedValue, out _));
        }

        [Fact]
        public void BrewCoffee_Returns503_OnFifthCall()
        {
            // Arrange
            var controller = new CoffeeController();
            var field = typeof(CoffeeController).GetField("_callCount", System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.NonPublic);
            if (field != null)
            {
                field.SetValue(null, 4);
            }

            // Act
            var result = controller.BrewCoffee();

            // Assert
            Assert.NotNull(result);
            var statusCode = (result as StatusCodeResult)?.StatusCode
                ?? (result as ObjectResult)?.StatusCode;
            Assert.Equal(503, statusCode);
        }
    }
}
