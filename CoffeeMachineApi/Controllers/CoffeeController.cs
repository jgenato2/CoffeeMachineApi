using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading;

namespace CoffeeMachineApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class CoffeeController : ControllerBase
    {
        private static int _callCount = 0;
        private static readonly Lock _lock = new();

        [HttpGet("/brew-coffee")]
        public virtual IActionResult BrewCoffee()
        {
            var now = GetNow();
            if (now.Month == 4 && now.Day == 1)
            {
                // April 1st: I'm a teapot
                return StatusCode(418);
            }

            int callNumber;
            lock (_lock)
            {
                _callCount++;
                callNumber = _callCount;
            }

            if (callNumber % 5 == 0)
            {
                // Every 5th call: Service Unavailable
                return StatusCode(503);
            }

            return Ok(new
            {
                message = "Your piping hot coffee is ready",
                prepared = now.ToString("yyyy-MM-ddTHH:mm:ssK")
            });
        }

        protected virtual DateTime GetNow() => DateTime.Now;
    }
}
