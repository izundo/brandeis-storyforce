using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using StoryForce.Shared.Models;

namespace StoryForce.Server.Data
{
    public class PgDbContext : IdentityDbContext<Person, IdentityRole<int>, int>
    {
        public DbSet<Tag> Tags { get; set; }

        public DbSet<Submission> Submissions { get; set; }

        public DbSet<StoryFile> StoryFiles { get; set; }

        public DbSet<Story> Stories { get; set; }

        public DbSet<Person> Persons { get; set; }

        public DbSet<Note> Notes { get; set; }

        public DbSet<Event> Events { get; set; }

        public DbSet<Comment> Comments { get; set; }

        public DbSet<AuditDetail> AuditDetails { get; set; }

        public PgDbContext(DbContextOptions options) : base(options)
        {
            
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Submission>()
                .HasMany(submission => submission.SubmittedFiles)
                .WithOne(sf => sf.Submission)
                .HasForeignKey(sf => sf.SubmissionId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Submission>()
                .HasMany(submission => submission.ApprovedFiles)
                .WithOne(sf => sf.ApprovedSubmission)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Submission>()
                .HasMany(submission => submission.RejectedFiles)
                .WithOne(sf => sf.RejectedSubmission)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Submission>()
                .HasMany(submission => submission.FeaturedPeople)
                .WithMany(person => person.FeaturedSubmissions);

            modelBuilder.Entity<Submission>()
                .HasOne(submission => submission.SubmittedBy)
                .WithMany(person => person.SubmittedSubmissions)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Submission>()
                .HasOne(submission => submission.ReviewedBy)
                .WithMany(person => person.ReviewedBySubmissions)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Submission>()
                .HasOne(submission => submission.ApprovedBy)
                .WithMany(person => person.ApprovedSubmissions)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Person>()
                .HasMany(person=> person.FeaturedStoryFile)
                .WithMany(sf => sf.FeaturedPeople);

            modelBuilder.Entity<StoryFile>()
                .HasOne(sf => sf.SubmittedBy)
                .WithMany(person => person.SubmittedStoryFiles)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<StoryFile>()
                .HasOne(sf => sf.Event)
                .WithMany(ev => ev.StoryFiles)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<StoryFile>()
                .HasOne(sf => sf.RequestedBy)
                .WithMany(person => person.RequestedStoryFiles)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<StoryFile>()
                .HasOne(sf => sf.UpdatedBy)
                .WithMany(person => person.UpdatedStoryFiles)
                .OnDelete(DeleteBehavior.Cascade);


            modelBuilder.Entity<StoryFile>()
                .HasMany(sf => sf.Categories)
                .WithMany(cat => cat.StoryFiles);

            modelBuilder.Entity<StoryFile>()
                .HasMany(sf => sf.Tags)
                .WithMany(tag => tag.StoryFiles);

            modelBuilder.Entity<Person>()
                .HasMany(person => person.FeaturedStoryFile)
                .WithMany(sf => sf.FeaturedPeople);

            modelBuilder.Entity<Person>()
                .HasMany(person => person.FeaturedStoryFile)
                .WithMany(sf => sf.FeaturedPeople);

            modelBuilder.Entity<Person>()
                .HasMany(person => person.FeaturedStoryFile)
                .WithMany(sf => sf.FeaturedPeople);

        }
    }
}
