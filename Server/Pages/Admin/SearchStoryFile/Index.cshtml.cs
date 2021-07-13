using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using Amazon.S3;
using Amazon.S3.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Configuration;
using StoryForce.Server.Services;
using StoryForce.Shared.Dtos;

namespace StoryForce.Server.Pages.Admin.SearchStoryFile
{
    [Authorize]
    public class IndexModel : PageModel
    {
        private readonly IStoryFileService _storyFileService;
        private readonly IAmazonS3 _s3Client;

        public IndexModel(IAmazonS3 s3Client, IStoryFileService storyFileService)
        {
            this._storyFileService = storyFileService;
            this._s3Client = s3Client;
            storyFiles = new List<StoryForce.Shared.Models.StoryFile>();
        }
        public IList<StoryForce.Shared.Models.StoryFile> storyFiles { get; set; }
        [Parameter]

        public string myvalue { get; set; }

        public async Task<IActionResult> OnGetAsync(string myvalue)
        {
            storyFiles = await _storyFileService.GetByStoryFileByInputValueAsync(myvalue);
            return Page();
        }
    }
}

