using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Runtime.Caching;
using System.Threading.Tasks;
using TinyUrlService.BL;
using TinyUrlService.Configuration;
using TinyUrlService.Repository;
using TinyUrlService.Utils;

namespace TinyUrlService
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
            // requires using Microsoft.Extensions.Options
            services.Configure<ShortUriDatabaseSettings>(
                Configuration.GetSection(nameof(ShortUriDatabaseSettings)));

            services.AddSingleton<IShortUriDatabaseSettings>(sp =>
                sp.GetRequiredService<IOptions<ShortUriDatabaseSettings>>().Value);

            services.AddSingleton<ITinyUrlBL, TinyUrlBL>();
            services.AddSingleton<IUrlShorterner, UrlShorterner>();
            services.AddSingleton<IUrlKeyRepository, UrlKeyRepository>();
            /*services.AddDistributedMemoryCache(setup =>
            {
                setup.SizeLimit = 1024;
            });*/

            var config = new NameValueCollection();
            config.Add("pollingInterval", "00:05:00");
            config.Add("physicalMemoryLimitPercentage", "0");
            config.Add("cacheMemoryLimitMegabytes", "10");

            var memoryCache = new MemoryCache("TinyUrlCache", config);

            services.AddSingleton<MemoryCache>(memoryCache);

            services.AddControllers();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "TinyUrlService", Version = "v1" });
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "TinyUrlService v1"));
            }

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}