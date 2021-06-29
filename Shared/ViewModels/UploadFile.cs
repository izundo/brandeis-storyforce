using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components.Forms;
using StoryForce.Shared.Models;

namespace StoryForce.Shared.ViewModels
{
    public class UploadFile
    {
        public string Title { get; set; }

        public string Key { get; set; }

        //[Required]
        public string Description { get; set; }

        public string MimeType { get; set; }

        public long? Size { get; set; }

        public string PreviewUrl { get; set; }

        public decimal UploadPercentage { get; set; }

        public StorageProvider StorageProvider { get; set; }

        public string ProviderFileId { get; set; }

        public string DownloadUrl { get; set; }

        public string ThumbnailUrl { get; set; }

        [ValidateComplexType]
        public List<Person> FeaturedPeople { get; set; }

        public Event Event { get; set; }

        public string EventName { get; set; }

        public string EventId { get; set; }

        public int? Class { get; set; }

        public Person RequestedBy { get; set; }

        public List<Note> Notes { get; set; }

        public UploadFile()
        {
            this.FeaturedPeople = new List<Person>();
            this.Event = new();
            this.Notes = new List<Note>();            
        }
        
    }

    public static class MyExtensions
    {
        public static string ToFileSizeString(this long size)
        {
            string[] suf = { "b", "kb", "mb", "gb", "tb", "pb", "eb" }; //Longs run out around EB
            if (size == 0)
                return "0" + suf[0];
            long bytes = Math.Abs(size);
            int place = Convert.ToInt32(Math.Floor(Math.Log(bytes, 1024)));
            double num = Math.Round(bytes / Math.Pow(1024, place), 1);
            return (Math.Sign(size) * num) + suf[place];
        }
    }
}
