using MudBlazor;
using PeterDoStuff.Extensions;
using System.Text.RegularExpressions;

namespace PeterDoStuff.MudWasmHosted.Client.Pages.DocTemplate
{
    internal static class TemplateExtensions
    {
        public static string[] ToParagraphs(this string input)
            => input?.Split("\n").Select(p => p.Trim()).Where(p => p.IsNullOrEmpty() == false).ToArray() ?? [];

        public static string TextOrUrl(this DocTemplateController.Link link)
            => link.Text.IsNullOrEmpty() ? link.Url : link.Text;

        public static string MonthYearOrPresent(this DateTime? dateTime)
            => dateTime?.ToString("MMM yyyy") ?? "present";

        public static string PhotoSrc(this DocTemplateController.Entity entity)
            => entity.PhotoType == DocTemplateController.PhotoType.Base64
            ? $"data:{entity.PhotoImageType};base64,{entity.PhotoBase64}"
            : entity.PhotoUrl;

        public static string? ToOneLine(this string input)
            => input?.Replace("\n", " ").Replace("\r", "");

        public static string RenderWithBold(this string input)
        {
            if (string.IsNullOrEmpty(input))
                return input;

            // Replace **text** with <strong>text</strong>
            var safeInput = Regex.Replace(input, @"\*\*(.+?)\*\*", "<strong>$1</strong>");

            // Encode other HTML to prevent XSS
            return System.Net.WebUtility.HtmlEncode(safeInput)
                .Replace("&lt;strong&gt;", "<strong>")
                .Replace("&lt;/strong&gt;", "</strong>");
        }
    }

    public class DocTemplateController
    {
        public class Resume
        {
            public Entity Person { get; set; } = new();

            public string Location { get; set; }

            public string PhoneLabel { get; set; } = "Phone";
            public string PhoneNumber { get; set; }

            public string FluencyLabel { get; set; } = "Fluency";
            public string Fluency { get; set; }
            
            public string Email { get; set; }

            public Link LinkedIn { get; set; } = new() { Label = nameof(LinkedIn) };
            public Link GitHub { get; set; } = new() { Label = nameof(GitHub) };

            public List<Link> OtherLinks { get; set; } = [];

            public string AboutMe { get; set; }

            public List<Experience> Experiences { get; set; } = [];

            public List<Education> Educations { get; set; } = [];

            public Dictionary<int, string> SkillLevels { get; set; } = new()
            {
                { 1, "Familiar" },
                { 2, "Proficient" },
                { 3, "Skilled" },
                //{ 4, "Expert" },
                //{ 5, "Master" },
            };

            public List<Skill> Skills { get; set; } = [];
        }

        public enum PhotoType
        {
            None, Url, Base64
        }

        public class Link
        {
            public string Label { get; set; }
            public string Url { get; set; }
            public string Text { get; set; }
            public string Icon { get; set; } = nameof(LinkIcons.Default);
        }

        public class LinkIcons
        {
            public const string Default = Icons.Material.Filled.Link;
            public const string LinkedIn = Icons.Custom.Brands.LinkedIn;
            public const string GitHub = Icons.Custom.Brands.GitHub;
            public const string Azure = Icons.Custom.Brands.MicrosoftAzure;
            public const string Instagram = Icons.Custom.Brands.Instagram;
        }

        public class Experience
        {
            public Entity Company { get; set; } = new();
            public List<ExperienceItem> Items { get; set; } = [];
        }

        public class ExperienceItem
        {
            public string Position { get; set; }
            public DateTime? From { get; set; }
            public DateTime? To { get; set; }
            public string Description { get; set; }
        }

        public class Education
        {
            public Entity Institution { get; set; } = new();
            public string Degree { get; set; }
            public string Description { get; set; }
            public DateTime? From { get; set; }
            public DateTime? To { get; set; }
        }

        public class Entity
        {
            public string Name { get; set; }

            public PhotoType PhotoType { get; set; }
            public string PhotoUrl { get; set; }
            public string PhotoImageType { get; set; } = "image/jpeg";
            public string PhotoBase64 { get; set; }
        }

        public class Skill
        {
            public string Group { get; set; }
            public string Name { get; set; }
            public int Level { get; set; } = 3;
        }
    }
}
