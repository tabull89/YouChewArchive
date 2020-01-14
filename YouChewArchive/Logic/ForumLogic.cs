using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YouChewArchive.Classes;
using YouChewArchive.Data;
using YouChewArchive.DataContracts;
using YouChewArchive.Logic;

namespace YouChewArchive
{
    public static class ForumLogic
    {
        private const int POSTS_PER_PAGE = 50;
        private const int TOPICS_PER_PAGE = 100;

        public static List<ForumContainer> GetAllForums()
        {
            HashSet<int> visibleIds = PermissionLogic.GetPermissions<Forum>();
            List<Forum> forums = DB.Instance.GetRecordsByIds<Forum>(visibleIds)
                                   .Where(f => !f.redirect_on)
                                   .ToList();

            Dictionary<int, ForumContainer> fcs = forums.ToDictionary(f => f.Id, f => new ForumContainer() { Forum = f });

            foreach(var kvp in fcs)
            {
                Forum forum = kvp.Value.Forum;

                if(forum.parent_id != -1 && forum.parent_id.HasValue)
                {
                    ForumContainer container;

                    if(fcs.TryGetValue(forum.parent_id.Value, out container))
                    {
                        container.ChildForums.Add(kvp.Value);
                        
                        container.ChildForums = container.ChildForums.OrderBy(c => c.Forum.position).ToList();
                    }
                }
            }

            
            return fcs.Values.Where(f => f.Forum.parent_id == -1 || !f.Forum.parent_id.HasValue).OrderBy(f => f.Forum.position).ToList();
        }

        public static List<int> GetAllForumIds()
        {
            List<ForumContainer> forums = GetAllForums();
            List<int> ids = new List<int>();

            foreach (var fc in forums)
            {
                ids.AddRange(GetAllForumIds(fc));
            }

            return ids;
            
        }

        private static List<int> GetAllForumIds(ForumContainer fc)
        {
            List<int> ids = new List<int>();

            if (fc.Forum.parent_id != -1)
            {
                ids.Add(fc.Forum.Id);
            }

            foreach (var child in fc.ChildForums)
            {
                ids.AddRange(GetAllForumIds(child));
            }
            
            return ids;

        }

        public static void GenerateBoardIndex()
        {
            EventLogic.RaiseEvent(ProgressEventArgs.App, "Board Index");

            List<ForumContainer> fcs = GetAllForums();

            string html = $"<div class='page-header'><h1>Forums</h1></div>";

            html += GenerateForumsOnBoardIndex(fcs);
            html += GenerateBoardIndexStats(fcs);

            Output output = new Output()
            {
                Content = html,
                FoldersDeep = 1,
                FileName = @"\forum\index.html",
                Title = "Forums",
                Breadcrumbs = new List<Tuple<string, string, bool>>()
                {
                    Tuple.Create("Forums", "forum/index.html", true),
                },
            };

            output.Generate();
           
        }

        private static string GenerateBoardIndexStats(List<ForumContainer> fcs)
        {
            Forum totalForum = new Forum()
            {
                posts = 0,
                topics = 0,
            };

            foreach(ForumContainer fc in fcs)
            {
                Forum subTotalForum = GetForumCounts(fc.ChildForums);

                totalForum.posts += subTotalForum.posts;
                totalForum.topics += subTotalForum.topics;
            }

            string html = "<ul class='list-group'>";

            html += $"<li class='list-group-item forum-category'><div class='row'><div class='col-xs-12'>Forum Statistics</div></div></li>" +
                $"<li class='list-group-item'><div class='row'>" +
                $"<div class='col-md-4 text-center'><strong>{LangLogic.FormatNumber(MemberLogic.GetContributedMemberCount(), "Member", "Members")}</strong></div>" +
                $"<div class='col-md-4 text-center'><strong>{LangLogic.FormatNumber(totalForum.topics, "Topic", "Topics")}</strong></div>" +
                $"<div class='col-md-4 text-center'><strong>{LangLogic.FormatNumber(totalForum.posts, "Post", "Posts")}</strong></div>" +
                $"</div></li>"; 

            html += "</ul>";

            return html;
        }

        private static Forum GetForumCounts(List<ForumContainer> childForums)
        {
            Forum subTotalForum = new Forum()
            {
                posts = 0,
                topics = 0,
            };

            foreach(ForumContainer child in childForums)
            {
                Forum childTotalForum = GetForumCounts(child.ChildForums);

                subTotalForum.posts += child.Forum.posts + childTotalForum.posts;
                subTotalForum.topics += child.Forum.topics + childTotalForum.topics;
            }

            return subTotalForum;
        }

        public static string GenerateForumsOnBoardIndex(List<ForumContainer> fcs, int offset = 0)
        {
            string html = "";

            bool isCategory = offset == 0;

            foreach (ForumContainer fc in fcs)
            {
                EventLogic.RaiseEvent(ProgressEventArgs.CurrentContainer, fc.Forum.Description);

                string rowStyle = offset == 0 ? " forum-category" : "";

                if (isCategory)
                {
                    html += "<ul class='list-group'>";
                }

                html += $"<li class='list-group-item{rowStyle}'><div class='row'>";

                if (isCategory)
                {
                    html += $"<div class='col-xs-12'>{fc.Forum.Description}</div>";
                }
                else
                {
                    RecalculateCounts(fc.Forum);


                    html += $"<div class='col-md-8'><a href='{fc.Forum.Url}' class='forum-offset-{offset}'>{fc.Forum.Description}</a></div>" +
                        $"<div class='col-md-2 text-right'>{LangLogic.FormatNumber(fc.Forum.topics, "topic", "topics")}</div>" +
                        $"<div class='col-md-2 text-right'>{LangLogic.FormatNumber(fc.Forum.posts, "post", "posts")}</div>";
                }

                html += $@"</div></li>" + Environment.NewLine;

                if (fc.ChildForums.Count > 0)
                {
                    html += GenerateForumsOnBoardIndex(fc.ChildForums, offset + 1);
                }

                if (isCategory)
                {
                    html += "</ul>" + Environment.NewLine;
                }
            }

            return html;
        }

        private static void RecalculateCounts(Forum forum)
        {
            string topicsQuery = $"SELECT COUNT(*) FROM {Topic.TableName} WHERE forum_id = {forum.Id} AND {ContentLogic.GeneratePermissionWhere<Topic>()}";
            forum.topics = DB.Instance.ExecuteScalar<int>(topicsQuery);

            string postsQuery = $"SELECT COUNT(*) FROM {Post.TableName} p JOIN {Topic.TableName} t on p.topic_id = t.tid WHERE ({ContentLogic.GeneratePermissionWhere<Topic>("t")}) AND ({ContentLogic.GeneratePermissionWhere<Post>("p")}) AND t.forum_id = {forum.Id}";

            if(!AppLogic.CanViewHidden())
            {
                List<int> excludeMembers = MemberLogic.GetExcludedMembers<Forum>(forum.Id);

                if(excludeMembers.Any())
                {
                    postsQuery += $" AND p.author_id NOT IN ({String.Join(",", excludeMembers)})";
                }
            }

            forum.posts = DB.Instance.ExecuteScalar<int>(postsQuery);

        }

        public static string GenerateForumsOnBoardIndex_OLD(List<ForumContainer> fcs, int offset=0)
        {
            string html = "";

            bool isCategory = offset == 0;

            foreach(ForumContainer fc in fcs)
            {
                string rowStyle = offset == 0 ? "forum-category" : "forum";

                if(isCategory)
                {
                    html += "<div class='forum-group'>";
                }

                html += $"<div class='row {rowStyle}'>";

                if (isCategory)
                {
                    html += $"<div class='col-xs-12'>{fc.Forum.Description}</div>";
                }
                else
                {
                    html += $"<div class='col-md-8'><a href='{fc.Forum.Url}' class='forum-offset-{offset}'>{fc.Forum.Description}</a></div>" +
                        $"<div class='col-md-2 text-right'>{LangLogic.FormatNumber(fc.Forum.topics, "topic", "topics")}</div>" +
                        $"<div class='col-md-2 text-right'>{LangLogic.FormatNumber(fc.Forum.posts, "post", "posts")}</div>";
                }
                   
                html += $@"</div>";

                if (fc.ChildForums.Count > 0)
                {
                    html += GenerateForumsOnBoardIndex_OLD(fc.ChildForums, offset + 1);
                }

                if(isCategory)
                {
                    html += "</div>";
                }
            }

            return html;
        }

        public static void GenerateTopicList()
        {
            EventLogic.RaiseEvent(ProgressEventArgs.App, "Forums");
            ForumsCompleted = 0;

            List<ForumContainer> fcs = GetAllForums();

            EventLogic.RaiseEvent(ProgressEventArgs.Containers, CountForums(fcs).ToString());


            foreach(ForumContainer fc in fcs)
            {
                GenerateTopicList(fc);
            }

        }

        private static int CountForums(List<ForumContainer> forums)
        {
            int count = 0;

            foreach(var forum in forums)
            {
                if(forum.Forum.parent_id != -1)
                {
                    count++;
                }

                count += CountForums(forum.ChildForums);
                
            }

            return count;
        }

        private static Dictionary<int, Forum> forumMap = new Dictionary<int, Forum>();

        public static Forum GetForum(Topic topic)
        {
            return GetForum(topic.Container);
        }

        public static Forum GetForum(int forumId)
        {
            Forum forum;

            if(!forumMap.TryGetValue(forumId, out forum))
            {
                forum = DB.Instance.GetRecordById<Forum>(forumId);
                forumMap.Add(forumId, forum);
            }

            return forum;
        }

        public static Forum GetParent(Forum forum)
        {
            if(forum.parent_id == -1 || !forum.parent_id.HasValue)
            {
                return null;
            }

            return GetForum(forum.parent_id.Value);
        }

        private static int ForumsCompleted = 0;

        public static void GenerateTopicList(ForumContainer fc)
        {
            if(fc.Forum.parent_id != -1)
            {
                GenerateTopicList(fc.Forum);
                EventLogic.RaiseEvent(ProgressEventArgs.ContainersCompleted, (++ForumsCompleted).ToString());
            }

            foreach(ForumContainer child in fc.ChildForums)
            {
                GenerateTopicList(child);
            }

        }

        public static void GenerateTopicList(Forum forum)
        {
            using (StopWatch sw = new StopWatch("\t" + forum.Description))
            {

                List<Topic> topics = ContainerLogic.GetItemsWithPermission<Topic>(forum.id);

                EventLogic.RaiseEvent(ProgressEventArgs.Items, topics.Count.ToString());

                int topicsCompleted = 0;
                EventLogic.RaiseEvent(ProgressEventArgs.ItemsCompleted, topicsCompleted.ToString());

                int page = 1;

                int totalPages = (int)Math.Ceiling((decimal)topics.Count / TOPICS_PER_PAGE);

                while (true)
                {
                    List<Topic> currentTopics = topics.Skip((page - 1) * TOPICS_PER_PAGE).Take(TOPICS_PER_PAGE).ToList();

                    if (currentTopics.Count == 0 && page != 1)
                    {
                        break;
                    }

                    GenerateTopicList(forum, currentTopics, page, totalPages, ref topicsCompleted);

                    //EventLogic.RaiseEvent(ProgressEventArgs.ItemsCompleted, (((page - 1) * TOPICS_PER_PAGE) + currentTopics.Count).ToString());
                    EventLogic.RaiseImportantEvent();

                    page++;
                }
            }
        }

        private static string ShortenSeo(string seo)
        {
            seo = seo.Replace("?", "")
                     .Replace("\\", "");

            if(seo == "\u001c\u001c701444\u001f982342ÿà-\v\b-9\u0001\u0001\u0001\u0011-ÿä")
            {
                seo = "ya";
            }
            
            if(seo.Length > 200)
            {
                seo = seo.Substring(0, 200);
            }

            return seo;
        }

        public static string GetTopicUrl(Forum forum, Topic topic, int pageNumber = 1)
        {
            return $"forum/{forum.Id}-{forum.name_seo}/{topic.Id}-{ShortenSeo(topic.title_seo)}/{pageNumber}.html";
        }

        public static string GetPostUrl(Forum forum, Topic topic, Post post, List<Post> allPosts = null)
        {
            int pageNumber = GetPostPage(post.Id, allPosts);

            return $"forum/{forum.Id}-{forum.name_seo}/{topic.Id}-{ShortenSeo(topic.title_seo)}/{pageNumber}.html#post-{post.Id}";
        }

        private static void GenerateTopicList(Forum forum, List<Topic> topics, int page, int totalPages, ref int topicsCompleted)
        {
            EventLogic.RaiseEvent(ProgressEventArgs.CurrentContainer, forum.Description);

            StringBuilder sb = new StringBuilder();

            sb.Append($"<div class='page-header'><h1>{forum.Description}</h1></div>");

            sb.Append(Output.GeneratePagination($"forum/{forum.Id}-{forum.name_seo}/{{id}}.html", page, totalPages, true));

            sb.Append("<ul class='list-group'>");

            foreach (Topic topic in topics)
            {
                GeneratePosts(topic);
                EventLogic.RaiseEvent(ProgressEventArgs.ItemsCompleted, (++topicsCompleted).ToString());

                string icons = "";

                if(topic.State == "closed")
                {
                    icons += "<span class='glyphicon glyphicon-lock'></span>";
                }

                if(topic.Pinned.GetValueOrDefault())
                {
                    icons += "<span class='glyphicon glyphicon-pushpin'></span>";
                }

                sb.Append("<li class='list-group-item'><div class='row topic'>")
                  .Append($"<div class='col-md-8'><p><a href='{GetTopicUrl(forum, topic)}'>{icons}{topic.title}</a></p>");

                if(!String.IsNullOrEmpty(topic.topic_description))
                {
                    sb.Append($"<small>{topic.topic_description}</small>");
                }

                sb.Append($"<p>By {MemberLogic.GetUrlHtml(topic.Author)}, {LangLogic.FormatDate(topic.Date, LangLogic.ShortDateFormat)}</p>")
                  .Append("</div>")
                  .Append($"<div class='col-md-2 text-right'>{LangLogic.FormatNumber(Math.Max(topic.posts.GetValueOrDefault() - 1, 0), "reply", "replies")}</div>")
                  .Append($"<div class='col-md-2'><p>{MemberLogic.GetUrlHtml(topic.LastCommentBy)}</p><p>{LangLogic.FormatDate(topic.Updated, LangLogic.ShortDateFormat)}</p></div>")
                  .AppendLine("</div></li>");
                
            }

            sb.AppendLine("</ul>");

            sb.Append(Output.GeneratePagination($"forum/{forum.Id}-{forum.name_seo}/{{id}}.html", page, totalPages, false));

            Output output = new Output()
            {
                Content = sb.ToString(),
                FileName = $@"\forum\{forum.id}-{forum.name_seo}\{page}.html",
                FoldersDeep = 2,
                Title = forum.Description,
                ActiveNav = "Forums",
            };

            output.Breadcrumbs.Add(Tuple.Create("Forums", "forum/index.html", false));

            output.Breadcrumbs.AddRange(GetForumBreadcrumbs(forum, true));

            output.Generate();
        }

        public static string GetPostUrl(Post post)
        {
            string url = null;

            if(post != null)
            {
                Topic topic = DB.Instance.GetRecordById<Topic>(post.Item);

                if(topic != null)
                {
                    Forum forum = GetForum(topic.Container);

                    if(forum != null)
                    {
                        return GetPostUrl(forum, topic, post);
                    }
                }
            }

            return url;
        }

        private static List<Tuple<string, string, bool>> GetForumBreadcrumbs(Forum forum, bool currentIsActive)
        {
            List<Tuple<string, string, bool>> forums = new List<Tuple<string, string, bool>>();
            Forum currentForum = forum;


            while (true)
            {
                if (currentForum.parent_id == -1)
                {
                    break;
                }

                bool active = false;

                if (currentForum == forum && currentIsActive)
                {
                    active = true;
                }

                forums.Add(Tuple.Create(currentForum.Description, currentForum.Url, active));

                currentForum = GetParent(currentForum);

            }

            forums.Reverse();

            return forums;
        }

        private static Dictionary<int, int> postPageMap = new Dictionary<int, int>();
        

        public static int GetPostPage(int postId, List<Post> fullTopicInOrder = null)
        {
            int pageNumber;

            if(!postPageMap.TryGetValue(postId, out pageNumber))
            {
                int count = 0;

                if (fullTopicInOrder != null)
                {
                    Post post = fullTopicInOrder.FirstOrDefault(p => p.Id == postId);

                    if(post != null)
                    {
                        count = fullTopicInOrder.Where(p => p.Date <= post.Date).Count();
                    }
                    else
                    {
                        count = GetPostNumber(postId);
                    }
                }
                else
                {
                    count = GetPostNumber(postId);
                }

                pageNumber = (int)Math.Ceiling(count / (decimal)POSTS_PER_PAGE);

                postPageMap.Add(postId, pageNumber);
            }

            return pageNumber;
        }

        public static void GenerateHighestReputation()
        {
            EventLogic.RaiseEvent(ProgressEventArgs.App, "Highest Repuation");

            List<HighestReputation> rep = ReputationLogic.GetHighestReputationPosts(100);

            int totalPages = (int)Math.Ceiling((decimal)rep.Count / POSTS_PER_PAGE);

            int page = 1;

            while (true)
            {
                List<HighestReputation> currentPosts = rep.Skip((page - 1) * POSTS_PER_PAGE).Take(POSTS_PER_PAGE).ToList();

                if (currentPosts.Count == 0 && page != 1)
                {
                    break;
                }

                GenerateHighestReputationPosts(currentPosts, page, totalPages);

                page++;
            }

        }

        private static void GenerateHighestReputationPosts(List<HighestReputation> currentPosts, int page, int totalPages)
        {
            string html = $"<div class='page-header'><h1>Highest Rated Posts</h1></div>";

            html += Output.GeneratePagination("extra/highest/{id}.html", page, totalPages, true);

            html += "<ul class='list-group'>";

            foreach(HighestReputation rep in currentPosts)
            {
                string repHtml = "";

                if (rep.rep != 0)
                {
                    string repClass = rep.rep > 0 ? "rep-positive" : "rep-negative";

                    repHtml = $"<div class='col-md-12 text-right'><span class='rep {repClass}'>{rep.rep}</span></div>";
                }

                Post post = DB.Instance.GetRecordById<Post>(rep.type_id);
                Topic topic = DB.Instance.GetRecordById<Topic>(post.Item);


                html += $"<li class='list-group-item'><div class='row post' id='post-{post.Id}'>";
                html += $"<div class='col-md-2'><p>{MemberLogic.GetUrlHtml(post.Author)}</p><p>{MemberLogic.GetGroupName(MemberLogic.GetMember(post.Author).member_group_id, false)}</p></div>";
                html += $"<div class='col-md-10'>" +
                    $"<div class='col-md-12 post-info-bar'><div class='col-md-3'>{LangLogic.FormatDate(post.Date, LangLogic.LongDateFormat)}</div><div class='col-md-9'><a class='pull-right' title='Link to this post' href='{GetPostUrl(post)}'>{topic.Title}</a></div></div>" +
                    $"<div class='col-md-12'>{post.Content}</div>" +
                    repHtml +
                    $"</div>";

                html += "</div></li>" + Environment.NewLine;
            }

            html += "</ul>" + Environment.NewLine;


            html += Output.GeneratePagination("extra/highest/{id}.html", page, totalPages, false);

            Output output = new Output()
            {
                Content = html,
                CleanupContent = true,
                DownloadImages = true,
                FileName = $"extra/highest/{page}.html",
                FoldersDeep = 2,
                Title = "Highest Rated Posts",
            };

            output.Generate();

        }

        public static void GeneratePosts(Topic topic)
        {
            EventLogic.RaiseEvent(ProgressEventArgs.CurrentItem, $"{topic.Id}: {topic.title}");
            EventLogic.RaiseEvent(ProgressEventArgs.CommentsCompleted, "0");

            List<Post> posts = ItemLogic.GetCommentsWithPermission<Post>(topic.Id);
            Forum forum = GetForum(topic.Container);

            // Update Topic info
            topic.posts = posts.Count();
            topic.last_poster_id = posts.Any() ? posts[posts.Count - 1].Author : 0;
            topic.last_post = posts.Any() ? posts[posts.Count - 1].Date.GetValueOrDefault() : topic.last_post;

            int totalPages = (int)Math.Ceiling((decimal)posts.Count / POSTS_PER_PAGE);

            EventLogic.RaiseEvent(ProgressEventArgs.Comments, posts.Count.ToString());
            

            int page = 1;

            while(true)
            {
                List<Post> currentPosts = posts.Skip((page - 1) * POSTS_PER_PAGE).Take(POSTS_PER_PAGE).ToList();

                if(currentPosts.Count == 0 && page != 1)
                {
                    break;
                }

                GeneratePosts(currentPosts, posts, topic, forum, page, totalPages);

                EventLogic.RaiseEvent(ProgressEventArgs.CommentsCompleted, (((page - 1) * POSTS_PER_PAGE) + currentPosts.Count).ToString());

                page++;
            }

        }

        private static void GeneratePosts(List<Post> currentPosts, List<Post> allPosts, Topic topic, Forum forum, int page, int totalPages)
        {
            string desc = !String.IsNullOrEmpty(topic.topic_description) ? $"<h4>{topic.topic_description}</h4>" : "";

            string html = $"<div class='page-header'><h1>{topic.Title}</h1>{desc}</div>";

            html += Output.GeneratePagination($"forum/{forum.Id}-{forum.name_seo}/{topic.Id}-{ShortenSeo(topic.title_seo)}/{{id}}.html", page, totalPages, true);

            string topicUrl = GetTopicUrl(forum, topic, page);

            html += "<ul class='list-group'>";

            foreach(Post post in currentPosts)
            {
                string repHtml = "";

                int rep = ReputationLogic.GetTotalRepuation<Post>(post.Id);

                if(rep != 0)
                {
                    string repClass = rep > 0 ? "rep-positive" : "rep-negative";

                    repHtml = $"<div class='col-md-12 text-right'><span class='rep {repClass}'>{rep}</span></div>";
                }

                html += $"<li class='list-group-item'><div class='row post' id='post-{post.Id}'>";
                html += $"<div class='col-md-2'><p>{MemberLogic.GetUrlHtml(post.Author)}</p><p>{MemberLogic.GetGroupName(MemberLogic.GetMember(post.Author).member_group_id, false)}</p></div>";
                html += $"<div class='col-md-10'>" +
                    $"<div class='col-md-12 post-info-bar'>{LangLogic.FormatDate(post.Date, LangLogic.LongDateFormat)}<a class='pull-right' title='Link to this post' href='{topicUrl}#post-{post.Id}'>#</a></div>" +
                    $"<div class='col-md-12'>{post.Content}</div>" +
                    repHtml+
                    $"</div>";

                html += "</div></li>" + Environment.NewLine;
            }

            html += "</ul>" + Environment.NewLine;

            html += Output.GeneratePagination($"forum/{forum.Id}-{forum.name_seo}/{topic.Id}-{ShortenSeo(topic.title_seo)}/{{id}}.html", page, totalPages, false);

            Output output = new Output()
            {
                Content = html,
                FileName = $"forum/{forum.Id}-{forum.name_seo}/{topic.Id}-{ShortenSeo(topic.title_seo)}/{page}.html",
                FoldersDeep = 3,
                Title = topic.Title,
                CleanupContent = true,
                DownloadImages = ShouldDownloadImages(topic),
                AllPostsInTopic = allPosts,
                Breadcrumbs = new List<Tuple<string, string, bool>>()
                {
                    Tuple.Create("Forums", "forum/index.html", false),
                },
                ActiveNav = "Forums",
            };

            output.Breadcrumbs.AddRange(GetForumBreadcrumbs(forum, false));

            output.Generate();
        }

        private static Dictionary<Tuple<int, int?, int?>, List<Member>> participants = new Dictionary<Tuple<int, int?, int?>, List<Member>>();

        public static List<Member> GetParticipantsOfForum(Forum forum, int? minDate, int? maxDate)
        {
            List<Member> members;

            Tuple<int, int?, int?> tuple = Tuple.Create(forum.Id, minDate, maxDate);

            if(!participants.TryGetValue(tuple, out members))
            {
                string query = $"SELECT DISTINCT p.author_id AS int_val FROM {Topic.TableName} t JOIN {Post.TableName} p ON p.topic_id = t.tid WHERE t.forum_id = {forum.Id}";

                if(minDate.HasValue)
                {
                    query += $" AND p.post_date >= {minDate}";
                }

                if(maxDate.HasValue)
                {
                    query += $" AND p.post_date <= {maxDate}";
                }

                members = DB.Instance.GetData<SingleInteger>(query).Select(i => MemberLogic.GetMember(i.int_val)).ToList();

                participants.Add(tuple, members);
            }

            return members;
        }

        public static bool WasParticipant(Member member, int forum_id, int? minDate = null, int? maxDate = null)
        {
            Forum forum = GetForum(forum_id);

            if(forum == null)
            {
                return false;
            }

            return GetParticipantsOfForum(forum, minDate, maxDate).Any(m => m.Id == member.Id);
                
        }

        private static bool ShouldDownloadImages(Topic topic)
        {
            if(topic.forum_id == 73 || topic.forum_id == 52 || topic.forum_id == 56)
            {
                return true;
            }

            switch(topic.Id)
            {
                case 108215:
                case 76781:
                    return true;
            }

            return false;
        }

        public static int GetPostNumber(int postId)
        {
            Post post = DB.Instance.GetRecordById<Post>(postId);

            string query = $"SELECT COUNT(*) FROM {Post.TableName} WHERE topic_id = {post.Item} AND post_date <= {post.Date} AND ({ContentLogic.GeneratePermissionWhere<Post>()})";

            return DB.Instance.ExecuteScalar<int>(query, null, 300);
        }

        public static Post GetPostFromNumber(Topic topic, int postNumber)
        {
            return ItemLogic.GetCommentsWithPermission<Post>(topic.Id, 1, postNumber).FirstOrDefault();
        }
    }
}
