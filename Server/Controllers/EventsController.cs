using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using StoryForce.Server.Services;
using StoryForce.Shared.Models;

namespace StoryForce.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EventsController : ControllerBase
    {
        private readonly IEventService _eventService;

        public EventsController(IEventService eventService)
        {
            this._eventService = eventService;
        }

        [HttpGet]
        public async Task<ActionResult<List<Event>>> Get()
        {
            var eve = await this._eventService.GetAsync();
            var Event = eve.FindAll(m => m.Name != null);
            return Ok(Event);
        }

        private async Task Seed()
        {
            var events = new List<Event>
            {
                new Event
                {
                    Name = "Passover",
                    Year = 2021
                },
                new Event
                {
                    Name = "Purim",
                    Year = 2021
                },
                new Event
                {
                    Name = "Yom Kippur",
                    Year = 2021
                }
            };

            await this._eventService.CreateMultipleAsync(events);
        }
    }
}
