using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Runtime.Caching;
using System.Threading.Tasks;
using TinyUrlService.BL;
using TinyUrlService.Repository;
using TinyUrlService.Utils;

namespace TinyUrlService.Configuration
{
    public static class StartupExtensionMethods
    {
        public static void AddSingletons(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddSingleton<IShortUriDatabaseSettings>(sp =>
               sp.GetRequiredService<IOptions<ShortUriDatabaseSettings>>().Value);

            services.AddSingleton<ITinyUrlBL, TinyUrlBL>();
            services.AddSingleton<IUrlShorterner, UrlShorterner>();
            services.AddSingleton<IUrlKeyRepository, UrlKeyRepository>();
            AddCache(services, configuration);
        }

        public static void AddConfiguration(this IServiceCollection services,
            IConfiguration configuration)
        {
            services.Configure<ShortUriDatabaseSettings>(
                configuration.GetSection(nameof(ShortUriDatabaseSettings)));
        }

        private static void AddCache(IServiceCollection services,
            IConfiguration configuration)
        {
            var config = new NameValueCollection();
            config.Add("pollingInterval", configuration["Cache:pollingInterval"]);
            config.Add("physicalMemoryLimitPercentage",
                configuration["Cache:physicalMemoryLimitPercentage"]);
            config.Add("cacheMemoryLimitMegabytes", configuration["Cache:cacheMemoryLimitMegabytes"]);

            var memoryCache = new MemoryCache("TinyUrlCache", config);

            services.AddSingleton<MemoryCache>(memoryCache);
        }
    }
}
