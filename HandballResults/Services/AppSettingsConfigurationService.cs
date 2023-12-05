using System.Configuration;
using HandballResults.Models;

namespace HandballResults.Services
{
    public class AppSettingsConfigurationService : IConfigurationService
    {
        public const string AppSettingsKeyName = "HandballResultsConfig";

        private readonly IConfiguration configuration;

        public AppSettingsConfigurationService(IConfiguration configuration)
        {
            this.configuration = configuration;
        }

        public Configuration Get()
        {
            var appConfiguration = new Configuration();
            configuration.GetSection(AppSettingsKeyName).Bind(appConfiguration);

            return appConfiguration;
        }
    }
}