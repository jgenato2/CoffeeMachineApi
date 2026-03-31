using CoffeeMachineApi.Controllers;
using Microsoft.AspNetCore.Mvc;
using System;
using Xunit;

namespace CoffeeMachineApi.Tests
{
    public class CoffeeControllerEdgeCaseTests()
    {
        [Fact]
        public void BrewCoffee_Returns418_OnApril1st()
        {
            // Arrange
            var controller = new CoffeeControllerTestable(() => new DateTime(2026, 4, 1));
            CoffeeControllerTestable.ResetCounter();

            // Act
            var result = controller.BrewCoffee() as StatusCodeResult;

            // Assert
            Assert.NotNull(result);
            Assert.Equal(418, result.StatusCode);
        }

        [Fact]
        public void BrewCoffee_Returns503_OnEveryFifthCall()
        {
            var controller = new CoffeeControllerTestable(() => DateTime.Now);
            Assert.NotNull(controller);
            CoffeeControllerTestable.ResetCounter();
            StatusCodeResult? result = null;
            for (int i = 0; i < 5; i++)
            {
                var res = controller.BrewCoffee();
                if (i == 4)
                    result = res as StatusCodeResult;
            }
            Assert.NotNull(result);
            Assert.Equal(503, result!.StatusCode);
        }

        [Fact]
        public void BrewCoffee_Returns200_AndIso8601Date()
        {
            var now = new DateTime(2026, 3, 31, 15, 30, 45, DateTimeKind.Local);
            var controller = new CoffeeControllerTestable(() => now);
            Assert.NotNull(controller);
            CoffeeControllerTestable.ResetCounter();
            var result = controller.BrewCoffee() as OkObjectResult;
            Assert.NotNull(result);
            var obj = result!.Value;
            Assert.NotNull(obj);
            var messageProp = obj.GetType().GetProperty("message");
            var preparedProp = obj.GetType().GetProperty("prepared");
            Assert.NotNull(messageProp);
            Assert.NotNull(preparedProp);
            var messageValue = messageProp!.GetValue(obj) as string;
            var preparedValue = preparedProp!.GetValue(obj) as string;
            Assert.NotNull(messageValue);
            Assert.NotNull(preparedValue);
            Assert.Equal("Your piping hot coffee is ready", messageValue);
            Assert.Equal(now.ToString("yyyy-MM-ddTHH:mm:ssK"), preparedValue);
        }
    }

    // Testable subclass to inject date/time
    public class CoffeeControllerTestable(Func<DateTime> nowProvider) : CoffeeController
    {
        private static Func<DateTime> _nowProvider = () => DateTime.Now;
        public static void ResetCounter()
        {
            var field = typeof(CoffeeController).GetField("_callCount", System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.NonPublic);
            if (field != null)
            {
                field.SetValue(null, 0);
            }
        }
        protected override DateTime GetNow() => nowProvider();
    }
}
