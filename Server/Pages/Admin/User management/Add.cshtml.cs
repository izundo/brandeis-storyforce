using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using StoryForce.Server.Services;
using StoryForce.Shared.Models;

namespace StoryForce.Server.Pages.Admin.Person
{
    [Authorize]
    public class AddModel : PageModel
    {
        private readonly IPeopleService _peopleService;

        public AddModel(IPeopleService peopleService)
        {
            this._peopleService = peopleService;
            User = new();
        }
        [BindProperty]
        public StoryForce.Shared.Models.Person User { get; set; }
        public async Task<IActionResult> OnGetAsync()
        {
            User =  new StoryForce.Shared.Models.Person();
            return Page();
        }         

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }            
            else
            {
                User.Name = User.Name;
                User.Email = User.Email;
                User.ClassOfYear = User.ClassOfYear;
                //User.AvatarUrl = User.AvatarUrl;
                //User.Type = User.Type;
                //User.SecurityStamp = User.SecurityStamp;
                //User.AccessFailedCount = User.AccessFailedCount;
                //User.ConcurrencyStamp = User.ConcurrencyStamp;
                //User.EmailConfirmed = User.EmailConfirmed;
                //User.LockoutEnabled = User.LockoutEnabled;
                //User.LockoutEnd = User.LockoutEnd;
                //User.NormalizedEmail = User.NormalizedEmail;
                //User.NormalizedUserName = User.NormalizedUserName;
                //User.PasswordHash = User.PasswordHash;
                User.PhoneNumber = User.PhoneNumber;
                //User.PhoneNumberConfirmed = User.PhoneNumberConfirmed;
                //User.TwoFactorEnabled = User.TwoFactorEnabled;
                //User.UserName = User.UserName;
                await _peopleService.CreateAsync(User);
            }
            return Redirect("/usermanagement");
        }
    }
}
