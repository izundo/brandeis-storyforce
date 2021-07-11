using Microsoft.Extensions.DependencyInjection;

namespace StoryForce.Server.Services
{
    public static class AddDataServicesExtension
    {
        public static IServiceCollection AddDataServices(this IServiceCollection services)
        {
            services.AddSingleton<SubmissionService>();
            services.AddSingleton<StoryFileService>();
            services.AddSingleton<PeopleService>();
            services.AddSingleton<EventService>();

            services.AddTransient<ISubmissionService, SubmissionServicePg>();
            services.AddTransient<IStoryFileService, StoryFileServicePg>();
            services.AddTransient<IPeopleService, PeopleServicePg>();
            services.AddTransient<IEventService, EventServicePg>();
            services.AddTransient<INoteService, NoteServicePg>();
            services.AddTransient<ITagService, TagServicePg>();
            return services;
        }
    }
}
