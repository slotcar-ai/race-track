using System.IO;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Swashbuckle.AspNetCore.Swagger;

namespace Scai.Driver.Web
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSwaggerGen(opt =>
            {
                opt.SwaggerDoc("v1", new Info { Title = "Driver AI API", Version = "v1" });
            });

            services
                .AddScoped<IAiCompiler, AiCompiler>()
                .AddMvc()
                .SetCompatibilityVersion(CompatibilityVersion.Version_2_2);
        }

        public void Configure(IApplicationBuilder application, IHostingEnvironment environment)
        {
            if (environment.IsDevelopment())
            {
                application.UseDeveloperExceptionPage();
            }
            else
            {
                application
                    .UseHsts()
                    .UseHttpsRedirection();
            }

            var fileProvider = new PhysicalFileProvider(Path.Combine(environment.ContentRootPath, "Client"));
            application
                .UseDefaultFiles(new DefaultFilesOptions
                {
                    DefaultFileNames = new string[] { "index.html" },
                    FileProvider = fileProvider,
                })
                .UseStaticFiles(new StaticFileOptions
                {
                    FileProvider = fileProvider,
                    RequestPath = new PathString(string.Empty),
                });

            application
                .UseSwagger(opt =>
                {
                    opt.RouteTemplate = "api/swagger/{documentname}/swagger.json";
                })
                .UseSwaggerUI(opt =>
                {
                    opt.SwaggerEndpoint("v1/swagger.json", "Driver AI API");
                    opt.RoutePrefix = "api/swagger";
                });

            application.UseMvc();
        }
    }
}
