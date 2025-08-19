using MudBlazor;
using PeterDoStuff.Extensions;
using System.Text.RegularExpressions;

namespace PeterDoStuff.MudWasmHosted.Client.Pages.DocTemplate
{
    internal static class TemplateExtensions
    {
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

            public string ProjectIcon { get; set; } = nameof(GeneralIcons.Shapes);
            public string ProjectLabel { get; set; } = "Projects";
            public List<Project> Projects { get; set; } = [];

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

        public static Resume VietnameseProfile()
        {
            return new()
            {
                AboutMeLabel = "Sơ lược bản thân",
                EducationsLabel = "Học vấn",
                ExperiencesLabel = "Kinh nghiệm",
                FluencyLabel = "Thông thạo",
                LocationLabel = "Địa điểm",
                PhoneLabel = "Điện thoại",
                SkillsLabel = "Kỹ năng",
                SkillLevels = new()
                {
                    { 1, "Hiểu biết" },
                    { 2, "Thành thạo" },
                    { 3, "Chuyên sâu" },
                }
            };
        }

        public static Resume SampleProfile()
        {
            Resume result = new();
            result.Person.Name = "John Doe";
            result.Person.PhotoType = PhotoType.Url;
            result.Person.PhotoUrl = "https://raw.githubusercontent.com/xuanphuoc92/SamplePhotos/refs/heads/main/SampleEuropeanMan.webp";

            result.Location = "New York, United States";
            result.PhoneNumber = "1234567890";
            result.Fluency = "English";
            result.Email = "JohnDoe_Sample@gmail.com";
            result.Links = [
                new()
                {
                    Icon = nameof(LinkIcons.LinkedIn),
                    Label = "LinkedIn",
                    Text = "in/test",
                },
            ];

            result.AboutMe = @"Results-driven Project Manager with **10 years of experience** in Procurement.
Passionate about creating innovative solutions and driving business growth.
Adept at **Data Analysis, Team Leadership**, and **Problem-solving**.
Committed to delivering high-quality work and continuously learning to stay ahead in the industry.";

            result.Educations = [
                new() 
                {
                    Institution = {
                        Name = "Kent State University",
                        PhotoType = PhotoType.Url,
                        PhotoUrl = "https://media.licdn.com/dms/image/v2/C4D0BAQERnKQo-ZgbSA/company-logo_200_200/company-logo_200_200/0/1631304074952?e=1747267200&v=beta&t=iv7znOIDwWff2B5w7I9m2bMh_foIYTdFHhBKXfo_N7A"
                    },
                    Degree = "Bachelor, Marketing",
                    From = new DateTime(2020, 06, 01),
                    To = new DateTime(2024, 06, 30)
                }
            ];

            result.Experiences = [
                new() 
                {
                    Company = {
                        Name = "Google",
                        PhotoType = PhotoType.Url,
                        PhotoUrl = "https://media.licdn.com/dms/image/v2/C4D0BAQHiNSL4Or29cg/company-logo_200_200/company-logo_200_200/0/1631311446380?e=1747267200&v=beta&t=vU6juPS-AVsn9MDUlF0vGmmRbPh0ubxT_u-g_1hfx6c"
                    },
                    Items = [
                        new()
                        {
                            Position = "Media and Communication",
                            From = new DateTime(2025,01,01),
                            Description = @"Passionate about crafting compelling narratives that engage, inform, and inspire.
With expertise in **Media Relations, Content Creation**, and **Strategic Communication**, I help brands and organizations build strong connections with their audiences.
Whether it’s crafting press releases, managing social media campaigns, or enhancing brand identity, I thrive on creating messages that resonate."
                        },
                        new()
                        {
                            Position = "Marketing",
                            From = new DateTime(2024,06,01),
                            To = new DateTime(2024,12,31),
                            Description = @"Driven by creativity and data, I specialize in crafting marketing strategies that captivate audiences, drive engagement, and accelerate business growth.
With a deep understanding of **Consumer Behavior** and **Market Trends**, I develop innovative campaigns that make an impact.
Whether it’s launching campaigns, optimizing digital channels, or building brand loyalty, I’m passionate about helping businesses grow."
                        }
                    ]
                }
            ];

            result.Skills = [
                new() {
                    Group = "Marketing",
                    Name = "Data Analysis",
                    Level = 3,
                },
                new() {
                    Group = "Marketing",
                    Name = "Customer Engagement",
                    Level = 2,
                },
                new() {
                    Group = "Management",
                    Name = "Project Management, Strategic Planning",
                    Level = 3,
                },
                new() {
                    Group = "Management",
                    Name = "Financial Acumen",
                    Level = 2,
                },
            ];

            return result;
        }

        public static Resume SampleVietnameseProfile()
        {
            Resume result = VietnameseProfile();
            result.Person.Name = "Nguyễn Văn A";
            result.Person.PhotoType = PhotoType.Url;
            result.Person.PhotoUrl = "https://raw.githubusercontent.com/xuanphuoc92/SamplePhotos/refs/heads/main/SampleAsianMan.webp";

            result.Location = "Việt Nam";
            result.PhoneNumber = "1234567890";
            result.Fluency = "Việt, Hoa";
            result.Email = "nguyen_van_a_Testing@gmail.com";
            result.Links = [
                new()
                {
                    Icon = nameof(LinkIcons.LinkedIn),
                    Label = "LinkedIn",
                    Text = "in/test",
                },
            ];

            result.AboutMe = @"Chuyên viên Marketing với **10 năm kinh nghiệm** trong lĩnh vực Mua Sắm.
Đam mê tạo ra các giải pháp sáng tạo và thúc đẩy tăng trưởng kinh doanh.
Thành thạo **Phân tích dữ liệu, Lãnh đạo nhóm**, và **Giải quyết vấn đề**.
Cam kết mang lại hiệu quả cao trong công việc và không ngừng học hỏi để phát triển bản thân.";

            result.Educations = [
                new()
                {
                    Institution = {
                        Name = "Đại học Ngoại Thương, Tp.HCM",
                        PhotoType = PhotoType.Url,
                        PhotoUrl = "https://media.licdn.com/dms/image/v2/C560BAQFrfd0C2eZg3w/company-logo_200_200/company-logo_200_200/0/1659501664684/foreigntradeuniversity_logo?e=1747267200&v=beta&t=EKDlMtJ2ew8b2vFB_B-O6uOcAZM-2Mki9HFMgtt3J6Y"
                    },
                    Degree = "Bachelor, Marketing",
                    From = new DateTime(2020, 06, 01),
                    To = new DateTime(2024, 06, 30)
                }
            ];

            result.Experiences = [
                new()
                {
                    Company = {
                        Name = "Google",
                        PhotoType = PhotoType.Url,
                        PhotoUrl = "https://media.licdn.com/dms/image/v2/C4D0BAQHiNSL4Or29cg/company-logo_200_200/company-logo_200_200/0/1631311446380?e=1747267200&v=beta&t=vU6juPS-AVsn9MDUlF0vGmmRbPh0ubxT_u-g_1hfx6c"
                    },
                    Items = [
                        new()
                        {
                            Position = "Truyền thông và Giao tiếp",
                            From = new DateTime(2025,01,01),
                            Description = @"Đam mê tạo ra những câu chuyện hấp dẫn để thu hút, cung cấp thông tin và truyền cảm hứng.
Với chuyên môn về **Quan hệ truyền thông, Sáng tạo nội dung**, và **Giao tiếp chiến lược**, tôi giúp các thương hiệu và tổ chức xây dựng mối liên kết vững chắc với khán giả của họ.
Dù là viết thông cáo báo chí, quản lý chiến dịch truyền thông xã hội hay nâng cao nhận diện thương hiệu, tôi luôn hướng đến việc tạo ra những thông điệp có sức ảnh hưởng."
                        },
                        new()
                        {
                            Position = "Tiếp thị",
                            From = new DateTime(2024,06,01),
                            To = new DateTime(2024,12,31),
                            Description = @"Kết hợp giữa sáng tạo và dữ liệu, tôi chuyên xây dựng chiến lược tiếp thị để thu hút khán giả, tăng cường tương tác và thúc đẩy tăng trưởng kinh doanh.
Với hiểu biết sâu sắc về **Hành vi người tiêu dùng** và **Xu hướng thị trường**, tôi phát triển các chiến dịch sáng tạo có sức tác động mạnh.
Dù là khởi động chiến dịch, tối ưu hóa kênh kỹ thuật số hay xây dựng lòng trung thành thương hiệu, tôi luôn đam mê giúp doanh nghiệp phát triển."
                        }
                    ]
                }
            ];


            result.Skills = [
                new() {
                    Group = "Tiếp thị",
                    Name = "Phân tích dữ liệu",
                    Level = 3,
                },
                new() {
                    Group = "Tiếp thị",
                    Name = "Tương tác khách hàng",
                    Level = 2,
                },
                new() {
                    Group = "Quản lý",
                    Name = "Quản lý dự án, Hoạch định chiến lược",
                    Level = 3,
                },
                new() {
                    Group = "Quản lý",
                    Name = "Khả năng tài chính",
                    Level = 2,
                },
            ];

            return result;
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
            public const string Twitter = Icons.Custom.Brands.Twitter;
            public const string X = Icons.Custom.Brands.X;
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
            public const string Wand = Icons.Material.Filled.AutoFixHigh;
            public const string Stars = Icons.Material.Filled.AutoAwesome;
            public const string Books = Icons.Material.Filled.AutoStories;
            public const string Apps = Icons.Material.Filled.Apps;
            public const string FerrisWheel = Icons.Material.Filled.Attractions;
            public const string MusicNote = Icons.Material.Filled.Audiotrack;
            public const string Shapes = Icons.Material.Filled.Category;
            public const string PaintPalette = Icons.Material.Filled.ColorLens;
            public const string Science = Icons.Material.Filled.Science;
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

        public class Project
        {
            public string Name { get; set; }
            public string Url { get; set; }
            public string Description { get; set; }
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
