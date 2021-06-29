using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using StoryForce.Server.Services;
using StoryForce.Shared.Models;

namespace StoryForce.Server.Pages.Admin
{
    [Authorize]
    public class ShowFileModel : PageModel
    {
        public UserManager<IdentityUser> _userManager;
        private readonly IStoryFileService _storyFileService;

        public ShowFileModel(UserManager<IdentityUser> userManager, IStoryFileService storyFileService)
        {
            _userManager = userManager;
            _storyFileService = storyFileService;
        }

        public IEnumerable<SubmittedByGroup> SubmittedByGroups { get; set; }

        public async Task<ActionResult> OnGetAsync()
        {
            var files = await this._storyFileService.GetByRequestedEmailAsync(_userManager.GetUserName(User));

            var requestedFiles = files.Where(x => x.RequestedBy != null).ToList();
            
            SubmittedByGroups = from file in requestedFiles
                                group file by file.SubmittedBy.Email into submittedBy
                                where submittedBy.Key is not null
                                select new SubmittedByGroup() { SubmittedBy = files.First(x => x.SubmittedBy.Email == submittedBy.Key).SubmittedBy, Files = submittedBy.ToList()};

            return Page();
        }
    }

    public class SubmittedByGroup
    {
        public StoryForce.Shared.Models.Person SubmittedBy { get; set; }

        public List<StoryFile> Files { get; set; }
    }
}
