# Peter Do Stuff

## Quick Start
Access the published app at [https://peter-do-stuff.azurewebsites.net/](https://peter-do-stuff.azurewebsites.net/).

Otherwise, feel free to download, fork, or clone the project and play with it. You should be able to open and run the `PeterDoStuff.MudWasm` with Visual Studio's IIS Express. If things do not go as easy as it seems for you, feel free to reach out to me via Issues or my [LinkedIn](https://www.linkedin.com/in/peter-vo-43bb1337/).

Please feel free to read the source code of existing app/games, and maybe create your own apps/games.

## What you will find in this project:
You will find 3 main projects in this project repository:

### PeterDoStuff
This is the core project that stores the features/games/apps. At the moment, I add 3 apps/games into it. However, there can be more in the future.

It is simply a .NET Standard 2.1 Class Library project. 

Due to the importance of it, I maximize its quality by making it as minimal as possible. And how do I do that? That's from the 2nd project:

### PeterDoStuff.Test
This is the test project that contains test suite covering the features in `PeterDoStuff`.

The test suite in this project covers 100% in both line coverage and branch coverage in the 1st project `PeterDoStuff`.

Yes, **100%**, not >90% or >80%. This is because it is harder to pick out 1 line of not-covered, which may contain critical defects, from many lines that are already compromised. Furthermore, `PeterDoStuff` is a class library project, not UI or Front project, so it is considered an easy type to fully-cover.

If you would like to observe the coverage but do not know which tool to do so, my recommendation is **Fine Code Coverage** extension in Visual Studio Community edition (if I recall correctly, Enterprise editon already contains built-in coverage monitoring).

### PeterDoStuff.MudWasm
This is the Front-end project to wire everything from `PeterDoStuff` to the users. 

It is a Blazor Web Assembly application, which is the type of application that is not mainly run on the web server. Instead, it will be downloaded and cached in user browser, so a user first time accessing the app may take a little longer time to download around 5 MB app size (may increase in the future, but not significantly), and subsequently just using the version cached in the browser if there is no update.

You may ask:
> **"Why did you not use server side web application? Like Blazor Server Web Application or .NET Framework?"**

The reason is I am using Free-tier subscription of Azure Web Service to host and publish this web application, which at this time (Jan-2024) only have:

- 1 Region from a limited pool (e.g. East US)
- 60 CPU minutes/day
- 1 GB RAM
- 1 GB Storage

With only 1 Region, this would mean my user experience with the app would differ if my app is 100% server side: Those accessing the app from East US will find it faster than those in Southeast Asia. This is not the problem for Web Assembly Apps wherever region they are hosted.

Furthermore, if the app is server side, the 60 CPU minutes/day and 1 GB RAM will be quickly used up, proportionately with the number of concurrent users. If the app is client side like Web Assembly, CPU and RAM usage will be distributed to users' devices.

In this case, you can say Azure Web Service acts and uses its resource as the app distributer, just like Google Play or Apple Store (except instead of mobile apps, it is for web apps here). With such a role, 1 GB storage is still more than enough.

Of course, server side web applications can still be deployed with Azure Web Service under Free Tier. I have tried out and observed that the app in that case is still decent, but the moment I switched to Web Assembly, the experience changes significantly, as if 10 times faster and smoother.

That being said, I may change the Front-end App to be Hybrid Web Assembly in the future, i.e. making the server side holding some other roles (e.g. cryptography, or virtual database which should not exceed 1 GB storage) besides the current distribution role.

The current project is named `PeterDoStuff.MudWasm` because it is making use of the [MudBlazor](https://www.mudblazor.com/). The MudBlazor is well implemented, documented, and tested .NET Blazor UI component framework. They are also the framework that helps developers avoid tinkering around JavaScript (which is flexible but also volatile and unpredictable) in order for the components to work.
