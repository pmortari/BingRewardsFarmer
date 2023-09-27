using Microsoft.Extensions.Configuration;
using OpenQA.Selenium.Edge;

namespace BingRewardsFarmer
{
    internal class Program
    {
        private static int NumberOfIterations;
        private static int NumberOfMobileIterations;        
        private static string EdgeDriverLocation;
        private static string HomePage;
        private static string ProfileDirectory;
        private static string UserDataLocation;
        private static int ThreadSleepInMiliseconds;

        static void Main(string[] args)
        {
            ValidateConfiguration();

            var options = new EdgeOptions();
            options.AddArgument($"--user-data-dir={UserDataLocation}");
            options.AddArgument($"--profile-directory={ProfileDirectory}");

            var driver = new EdgeDriver(EdgeDriverLocation, options) { Url = HomePage };

            Thread.Sleep(ThreadSleepInMiliseconds);

            for (int i = 0; i < NumberOfIterations; i++)
            {
                driver.Navigate().GoToUrl($"https://www.bing.com/search?q={Guid.NewGuid()}");
                Thread.Sleep(ThreadSleepInMiliseconds);
            }

            driver.Quit();

            Thread.Sleep(ThreadSleepInMiliseconds);

            options = new EdgeOptions();
            options.AddArgument($"--user-data-dir={UserDataLocation}");
            options.AddArgument($"--profile-directory={ProfileDirectory}");
            options.EnableMobileEmulation("Pixel 2");
            options.AddUserProfilePreference("safebrowsing.enabled", true);
            options.AddUserProfilePreference("credentials_enable_service", false);
            options.AddUserProfilePreference("profile.password_manager_enabled", false);

            driver = new EdgeDriver(EdgeDriverLocation, options) { Url = HomePage };

            Thread.Sleep(ThreadSleepInMiliseconds);

            for (int i = 0; i < NumberOfMobileIterations; i++)
            {
                driver.Navigate().GoToUrl($"https://www.bing.com/search?q={Guid.NewGuid()}");
                Thread.Sleep(ThreadSleepInMiliseconds);
            }

            driver.Quit();
        }

        private static void ValidateConfiguration()
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, true);

            IConfiguration config = builder.Build();

            try
            {
                NumberOfIterations = Convert.ToInt16(config["NumberOfIterations"]);
                NumberOfMobileIterations = Convert.ToInt16(config["NumberOfMobileIterations"]);
                EdgeDriverLocation = config["EdgeDriverLocation"];
                HomePage = config["HomePage"];
                UserDataLocation = config["UserDataLocation"];
                ProfileDirectory = config["ProfileDirectory"];
                ThreadSleepInMiliseconds = Convert.ToInt32(config["ThreadSleepInMiliseconds"]);
            }
            catch (Exception ex)
            {
                Console.WriteLine("One or more parameters were not provided or are invalid. Please validate your appsettings.json file.", ex);
            }
        }
    }
}