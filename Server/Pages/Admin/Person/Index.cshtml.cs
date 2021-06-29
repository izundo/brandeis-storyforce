using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using StoryForce.Server.Services;

namespace StoryForce.Server.Pages.Admin.Person
{
    public class IndexModel : PageModel
    {
        private readonly IPeopleService _peopleService;
        public IndexModel(IPeopleService peopleService)
        {
            this._peopleService = peopleService;
        }

        [BindProperty]
        public IEnumerable<StoryForce.Shared.Models.Person> People { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            this.People = await this._peopleService.GetAsync();
            return Page();
        }

        public async Task<IActionResult> OnPostDeleteAsync(int id)
        {
            await this._peopleService.RemoveAsync(id);
            return new RedirectToPageResult("/Admin/Person/Index");
        }
    }
}
