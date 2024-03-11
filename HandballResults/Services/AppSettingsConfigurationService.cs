using System.Configuration;
using HandballResults.Models;

namespace HandballResults.Services
{
    public class AppSettingsConfigurationService : IConfigurationService
    {
        public const string AppSettingsKeyName = "HandballResults";
        public const string ShvApiKeyEnvironmentVariable = "SHV_API_KEY";

        private readonly IConfiguration configuration;
        private readonly ILogger<AppSettingsConfigurationService> logger;

        public AppSettingsConfigurationService(ILogger<AppSettingsConfigurationService> logger, IConfiguration configuration)
        {
            this.logger = logger;
            this.configuration = configuration;
        }

        public Configuration Get()
        {
            var appConfiguration = new Configuration();
            configuration.GetSection(AppSettingsKeyName).Bind(appConfiguration);

            var shvApiKey = configuration.GetValue<string>(ShvApiKeyEnvironmentVariable);
            if (string.IsNullOrWhiteSpace(shvApiKey))
            {
                logger.LogCritical("Ensure that a valid SHV API Key is set in an Environment Variable named '{0}'",
                    ShvApiKeyEnvironmentVariable);
                throw new InvalidOperationException("SHV API Key is missing");
            }
            else
            {
                appConfiguration.ShvApiKey = shvApiKey;
            }


            return appConfiguration;
        }
    }
}