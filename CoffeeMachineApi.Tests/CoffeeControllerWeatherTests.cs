using CoffeeMachineApi.Controllers;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Xunit;
using Moq.Protected;

namespace CoffeeMachineApi.Tests
{
    public class CoffeeControllerWeatherTests()
    {
        [Fact]
        public async Task BrewCoffee_ReturnsIcedCoffee_WhenTempAbove30()
        {
            // Arrange
            var httpClientFactory = MockHttpClientFactoryWithWeather(35);
            var controller = new CoffeeController(httpClientFactory);
            var field = typeof(CoffeeController).GetField("_callCount", System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.NonPublic);
            field?.SetValue(null, 0);

            // Act
            var result = await controller.BrewCoffee() as OkObjectResult;

            // Assert
            Assert.NotNull(result);
            var obj = result.Value;
            Assert.NotNull(obj);
            var messageProp = obj.GetType().GetProperty("message");
            Assert.NotNull(messageProp);
            var messageValue = messageProp.GetValue(obj) as string;
            Assert.Equal("Your refreshing iced coffee is ready", messageValue);
        }

        [Fact]
        public async Task BrewCoffee_ReturnsHotCoffee_WhenTempBelowOrEqual30()
        {
            // Arrange
            var httpClientFactory = MockHttpClientFactoryWithWeather(25);
            var controller = new CoffeeController(httpClientFactory);
            var field = typeof(CoffeeController).GetField("_callCount", System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.NonPublic);
            field?.SetValue(null, 0);

            // Act
            var result = await controller.BrewCoffee() as OkObjectResult;

            // Assert
            Assert.NotNull(result);
            var obj = result.Value;
            Assert.NotNull(obj);
            var messageProp = obj.GetType().GetProperty("message");
            Assert.NotNull(messageProp);
            var messageValue = messageProp.GetValue(obj) as string;
            Assert.Equal("Your piping hot coffee is ready", messageValue);
        }

        private static IHttpClientFactory MockHttpClientFactoryWithWeather(double temp)
        {
            var handler = new Mock<HttpMessageHandler>();
            handler.Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent($"{{\"main\":{{\"temp\":{temp}}}}}")
                });
            var httpClient = new HttpClient(handler.Object);
            var factory = new Mock<IHttpClientFactory>();
            factory.Setup(f => f.CreateClient(It.IsAny<string>())).Returns(httpClient);
            return factory.Object;
        }
    }
}
