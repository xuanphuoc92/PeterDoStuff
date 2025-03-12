using MudBlazor;

namespace PeterDoStuff.MudWasmHosted.Client
{
    public class PlaygroundItem
    {
        public PlaygroundGroup Group;
        public string Link;
        public string Icon;
        public string Title;
        public string Description;

        private PlaygroundItem(PlaygroundGroup group, string link, string icon, string title, string description)
        {
            Group = group;
            Link = link;
            Icon = icon;
            Title = title;
            Description = description;
        }

        public static readonly List<PlaygroundItem> Items = new List<PlaygroundItem>()
        {
            new(PlaygroundGroup.Demos, "authDemo", Icons.Material.Filled.People, "Auth Demo", "This page demonstrates different forms of Authentications."),
            new(PlaygroundGroup.Tools, "myFinance", Icons.Material.Filled.AttachMoney, "My Finance", "This page demonstrates the tools to manage your Finance."),
            new(PlaygroundGroup.Tools, "estimator", Icons.Material.Filled.Balance, "Estimator", "This page demonstrates the tools to support Estimating."),
            new(PlaygroundGroup.Tools, "schemaTools", Icons.Material.Filled.Schema, "Schema Tools", "This page demonstrates the tools for schema designing."),
            new(PlaygroundGroup.Demos, "svg", @Icons.Custom.Uncategorized.Fish, "SVG", "This page demonstrates SVG rendering and animations."),
            new(PlaygroundGroup.Demos, "whatsapp", Icons.Custom.Brands.WhatsApp, "Chat on WhatsApp", "This page demonstrates WhatsApp How to use click to chat."),
            new(PlaygroundGroup.Demos, "smartComponent", Icons.Material.Filled.Lightbulb, "Smart Components", "This page demonstrates of smart components."),
            new(PlaygroundGroup.Tools, "qrCode", Icons.Material.Filled.QrCode, "QR Code", "This page demonstrates the QR code generating and scanning tools."),
            new(PlaygroundGroup.Games, "chaos2048", Icons.Material.Outlined._2k, "Chaos 2048", "This page demonstrates the game 2048 (with chaos animation mode)"),
            new(PlaygroundGroup.Games, "snake", Icons.Material.Outlined.Earbuds, "Snake", "This page demonstrates the game Snake."),
            new(PlaygroundGroup.Games, "lingo", Icons.Material.Filled.FontDownload, "Lingo (not Wordle)", "This page demonstrates the game Lingo (not Wordle)."),
            new(PlaygroundGroup.Demos, "workflow", Icons.Material.Filled.AccountTree, "Workflow", "This page demonstrates the workflow presentation in waterfall model (linear vertical in mobile view)."),
            new(PlaygroundGroup.Demos, "animationGallery", Icons.Material.Filled.AutoFixHigh, "Animation Gallery", "This page demonstrates the web animations."),
            new(PlaygroundGroup.Demos, "dashboard", Icons.Material.Filled.InsertChartOutlined, "Dashboard", "This page showcases presenting data into different presentations."),
            new(PlaygroundGroup.Tools, "cryptography", Icons.Material.Filled.Security, "Cryptography", "This page provides some well known Cryptography services."),
            new(PlaygroundGroup.Demos, "mobile", Icons.Material.Filled.Smartphone, "As if a Mobile App", "This page demonstrates how a web page can perform mobile app features like opening Camera or GPS."),
            new(PlaygroundGroup.Tools, "database", Icons.Material.Filled.Storage, "Database", "This page demonstrates memory database of the server."),
            new(PlaygroundGroup.Games, "gameOfLife", Icons.Material.Filled.CrueltyFree, "Conway's Game of Life", "This is playground for Conway's Game of Life. Limitted size: 30x30."),
            new(PlaygroundGroup.Games, "mineSweeper", Icons.Material.Filled.BrightnessHigh, "Minesweeper", "Just a basic minesweeper game."),
            new(PlaygroundGroup.Games, "matchFinder", Icons.Material.Filled.Search, "Match Finder", "Just a basic match finder game."),
            new(PlaygroundGroup.Tools, "fileScanner", Icons.Material.Filled.InsertDriveFile, "File Scanner", "Select your zip file to scan for statistics."),
            new(PlaygroundGroup.Demos, "docTemplate", @Icons.Custom.FileFormats.FileImage, "Template", "This page demonstrates template rendering."),
        };
    }

    public enum PlaygroundGroup
    {
        Tools, Demos, Games
    }

    public static class PlaygroundExtensions
    {
        public static string GetIcon(this PlaygroundGroup group)
        {
            return group switch
            {
                PlaygroundGroup.Tools => Icons.Material.Filled.BuildCircle,
                PlaygroundGroup.Demos => Icons.Material.Filled.StarPurple500,
                PlaygroundGroup.Games => Icons.Material.Filled.BedroomBaby,
                _ => Icons.Material.Filled.QuestionMark
            };
        }

        public static string GetColor(this PlaygroundGroup group)
        {
            return group switch
            {
                PlaygroundGroup.Tools => Colors.DeepPurple.Default,
                PlaygroundGroup.Demos => Colors.Teal.Default,
                PlaygroundGroup.Games => Colors.Amber.Darken4,
                _ => Colors.Pink.Default,
            };
        }
    }
}
