using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using TidyManaged;
using YouChewArchive.DataContracts;

namespace YouChewArchive.Logic
{
    public class Output
    {
        private static string TemplateDirectory = @"C:\Users\tabull\source\repos\YouChewArchive\YouChewArchive\Templates";
        public static string OutputDirectory = @"C:\YC_Full";

        public string Title { get; set; }
        public string Content { get; set; }
        public string ContentTemplate { get; set; }

        public int FoldersDeep { get; set; }
        public bool DownloadImages { get; set; }
        public Member PMMember { get; set; }
        public string ActiveNav { get; set; }
        public string BaseUrl { get; set; } = null;
        public bool NoNav { get; set; } = false;

        public List<Tuple<string, string, bool>> Breadcrumbs { get; set; } = new List<Tuple<string, string, bool>>();

        public string Base {
            get
            {
                if(FoldersDeep == 0)
                {
                    return "";
                }
                else
                {
                    string _base = "";
                    for(int i = 0; i < FoldersDeep; i++)
                    {
                        _base += "../";
                    }

                    return _base;
                }
            }
        }

        private static Dictionary<string, string> TemplateMap = new Dictionary<string, string>();

        private static string GetTemplate(string template)
        {
            string html;

            if(!TemplateMap.TryGetValue(template, out html))
            {
                using (System.IO.StreamReader sr = new System.IO.StreamReader($@"{TemplateDirectory}\{template}.html"))
                {
                    html = sr.ReadToEnd();
                }

                TemplateMap.Add(template, html);
            }

            return html;
            
        }

        public string Breadcrumb
        {
            get
            {
                if(Breadcrumbs == null || Breadcrumbs.Count == 0)
                {
                    return "";
                }

                string html = "<ol class='breadcrumb'>";

                foreach(var tuple in Breadcrumbs)
                {
                    if (tuple.Item3)
                    {
                        html += $"<li class='active'>{tuple.Item1}</li>";
                    }
                    else
                    {
                        html += $"<li><a href='{tuple.Item2}'>{tuple.Item1}</a></li>";
                    }
                }

                html += "</ol>";

                return html;
            }
        }

        public class Nav
        {
            public string Title { get; set; }
            public string Link { get; set; }
            public bool Active { get; set; } = false;
            public List<Nav> ChildNavs { get; set; } = new List<Nav>();

            public Nav(string title, string link, List<Nav> childNavs = null)
            {
                Title = title;
                Link = link;

                if(childNavs != null)
                {
                    ChildNavs = childNavs;
                }
            }
        }

        private static List<Nav> Navs = new List<Nav>()
        {
            new Nav("Home", "index.html"),
            new Nav("Forums", "forum/index.html"),
            new Nav("Blogs", "blog/1.html"),
            new Nav("Videos", "video/index.html"),
            new Nav("Status Updates", "status/1.html"),
            new Nav("Members", "member/list/joined/1.html"),
            new Nav("Extra", "#", new List<Nav>()
                {
                    new Nav("Highest Rated Posts", "extra/highest/1.html"),
                    new Nav("Awards", "extra/awards.html"),
                    new Nav("Banners", "extra/banners/1.html"),
                    new Nav("Trading Card Game", "extra/tcg.html"),
                }),
           // new Nav("<span class='glyphicon glyphicon-search'></span>", "search.html"),
        };

        private static List<Nav> PmNavs = new List<Nav>()
        {
            new Nav("Home", "index.html"),
            new Nav("Inbox", "message/1.html"),
        };

        public string NavBar
        {
            get
            {
                if(NoNav)
                {
                    return "";
                }

                if(PMMember == null)
                {
                    return GenerateNavigator(Navs);
                }
                else
                {
                    return GenerateNavigator(PmNavs);
                }

                /*
                if(PMMember == null)
                {
                    return @"<li><a href='index.html'>Home</a></li><li><a href='forum/index.html'>Forums</a></li>"
                         + @"<li><a href='blog/1.html'>Blogs</a></li>"
                         + @"<li><a href='video/index.html'>Videos</a></li>"
                         + @"<li><a href='status/1.html'>Status Updates</a></li>"
                         + @"<li><a href='member/list/joined/1.html'>Members</a></li>"
                         + @"<li class='dropdown'><a href='#' class=dropdown-toggle' data-toggle='dropdown' role='button'>Extra<span class='caret'></span></a>"
                          + @"<ul class='dropdown-menu'>"
                                + "<li><a href='extra/highest/1.html'>Highest Rated Posts</li>"
                                + "<li><a href='extra/awards.html'>Awards</a></li>"
                                + "<li><a href='extra/banners/1.html'>Banners</li>"
                                + "<li><a href='extra/tcg.html'>Trading Card Game</a></li>"
                          + "</ul>"
                          +"</li>"
                          + "<li><a href='search.html'><span class='glyphicon glyphicon-search'></span></a></li>";
                }
                else
                {
                    return @"<li><a href='index.html'>Home</a></li><li><a href='message/1.html'>Inbox</a></li>";
                }
                */
            }
        }

        private string GenerateNavigator(List<Nav> navs)
        {
            StringBuilder sb = new StringBuilder();

            foreach(Nav nav in navs)
            {
                string activeNav = String.IsNullOrEmpty(ActiveNav) ? Title : ActiveNav;

                bool isActive = nav.Title == activeNav || nav.Link == activeNav;

                if(nav.ChildNavs.Count > 0)
                {
                    string activeClass = isActive || IsExtra(activeNav) ? " active" : "";

                    sb.Append($"<li class='dropdown{activeClass}'><a href='#' class=dropdown-toggle' data-toggle='dropdown' role='button'>{nav.Title}<span class='caret'></span></a>")
                      .Append("<ul class='dropdown-menu'>")
                      .Append(GenerateNavigator(nav.ChildNavs))
                      .Append("</ul></li>");
                }
                else
                {
                    string activeClass = isActive ? " class='active'" : "";

                    sb.Append($"<li{activeClass}><a href='{nav.Link}'>{nav.Title}</a></li>");
                }
            }

            return sb.ToString();
        }

        private bool IsExtra(string activeNav)
        {
            return activeNav == "Highest Rated Posts" || activeNav == "Awards" || activeNav == "Banners" || activeNav == "Trading Card Game";
        }

        public string FileName { get; set; }
        public bool CleanupContent { get; set; }

        //Bad practice, but I don't care.
        public List<Post> AllPostsInTopic { get; set; }       

        public void Generate()
        {
            string html = GetTemplate("General")
                                .Replace("{Title}", Title + " - YouChew Archive")
                                .Replace("{Base}", Base)
                                .Replace("{Breadcrumb}", Breadcrumb)
                                .Replace("{NavBar}", NavBar);

            if(!String.IsNullOrEmpty(ContentTemplate))
            {
                html = html.Replace("{Content}", GetTemplate(ContentTemplate));
            }
            else
            {
                html = html.Replace("{Content}", Content);
            }
                                         

            if(CleanupContent)
            {
                int? pmMemberId = PMMember != null ? (int?)PMMember.Id : null;

                html = ContentLogic.TransformContent(html, AllPostsInTopic, pmMemberId, DownloadImages, BaseUrl);
            }

            //html = TidyHtml(html);

            string outputDir = OutputDirectory;

            if(PMMember != null)
            {
                outputDir = MessageLogic.GetMemberDirectory(PMMember);
            }

            string file = $@"{outputDir}\{FileName}";

            CreateDirectoryIfNotExists(file);

            using (StreamWriter sw = new StreamWriter(file,false))
            {
                sw.Write(html);
            }
        }

        private string TidyHtml(string html)
        {
            using (Document doc = Document.FromString(html))
            {
                doc.ShowWarnings = false;
                doc.Quiet = true;
                doc.DocType = DocTypeMode.Strict;
                doc.OutputXhtml = false;
                doc.OutputXml = false;
                doc.OutputHtml = true;
                doc.MakeClean = true;
                doc.DropEmptyParagraphs = false;
                doc.WrapAt = 0;
                doc.IndentBlockElements = AutoBool.Yes;
                doc.CharacterEncoding = EncodingType.Utf8;
                doc.WrapSections = false;
                doc.WrapAttributeValues = false;
                doc.WrapScriptLiterals = false;
                doc.JoinStyles = false;
                doc.JoinClasses = false;
                doc.SkipNestedTags = true;
                doc.MergeSpans = AutoBool.No;
                doc.DropEmptyElements = false;
                doc.IndentWithTabs = true;
                doc.EncloseBlockText = true;
                doc.AddVerticalSpace = false;
                doc.AddTidyMetaElement = false;
                doc.MergeDivs = AutoBool.No;
                doc.CleanAndRepair();

                return doc.Save();
            }
        }

        public static void CreateDirectoryIfNotExists(string file)
        {
            FileInfo fi = new FileInfo(file);

            if(!fi.Directory.Exists)
            {
                Directory.CreateDirectory(fi.Directory.FullName);
            }
        }

        public static void Copy(string sourceDirectory, string targetDirectory)
        {
            DirectoryInfo diSource = new DirectoryInfo(sourceDirectory);
            DirectoryInfo diTarget = new DirectoryInfo(targetDirectory);

            CopyAll(diSource, diTarget);
        }

        public static void CopyAll(DirectoryInfo source, DirectoryInfo target)
        {
            Directory.CreateDirectory(target.FullName);

            // Copy each file into the new directory.
            foreach (FileInfo fi in source.GetFiles())
            {
                fi.CopyTo(Path.Combine(target.FullName, fi.Name), true);
            }

            // Copy each subdirectory using recursion.
            foreach (DirectoryInfo diSourceSubDir in source.GetDirectories())
            {
                DirectoryInfo nextTargetSubDir =
                    target.CreateSubdirectory(diSourceSubDir.Name);
                CopyAll(diSourceSubDir, nextTargetSubDir);
            }
        }

        public static void Zip(Member member)
        {
            string zipFile = $"{OutputDirectory}\\ZippedMessages\\{CreateMD5(member)}.zip";

            CreateDirectoryIfNotExists(zipFile);

            if(File.Exists(zipFile))
            {
                File.Delete(zipFile);
            }

            ZipFile.CreateFromDirectory(MessageLogic.GetMemberDirectory(member), zipFile);
        }

        public static string CreateMD5(Member member)
        {
            string hash = "";

            List<string> toHash = new List<string>()
            {
                member.Id.ToString(),
                member.joined.ToString(),
                member.ip_address ?? "0.0.0.0",
            };

            foreach(string input in toHash)
            {
                hash = hash == "" ? CreateMD5(input) : CreateMD5(CreateMD5(hash) + CreateMD5(input));
            }

            return hash;

        }

        public static string CreateMD5(string input)
        {
            using (var md5 = MD5.Create())
            {
                byte[] inputBytes = Encoding.ASCII.GetBytes(input);
                byte[] hashBytes = md5.ComputeHash(inputBytes);

                // Convert the byte array to hexadecimal string
                StringBuilder sb = new StringBuilder();
                for (int i = 0; i < hashBytes.Length; i++)
                {
                    sb.Append(hashBytes[i].ToString("x2"));
                }
                return sb.ToString();
            }
        }

        public static void Zip(string sourceDirectory, string zipFile)
        {
            ZipFile.CreateFromDirectory(sourceDirectory, zipFile);
        }

        public static string GeneratePagination(string urlTemplate, int currentPage, int totalPages, bool isFirst)
        {
            if(totalPages <= 1)
            {
                return "";
            }

            bool hasPrevious = currentPage > 1;
            bool hasNext = currentPage < totalPages;

            string html = "<nav class='pagination-nav'><ul class='pagination'>";

            if(hasPrevious)
            {
                html += $"<li><a href='{urlTemplate.Replace("{id}", "1")}'><span>&laquo;</span></li>"
                      + $"<li><a href='{urlTemplate.Replace("{id}", (currentPage-1).ToString())}'>Prev</a></li>";
            }

            int min = Math.Max(currentPage - 5, 1);
            int max = Math.Min(currentPage + 5, totalPages);

            for(int i = min; i <= max; i++)
            {
                string @class = "";

                if(i == currentPage)
                {
                    @class = " class='active'";
                }

                html += $"<li{@class}><a href='{urlTemplate.Replace("{id}", i.ToString())}'>{i}</a></li>";
            }

            if (hasNext)
            {
                html += $"<li><a href='{urlTemplate.Replace("{id}", (currentPage + 1).ToString())}'><span>Next</span></li>"
                      + $"<li><a href='{urlTemplate.Replace("{id}", totalPages.ToString())}'>&raquo;</a></li>";
            }

            html += $"</ul><small data-pop='form-popover-{CreateMD5(urlTemplate)}' class='pagination-page'>Page {currentPage} of {totalPages}<span class='glyphicon glyphicon-chevron-down'></span></small></nav>";

            if (isFirst)
            {
                html += $"<div id='form-popover-{CreateMD5(urlTemplate)}' class='hide'>{GeneratePaginationForm(urlTemplate, totalPages)}</div>";
            }
            

            return html;
        }

        private static string GeneratePaginationForm(string urlTemplate, int totalPages)
        {
            string html = "<form class='pagination-button'>" +
                $"<input class='form-control' type='number' min='1' max='{totalPages}' placeholder='Page number' name='page' />" +
                $"<input type='hidden' name='urlTemplate' value='{WebUtility.HtmlEncode(urlTemplate)}' />" +
                $"<button type='button' class='btn btn-primary btn-block'>Go</button>" +
                $"</form>";

            return html;
        }

        public static string CreatePanel(string header = null, string content = null, string listGroup = null, bool nonResponsive = false)
        {
            if (!String.IsNullOrEmpty(header))
            {
                header = $"<div class='panel-heading'><h3 class='panel-title'>{header}</h3></div>";
            }
            else
            {
                header = String.Empty;
            }

            if (!String.IsNullOrEmpty(content))
            {
                content = $"<div class='panel-body'>{content}</div>";
            }
            else
            {
                content = String.Empty;
            }

            if (String.IsNullOrEmpty(listGroup))
            {
                listGroup = String.Empty;
            }

            string @class = "";

            if (nonResponsive)
            {
                @class = " non-responsive";
            }

            return $"<div class='panel panel-default{@class}'>{header}{content}{listGroup}</div>";
        }
    }
}
