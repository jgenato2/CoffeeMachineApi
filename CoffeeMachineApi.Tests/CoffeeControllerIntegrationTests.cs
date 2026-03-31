using System.Net;
using System.Net.Http.Json;
using System.Threading.Tasks;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;

namespace CoffeeMachineApi.Tests
{
    public class CoffeeControllerIntegrationTests : IClassFixture<WebApplicationFactory<CoffeeMachineApi.Program>>
    {
        private readonly WebApplicationFactory<CoffeeMachineApi.Program> _factory;

        public CoffeeControllerIntegrationTests(WebApplicationFactory<CoffeeMachineApi.Program> factory)
        {
            _factory = factory;
        }

        [Fact]
        public async Task BrewCoffee_Returns200_AndJson()
        {
            var client = _factory.CreateClient();
            var response = await client.GetAsync("/brew-coffee");
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            var json = await response.Content.ReadFromJsonAsync<JsonElement>();
            Assert.Equal("Your piping hot coffee is ready", json.GetProperty("message").GetString());
        }
    }
}
