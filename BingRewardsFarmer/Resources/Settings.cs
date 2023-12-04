namespace BingRewardsFarmer.Resources
{
    internal class Settings
    {
        public int NumberOfIterations { get; set; }
        public int NumberOfMobileIterations { get; set; }
        public string EdgeDriverLocation { get; set; }
        public string HomePage { get; set; }
        public string ProfileDirectory { get; set; }
        public string UserDataLocation { get; set; }
        public int ThreadSleepInMilliseconds { get; set; }
        public int SearchesPerCooldown { get; set; }
        public int CooldownPeriodInMilliseconds { get; set; }
        public string BingSearchPage { get; set; }
        public string MobileDeviceToEmulate { get; set; }
    }
}