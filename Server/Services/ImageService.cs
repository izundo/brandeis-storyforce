using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Amazon.S3;
using Amazon.S3.Model;
using Microsoft.Extensions.Configuration;

namespace StoryForce.Server.Services
{
    public class ImageService
    {
        private readonly IConfiguration _configuration;
        private IAmazonS3 _s3Client;
        private WebClient _webClient;

        public ImageService(IConfiguration configuration
            , IAmazonS3 s3Client)
        {
            this._configuration = configuration;
            this._s3Client = s3Client;
            this._webClient = new WebClient();
        }

        public string GetPreSignedUrl(string fileName)
        {
            var s3bucketName = this._configuration.GetSection("AWS:S3:BucketName").Value;
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
