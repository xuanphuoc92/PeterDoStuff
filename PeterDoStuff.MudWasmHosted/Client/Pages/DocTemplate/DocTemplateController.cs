using PeterDoStuff.Extensions;

namespace PeterDoStuff.MudWasmHosted.Client.Pages.DocTemplate
{
    public class DocTemplateController
    {
        public class Resume
        {
            public Entity Person { get; set; } = new();

            public string Location { get; set; }

            public string PhoneLabel { get; set; } = "Phone";
            public string PhoneNumber { get; set; }

            public string Fluency { get; set; }
            public string Email { get; set; }

            public Link LinkedIn { get; set; } = new();
            public Link GitHub { get; set; } = new();

            public List<(string, Link)> OtherLinks { get; set; } = [];

            public string AboutMe { get; set; }
            public string[] AboutMeParagraphs => AboutMe?.Split("\n").Select(p => p.Trim()).Where(p => p.IsNullOrEmpty() == false).ToArray() ?? [];

            public List<Experience> Experiences { get; set; } = [];

            public List<Education> Educations { get; set; } = [];

            public Dictionary<int, string> SkillLevels { get; set; } = new()
            {
                { 1, "Familiar" },
                { 2, "Proficient" },
                { 3, "Skilled" },
                { 4, "Expert" },
                { 5, "Master" },
            };

            public List<Skill> Skills { get; set; } = [];

            public List<(string,List<Skill>)> SkillByGroup => Skills
                .GroupBy(skill => skill.Group)
                .Select(group => (group.Key, group.OrderByDescending(skill => skill.Level).ToList()))
                .ToList();
        }

        public enum PhotoType
        {
            None, Url, Base64
        }

        public class Link
        {
            public string Url { get; set; }
            public string Text { get; set; }
        }

        public class Experience
        {
            public Entity Company { get; set; } = new();
            public List<ExperienceItem> Items {get;set;}
        }

        public class ExperienceItem
        {
            public string Position { get; set; }
            public DateTime From { get; set; }
            public DateTime? To { get; set; }
            public string Description { get; set; }
            public string[] DescriptionParagraphs => Description?.Split("\n").Select(p => p.Trim()).Where(p => p.IsNullOrEmpty() == false).ToArray() ?? [];
        }

        public class Education
        {
            public Entity Institution { get; set; } = new();
            public string Description { get; set; }
            public DateTime From { get; set; }
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
            public int Level { get; set; }
        }

        public const string SampleHtml = @"
<!DOCTYPE html>
<html>
    <head>
        <link href=""https://maxcdn.bootstrapcdn.com/font-awesome/4.2.0/css/font-awesome.min.css"" rel=""stylesheet"">

        <style>
            body {
                font-family: 'Segoe UI', Tahoma, Geneva, Verdana, sans-serif;
                color: #555555;
                font-size: 0.85em;
            }

            h1 {
                font-size: 2em;
            }

            h2 {
                font-size: 1.2em;
            }

            h3 {
                font-size: 1em;
                margin-block-end: 0;
            }

            h4 {
                font-size: 0.85em;
                margin-block-start: 0.5em;
                margin-block-end: 0;
            }

            .sub {
                font-size: 0.8em;
            }

            page {
                background: white;
                display: block;
                margin: 0 auto;
                margin-bottom: 0.5cm;
                width: 21cm;
                height: 29.7cm;;
            }

            .header {
                display: grid;
                grid-template-columns: auto auto;
                background-color: #333333;
                color: #BBBBBB;
            }

            .header a {
                color: #BBBBBB;
            }

            .avatar {
                margin: 25px;
                grid-area: 1 / 2 / 3 / 2
            }

            .avatar img {
                width: 150px;
                height: 150px;
                border-radius: 50%;
                border: 0.15cm solid #777777;
                object-fit: cover;
                object-position: 50% 50%;
            }

            .title, .sectionTitle {
                font-family:'Franklin Gothic Medium', 'Arial Narrow', Arial, sans-serif;
            }

            .title {
                justify-self: center;
            }

            .subtitle {
                margin: 20px;
            }

            .subtitleItem {
                display: grid;
                grid-template-columns: 40px 120px auto;
                align-items: center;
            }

            .fa {
                justify-self: center;
                margin-right: 0.1em;
            }

            .body {
                display: grid;
                grid-template-columns: auto auto;
            }

            .section {
                padding-left: 1em;
                padding-right: 1em;
            }

            ul {
                padding-inline-start: 1em;
            }

            .skillGrid {
                display: grid;
                grid-template-columns: 40% 40% 20%
            }

            hr {
                margin-block-start: 0em;
            }
        </style>
    </head>
    <body>
        <page size=""A4"">
            <div class=""header"">
                <div class=""avatar"" >
                    <img width=""150px""
                    src=""https://media.licdn.com/dms/image/v2/D5603AQH7MIFQCAHkog/profile-displayphoto-shrink_800_800/B56ZQdU7y7GsAg-/0/1735658815028?e=1742428800&v=beta&t=4a_diiWwXw2AN1NTRR-tHetGEx9AAgt9fO5VmO7AAGc"" />
                </div>
                <div class=""title""><h1>Peter Vo (Vo Xuan Phuoc)</h1></div>
                <div class=""subtitle"">
                    <div class=""subtitleItem"">
                        <i class=""fa fa-map-marker contactIcon"" aria-hidden=""true""></i>
                        <span>
                            Location:
                        </span>
                        <span>
                            Singapore
                        </span>
                    </div>
                    <div class=""subtitleItem"">
                        <i class=""fa fa-envelope contactIcon"" aria-hidden=""true""></i>
                        <span>Email:</span>
                        <a href=""mailto:xuanphuoc92@gmail.com"">
                            xuanphuoc92@gmail.com
                        </a>
                    </div>
                    <div class=""subtitleItem"">
                        <i class=""fa fa-linkedin-square contactIcon"" aria-hidden=""true""></i>
                        <span>LinkedIn:</span>
                        <a href=""https://www.linkedin.com/in/peter-vo-43bb1337/"">
                            in/peter-vo-43bb1337
                        </a>
                    </div>
                    <div class=""subtitleItem"">
                        <i class=""fa fa-github contactIcon"" aria-hidden=""true""></i>
                        <span>GitHub:</span>
                        <a href=""https://github.com/xuanphuoc92/PeterDoStuff"" >
                            xuanphuoc92/PeterDoStuff
                        </a>
                    </div>
                    <div class=""subtitleItem"">
                        <i class=""fa fa-globe contactIcon white"" aria-hidden=""true""></i>
                        <span>Portfolio App:</span>
                        <a href=""https://peter-do-stuff.azurewebsites.net/"">
                            peter-do-stuff.azurewebsites.net
                        </a>
                    </div>
                </div>
            </div>
            <hr/>
            <div class=""body"">
                <div class=""section"">
                    <div class=""sectionTitle"">
                        <h2>
                            <i class=""fa fa-user""></i>
                            About me
                        </h2>
                    </div>
                    <div class=""sectionItem"">
                        <p>An easy-going software engineer whose interests spread in wide areas from the back-end to the front-end, though you can say I'm deeper in the back-end side, like Database, Domain Implementation, and Testing.</p>
                        <p>Most comfortable with C#, since I tend to swing between Object-Oriented and Functional paradigms (and C#, especially newer ones, caters both pretty well). Usually struggle with JavaScript, so I tend to minimize its exposure (until I have to) by playing with Blazor Web Assembly and its frameworks (e.g. MudBlazor).</p>
                        <p>Spend free time with building Proof-of-Concepts in portfolio,  exploring Generative AI,  writing Fantasy Novel, and gaming.</p>
                    </div>
                </div>
            </div>
            <hr/>
            <div class=""body"">
                <div class=""section"">
                    <div class=""sectionTitle"">
                        <h2>
                            <i class=""fa fa-graduation-cap""></i>
                            Education
                        </h2>
                    </div>
                    <div class=""sectionItem"">
                        <h3>Master of Computing,<br/>Computer Science</h3>
                        <div>National University of Singapore</div>
                        <div class=""sub"">Jan 2020 - Jan 2022</div>
                    </div>
                    <div class=""sectionItem"">
                        <h3>Bachelor's degree,<br/>Information Engineering & Media</h3>
                        <div>Nanyang Technological University Singapore</div>
                        <div class=""sub"">Jun 2010 - Jun 2014</div>
                    </div>
                </div>
                <div class=""section"">
                    <div class=""sectionTitle"">
                        <h2>
                            <i class=""fa fa-suitcase""></i>
                            Experience
                        </h2>
                    </div>
                    <div class=""sectionItem"">
                        <h3>Anacle Systems Ltd<br/>Software Engineer</h3>
                        <div class=""sub"">Jun 2014 - present</div>
                        <p>
                            <ul>
                                <li>
                                    Technical Lead: Maintain live projects, overseeing their operations while minimizing disruptions.
                                </li>
                                <li>
                                    Developer: Develop new products, features, and tools. Supporting wide range of activities, including consulting, estimating, implementing, testing, and deploying.
                                </li>
                                <li>
                                    Maintainer: Investigate and replicate bugs, vulnerabilities, slowness; then implement appropriate fixes, hardenings, optimization.
                                </li>
                            </ul>
                        </p>
                    </div>
                    <div class=""sectionItem"">
                        <h3>CNBC Asia<br/>Advertising Sales and Operation Internship - Graphics</h3>
                        <div class=""sub"">Jun 2014 - present</div>
                        <p>
                            <ul>
                                <li>
                                    Intern: Support Creative Solution team during the production for advertising clients, including creating presentation, storyboard, and concept treatments.
                                </li>
                            </ul>
                        </p>
                    </div>
                </div>
            </div>
            <hr/>
            <div class=""section"">
                <div class=""sectionTitle"">
                    <h2>
                        <i class=""fa fa-solid fa-wrench""> </i>
                        Skills
                    </h2>
                </div>

                <h3>Languages</h3>
                <div class=""skillGrid"">
                    <div class=""sectionItem"">
                        <h4>
                            <i class=""fa fa-solid fa-star""> </i>
                            <i class=""fa fa-solid fa-star""> </i>
                            <i class=""fa fa-solid fa-star""> </i>
                            Skilled
                        </h4>
                        <div>C#, SQL (focus: MS-SQL, PostgreSQL, SQLite)</div>
                    </div>
                    <div class=""sectionItem"">
                        <h4>
                            <i class=""fa fa-solid fa-star""> </i>
                            <i class=""fa fa-solid fa-star""> </i>
                            Proficient
                        </h4>
                        <div>HTML, CSS</div>
                    </div>
                    <div class=""sectionItem"">
                        <h4>
                            <i class=""fa fa-solid fa-star""> </i>
                            Familiar
                        </h4>
                        <div>Java, Python, JavaScript</div>
                    </div>
                </div>

                <h3>Frameworks and Libraries</h3>
                <div class=""skillGrid"">
                    <div class=""sectionItem"">
                        <h4>
                            <i class=""fa fa-solid fa-star""> </i>
                            <i class=""fa fa-solid fa-star""> </i>
                            <i class=""fa fa-solid fa-star""> </i>
                            Skilled
                        </h4>
                        <div>Blazor, Web Assembly, .NET Core, Dapper</div>
                    </div>
                    <div class=""sectionItem"">
                        <h4>
                            <i class=""fa fa-solid fa-star""> </i>
                            <i class=""fa fa-solid fa-star""> </i>
                            Proficient
                        </h4>
                        <div>.NET Standard, .NET Framework</div>
                    </div>
                    <div class=""sectionItem"">
                        <h4>
                            <i class=""fa fa-solid fa-star""> </i>
                            Familiar
                        </h4>
                        <div>EF Core</div>
                    </div>
                </div>

                <h3>Tools and Platforms</h3>
                <div class=""skillGrid"">
                    <div class=""sectionItem"">
                        <h4>
                            <i class=""fa fa-solid fa-star""> </i>
                            <i class=""fa fa-solid fa-star""> </i>
                            <i class=""fa fa-solid fa-star""> </i>
                            Skilled
                        </h4>
                        <div>Git, SQL Management Studio, pgAdmin</div>
                    </div>
                    <div class=""sectionItem"">
                        <h4>
                            <i class=""fa fa-solid fa-star""> </i>
                            <i class=""fa fa-solid fa-star""> </i>
                            Proficient
                        </h4>
                        <div>GitHub, GitLab, SonarQube</div>
                    </div>
                    <div class=""sectionItem"">
                        <h4>
                            <i class=""fa fa-solid fa-star""> </i>
                            Familiar
                        </h4>
                        <div>Azure, AWS</div>
                    </div>
                </div>

                <h3>General Skills and Domains</h3>
                <div class=""skillGrid"">
                    <div class=""sectionItem"">
                        <h4>
                            <i class=""fa fa-solid fa-star""> </i>
                            <i class=""fa fa-solid fa-star""> </i>
                            <i class=""fa fa-solid fa-star""> </i>
                            Skilled
                        </h4>
                        <div>TDD, Refactoring, Estimating</div>
                    </div>
                    <div class=""sectionItem"">
                        <h4>
                            <i class=""fa fa-solid fa-star""> </i>
                            <i class=""fa fa-solid fa-star""> </i>
                            Proficient
                        </h4>
                        <div>Security Hardening, Cryptography,<br/>Impact Analysis, Documenting</div>
                    </div>
                    <div class=""sectionItem"">
                        <h4>
                            <i class=""fa fa-solid fa-star""> </i>
                            Familiar
                        </h4>
                        <div>Scrum, Procurement</div>
                    </div>
                </div>

            </div>
        </page>
    </body>
</html>";
    }
}
