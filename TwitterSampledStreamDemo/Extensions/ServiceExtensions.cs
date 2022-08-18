using StackExchange.Redis;
using TwitterService.API.Configuration;
using TwitterService.API.Repository;
using TwitterService.API.Services;
using TwitterService.Contracts;
using TwitterService.LoggerService;

namespace TweeterSampledStreamDemo.Extensions
{
    public static class ServiceExtensions
    {
        //configure CORS
        public static void ConfigureCors(this IServiceCollection services) =>
            services.AddCors(options =>
            {
                options.AddPolicy("CorsPolicy", builder =>
                    builder.AllowAnyOrigin()
                    .AllowAnyMethod()
                    .AllowAnyHeader());
            });

        //configure IIS
        public static void ConfigureIISIntegration(this IServiceCollection services) =>
            services.Configure<IISOptions>(options =>
            {
                //use default values
            });

        public static void ConfigureLoggerService(this IServiceCollection services) =>
            services.AddSingleton<ILoggerManager, LoggerManager>();

        public static void ConfigureTwitterApiServices(this IServiceCollection services, IConfigurationSection twitterApiConfigurationSection)
        {
            services.Configure<TwitterApiConfiguration>(twitterApiConfigurationSection);
            services.AddSingleton<ITwitterApiAuthService, TwitterApiAuthService>();
            services.AddSingleton<ITwitterApiTweetService, TwitterApiTweetService>();
            services.AddSingleton<ITweetReportService, TweetReportService>();
        }

        //configure Redis
        public static void ConfigureRedisDB(this IServiceCollection services, IConfiguration configuration) =>
            services.AddSingleton<IConnectionMultiplexer>(opt =>
                ConnectionMultiplexer.Connect(configuration.GetConnectionString("RedisConnection"))
            );

        public static void ConfigureRedisTweetRepo(this IServiceCollection services) =>
            services.AddSingleton<ITweetRepo, RedisTweetRepo>();

        public static void ConfigureSwagger(this IServiceCollection services)
        {
            services.AddSwaggerGen(s =>
            {
                s.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo { Title = "Twitter Service API", Version = "v1" });
                s.SwaggerDoc("v2", new Microsoft.OpenApi.Models.OpenApiInfo { Title = "Twitter Service API", Version = "v2" });
            });
        }
    }
}
