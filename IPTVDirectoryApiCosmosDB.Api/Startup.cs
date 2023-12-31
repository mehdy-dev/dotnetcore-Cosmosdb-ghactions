using IPTVDirectoryApiCosmosDB.Core.Extensions;
using IPTVDirectoryApiCosmosDB.Infrastructure.Contexts;
using IPTVDirectoryApiCosmosDB.Infrastructure.Extentions;
using IPTVDirectoryApiCosmosDB.Infrastructure.Initialization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using System.Threading.Tasks;

namespace IPTVDirectoryApiCosmosDB
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
            services.AddControllers();


            services.AddDbContext<DBInitContext>(options =>
            options.UseCosmos(
            Configuration["Cosmos:AccountEndpoint"],
            Configuration["Cosmos:AccountKey"],
            Configuration["Cosmos:DatabaseName"]));

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "IPTVDirectoryApiCosmosDB", Version = "v1" });
            });
            services.AddCoreServices();
            services.AddInfrastructureServices(Configuration);

            services.AddHostedService<CosmosInitializer>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "IPTVDirectoryApiCosmosDB v1"));
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();


            if (env.IsDevelopment())
            {
                app.UseEndpoints(endpoints =>
                {
                    endpoints.MapControllers();
                });
            }
            else { 
            app.UseEndpoints(endpoints =>
            {


                endpoints.MapDefaultControllerRoute();

                endpoints.MapGet("/", context =>
                {
                    return Task.Run(() => context.Response.Redirect("/api/channel"));
                });
            });
                }

        }
    }
}
