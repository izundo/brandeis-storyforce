using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Amazon.S3;
using Amazon.S3.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Configuration;
using StoryForce.Server.Services;
using StoryForce.Shared.Dtos;

namespace StoryForce.Server.Pages.Admin
{
    [Authorize]
    public class IndexModel : PageModel
    {
        private readonly IConfiguration _configuration;
        private readonly ISubmissionService _submissionService;
        private readonly IAmazonS3 _s3Client;
        private WebClient _webClient;

        public IndexModel(IConfiguration configuration
            , ISubmissionService submissionService
            , IAmazonS3 s3Client)
        {
            this._configuration = configuration;
            this._submissionService = submissionService;
            this._s3Client = s3Client;
            this._webClient = new WebClient();
        }

        public IList<SubmissionDto> Submissions { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            var submissionModels = await this._submissionService.GetAsync();
            Submissions = submissionModels.Select(submission => SubmissionDto.ConvertFromEntity(submission)).OrderByDescending(s => s.CreatedAt).ToList();
            //foreach (var submission in Submissions)
            //{
            //    foreach (var file in submission.SubmittedFiles)
            //    {
            //        file.ThumbnailUrl = GetPreSignedUrl(file.Key);
            //    }
            //}
            return Page();
        }

        protected string GetPreSignedUrl(string fileName)
        {
            var s3bucketName= this._configuration.GetSection("AWS:S3:BucketName").Value;
            var url = this._s3Client.GetPreSignedURL(
                new GetPreSignedUrlRequest
                {
                    BucketName = s3bucketName,
                    Key = fileName,
                    Verb = HttpVerb.GET,
                    Expires = DateTime.UtcNow.AddHours(2)
                });

            return url;
        }
    }
}
