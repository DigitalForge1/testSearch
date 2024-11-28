using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace alphaData
{
    public class Startup
    {
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc(option => option.EnableEndpointRouting = false);

            services.AddRazorPages()
                .AddRazorRuntimeCompilation();

            services.AddLogging(loggingBuilder =>
                 {
                     loggingBuilder.AddFile("log\\app_{0:yyyy}-{0:MM}-{0:dd}.log", fileLoggerOpts =>
                     {
                         fileLoggerOpts.FormatLogFileName = fName =>
                         {
                             return String.Format(fName, DateTime.UtcNow);
                         };
                         fileLoggerOpts.FileSizeLimitBytes = (long)(50 * Math.Pow(10, 6)); // ~50mb
                         fileLoggerOpts.MaxRollingFiles = 10;
                     });
                 });
        
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseStaticFiles();
            app.UseRouting();
            app.UseStatusCodePagesWithReExecute("/Error", "?statusCode={0}");

            // Add MVC to the request pipeline.
            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Statistics}/{action=Journal}/{id?}");
            });
        }
    }
}
