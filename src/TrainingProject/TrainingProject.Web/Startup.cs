using AutoMapper;
using FluentScheduler;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.SignalR;
using Microsoft.AspNetCore.SpaServices.ReactDevelopmentServer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using Serilog;
using System;
using System.Threading.Tasks;
using TrainingProject.Common;
using TrainingProject.Data;
using TrainingProject.DomainLogic.Helpers;
using TrainingProject.DomainLogic.Interfaces;
using TrainingProject.DomainLogic.Managers;
using TrainingProject.DomainLogic.Services;
using TrainingProject.Web.Filters;
using TrainingProject.Web.Helpers;
using TrainingProject.Web.Hubs;
using TrainingProject.Web.Interfaces;
using TrainingProject.Web.Jobs;
using TrainingProject.Web.Services;
using AppContext = TrainingProject.Data.AppContext;

namespace TrainingProject.Web
{
    public class Startup
    {
        public Startup(IConfiguration configuration, IWebHostEnvironment environment)
        {
            Configuration = configuration;
            Environment = environment;
        }

        public IConfiguration Configuration { get; }
        public IWebHostEnvironment Environment { get; }
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();

            services.AddSpaStaticFiles(configuration =>
            {
                configuration.RootPath = "EventObserver/build";
            });

            services.AddSwaggerDocument();

            services.AddAuthentication(x =>
            {
                x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.RequireHttpsMetadata = false;
                options.SaveToken = true;
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidIssuer = AuthOptions.ISSUER,
                    ValidAudience = AuthOptions.AUDIENCE,
                    IssuerSigningKey = AuthOptions.GetSymmetricSecurityKey(),
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ClockSkew = TimeSpan.Zero
                };
                options.Events = new JwtBearerEvents
                {
                    OnMessageReceived = context =>
                    {
                        var accessToken = context.Request.Query["access_token"];

                        var path = context.HttpContext.Request.Path;
                        if (!string.IsNullOrEmpty(accessToken) &&
                            (path.StartsWithSegments("/chat")))
                        {
                            context.Token = accessToken;
                        }
                        return Task.CompletedTask;
                    },
                    OnAuthenticationFailed = context =>
                    {
                        if (context.Exception.GetType() == typeof(SecurityTokenExpiredException))
                        {
                            context.Response.Headers.Add("Token-Expired", "true");
                        }
                        return Task.CompletedTask;
                    }
                };
            })
            .AddGoogle(options =>
            {
                options.ClientId = "573251226406-6ubk6kmrdvn698ug6nuskodgneb4g9q8.apps.googleusercontent.com";
                options.ClientSecret = "kewmcA1o0j9WDBpXwcbZ3JbT";
            });


            var mappingConfig = new MapperConfiguration(mc =>
            {
                mc.AddProfile(new MappingProfile());
            });
            IMapper mapper = mappingConfig.CreateMapper();
            services.AddSingleton(mapper);
            services.AddDbContext<IAppContext, AppContext>(options =>
                options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")));
            services.AddScoped<IUserManager, UserManager>();
            services.AddScoped<ICategoryManager, CategoryManager>();
            services.AddScoped<IEventManager, EventManager>();
            services.AddScoped<IHostServices, HostServices>();
            services.AddScoped<INotificator, Notificator>();
            services.AddSingleton<IWebHostEnvironment>(Environment);
            services.AddTransient<SendMessageJob>();
            services.AddScoped<ExceptionHandlingFilter>();
            services.AddSingleton<ILogger>(Log.Logger);
            services.AddScoped<ILogHelper, LogHelper>();
            services.AddSingleton<IUserIdProvider, CustomUserIdProvider>();
            services.AddScoped<ISwearingProvider, SwearingProvider>();
            services.AddScoped<ICensor, Censor>();
            services.AddSignalR();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            app.UseDeveloperExceptionPage();

            app.UseSerilogRequestLogging();
            app.UseAuthentication();
            app.UseHttpsRedirection();
            app.UseStaticFiles(new StaticFileOptions()
            {
                OnPrepareResponse = context =>
                {
                    context.Context.Response.Headers.Add("Cache-Control", "no-cache, no-store");
                    context.Context.Response.Headers.Add("Expires", "-1");
                }
            });
            app.UseSpaStaticFiles();

            app.UseOpenApi();
            app.UseSwaggerUi3();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller}/{action=Index}/{id?}");
                endpoints.MapHub<ChatHub>("/chat");
            });

            app.UseSpa(spa =>
            {
                spa.Options.SourcePath = "EventObserver";

                if (env.IsDevelopment())
                {
                    spa.UseReactDevelopmentServer(npmScript: "start");
                }
            });

            JobManager.Initialize(new Sheduler(app.ApplicationServices));
        }
    }
}