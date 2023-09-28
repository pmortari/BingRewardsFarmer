using BingRewardsFarmer.Resources;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using OpenQA.Selenium.Edge;
using System.Reflection;

namespace BingRewardsFarmer
{
    internal class Program
    {
        private static Settings ConfigurationSettings;

        private static IConfiguration? Config;
        private static ILogger? Log;

        private static void Main(string[] args)
        {
            InitializeProperties();

            ConfigureLogging();

            ValidateConfiguration();

            FarmPoints(SearchType.Regular);

            Thread.Sleep(ConfigurationSettings.ThreadSleepInMiliseconds);

            FarmPoints(SearchType.Mobile);
        }

        private static void FarmPoints(SearchType searchType)
        {
            var driver = SetupDriver(searchType);

            Thread.Sleep(ConfigurationSettings.ThreadSleepInMiliseconds);

            SearchOnBing(driver, searchType);

            DisposeDriver(driver);
        }

        private static void InitializeProperties()
        {
            ConfigurationSettings = new Settings();
        }

        private static void ConfigureLogging()
        {
            using var loggerFactory = LoggerFactory.Create(builder =>
            {
                builder
                    .AddFilter("Microsoft", LogLevel.Warning)
                    .AddFilter("System", LogLevel.Warning)
                    .AddFilter("NonHostConsoleApp.Program", LogLevel.Debug)
                    .AddConsole();
            });

            Log = loggerFactory.CreateLogger<Program>();
        }

        private static void ValidateConfiguration()
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, true);

            Config = builder.Build();

            foreach (var propertyInfo in ConfigurationSettings.GetType()
                            .GetProperties(
                                    BindingFlags.Public
                                    | BindingFlags.Instance))
            {
                ValidateProperty(propertyInfo.Name);
            }
        }

        private static void DisposeDriver(EdgeDriver driver)
        {
            driver.Quit();
            driver.Dispose();
        }

        private static EdgeDriver SetupDriver(SearchType searchType)
        {
            var options = new EdgeOptions();
            options.AddArgument($"--user-data-dir={ConfigurationSettings.UserDataLocation}");
            options.AddArgument($"--profile-directory={ConfigurationSettings.ProfileDirectory}");

            if (searchType == SearchType.Mobile)
            {
                options.EnableMobileEmulation(ConfigurationSettings.MobileDeviceToEmulate);
                options.AddUserProfilePreference("safebrowsing.enabled", true);
                options.AddUserProfilePreference("credentials_enable_service", false);
                options.AddUserProfilePreference("profile.password_manager_enabled", false);
            }

            var driver = new EdgeDriver(ConfigurationSettings.EdgeDriverLocation, options) { Url = ConfigurationSettings.HomePage };

            return driver;
        }

        private static void SearchOnBing(EdgeDriver driver, SearchType searchType)
        {
            var iterations = searchType switch
            {
                SearchType.Regular => ConfigurationSettings.NumberOfIterations,
                SearchType.Mobile => ConfigurationSettings.NumberOfMobileIterations,
                _ => 0,
            };

            for (int i = 0; i < iterations; i++)
            {
                var url = $"{ConfigurationSettings.BingSearchPage}{Guid.NewGuid()}";

                driver.Navigate().GoToUrl(url);
                Thread.Sleep(ConfigurationSettings.ThreadSleepInMiliseconds);
            }
        }

        private static void ValidateProperty(string propertyName)
        {
            Log.LogInformation($"Validating {propertyName}");
            var prop = ConfigurationSettings.GetType().GetProperty(propertyName, BindingFlags.Public | BindingFlags.Instance);

            var providedValue = Convert.ChangeType(Config[propertyName], prop.PropertyType);

            prop.SetValue(ConfigurationSettings, providedValue, null);

            var value = ConfigurationSettings.GetType().GetProperty(propertyName).GetValue(ConfigurationSettings, null);

            if (value == null)
            {
                throw new Exception($"{propertyName} was not provided or has an invalid value that couldn't be validated. Provided value: {providedValue} - Value: {value}");
            }

            Log.LogInformation($"{propertyName}: {value}");
        }
    }
}