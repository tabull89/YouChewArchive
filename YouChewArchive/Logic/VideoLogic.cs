using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YouChewArchive.Data;
using YouChewArchive.DataContracts;

namespace YouChewArchive.Logic
{
    public static class VideoLogic
    {
        public static string GetVideoCategoryUrl(VideoCategory vc, int page = 1)
        {
            return $"video/{vc.Id}-{vc.seo_name}/{page}.html";
        }

        private static string ShortenSeo(string seo)
        {
            seo = seo.Replace("?", "");

            if (seo.Length > 200)
            {
                seo = seo.Substring(0, 200);
            }

            return seo;
        }

        public static string GetVideoUrl(Video video)
        {
            VideoCategory vc = DB.Instance.GetRecordById<VideoCategory>(video.Container);

            if (vc != null)
            {
                return $"video/{vc.Id}-{vc.seo_name}/{video.Id}-{ShortenSeo(video.seo_title)}.html";
            }

            return null;
        }

        public static void GenerateVideos()
        {
            EventLogic.RaiseEvent(ProgressEventArgs.App, "videos");

            string query = $"SELECT * FROM {VideoCategory.TableName} WHERE parent_id = 0 ORDER BY position";

            List<VideoCategory> rootCategories = DB.Instance.GetData<VideoCategory>(query);

            List<VideoCategory> allCategories = new List<VideoCategory>(rootCategories);

            string html = $"<div class='page-header'><h1>Videos</h1></div>";

            html += "<ul class='list-group'>";

            foreach (VideoCategory category in rootCategories)
            {
                html += $"<li class='list-group-item'><div class='row'><div class='col-md-10'><a href='{GetVideoCategoryUrl(category)}'>{category.Description}</a></div>" +
                        $"<div class='col-md-2 text-right'>{LangLogic.FormatNumber(GetVideoCount(category), "video", "videos")}</div></div></li>" + Environment.NewLine;

                List<VideoCategory> children = GetCategoryChildren(category);

                foreach (VideoCategory child in children)
                {
                    allCategories.Add(child);

                    html += $"<li class='list-group-item'><div class='row'><div class='col-md-10'><a href='{GetVideoCategoryUrl(child)}' class='forum-offset-1'>{child.Description}</a></div>" +
                            $"<div class='col-md-2 text-right'>{LangLogic.FormatNumber(GetVideoCount(child), "video", "videos")}</div></div></li>" + Environment.NewLine;
                    
                }
            }

            html += "</ul>" + Environment.NewLine;

            Output output = new Output()
            {
                Content = html,
                FileName = "video/index.html",
                FoldersDeep = 1,
                Title = "Videos",
                Breadcrumbs = new List<Tuple<string, string, bool>>()
                {
                    Tuple.Create("Videos", "video/index.html", true),
                }
            };

            output.Generate();

            int containersCompleted = 0;

            EventLogic.RaiseEvent(ProgressEventArgs.Containers, allCategories.Count.ToString());
            EventLogic.RaiseEvent(ProgressEventArgs.ContainersCompleted, containersCompleted.ToString());

            foreach (VideoCategory category in allCategories)
            {
                GenerateVideos(category);

                EventLogic.RaiseEvent(ProgressEventArgs.ContainersCompleted, (++containersCompleted).ToString());
            }
        }

        private const int VIDEOS_PER_PAGE = 100;

        private static void GenerateVideos(VideoCategory category)
        {
            EventLogic.RaiseEvent(ProgressEventArgs.CurrentContainer, category.Description);

            List<Video> videos = ContainerLogic.GetItemsWithPermission<Video>(category.Id);

            foreach(VideoCategory vc in GetCategoryChildren(category))
            {
                videos.AddRange(ContainerLogic.GetItemsWithPermission<Video>(vc.Id));
            }

            videos = videos.OrderByDescending(v => v.Date).ToList();

            EventLogic.RaiseEvent(ProgressEventArgs.Items, videos.Count.ToString());

            int videosCompleted = 0;
            EventLogic.RaiseEvent(ProgressEventArgs.ItemsCompleted, videosCompleted.ToString());

            int page = 1;

            int totalPages = (int)Math.Ceiling((decimal)videos.Count / VIDEOS_PER_PAGE);

            while (true)
            {
                List<Video> currentVideos = videos.Skip((page - 1) * VIDEOS_PER_PAGE).Take(VIDEOS_PER_PAGE).ToList();

                if (currentVideos.Count == 0 && page != 1)
                {
                    break;
                }

                GenerateVideos(category, currentVideos, page, totalPages, ref videosCompleted);

                EventLogic.RaiseImportantEvent();

                page++;
            }
        }

        private static void GenerateVideos(VideoCategory vc, List<Video> videos, int page, int totalPages, ref int videosCompleted)
        {
            EventLogic.RaiseEvent(ProgressEventArgs.CurrentContainer, vc.Description);

            string html = $"<div class='page-header'><h1>{vc.Description}</h1></div>";


            html += Output.GeneratePagination($"video/{vc.Id}-{vc.seo_name}/{{id}}.html", page, totalPages, true);

            html += "<div class='row flex-row'>";

            foreach (Video video in videos)
            {
                string path = "uploads/videos/thumbnails/" + (String.IsNullOrEmpty(video.thumbnail) ? "nothumb.jpg" : video.thumbnail);

                string thumbnailUrl = $"http://youchew.net/{path}";

                ImageLogic.IncludeImage(thumbnailUrl, path.Replace("/", "\\"));

                html += $"<div class='col-md-3 col-sm-6 overflow-ellipsis video-block'><div class='thumbnail'><a href='{GetVideoUrl(video)}'><img class='img-responsive' src='{path}' /></a><div class='caption'>" +
                    $"<h4><a href='{GetVideoUrl(video)}'>{video.title}</a></h4>" +
                    $"<p>{MemberLogic.GetUrlHtml(video.Author)} - {LangLogic.FormatDate(video.Date, LangLogic.ShortDateFormat)}</p><p>{LangLogic.FormatNumber(video.NumComments, "comment", "comments")}</p></div></div></div>";

            }

            html += "</div>";

            html += Output.GeneratePagination($"video/{vc.Id}-{vc.seo_name}/{{id}}.html", page, totalPages, false);

            Output output = new Output()
            {
                Content = html,
                FileName = GetVideoCategoryUrl(vc, page),
                Title = vc.Description,
                FoldersDeep = 2,
                Breadcrumbs = new List<Tuple<string, string, bool>>()
                {
                    Tuple.Create("Videos", "video/index.html", false),
                    Tuple.Create(vc.Description, GetVideoCategoryUrl(vc), true),
                },
                ActiveNav = "Videos",
            };

            output.Generate();

            foreach (Video video in videos)
            {
                GenerateVideo(video);

                EventLogic.RaiseEvent(ProgressEventArgs.ItemsCompleted, (++videosCompleted).ToString());
            }
        }

        public static void GenerateVideo(Video video)
        {
            EventLogic.RaiseEvent(ProgressEventArgs.CurrentItem, video.Title);

            string desc = !String.IsNullOrEmpty(video.short_desc) ? $"<h4>{video.short_desc}</h4>" : "";
            string html = $"<div class='page-header'><h1>{video.Title}</h1>{desc}<h5>By {MemberLogic.GetUrlHtml(video.Author)}, {LangLogic.FormatDate(video.Date, LangLogic.ShortDateFormat)}</h5></div>";
            

            string repHtml = "";

            int rep = ReputationLogic.GetTotalRepuation<Video>(video.Id);

            if (rep != 0)
            {
                string repClass = rep > 0 ? "rep-positive" : "rep-negative";

                repHtml = $"<div class='col-md-12 text-right'><span class='rep {repClass}'>{rep}</span></div>";
            }

            html += $"<ul class='list-group'><li class='list-group-item'><div class='row post'><div class='col-md-12'><p>{video.embed}</p><p>{video.description}</p>{repHtml}</div></div></li></ul>" + Environment.NewLine;

            List<VideoComment> comments = ItemLogic.GetCommentsWithPermission<VideoComment>(video.Id);

            if (comments.Count > 0)
            {
                html += "<h3>Comments</h3><ul class='list-group'>";
            }

            foreach (VideoComment comment in comments)
            {
                repHtml = "";

                rep = ReputationLogic.GetTotalRepuation<VideoComment>(comment.Id);

                if (rep != 0)
                {
                    string repClass = rep > 0 ? "rep-positive" : "rep-negative";

                    repHtml = $"<div class='col-md-12 text-right'><span class='rep {repClass}'>{rep}</span></div>";
                }

                html += $"<li class='list-group-item'><div class='row post' id='comment-{comment.Id}'>";
                html += $"<div class='col-md-2'><p>{MemberLogic.GetUrlHtml(comment.Author.GetValueOrDefault())}</p><p>{MemberLogic.GetGroupName(MemberLogic.GetMember(comment.Author.GetValueOrDefault()).member_group_id, false)}</p></div>";
                html += $"<div class='col-md-10'>" +
                    $"<div class='col-md-12 post-info-bar'>{LangLogic.FormatDate(comment.Date, LangLogic.LongDateFormat)}</div>" +
                    $"<div class='col-md-12'>{comment.Content}</div>" +
                    repHtml +
                    $"</div>";

                html += "</div></li>" + Environment.NewLine;
            }

            if(comments.Count > 0)
            {
                html += "</ul>" + Environment.NewLine;
            }

            VideoCategory vc = DB.Instance.GetRecordById<VideoCategory>(video.Container);

            Output output = new Output()
            {
                Content = html,
                CleanupContent = true,
                DownloadImages = true,
                FileName = GetVideoUrl(video),
                FoldersDeep = 2,
                Title = video.Title,
                Breadcrumbs = new List<Tuple<string, string, bool>>()
                {
                    Tuple.Create("Videos", "video/index.html", false),
                    Tuple.Create(vc.Description, GetVideoCategoryUrl(vc), false),
                    Tuple.Create(video.Title, GetVideoUrl(video), true),
                },
                ActiveNav = "Videos",
            };

            output.Generate();
        }

        private static Dictionary<int, List<VideoCategory>> categoryChildMap = new Dictionary<int, List<VideoCategory>>();

        public static List<VideoCategory> GetCategoryChildren(VideoCategory vc)
        {
            List<VideoCategory> children;

            if (!categoryChildMap.TryGetValue(vc.Id, out children))
            {
                children = DB.Instance.GetData<VideoCategory>($"SELECT * FROM {VideoCategory.TableName} WHERE parent_id = {vc.Id} ORDER BY position");

                categoryChildMap.Add(vc.Id, children);
            }

            return children;
        }

        public static int GetVideoCount(VideoCategory vc)
        {
            List<VideoCategory> categories = GetCategoryChildren(vc).Concat(new List<VideoCategory>() { vc }).ToList(); ;
            

            string query = $"SELECT COUNT(*) FROM {Video.TableName} WHERE {ContentLogic.GeneratePermissionWhere<Video>()} AND cid IN({String.Join(",", categories.Select(c => c.Id.ToString()))})";

            return DB.Instance.ExecuteScalar<int>(query);




        }
    }
}
