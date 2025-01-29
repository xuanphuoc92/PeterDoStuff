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

        public static Dictionary<string, string> GenerateIconDictionary(this Type iconClassType)
            => iconClassType
            ?.GetFields(System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.Public)
            ?.ToDictionary(fi => fi.Name, fi => fi.GetValue(null)?.ToString() ?? "")
            ?? [];
    }

    public class DocTemplateController
    {
        public class Resume
        {
            public Entity Person { get; set; } = new();

            public string LocationIcon { get; set; } = nameof(GeneralIcons.Location);
            public string LocationLabel { get; set; } = "Location";
            public string Location { get; set; }

            public string PhoneIcon { get; set; } = nameof(GeneralIcons.Phone);
            public string PhoneLabel { get; set; } = "Phone";
            public string PhoneNumber { get; set; }

            public string FluencyIcon { get; set; } = nameof(GeneralIcons.Language);
            public string FluencyLabel { get; set; } = "Fluency";
            public string Fluency { get; set; }

            public string EmailIcon { get; set; } = nameof(GeneralIcons.Email);
            public string EmailLabel { get; set; } = "Email";
            public string Email { get; set; }

            public List<Link> Links { get; set; } = [];

            public string AboutMeIcon { get; set; } = nameof(GeneralIcons.Person);
            public string AboutMeLabel { get; set; } = "About Me";
            public string AboutMe { get; set; }

            public string ExperiencesIcon { get; set; } = nameof(GeneralIcons.Work);
            public string ExperiencesLabel { get; set; } = "Experience";
            public List<Experience> Experiences { get; set; } = [];

            public string EducationsIcon { get; set; } = nameof(GeneralIcons.School);
            public string EducationsLabel { get; set; } = "Education";
            public List<Education> Educations { get; set; } = [];

            public Dictionary<int, string> SkillLevels { get; set; } = new()
            {
                { 1, "Familiar" },
                { 2, "Proficient" },
                { 3, "Skilled" },
            };

            public string SkillsIcon { get; set; } = nameof(GeneralIcons.Build);
            public string SkillsLabel { get; set; } = "Skills";
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
            private static Dictionary<string, string> _iconDict = null;
            public static Dictionary<string, string> GetDictionary()
            {
                _iconDict ??= typeof(LinkIcons).GenerateIconDictionary();
                return _iconDict;
            }

            public const string Default = Icons.Material.Filled.Link;
            public const string LinkedIn = Icons.Custom.Brands.LinkedIn;
            public const string GitHub = Icons.Custom.Brands.GitHub;
            public const string Azure = Icons.Custom.Brands.MicrosoftAzure;
            public const string Instagram = Icons.Custom.Brands.Instagram;
        }

        public class GeneralIcons
        {
            private static Dictionary<string, string> _iconDict = null;
            public static Dictionary<string, string> GetDictionary()
            {
                _iconDict ??= typeof(GeneralIcons).GenerateIconDictionary();
                return _iconDict;
            }

            public const string Location = Icons.Material.Filled.LocationOn;
            public const string Phone = Icons.Material.Filled.Phone;
            public const string WhatsApp = Icons.Custom.Brands.WhatsApp;
            public const string Language = Icons.Material.Filled.Language;
            public const string Email = Icons.Material.Filled.Email;
            public const string Person = Icons.Material.Filled.Person;
            public const string Work = Icons.Material.Filled.Work;
            public const string School = Icons.Material.Filled.School;
            public const string Build = Icons.Material.Filled.Build;
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
