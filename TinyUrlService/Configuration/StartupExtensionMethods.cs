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
        public static void AddSingletons(this IServiceCollection services)
        {
            services.AddSingleton<IShortUriDatabaseSettings>(sp =>
               sp.GetRequiredService<IOptions<ShortUriDatabaseSettings>>().Value);

            services.AddSingleton<ITinyUrlBL, TinyUrlBL>();
            services.AddSingleton<IUrlShorterner, UrlShorterner>();
            services.AddSingleton<IUrlKeyRepository, UrlKeyRepository>();
            AddCache(services);
        }

        public static void AddConfiguration(this IServiceCollection services,
            IConfiguration configuration)
        {
            services.Configure<ShortUriDatabaseSettings>(
                configuration.GetSection(nameof(ShortUriDatabaseSettings)));
        }

        private static void AddCache(IServiceCollection services)
        {
            var config = new NameValueCollection();
            config.Add("pollingInterval", "00:05:00");
            config.Add("physicalMemoryLimitPercentage", "0");
            config.Add("cacheMemoryLimitMegabytes", "10");

            var memoryCache = new MemoryCache("TinyUrlCache", config);

            services.AddSingleton<MemoryCache>(memoryCache);
        }
    }
}
