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
    public class UserDetail : PageModel
    {
        private readonly IPeopleService _peopleService;

        public UserDetail(IConfiguration configuration
            , IPeopleService peopleService)
        {
            this._peopleService = peopleService;
            User = new();
        }

        [BindProperty]
        public StoryForce.Shared.Models.Person User { get; set; }

        public async Task<IActionResult> OnGetAsync(int id)
        {
            User = await this._peopleService.GetAsync(id);
            return Page();
        }

        [BindProperty]
        public StoryForce.Shared.Models.Person Person { get; set; }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }
            if (Person.Id > 0)
            {
                var person = await _peopleService.GetAsync(Person.Id);
                person.Name = User.Name;
                person.Email = User.Email;
                person.ClassOfYear = User.ClassOfYear;
                //person.AvatarUrl = User.AvatarUrl;
                //person.Type = User.Type;
                //person.SecurityStamp = User.SecurityStamp;
                //person.AccessFailedCount = User.AccessFailedCount;
                //person.ConcurrencyStamp = User.ConcurrencyStamp;
                //person.EmailConfirmed = User.EmailConfirmed;
                //person.LockoutEnabled = User.LockoutEnabled;
                //person.LockoutEnd = User.LockoutEnd;
                //person.NormalizedEmail = User.NormalizedEmail;
                //person.NormalizedUserName = User.NormalizedUserName;
                //person.PasswordHash = User.PasswordHash;
                person.PhoneNumber = User.PhoneNumber;
                //person.PhoneNumberConfirmed = User.PhoneNumberConfirmed;
                //person.TwoFactorEnabled = User.TwoFactorEnabled;
                //person.UserName = User.UserName;
                await _peopleService.UpdateAsync(Person.Id, person);
            }            
            return Redirect("/usermanagement");
        }
    }
}
