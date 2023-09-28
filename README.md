# BingRewardsFarmer

This project aims to provide a way to farm your daily points on Bing Rewards without having to actually use Bing as your search engine.

With this application, based on a set of parameters that are provided through a settings file, you can simple execute (on your IDE and/or executable file) and the job is done: Microsoft Edge will be executed and perform the necessary searches on Bing for you (Mobile included!).
Microsoft Edge was the go-to option since Bing Rewards provides a few extra points if you are using Edge, so it was a no-brainer. Maybe so, but maybe not: further browsers to be added in the future.

## Prerequisites

.NET 8 SDK - Download it from: https://dotnet.microsoft.com/pt-br/download/dotnet/8.0

Microsoft Edge - Download it from: https://www.microsoft.com/pt-br/edge/download?form=MA13FJ

Edge WebDriver - Download the latest version (or based on your Microsoft Edge version) from: https://developer.microsoft.com/en-us/microsoft-edge/tools/webdriver/

## How to use

Log in with your Microsoft account on rewards.bing.com prior to the first execution. You necessarily have to be authenticated already for this solution.

Update the appsettings.json with your desired settings and you're good to go.

Required properties:

NumberOfIterations: The amount of searches you need to be executed.

NumberOfMobileIterations: The amount of searches you need to be executed on mobile mode.

EdgeDriverLocation": Location of Edge's Web Driver (msedgedriver.exe)

HomePage: The initial page loaded as soon as the application is executed for the first time.

UserDataLocation: The location of your user data from Edge. By default, it will be on: "C:\\Users\\{yourUser}\\AppData\\Local\\Microsoft\\Edge\\User Data"

ProfileDirectory: The folder of the profile to be used. Depending on the amount of profiles you have set up for your browser, it might be "Default", "Profile 1", etc.

ThreadSleepInMiliseconds: This is the amount of time the application will wait until each search is performed on your browser. The recommended value is 2000 (2 seconds).

BingSearchPage: Bing search endpoint, by default: "https://www.bing.com/search?q="

MobileDeviceToEmulate: The device to be emulated on mobile. "Pixel 2" is the recommended device.
