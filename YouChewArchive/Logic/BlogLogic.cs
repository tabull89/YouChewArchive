using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YouChewArchive.Data;
using YouChewArchive.DataContracts;

namespace YouChewArchive.Logic
{
    public static class BlogLogic
    {
        static BlogLogic()
        {
            UpdateBlogData();
        }

        

        public static string GetBlogUrl(Blog blog)
        {
            return $"blog/{blog.Id}-{blog.seo_name}/index.html";
        }

        public static string GetBlogEntryUrl(BlogEntry blogEntry)
        {
            Blog blog = DB.Instance.GetRecordById<Blog>(blogEntry.Container);

            if (blog != null)
            {
                return $"blog/{blog.Id}-{blog.seo_name}/{blogEntry.Id}-{blogEntry.name_seo}.html";
            }

            return null;
        }

        public static string GetBlogEntryCommentUrl(BlogComment comment)
        {
            BlogEntry entry = DB.Instance.GetRecordById<BlogEntry>(comment.Item);

            string url = null;

            if (entry != null)
            {
                url = GetBlogEntryUrl(entry);

                if (!String.IsNullOrEmpty(url))
                {
                    url += $"#comment-{comment.id}";
                }
            }

            return url;
        }

        private const int BLOGS_PER_PAGE = 50;

        public static void GenerateBlogs()
        {
            string where = ContentLogic.GeneratePermissionWhere<Blog>();

            if(!String.IsNullOrEmpty(where))
            {
                where += " AND";
            }

            where += " (blog_count_entries + blog_count_entries_hidden) > 0";

            string query = $"SELECT * FROM {Blog.TableName} WHERE {where} ORDER BY {DB.GetDatabasePrefix<Blog>()}{ContentLogic.GetDatabaseColumnMap<Blog>()["Date"]} DESC";

            List<Blog> blogs = DB.Instance.GetData<Blog>(query);

            EventLogic.RaiseEvent(ProgressEventArgs.App, "blogs");
            EventLogic.RaiseEvent(ProgressEventArgs.Containers, blogs.Count.ToString());

            int blogsCompleted = 0;
            EventLogic.RaiseEvent(ProgressEventArgs.ContainersCompleted, blogsCompleted.ToString());

            int page = 1;

            int totalPages = (int)Math.Ceiling((decimal)blogs.Count / BLOGS_PER_PAGE);

            while (true)
            {
                List<Blog> currentBlogs = blogs.Skip((page - 1) * BLOGS_PER_PAGE).Take(BLOGS_PER_PAGE).ToList();

                if (currentBlogs.Count == 0 && page != 1)
                {
                    break;
                }

                GenerateBlogs(currentBlogs, page, totalPages, ref blogsCompleted);


                EventLogic.RaiseImportantEvent();

                page++;
            }

        }

        public static void UpdateBlogData()
        {
            DB.Instance.ExecuteNonQuery($"UPDATE {Blog.TableName} SET blog_count_entries = 0, blog_count_entries_hidden = 0, blog_count_comments = 0, blog_count_comments_hidden = 0");
            DB.Instance.ExecuteNonQuery($"UPDATE {BlogEntry.TableName} SET entry_num_comments = 0");

            string where = ContentLogic.GeneratePermissionWhere<BlogEntry>();

            if(!String.IsNullOrEmpty(where))
            {
                where = $"WHERE {where}";
            }

            string query = $"UPDATE {Blog.TableName} JOIN (SELECT entry_blog_id, MAX(entry_date) lastEntry, COUNT(*) entries FROM {BlogEntry.TableName} {where} GROUP BY entry_blog_id ) x ON x.entry_blog_id = blog_id SET blog_last_edate = lastEntry, blog_count_entries = entries";

            DB.Instance.ExecuteNonQuery(query);

            UpdateCommentCounts();

        }

        private static void UpdateCommentCounts()
        {
            //Blog
            string where = "";

            List<string> wheres = new List<string>();
            string entryWhere = ContentLogic.GeneratePermissionWhere<BlogEntry>("e");
            string commentWhere = ContentLogic.GeneratePermissionWhere<BlogComment>("c");

            if (!String.IsNullOrEmpty(entryWhere))
            {
                wheres.Add($"({entryWhere})");
            }

            if(!String.IsNullOrEmpty(commentWhere))
            {
                wheres.Add($"({commentWhere})");
            }

            if (wheres.Count > 0)
            {
                where = $"WHERE {String.Join(" AND ", wheres)}";
            }

            string query = $"UPDATE {Blog.TableName} JOIN (SELECT entry_blog_id, COUNT(*) comments FROM {BlogEntry.TableName} e JOIN {BlogComment.TableName} c ON c.comment_entry_id = e.entry_id {where} GROUP BY entry_blog_id) x ON x.entry_blog_id = blog_id SET blog_count_comments = comments";

            DB.Instance.ExecuteNonQuery(query);

            //Entry

            where = "";
            if(!String.IsNullOrEmpty(commentWhere))
            {
                where = $"WHERE {commentWhere}";
            }

            query = $"UPDATE {BlogEntry.TableName} JOIN (SELECT c.comment_entry_id, COUNT(*) comments FROM {BlogComment.TableName} c {where} GROUP BY c.comment_entry_id) x ON comment_entry_id = entry_id SET entry_num_comments = comments";

            DB.Instance.ExecuteNonQuery(query);
        }

        private static void GenerateBlogs(List<Blog> currentBlogs, int page, int totalPages, ref int blogsCompleted)
        {
            string html = "<div class='page-header'><h1>Blogs</h1></div>";

            html += Output.GeneratePagination("blog/{id}.html", page, totalPages, true);

            html += "<ul class='list-group'>";

            foreach (Blog blog in currentBlogs)
            {
                html += "<li class='list-group-item'><div class='row topic'>"
                      + $"<div class='col-md-8'><p><a href='{GetBlogUrl(blog)}'>{blog.Description}</a></p>";

                html += $"<p>By {MemberLogic.GetUrlHtml(blog.Author)}, {LangLogic.FormatDate(blog.Date, LangLogic.ShortDateFormat)}</p>"
                      + "</div>"
                      + $"<div class='col-md-2 text-right'><p>{LangLogic.FormatNumber(blog.count_entries, "entry", "entries")}</p><p>{LangLogic.FormatNumber(blog.count_comments, "comment", "comments")}</p></div>"
                      + $"<div class='col-md-2'>{LangLogic.FormatDate(blog.Date, LangLogic.ShortDateFormat)}</div>"
                      + "</div></li>" + Environment.NewLine;

            }

            html += "</ul>" + Environment.NewLine;

            html += Output.GeneratePagination("blog/{id}.html", page, totalPages, false);

            Output output = new Output()
            {
                Content = html,
                FileName = $"blog/{page}.html",
                FoldersDeep = 1,
                Title = "Blogs",
                Breadcrumbs = new List<Tuple<string, string, bool>>()
                {
                    Tuple.Create("Blogs", "blog/1.html", true)
                },
            };

            output.Generate();

            foreach (Blog blog in currentBlogs)
            {
                GenerateBlog(blog);
                EventLogic.RaiseEvent(ProgressEventArgs.ContainersCompleted, (++blogsCompleted).ToString());
            }


        }

        private static void GenerateBlog(Blog blog)
        {
            EventLogic.RaiseEvent(ProgressEventArgs.CurrentContainer, blog.Description);

            List<BlogEntry> entries = ContainerLogic.GetItemsWithPermission<BlogEntry>(blog.Id);

            string html = $"<div class='page-header'><h1>{blog.name}</h1></div>";

            html += "<ul class='list-group'>";

            foreach (BlogEntry entry in entries)
            {
                int lastCommentBy = entry.LastCommentBy.GetValueOrDefault(0);

                if (entry.num_comments == 0)
                {
                    lastCommentBy = blog.Author;
                }

                html += "<li class='list-group-item'><div class='row topic'>"
                      + $"<div class='col-md-8'><p><a href='{GetBlogEntryUrl(entry)}'>{entry.name}</a></p>";

                html += $"<p>By {MemberLogic.GetUrlHtml(entry.Author)}, {LangLogic.FormatDate(entry.Date, LangLogic.ShortDateFormat)}</p>"
                      + "</div>"
                      + $"<div class='col-md-2 text-right'>{LangLogic.FormatNumber(entry.NumComments.GetValueOrDefault(0), "comment", "comments")}</div>"
                      + $"<div class='col-md-2'><p>{MemberLogic.GetUrlHtml(lastCommentBy)}</p><p>{LangLogic.FormatDate(entry.Updated, LangLogic.ShortDateFormat)}</p></div>"
                      + "</div></li>" + Environment.NewLine;
            }

            html += "</ul>" + Environment.NewLine;

            Output output = new Output()
            {
                Content = html,
                FileName = GetBlogUrl(blog),
                FoldersDeep = 2,
                Title = blog.Title,
                Breadcrumbs = new List<Tuple<string, string, bool>>()
                {
                    Tuple.Create("Blogs", "blog/1.html", false),
                    Tuple.Create(blog.Title, GetBlogUrl(blog), true),
                },
                ActiveNav = "Blogs",
            };

            output.Generate();

            int entriesCompleted = 0;
            EventLogic.RaiseEvent(ProgressEventArgs.ItemsCompleted, entriesCompleted.ToString());
            EventLogic.RaiseEvent(ProgressEventArgs.Items, entries.Count.ToString());

            foreach (BlogEntry entry in entries)
            {
                GenerateEntry(blog, entry);

                EventLogic.RaiseEvent(ProgressEventArgs.ItemsCompleted, (++entriesCompleted).ToString());
            }
        }

        private static void GenerateEntry(Blog blog, BlogEntry entry)
        {
            EventLogic.RaiseEvent(ProgressEventArgs.CurrentItem, entry.Title);

            string html = $"<div class='page-header'><h1>{entry.Title}</h1></div>";

            string repHtml = "";

            int rep = ReputationLogic.GetTotalRepuation<BlogEntry>(entry.Id);

            if (rep != 0)
            {
                string repClass = rep > 0 ? "rep-positive" : "rep-negative";

                repHtml = $"<div class='col-md-12 text-right'><span class='rep {repClass}'>{rep}</span></div>";
            }

            html += $"<ul class='list-group'><li class='list-group-item'><div class='row post'>";
            html += $"<div class='col-md-2'><p>{MemberLogic.GetUrlHtml(entry.Author)}</p><p>{MemberLogic.GetGroupName(MemberLogic.GetMember(entry.Author).member_group_id, false)}</p></div>";
            html += $"<div class='col-md-10'>" +
                $"<div class='col-md-12 post-info-bar'>{LangLogic.FormatDate(entry.Date, LangLogic.LongDateFormat)}</div>" +
                $"<div class='col-md-12'>{entry.Content}</div>" +
                repHtml +
                $"</div>";

            html += "</div></li>" + Environment.NewLine  + "</ul>" + Environment.NewLine;

            List<BlogComment> comments = ItemLogic.GetCommentsWithPermission<BlogComment>(entry.Id);

            if(comments.Count > 0)
            {
                html += "<h3>Comments</h3><ul class='list-group'>";
            }

            foreach (BlogComment comment in comments)
            {
                repHtml = "";

                rep = ReputationLogic.GetTotalRepuation<BlogComment>(comment.Id);

                if (rep != 0)
                {
                    string repClass = rep > 0 ? "rep-positive" : "rep-negative";

                    repHtml = $"<div class='col-md-12 text-right'><span class='rep {repClass}'>{rep}</span></div>";
                }

                html += $"<li class='list-group-item'><div class='row post' id='comment-{comment.Id}'>";
                html += $"<div class='col-md-2'><p>{MemberLogic.GetUrlHtml(comment.Author.GetValueOrDefault())}</p><p>{MemberLogic.GetGroupName(MemberLogic.GetMember(comment.Author.GetValueOrDefault()).member_group_id, false)}</p></div>";
                html += $"<div class='col-md-10'>" +
                    $"<div class='col-md-12 post-info-bar'>{LangLogic.FormatDate(comment.Date, LangLogic.LongDateFormat)}<a class='pull-right' title='Link to this post' href='{GetBlogEntryCommentUrl(comment)}'>#</a></div>" +
                    $"<div class='col-md-12'>{comment.Content}</div>" +
                    repHtml +
                    $"</div>";

                html += "</div></li>" + Environment.NewLine;
            }

            if(comments.Count > 0)
            {
                html += "</ul>" + Environment.NewLine;
            }



            Output output = new Output()
            {
                Content = html,
                CleanupContent = true,
                DownloadImages = true,
                FileName = GetBlogEntryUrl(entry),
                FoldersDeep = 2,
                Title = entry.Title,
                Breadcrumbs = new List<Tuple<string, string, bool>>()
                {
                    Tuple.Create("Blogs", "blog/1.html", false),
                    Tuple.Create(blog.Title, GetBlogUrl(blog), false),
                    Tuple.Create(entry.Title, GetBlogEntryUrl(entry), true),
                },
                ActiveNav = "Blogs",
            };

            output.Generate();
            
        }

        public static List<Blog> GetBlogs(Member member)
        {
            string permissionWhere = ContentLogic.GeneratePermissionWhere<Blog>();

            if(!String.IsNullOrEmpty(permissionWhere))
            {
                permissionWhere = $"AND ({permissionWhere})";
            }

            return DB.Instance.GetData<Blog>($"SELECT * FROM {Blog.TableName} WHERE blog_count_entries > 0 {permissionWhere} AND {DB.GetDatabasePrefix<Blog>() + Blog.DatabaseColumnMap["Author"]} = {member.Id}");
        }
    }
}
