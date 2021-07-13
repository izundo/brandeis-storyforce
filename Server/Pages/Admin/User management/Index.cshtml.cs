using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Amazon.S3;
using Amazon.S3.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Configuration;
using StoryForce.Server.Services;
using StoryForce.Shared.Dtos;
using StoryForce.Shared.Models;

namespace StoryForce.Server.Pages.Admin.Person
{
    [Authorize]
    public class User_management : PageModel
    {
        private readonly IConfiguration _configuration;
        private readonly IPeopleService _peopleService;

        public User_management(IConfiguration configuration
            , IPeopleService peopleService)
        {
            this._configuration = configuration;
            this._peopleService = peopleService;
        }
        public IList<StoryForce.Shared.Models.Person> User { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            User = await this._peopleService.GetAsync();
            return Page();
        }
    }
}
