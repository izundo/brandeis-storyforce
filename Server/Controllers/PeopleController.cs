using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using StoryForce.Server.Services;
using StoryForce.Shared.Dtos;
using StoryForce.Shared.Models;

namespace StoryForce.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PeopleController : ControllerBase
    {
        private readonly IPeopleService _peopleService;

        public PeopleController(IPeopleService peopleService)
        {
            this._peopleService = peopleService;
        }

        [HttpGet]
        public async Task<ActionResult<List<Person>>> Get()
        {
            return await this._peopleService.GetAsync();
        }

        private async Task Seed()
        {
            /*
             *  Chloe Lewis	2025
                Claire Banks	2020
                Naomi Lewis	2023
                Gabby Lewis 	2029
                Dash Rennie	2025
                Siena Rennie	2022
                Yohan Hong	2020
                Greg Franklin-Sparks	2025
                Kyle Penczak	2022
                Greg Goldstein	2022
                Jake Horowitz	2021
                Quinn Horowitz	2029
                Gavin Horowitz 	2024
                Maya Kux	2022
                Lyla Kux	2027
                Charley Goldstein	2020
                Isaac Lendl	2025
                Lev Khersonsky	2029
                Zara Khersonsky	2027
                Micah Traeger Hirschfelder	2022
                Elias Traeger Hirschfelder	2026
                Jacob Goldberg	2022
                Sydelle Goldberg	2025

             */
            var people = new List<Person>
            {
                new Person
                {
                    Name = "Nataan Hong",
                    ClassOfYear = 2021
                },
                new Person
                {
                    Name = "Ezra Rosen",
                    ClassOfYear = 2021
                },
                new Person
                {
                    Name = "Aviv Shakked",
                    ClassOfYear = 2021
                },
                new Person
                {
                    Name = "David Flores",
                    ClassOfYear = 2021
                }
            };

            await this._peopleService.CreateMultipleAsync(people);
        }
    }
}
