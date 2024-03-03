using MudBlazor;

namespace PeterDoStuff.MudWasmHosted.Client
{
    public class PlaygroundItem
    {
        public string Link;
        public string Icon;
        public string Title;
        public string Description;

        private PlaygroundItem(string link, string icon, string title, string description)
        {
            Link = link;
            Icon = icon;
            Title = title;
            Description = description;
        }

        public static readonly List<PlaygroundItem> Items = new List<PlaygroundItem>()
        {
            new("security", Icons.Material.Filled.Security, "Security", "This page demonstrates the basic security functions."),
            new("mobile", Icons.Material.Filled.Smartphone, "As if a Mobile App", "This component demonstrates how a web page can perform mobile app features like opening Camera or GPS."),
            new("database", Icons.Material.Filled.Storage, "Database", "This component demonstrates memory database of the server."),
            new("gameOfLife", Icons.Material.Filled.CrueltyFree, "Conway's Game of Life", "This is playground for Conway's Game of Life. Limitted size: 30x30."),
            new("mineSweeper", Icons.Material.Filled.BrightnessHigh, "Minesweeper", "Just a basic minesweeper game."),
            new("matchFinder", Icons.Material.Filled.Search, "Match Finder", "Just a basic match finder game."),
            new("fileScanner", Icons.Material.Filled.InsertDriveFile, "File Scanner", "Select your zip file to scan for statistics.")
        };
    }
}
