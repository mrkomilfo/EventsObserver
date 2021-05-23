using AutoMapper;

using FluentScheduler;

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

using NSwag;
using NSwag.Generation.Processors.Security;

using Serilog;

using System;
using System.Linq;
using System.Threading.Tasks;

using TrainingProject.Common;
using TrainingProject.Data;
using TrainingProject.DomainLogic.Helpers;
using TrainingProject.DomainLogic.Interfaces;
using TrainingProject.DomainLogic.Managers;
using TrainingProject.DomainLogic.Services;
using TrainingProject.Web.Filters;
using TrainingProject.Web.Helpers;
using TrainingProject.Web.Interfaces;
using TrainingProject.Web.Jobs;
using TrainingProject.Web.Services;
using AppContext = TrainingProject.Data.AppContext;

namespace TrainingProject.Web
{
    public class Startup
    {
        private IConfiguration Configuration { get; }

        private IWebHostEnvironment Environment { get; }

        public Startup(IConfiguration configuration, IWebHostEnvironment environment)
        {
            Configuration = configuration;
            Environment = environment;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllersWithViews();

            services.AddSpaStaticFiles(configuration =>
            {
                configuration.RootPath = "EventObserver/build";
            });

            services.AddSwaggerDocument(config =>
            {
                config.DocumentName = "OpenAPI 2";
                config.OperationProcessors.Add(new OperationSecurityScopeProcessor("JWT Token"));
                config.AddSecurity("JWT Token", Enumerable.Empty<string>(),
                    new OpenApiSecurityScheme
                    {
                        Type = OpenApiSecuritySchemeType.ApiKey,
                        Name = "Authorization",
                        In = OpenApiSecurityApiKeyLocation.Header,
                        Description = "Copy this into the value field: Bearer {token}"
                    }
                );
            });

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
            });

            var mappingConfig = new MapperConfiguration(mc =>
            {
                mc.AddProfile(new MappingProfile());
            });
            var mapper = mappingConfig.CreateMapper();

            services.AddSingleton(mapper);
            services.AddDbContext<IAppContext, AppContext>(options =>
                options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")));
            services.AddScoped<IUserManager, UserManager>();
            services.AddScoped<ICategoryManager, CategoryManager>();
            services.AddScoped<IEventManager, EventManager>();
            services.AddScoped<ICommentManager, CommentManager>();
            services.AddScoped<IHostServices, HostServices>();
            services.AddScoped<INotificator, Notificator>();
            services.AddSingleton(Environment);
            services.AddTransient<SendMessageJob>();
            services.AddScoped<ExceptionHandlingFilter>();
            services.AddSingleton(Log.Logger);
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