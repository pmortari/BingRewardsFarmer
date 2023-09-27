using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using OpenQA.Selenium.Edge;

namespace BingRewardsFarmer
{
    internal class Program
    {
        private static int? NumberOfIterations;
        private static int? NumberOfMobileIterations;
        private static string? EdgeDriverLocation;
        private static string? HomePage;
        private static string? ProfileDirectory;
        private static string? UserDataLocation;
        private static int ThreadSleepInMiliseconds;
        private static string? BingSearchPage;
        private static string? MobileDeviceToEmulate;
        private static IConfiguration? Config;
        private static ILogger? Log;

        private static void Main(string[] args)
        {
            ConfiguraLogging();

            ValidateConfiguration();

            FarmRegularPoints();

            Thread.Sleep(ThreadSleepInMiliseconds);

            FarmMobilePoints();
        }

        private static void ConfiguraLogging()
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

        private static void FarmRegularPoints()
        {
            var driver = SetupDriver(SearchType.Regular);

            Thread.Sleep(ThreadSleepInMiliseconds);

            SearchOnBing(driver, NumberOfIterations);

            CloseDriver(driver);
        }

        private static void FarmMobilePoints()
        {
            var driver = SetupDriver(SearchType.Mobile);

            Thread.Sleep(ThreadSleepInMiliseconds);

            SearchOnBing(driver, NumberOfMobileIterations);

            CloseDriver(driver);
        }

        private static void CloseDriver(EdgeDriver driver)
        {
            driver.Quit();
            driver.Dispose();
        }

        private static EdgeDriver SetupDriver(SearchType searchType)
        {
            var options = new EdgeOptions();
            options.AddArgument($"--user-data-dir={UserDataLocation}");
            options.AddArgument($"--profile-directory={ProfileDirectory}");

            if (searchType == SearchType.Mobile)
            {
                options.EnableMobileEmulation(MobileDeviceToEmulate);
                options.AddUserProfilePreference("safebrowsing.enabled", true);
                options.AddUserProfilePreference("credentials_enable_service", false);
                options.AddUserProfilePreference("profile.password_manager_enabled", false);
            }

            var driver = new EdgeDriver(EdgeDriverLocation, options) { Url = HomePage };

            return driver;
        }

        private static void SearchOnBing(EdgeDriver driver, int? iterations)
        {
            for (int i = 0; i < iterations; i++)
            {
                var url = $"{BingSearchPage}{Guid.NewGuid()}";

                driver.Navigate().GoToUrl(url);
                Thread.Sleep(ThreadSleepInMiliseconds);
            }
        }

        //TODO: Still need proper implementation
        private static void ValidateConfiguration()
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, true);

            Config = builder.Build();

            try
            {
                Log.LogInformation("Validating Number of Iterations");
                NumberOfIterations = Convert.ToInt16(Config["NumberOfIterations"]);
                Log.LogInformation($"Number of Iterations: {NumberOfIterations}");

                Log.LogInformation("Validating Number of Mobile Iterations");
                NumberOfMobileIterations = Convert.ToInt16(Config["NumberOfMobileIterations"]);
                Log.LogInformation($"Number of Mobile Iterations: {NumberOfMobileIterations}");

                Log.LogInformation("Validating Edge Driver Location");
                EdgeDriverLocation = Config["EdgeDriverLocation"];
                Log.LogInformation($"Edge Driver Location: {EdgeDriverLocation}");

                Log.LogInformation("Validating Home Page");
                HomePage = Config["HomePage"];
                Log.LogInformation($"Home Page: {HomePage}");

                Log.LogInformation("Validating User Data Location");
                UserDataLocation = Config["UserDataLocation"];
                Log.LogInformation($"User Data Location: {UserDataLocation}");

                Log.LogInformation("Validating Profile Directory");
                ProfileDirectory = Config["ProfileDirectory"];
                Log.LogInformation($"Profile Directory: {ProfileDirectory}");

                Log.LogInformation("Validating Thread Sleep In Miliseconds");
                ThreadSleepInMiliseconds = Convert.ToInt32(Config["ThreadSleepInMiliseconds"]);
                Log.LogInformation($"Thread Sleep In Miliseconds: {ThreadSleepInMiliseconds}");

                Log.LogInformation("Validating Bing Search Page");
                BingSearchPage = Config["BingSearchPage"];
                Log.LogInformation($"Bing Search Page: {BingSearchPage}");

                Log.LogInformation("Validating Mobile Device to Emulate");
                MobileDeviceToEmulate = Config["MobileDeviceToEmulate"];
                Log.LogInformation($"Mobile Device to Emulate: {MobileDeviceToEmulate}");
            }
            catch (Exception ex)
            {
                Log.LogInformation("One or more parameters were not provided or are invalid. Please validate your appsettings.json file.", ex);
            }
        }

        private enum SearchType
        {
            Regular = 0,
            Mobile = 1
        }
    }
}