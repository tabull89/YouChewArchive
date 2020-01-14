using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using YouChewArchive.Data;
using YouChewArchive.DataContracts;
using YouChewArchive.Logic;

namespace YouChewArchive
{
    public static class ImageLogic
    {
        private static HashSet<string> images = new HashSet<string>();
        public static string ImageRepository = @"C:\Users\tabull\source\repos\YouChewArchive\YouChewArchive\Output";
        private static string BadImagesFile = $"{Output.OutputDirectory}\\_Not archived\\badImages.txt";

        static ImageLogic()
        {
            if(File.Exists(BadImagesFile))
            {
                using (StreamReader sr = new StreamReader(BadImagesFile))
                {
                    while(!sr.EndOfStream)
                    {
                        string href = sr.ReadLine();

                        if(!String.IsNullOrEmpty(href))
                        {
                            invalidImages.Add(href);
                        }
                    }
                }
            }
        }

        public static void SaveInvalidImages()
        {
            using (StreamWriter sw = new StreamWriter(BadImagesFile, false))
            {
                foreach(string img in invalidImages)
                {
                    sw.WriteLine(img);
                }
            }
        }

        public static void IncludeImage(string src, string path, int? pmMember = null)
        {
            string filePath = path.Replace("/", "\\");

            string sourceFile = $@"{ImageRepository}\{filePath}";
            string destFile = $@"{Output.OutputDirectory}\{filePath}";

            if(pmMember.HasValue)
            {
                Member member = MemberLogic.GetMember(pmMember.Value);

                destFile = $@"{MessageLogic.GetMemberDirectory(member)}\{filePath}";
            }
                
            if(!File.Exists(destFile) && File.Exists(sourceFile))
            {
                Output.CreateDirectoryIfNotExists(destFile);
                File.Copy(sourceFile, destFile);
            }
            else if(!File.Exists(sourceFile))
            {
                LogLogic.Log(LogLogic.Image, src);
            }

            
            
        }

        public static List<Image> GetBanners()
        {
            string query = "SELECT * FROM images_images WHERE group_id = 3 AND field_3 = 1 ORDER BY upload_date DESC";

            return DB.Instance.GetData<Image>(query);
        }

        public static List<Image> GetTradingCardImages()
        {
            string query = "SELECT * FROM images_images WHERE group_id = 1 ORDER BY field_1";

            return DB.Instance.GetData<Image>(query);
        }

        private const int BANNERS_PER_PAGE = 10;

        public static void GenerateBanners()
        {
            EventLogic.RaiseEvent(ProgressEventArgs.App, "Banners");

            List<Image> banners = GetBanners();

            int totalPages = (int)Math.Ceiling((decimal)banners.Count / BANNERS_PER_PAGE);

            int page = 1;

            while (true)
            {
                List<Image> images = banners.Skip((page - 1) * BANNERS_PER_PAGE).Take(BANNERS_PER_PAGE).ToList();

                if (images.Count == 0 && page != 1)
                {
                    break;
                }

                GenerateBannerPage(images, page, totalPages);

                page++;
            }
        }

        private static void GenerateBannerPage(List<Image> images, int page, int totalPages)
        {
            string html = $"<div class='page-header'><h1>Banners</h1></div>";

            html += Output.GeneratePagination("extra/banners/{id}.html", page, totalPages, true);

            html += "<div class='container'>";

            foreach(Image image in images)
            {
                html += $"<div class='centered text-center'><img src='http://i.youchew.net/{image.image}' /></div>";
            }

            html += "</div>";

            html += Output.GeneratePagination("extra/banners/{id}.html", page, totalPages, false);

            Output output = new Output()
            {
                Content = html,
                CleanupContent = true,
                FileName = $"extra/banners/{page}.html",
                FoldersDeep = 2,
                Title = "Banners",
            };

            output.Generate();
        }

        public static void GenerateTradingCardGame()
        {
            EventLogic.RaiseEvent(ProgressEventArgs.App, "Trading Card Game");

            List<Image> tcgs = GetTradingCardImages();

            string html = $"<div class='page-header'><h1>Trading Card Game</h1></div>";

            html += "<div class='row flex-row'>";

            foreach(Image tcg in tcgs)
            {
                html += "<div class='tcg col-md-3 col-sm-6'><div class='thumbnail'>" +
                            $"<a href='http://i.youchew.net/{tcg.image}'><img src='http://i.youchew.net/{tcg.thumbnail}' /></a>" +
                            $"<div class='caption'><h4>#{tcg.field_1}: {tcg.field_2}</h4></div>" +
                    "</div></div>";
            }

            html += "</div>";

            Output output = new Output()
            {
                Content = html,
                CleanupContent = true,
                FileName = $"extra/tcg.html",
                FoldersDeep = 1,
                Title = "Trading Card Game",
            };

            output.Generate();

        }

        public static string DownloadImage(string href, bool downloadImages)
        {
            href = href.Replace("&amp;", "&");

            string path = CreateImagePath(href);
            string localPath = $"{Output.OutputDirectory}\\{path.Replace("/", "\\")}";

            bool fileExists = File.Exists(localPath);

            if(!downloadImages && fileExists)
            {
                return path;
            }

            if(ImageSiteIsBlacklisted(href) || invalidImages.Contains(href))
            {
                return href;
            }

            if(!downloadImages)
            {
                return href;
            }

            if(!fileExists)
            {
                Output.CreateDirectoryIfNotExists(localPath);

                
                if (DownloadImage(href, localPath))
                {
                    return path;
                }
                else
                {
                    AddInvalidImage(href);
                }
                
            }
            else
            {
                return path;
            }

            return href;


        }

        private static int invalidImageCount = 0;

        private static void AddInvalidImage(string href)
        {
            invalidImages.Add(href);
            invalidImageCount++;

            if(invalidImageCount > 15)
            {
                SaveInvalidImages();
                invalidImageCount = 0;
            }
        }

        private static List<string> blacklistedSites = new List<string>()
        {
            "gifsoup.com",
            "pooparchive.com",
            "youchewpoop.com",
            "nettby.no",
            "imageshack.us",
            "www.catholic-ew.org.uk",
        };

        private static bool ImageSiteIsBlacklisted(string href)
        {
            Uri url = new Uri(href);

            return blacklistedSites.Any(s => url.Host.EndsWith(s));
        }

        private static HashSet<string> invalidImages = new HashSet<string>();

        private static bool DownloadImage(string href, string localPath)
        {
            try
            {
                using (WebClient webClient = new WebClient())
                {
                    webClient.Headers.Add("User-Agent: Mozilla/5.0 (Windows NT 10.0; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/55.0.2883.87 Safari/537.36");

                    webClient.DownloadFile(href, localPath);
                    return true;
                }
            }
            catch(WebException ex)
            {
                var status = ex.Status;

                if(status == WebExceptionStatus.Timeout)
                {
                    SaveInvalidImages();
                    throw;
                }


                if (ex.Response == null)
                {
                    Uri url = new Uri(href);

                    blacklistedSites.Add(url.Host);

                    return false;
                }

                var statusCode = ((HttpWebResponse)ex.Response).StatusCode;

                switch(statusCode)
                {
                    case HttpStatusCode.NotFound:
                    case HttpStatusCode.InternalServerError:
                    case HttpStatusCode.ServiceUnavailable:
                    case HttpStatusCode.BadRequest:
                    case HttpStatusCode.Unauthorized:
                        break;

                    default:
                        LogLogic.Log(LogLogic.Error, $"{statusCode.ToString()}: {href}");
                        break;
                }

                return false;
            }
        }

        private static List<string> invalidCharacters = new List<string>()
        {
            "<", ">", ":", "\"", "|", "?", "*"
        };

        private static string CreateImagePath(string href)
        {
            Uri url = new Uri(href);

            string host = url.Host;
            string path = url.LocalPath
                                .TrimStart('/', '\\')
                                .Replace("\\", "/");

            foreach(string inv in invalidCharacters)
            {
                path = path.Replace(inv, "");
            }

            int prevLen = -1;

            while (prevLen != path.Length)
            {
                prevLen = path.Length;

                path = path.Replace("//", "/");
            } 

            if(url.Segments[url.Segments.Length - 1].Length > 200)
            {
                path = path.Replace(url.Segments[url.Segments.Length - 1], Output.CreateMD5(url.Segments[url.Segments.Length - 1]));
            }

            return $"images/external/{host}/{path}";
        }
    }
}
