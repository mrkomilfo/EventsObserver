using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using TrainingProject.Domain.Logic;
using TrainingProject.Data.Interfaces;
using TrainingProject.Data;
using TrainingProject.Data.Repositories;
using AppContext = TrainingProject.Data.AppContext;

namespace TrainingProject.Web
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDomainServices();
            
            services.AddOpenApiDocument();
            services.AddControllers();
            services.AddAutoMapper(typeof(Startup).Assembly);

            services.AddScoped<IAppContextFactory, AppContextFactory>();
            services.AddScoped<IUserRepository>(provider => new UserRepository(Configuration.GetConnectionString("DefaultConnection"), provider.GetService<IAppContextFactory>()));
            services.AddScoped<IEventRepository>(provider => new EventRepository(Configuration.GetConnectionString("DefaultConnection"), provider.GetService<IAppContextFactory>()));


            services.AddDbContext<AppContext>(options =>
                options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")));
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseOpenApi().UseSwaggerUi3();
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints => { endpoints.MapControllers(); });
        }
    }
}