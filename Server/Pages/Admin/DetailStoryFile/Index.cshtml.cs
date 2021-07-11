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
using StoryForce.Shared.ViewModels;

namespace StoryForce.Server.Pages.Admin
{
    [Authorize]
    public class StoryFileDetail : PageModel
    {
        private readonly IStoryFileService _storyFileService;
        private readonly IPeopleService _peopleService;
        private readonly ISubmissionService _submissionService;

        public StoryFileDetail(IStoryFileService storyFileService, IPeopleService peopleService, ISubmissionService submissionService)
        {
            this._storyFileService = storyFileService;
            this._peopleService = peopleService;
            StoryFile = new();            
            this._submissionService = submissionService;
        }

        [BindProperty]
        public StoryForce.Shared.Models.StoryFile StoryFile { get; set; }
        public async Task<IActionResult> OnGetAsync(int id)
        {
            StoryFile = await this._storyFileService.GetAsync(id);             
            return Page();
        }

        [BindProperty]
        public StoryForce.Shared.Models.StoryFile File { get; set; }      

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }
            if (File.Id > 0)
            {
                var file = await _storyFileService.GetAsync(File.Id);
                file.Description = StoryFile.Description;                           
                await _storyFileService.UpdateAsync(file.Id, file);                
            }
            return Page();
        }

        [BindProperty]
        public IList<StoryForce.Shared.Models.Person> RequestByPerson { get; set; }
    }
}
