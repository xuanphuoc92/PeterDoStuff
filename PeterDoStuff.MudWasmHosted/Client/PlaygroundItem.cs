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
            new("chaos2048", Icons.Material.Outlined._2k, "Chaos 2048", "This page demonstrates the game 2048 (with chaos animation mode)"),
            new("lingo", Icons.Material.Filled.FontDownload, "Lingo (not Wordle)", "This page demonstrates the game Lingo (not Wordle)."),
            new("workflow", Icons.Material.Filled.AccountTree, "Workflow", "This page demonstrates the workflow presentation in waterfall model."),
            new("animationGallery", Icons.Material.Filled.AutoFixHigh, "Animation Gallery", "This page demonstrates the web animations."),
            new("dashboard", Icons.Material.Filled.InsertChartOutlined, "Dashboard", "This page showcases presenting data into different presentations."),
            new("cryptography", Icons.Material.Filled.Security, "Cryptography", "This page provides some well known Cryptography services."),
            new("mobile", Icons.Material.Filled.Smartphone, "As if a Mobile App", "This page demonstrates how a web page can perform mobile app features like opening Camera or GPS."),
            new("database", Icons.Material.Filled.Storage, "Database", "This page demonstrates memory database of the server."),
            new("gameOfLife", Icons.Material.Filled.CrueltyFree, "Conway's Game of Life", "This is playground for Conway's Game of Life. Limitted size: 30x30."),
            new("mineSweeper", Icons.Material.Filled.BrightnessHigh, "Minesweeper", "Just a basic minesweeper game."),
            new("matchFinder", Icons.Material.Filled.Search, "Match Finder", "Just a basic match finder game."),
            new("fileScanner", Icons.Material.Filled.InsertDriveFile, "File Scanner", "Select your zip file to scan for statistics.")
        };
    }
}
