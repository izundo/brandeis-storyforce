using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Amazon.S3;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using StoryForce.Server.Data;
using StoryForce.Server.Services;
using StoryForce.Shared.Models;
using Microsoft.AspNetCore.Identity.UI.Services;
using StoryForce.Server.Models.Options;
using MatBlazor;

namespace StoryForce.Server
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton(Configuration);
            // services.AddDbContext<ApplicationDbContext>(options =>
            //     options.UseSqlServer(
            //         Configuration.GetConnectionString("DefaultConnection")));

            services.AddDbContext<PgDbContext>(options =>
                options.UseNpgsql(Configuration.GetConnectionString("PostgreConnection")));

            services.AddDatabaseDeveloperPageExceptionFilter();
            services.AddIdentity<Person, IdentityRole<int>>(options => options.SignIn.RequireConfirmedAccount = true)
                .AddDefaultUI()
                .AddDefaultTokenProviders()
                .AddEntityFrameworkStores<PgDbContext>();

            services.AddDefaultAWSOptions(Configuration.GetAWSOptions());
            services.AddAWSService<IAmazonS3>();

            // requires using Microsoft.Extensions.Options
            services.Configure<MongoDbDatabaseSettings>(
                Configuration.GetSection(nameof(MongoDbDatabaseSettings)));

            

            services.AddSingleton<IMongoDbDatabaseSettings>(sp =>
                sp.GetRequiredService<IOptions<MongoDbDatabaseSettings>>().Value);

            services.AddDataServices();

            services.AddSingleton<ImageService>();

            services.Configure<SendGridOptions>(Configuration.GetSection("SendGrid"));
            services.AddSingleton<SendMailJobService>();
            services.AddSingleton<IHostedService>(serviceProvider => serviceProvider.GetService<SendMailJobService>());
            services.AddSingleton<ISendMailJobService>(serviceProvider => serviceProvider.GetService<SendMailJobService>());

            services.AddMatBlazor();

            services.AddControllersWithViews().AddNewtonsoftJson(options =>
                options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore
            );
            services.AddRazorPages();

            services.AddAuthentication()
                .AddGoogle(
                    options =>
                    {
                        IConfigurationSection googleAuthNSection =
                            Configuration.GetSection("Authentication:Google");

                        options.ClientId = googleAuthNSection["ClientId"];
                        options.ClientSecret = googleAuthNSection["ClientSecret"];
                        options.ClaimActions.MapJsonKey("urn:google:picture", "picture", "url");
                        options.ClaimActions.MapJsonKey("urn:google:locale", "locale", "string");
                        options.SaveTokens = true;

                        options.Events.OnCreatingTicket = ctx =>
                        {
                            List<AuthenticationToken> tokens = ctx.Properties.GetTokens().ToList();

                            tokens.Add(new AuthenticationToken()
                            {
                                Name = "TicketCreated",
                                Value = DateTime.UtcNow.ToString()
                            });

                            ctx.Properties.StoreTokens(tokens);

                            return Task.CompletedTask;
                        };
                    });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseForwardedHeaders(new ForwardedHeadersOptions
            {
                ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
            });

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseBrowserLink();
                app.UseWebAssemblyDebugging();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            /*
                This line causes conflict with Apache/Nginx config if the reverse proxy passthrough occurs over http.
                Disable this option in code and allow the server config to handle https redirection.

                app.UseHttpsRedirection();
            */
            app.UseBlazorFrameworkFiles();
            app.UseStaticFiles();
            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapRazorPages();
                endpoints.MapControllers();
                endpoints.MapFallbackToFile("index.html");
            });
        }
    }
}
