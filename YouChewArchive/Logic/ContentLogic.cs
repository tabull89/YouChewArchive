using HtmlAgilityPack;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using YouChewArchive.Classes;
using YouChewArchive.Data;
using YouChewArchive.DataContracts;
using YouChewArchive.Logic;

namespace YouChewArchive
{
    public static class ContentLogic
    {
        public static Dictionary<string, string> GetDatabaseColumnMap(Type type)
        {
            Dictionary<string, string> databaseColumnMap = new Dictionary<string, string>();

            if (AppLogic.HasStaticField(type, "DatabaseColumnMap"))
            {
                databaseColumnMap = AppLogic.GetStaticField<Dictionary<string, string>>(type, "DatabaseColumnMap");
            }

            return databaseColumnMap;
        }
        public static Dictionary<string, string> GetDatabaseColumnMap<T>()
        {
            return GetDatabaseColumnMap(typeof(T));
        }

        private static Dictionary<string, int> newsTopicMap = new Dictionary<string, int>();
        static ContentLogic()
        {
            newsTopicMap.Add("a-recreational-guide-to-poop-tennis", 75999);
            newsTopicMap.Add("as-official-as-it-can-bethe-december-top-10", 75715);
            newsTopicMap.Add("bigmarioandtailsfan-interview", 76038);
            newsTopicMap.Add("celebrating-the-birthday-of-dr-suess-the-way-only-poopers-can", 76160);
            newsTopicMap.Add("deepercutt-interview", 75593);
            newsTopicMap.Add("flash-poop-now-a-sub-genre", 76121);
            newsTopicMap.Add("girlaphs-legal-victory-over-cd-i-aids-case", 72515);
            newsTopicMap.Add("grand-theft-poop-by-christoph", 76136);
            newsTopicMap.Add("greenalink-interview", 72562);
            newsTopicMap.Add("high-watermark-for-meta-poop", 72641);
            newsTopicMap.Add("iconoclasm-planet-freedom-and-youchewpoop", 76169);
            newsTopicMap.Add("i-know-the-story-behind-poop-lolocaust-survivor-speaks-out", 72925);
            newsTopicMap.Add("interview-with-the-pope", 72928);
            newsTopicMap.Add("kajetokun-interview", 72533);
            newsTopicMap.Add("konxii-kod-from-youtube", 76166);
            newsTopicMap.Add("marc-graue-interview", 76039);
            newsTopicMap.Add("mrdrunkenfox-interview", 75589);
            newsTopicMap.Add("new-major-campaign-begins-the-perfect-poop", 76120);
            newsTopicMap.Add("youchew-poopcast-episode-3-what-the-fuck-was-that", 76285);
            newsTopicMap.Add("nextg-poop-project-added-to-directory", 72537);
            newsTopicMap.Add("nuthead-review-of-thereisnospork303s-o-hai-i-kiled-ur-presidnet", 76138);
            newsTopicMap.Add("poopers-unite-finishes-as-chain-poop-begins", 72612);
            newsTopicMap.Add("poop-reviews-spelling-bees-are-serious-shit-o-by-deepercutt", 94177);
            newsTopicMap.Add("seductivebaz-scotsman-or-fraud", 76159);
            newsTopicMap.Add("thebigl1-successfully-create-post-aids-poop", 75587);
            newsTopicMap.Add("the-poop-tennis-cafe-open-all-day-and-all-night", 76137);
            newsTopicMap.Add("the-poop-tennis-league-as-competetive-as-poop-tennis-can-get", 76000);
            newsTopicMap.Add("the-true-cause-of-global-warming", 76161);
            newsTopicMap.Add("the-truth-about-mrsimon", 76157);
            newsTopicMap.Add("the-youtube-poop-lolocaust", 72617);
            newsTopicMap.Add("truly-what-is-aids-by-christoph", 76132);
            newsTopicMap.Add("unclechuckths-poop-tennis-review-furnessly-vs-misselaineous10", 76140);
            newsTopicMap.Add("yaminomalex-interview", 72472);
            newsTopicMap.Add("youtube-kos-call-for-inclusion-on-the-news-page", 76165);
            newsTopicMap.Add("youtube-poop-does-cd-i-policy-u-turn", 72510);
            newsTopicMap.Add("anniversary-interview-scimath12", 76235);
            newsTopicMap.Add("art-staff-update-day-3", 76478);
            newsTopicMap.Add("dukeoffortuneman-interview", 76226);
            newsTopicMap.Add("hope-after-all-in-the-new-generation-a-critical-analysis-of-mastergwos-poops", 76316);
            newsTopicMap.Add("its-all-about-the-news-part-2-rabbit-boogaloo", 76278);
            newsTopicMap.Add("pooper-round-table-discussion-i-youtube-suspensions-and-conrad-slater-an-aftermath", 76237);
            newsTopicMap.Add("pope-game-reviews-super-mario-galaxy", 76307);
            newsTopicMap.Add("rabbitsnores-poop-tennis-review", 73921);
            newsTopicMap.Add("sonicnerd23s-poop-tennis-review-nuthead-el-majestico-vs-strong414bad", 76228);
            newsTopicMap.Add("the-youchew-poop-comic-gallery", 76196);
            newsTopicMap.Add("article-about-youtube-poop-on-squidcancom", 76170);
            newsTopicMap.Add("animated-blob-of-slime-found-shot-in-colorado-vacation-home", 76322);


        }

        public static void GenerateHomePage()
        {
            Output output = new Output()
            {
                ContentTemplate = "Home",
                FoldersDeep = 0,
                FileName = "index.html",
                Title = "Home",
            };

            output.Generate();
        }

        public static void GenerateSearchPage()
        {
            Output output = new Output()
            {
                ContentTemplate = "Search",
                FoldersDeep = 0,
                FileName = "search.html",
                Title = "Search",
                ActiveNav = "search.html",
            };

            output.Generate();
        }

        public static string GeneratePermissionWhere<T>(string alias = "")
        {
            Dictionary<string, string> databaseColumnMap = GetDatabaseColumnMap<T>();
            string databasePrefix = DB.GetDatabasePrefix<T>();

            if(!String.IsNullOrEmpty(alias))
            {
                alias += ".";
            }

            string query = "";

            if (!AppLogic.CanViewHidden())
            {
                if (databaseColumnMap.ContainsKey("Approved"))
                {
                    query += $" AND {alias}{databasePrefix}{databaseColumnMap["Approved"]} = 1";
                }

                if (databaseColumnMap.ContainsKey("Hidden"))
                {
                    query += $" AND {alias}{databasePrefix}{databaseColumnMap["Hidden"]} = 0";
                }

                Type type = typeof(T);

                if(type == typeof(Blog))
                {
                    query += " AND blog_social_group IS NULL";
                }

                if (type == typeof(BlogEntry))
                {
                    query += " AND entry_status='published'";
                }

                string memberFilterWhere = GetMemberFilterWhere<T>(alias);

                if(!String.IsNullOrEmpty(memberFilterWhere))
                {
                    query += " AND " + memberFilterWhere;
                }
            }

            if (databaseColumnMap.ContainsKey("Approved"))
            {
                query += $" AND {alias}{databasePrefix}{databaseColumnMap["Approved"]} <> -2";
            }

            if (databaseColumnMap.ContainsKey("Hidden"))
            {
                query += $" AND {alias}{databasePrefix}{databaseColumnMap["Hidden"]} <> -2";
            }


            string excluded = GetExcludedIdsWhere<T>(alias);

            if(!String.IsNullOrEmpty(excluded))
            {
                query += $" AND {excluded}";
            }

            if (query.Length > 0)
            {
                query = query.Substring(" AND ".Length);
            }

            return query;
        }

        private static string GetMemberFilterWhere<T>(string alias)
        {
            Dictionary<string, string> columnMap = GetDatabaseColumnMap<T>();

            if(columnMap == null || !columnMap.ContainsKey("Author"))
            {
                return "";
            }

            Type type = typeof(T);

            bool isTopic = type == typeof(Topic);
            bool isPost = type == typeof(Post);

            List<string> where = new List<string>();
            string databasePrefix = DB.GetDatabasePrefix<T>();

            if(!String.IsNullOrEmpty(alias))
            {
                databasePrefix = alias + (alias.EndsWith(".") ? "" : ".") + databasePrefix;
            }

            foreach(MemberFilter filter in MemberLogic.GetMemberFilters())
            {
                string membersWhere = $"{databasePrefix}{columnMap["Author"]} <> {filter.MemberId} OR ({databasePrefix}{columnMap["Author"]} = {filter.MemberId} AND NOT (";

                List<string> whereList = new List<string>();
                List<int> containerIds = new List<int>();
                List<int> itemIds = new List<int>();

                if (isTopic)
                {
                    containerIds = filter.GetExcludedIds<Forum>();
                }

                if(isPost)
                {
                    itemIds = filter.GetExcludedIds<Topic>();
                }

                if (containerIds.Any())
                {
                    whereList.Add($"{databasePrefix}{columnMap["Container"]} IN ({String.Join(",", containerIds)})");
                }

                if(itemIds.Any())
                {
                    whereList.Add($"{databasePrefix}{columnMap["Item"]} IN ({String.Join(",", itemIds)})");
                }

                if (columnMap.ContainsKey("Date"))
                {
                    if (filter.minDate.HasValue)
                    {
                        whereList.Add($"{databasePrefix}{columnMap["Date"]} < {filter.minDate.Value}");
                    }

                    if (filter.maxDate.HasValue)
                    {
                        whereList.Add($"{databasePrefix}{columnMap["Date"]} > {filter.maxDate.Value}");
                    }
                }

                if(whereList.Any())
                {
                    membersWhere += $"{String.Join(" OR ", whereList)}))";

                    where.Add($"({membersWhere})");
                }

            }

            if(where.Any())
            {
                return $"({String.Join(" AND ", where)})";
            }

            return "";
        }

        private static Dictionary<Type, List<int>> excludedMap = null;

        public static string GetExcludedIdsWhere<T>(string alias="")
        {
            if (!AppLogic.CanViewHidden())
            {

                if (excludedMap == null)
                {
                    excludedMap = new Dictionary<Type, List<int>>();
                    excludedMap.Add(typeof(Topic), new List<int>()
                        {
                            80226, 106369, 453, 120319, 21559, 114749, 21334, 113983, 120589, 120586, 120583, 51372,
                        });
                }

                List<int> ids;

                if (excludedMap.TryGetValue(typeof(T), out ids))
                {
                    if (!String.IsNullOrEmpty(alias) && !alias.EndsWith("."))
                    {
                        alias += ".";
                    }

                    if (ids.Count > 0)
                    {
                        string idColumn = AppLogic.GetStaticField<T, string>("DatabaseColumnId");

                        return $"{alias}{DB.GetDatabasePrefix<T>()}{idColumn} NOT IN ({String.Join(",", ids)})";
                    }
                }
            }

            return "";
        }

        public static string TransformContent(string html, List<Post> posts, int? pmMember, bool downloadImages, string baseUrl)
        {
            HtmlDocument doc = new HtmlDocument();
            doc.LoadHtml(html);

            HtmlNode contentNode = doc.GetElementbyId("content");

            ConvertImages(contentNode, pmMember, downloadImages);
            ConvertLinks(contentNode, pmMember, baseUrl);
            MakeEmbedsResponseive(contentNode);
            ConvertInternalEmbedsToLinks(contentNode, pmMember, baseUrl);
            ConvertQuotes(contentNode, posts, pmMember, baseUrl);
            ConvertSpoilers(contentNode);
            


            return doc.DocumentNode.OuterHtml; 
        }

        private static List<string> UnneededMentionAttributes = new List<string>() { "contenteditable", "data-ipshover", "data-ipshover-target", "data-mentionid" };

        private static string GetFirstNotNullGroup(GroupCollection gc, params int[] groupIndexes)
        {
            foreach(int i in groupIndexes)
            {
                string str = gc[i].Value;

                if(!String.IsNullOrEmpty(str))
                {
                    return str;
                }
            }

            return null;
        }

        private static string ConvertForumLinks(string href, out string title)
        {
            Match forumMatch = forumRegex.Match(href);

            if (forumMatch.Success)
            {
                int forumId = Int32.Parse(forumMatch.Groups[1].Value);

                Forum forum = ForumLogic.GetForum(forumId);

                if (forum != null)
                {
                    title = forum.Description;
                    return forum.Url;
                }

            }

            Match topicMatch = topicRegex.Match(href);

            if (topicMatch.Success)
            {
                int topicId = Int32.Parse(GetFirstNotNullGroup(topicMatch.Groups, 1, 2));

                Topic topic = DB.Instance.GetRecordById<Topic>(topicId);

                if (topic != null)
                {
                    Match pageMatch = pageRegex.Match(href);
                    Match commentMatch = commentRegex.Match(href);
                    Match pageStartMatch = pageStartRegex.Match(href);
                    Match findPostMatch = findPostRegex.Match(href);

                    Post post = null;

                    if (commentMatch.Success)
                    {
                        int pid = Int32.Parse(commentMatch.Groups[1].Value);

                        post = DB.Instance.GetRecordById<Post>(pid);
                    }

                    if(post == null & findPostMatch.Success)
                    {
                        int pid = Int32.Parse(findPostMatch.Groups[1].Value);

                        post = DB.Instance.GetRecordById<Post>(pid);
                    }

                    if (post == null && pageStartMatch.Success)
                    {
                        int pageSt = Int32.Parse(pageStartMatch.Groups[1].Value);

                        post = ItemLogic.GetCommentsWithPermission<Post>(topic.Id, 1, pageSt).FirstOrDefault();
                    }

                    if (post == null && pageMatch.Success)
                    {
                        int page = Int32.Parse(pageMatch.Groups[1].Value);

                        if (page > 1)
                        {
                            post = ItemLogic.GetCommentsWithPermission<Post>(topicId, 1, (page - 1) * 20).FirstOrDefault();
                        }
                    }

                    if (post != null)
                    {
                        title = $"{MemberLogic.GetMember(post.Author).name} replied to {topic.Title}";
                        return ForumLogic.GetPostUrl(post);
                    }
                    else
                    {
                        Forum forum = ForumLogic.GetForum(topic.Container);

                        if (forum != null)
                        {
                            title = topic.Title;
                            return ForumLogic.GetTopicUrl(forum, topic);
                        }
                    }
                }

            }

            Match postMatch = postRegex.Match(href);
            Match postMatch2 = postRegex2.Match(href);
           

            if (postMatch.Success || postMatch2.Success)
            {
                string value = postMatch.Success ? postMatch.Groups[1].Value : postMatch2.Groups[1].Value;

                int postId = Int32.Parse(value);

                Post post = DB.Instance.GetRecordById<Post>(postId);

                string postUrl = ForumLogic.GetPostUrl(post);

                if (!String.IsNullOrEmpty(postUrl))
                {
                    Topic topic = DB.Instance.GetRecordById<Topic>(post.Item);

                    title = $"{MemberLogic.GetMember(post.Author).name} replied to {topic?.Title ?? "Topic"}";
                    return postUrl;
                }
            }

            title = "Unknown Topic";
            return null;
        }

        private static void ConvertLinks(HtmlNode node, int? pmMember, string baseUrl)
        {
            var links = node.SelectNodes(".//a");

            if(links != null)
            {
                baseUrl = GetBaseUrl(pmMember, baseUrl);

                foreach(var link in links)
                {
                    //mentions
                    if (link.Attributes.Any(a => a.Name == "data-mentionid"))
                    {
                        ConvertMentions(link, pmMember, baseUrl);
                    }
                    else if(link.Attributes.Any(a => a.Name == "href"))
                    {
                        string href = link.GetAttributeValue("href", "");

                        //Forum Link
                        string forumTitle;
                        string forumUrl = ConvertForumLinks(href, out forumTitle);

                        if(!String.IsNullOrEmpty(forumUrl))
                        {
                            link.SetAttributeValue("href", baseUrl + forumUrl);

                            continue;
                        }

                        
                        Match statusMatch = statusRegex.Match(href);

                        if (statusMatch.Success)
                        {
                            ConvertStatusLink(link, statusMatch, pmMember, baseUrl);

                            continue;
                        }

                        Match memberMatch = memberRegex.Match(href);

                        if (memberMatch.Success)
                        { 
                            ConvertMemberLinks(link, memberMatch, pmMember, baseUrl);

                            continue;
                        }

                        Match vanillaMatch = vanillaTopicRegex.Match(href);

                        if(vanillaMatch.Success)
                        {
                            ConvertVanillaLinks(link, href, pmMember, baseUrl);

                            continue;
                        }

                        Match videoCatMatch = videoCategoryRegex.Match(href);

                        if(videoCatMatch.Success)
                        {
                            int catId = Int32.Parse(videoCatMatch.Groups[1].Value);

                            VideoCategory cat = DB.Instance.GetRecordById<VideoCategory>(catId);

                            if(cat != null)
                            {
                                string url = VideoLogic.GetVideoCategoryUrl(cat);

                                if (!String.IsNullOrEmpty(url))
                                {
                                    link.SetAttributeValue("href", baseUrl + url);
                                }
                            }

                            continue;
                        }

                        Match videoMatch = videoRegex.Match(href);

                        if(videoMatch.Success)
                        {
                            int videoId = Int32.Parse(videoMatch.Groups[1].Value);

                            Video video = DB.Instance.GetRecordById<Video>(videoId);

                            if(video != null)
                            {
                                string url = VideoLogic.GetVideoUrl(video);

                                if(!String.IsNullOrEmpty(url))
                                {
                                    link.SetAttributeValue("href", baseUrl + url);
                                }
                            }

                            continue;
                        }

                        Match blogMatch = blogRegex.Match(href);

                        if(blogMatch.Success)
                        {
                            int blogId = Int32.Parse(blogMatch.Groups[1].Value);

                            Blog blog = DB.Instance.GetRecordById<Blog>(blogId);

                            if (blog != null)
                            {
                                string url = BlogLogic.GetBlogUrl(blog);

                                if(!String.IsNullOrEmpty(url))
                                {
                                    link.SetAttributeValue("href", baseUrl + url);
                                }
                            }

                            continue;
                        }

                        Match blogEntryCommentMatch = blogEntryCommentRegex.Match(href);

                        if (blogEntryCommentMatch.Success)
                        {
                            int commentId = Int32.Parse(blogEntryCommentMatch.Groups[1].Value);

                            BlogComment comment = DB.Instance.GetRecordById<BlogComment>(commentId);

                            if(comment != null)
                            {
                                string url = BlogLogic.GetBlogEntryCommentUrl(comment);

                                if(!String.IsNullOrEmpty(url))
                                {
                                    link.SetAttributeValue("href", baseUrl + url);
                                }
                            }

                            continue;
                        }

                        Match blogEntryMatch = blogEntryRegex.Match(href);

                        if (blogEntryMatch.Success)
                        {
                            int entryId = Int32.Parse(GetFirstNotNullGroup(blogEntryMatch.Groups, 1, 2));

                            BlogEntry entry = DB.Instance.GetRecordById<BlogEntry>(entryId);

                            if (entry != null)
                            {
                                string url = BlogLogic.GetBlogEntryUrl(entry);

                                if (!String.IsNullOrEmpty(url))
                                {
                                    link.SetAttributeValue("href", baseUrl + url);
                                }
                            }

                            continue;
                        }

                        Match articleMatch = articleRegex.Match(href);
                        Match articleMatch2 = articleRegex2.Match(href);

                        if (articleMatch.Success || articleMatch2.Success)
                        {
                            int articleId = Int32.Parse(articleMatch.Success ? articleMatch.Groups[1].Value : articleMatch2.Groups[1].Value);

                            Topic topic = GetArticleTopic(articleId);

                            if (topic != null)
                            {
                                Forum forum = ForumLogic.GetForum(topic);

                                if (forum != null)
                                {
                                    string url = ForumLogic.GetTopicUrl(forum, topic);

                                    if (!String.IsNullOrEmpty(url))
                                    {
                                        link.SetAttributeValue("href", baseUrl + url);
                                    }
                                }
                            }

                            continue;
                        }

                        Match mediaMatch = mediaRegex.Match(href);

                        if(mediaMatch.Success)
                        {
                            string url = ConvertImage(mediaMatch.Value, pmMember, false);

                            if(!pmMember.HasValue)
                            {
                                url = baseUrl + url;
                            }

                            link.SetAttributeValue("href", url);

                            continue;
                        }

                        Match newsMatch = newsRegex.Match(href);

                        if (newsMatch.Success)
                        {
                            ConvertNewsLinks(link, href, newsMatch, baseUrl);

                            continue;
                        }

                        if (HardCodeCoverts(link, href, baseUrl))
                        {
                            continue;
                        }


                        if (baseSiteRegex.IsMatch(href) && !href.Contains("/wiki/"))
                        {
                            LogLogic.Log(LogLogic.Href, href);
                        }
                    }
                }
            }
        }

        private static Regex baseSiteOnlyRegex = new Regex(@"^https?:\/\/(?:www\.)?(?:youchew\.net|youchewpoop\.com|youtubepoops?\.com|pooparchive.com)(?:\/?forum|\/?archive)?\/?$");
        private static Regex oldNewsRegex = new Regex(@"http:\/\/youchewpoop\.com\/(.*?)\/");

        private static bool HardCodeCoverts(HtmlNode link, string href, string baseUrl)
        {
            int topicId;

            Match oldNewsMatch = oldNewsRegex.Match(href);

            if (oldNewsMatch.Success)
            { 
                if (newsTopicMap.TryGetValue(oldNewsMatch.Groups[1].Value, out topicId))
                {
                    Topic topic = DB.Instance.GetRecordById<Topic>(topicId);

                    if (topic != null)
                    {
                        Forum forum = ForumLogic.GetForum(topic.Container);

                        if (forum != null)
                        {
                            link.SetAttributeValue("href", baseUrl + ForumLogic.GetTopicUrl(forum, topic));
                            return true;
                        }
                    }
                }
            }

            if(href == "http://youchewpoop.com/news/youchew-poopcast-episode-3-what-the-fuck-was-that/#comments")
            {
                Topic topic = DB.Instance.GetRecordById<Topic>(76285);

                if (topic != null)
                {
                    Forum forum = ForumLogic.GetForum(topic.Container);

                    if (forum != null)
                    {
                        link.SetAttributeValue("href", baseUrl + ForumLogic.GetTopicUrl(forum, topic));
                        return true;
                    }
                }
            }
          
            

            if(baseSiteOnlyRegex.IsMatch(href))
            {
                link.SetAttributeValue("herf", baseUrl + "index.html");
                return true;
            }

            return false;
        }

        private static void ConvertStatusLink(HtmlNode link, Match statusMatch, int? pmMember, string baseUrl)
        {
            int statusId = Int32.Parse(statusMatch.Groups[1].Value);

            Status status = StatusLogic.GetStatus(statusId);

            if(status != null)
            {
                link.SetAttributeValue("href", GetBaseUrl(pmMember, baseUrl) + StatusLogic.GetStatusUrl(status));
            }
        }

        private static void ConvertMentions(HtmlNode link, int? pmMember, string baseUrl)
        {
            int member_id = link.GetAttributeValue("data-mentionid", 0);
            link.SetAttributeValue("href", GetBaseUrl(pmMember, baseUrl) + MemberLogic.GetMember(member_id).Url);

            link.AddClass("member-mention");

            RemoveAttributes(link, UnneededMentionAttributes);
        }

        private static void ConvertMemberLinks(HtmlNode link, Match memberMatch, int? pmMember, string baseUrl)
        {
            string memberStr = GetFirstNotNullGroup(memberMatch.Groups, 1, 2, 3);

            int memberId = Int32.Parse(memberStr);

            Member member = MemberLogic.GetMember(memberId);

            if (member.Id != 0)
            {
                link.SetAttributeValue("href", GetBaseUrl(pmMember, baseUrl) + member.Url);
            }
        }

        private static void ConvertNewsLinks(HtmlNode link, string href, Match newsMatch, string baseUrl)
        {
            string seo = newsMatch.Groups[1].Value;

            Topic newsTopic;
            int topic_id;

            if (newsTopicMap.TryGetValue(seo, out topic_id))
            {
                newsTopic = DB.Instance.GetRecordById<Topic>(topic_id);
            }
            else
            {
                string query = $"SELECT * FROM {Topic.TableName} WHERE forum_id IN (56, 35) AND title_seo=@title_seo ORDER BY forum_id DESC";
                List<MySqlParameter> parameters = new List<MySqlParameter>() { new MySqlParameter("@title_seo", MySqlDbType.String) { Value = seo } };

                newsTopic = DB.Instance.GetData<Topic>(query, parameters).FirstOrDefault();
            }

            if (newsTopic != null)
            {
                Forum forum = ForumLogic.GetForum(newsTopic.Container);

                link.SetAttributeValue("href", GetBaseUrl(null, baseUrl) + ForumLogic.GetTopicUrl(forum, newsTopic));
            }
            else
            {
                LogLogic.Log(LogLogic.Href, href);
                LogLogic.Log(LogLogic.News, seo);
            }
        }

        private static void ConvertVanillaLinks(HtmlNode link, string href, int? pmMember, string baseUrl)
        {
            int? discussionId = null;
            int? page = null;
            int? item = null;
            int? focus = null;

            Match discussionMatch = discussionIdRegex.Match(href);
            Match pageMatch = pageRegex.Match(href);
            Match itemMatch = itemRegex.Match(href);
            Match focusMatch = focusRegex.Match(href);

            baseUrl = GetBaseUrl(pmMember, baseUrl);

            if (discussionMatch.Success)
            {
                discussionId = Int32.Parse(discussionMatch.Groups[1].Value);
            }

            if (pageMatch.Success)
            {
                page = Int32.Parse(pageMatch.Groups[1].Value);
            }

            if (itemMatch.Success)
            {
                item = Int32.Parse(itemMatch.Groups[1].Value);
            }

            if (focusMatch.Success)
            {
                focus = Int32.Parse(focusMatch.Groups[1].Value);
            }

            if (discussionId.HasValue && !focus.HasValue)
            {
                int topicId = discussionId.Value + vanillaTopicOffset;
                Topic topic = DB.Instance.GetRecordById<Topic>(topicId);

                if (topic != null)
                {
                    Forum forum = ForumLogic.GetForum(topic);


                    if ((page ?? 1) == 1 && (item ?? 0) == 0)
                    {
                        link.SetAttributeValue("href", baseUrl + ForumLogic.GetTopicUrl(forum, topic));

                    }
                    else
                    {
                        int postNumber = (((page ?? 1) - 1) * VANILLA_POSTS_PER_PAGE) + ((item ?? 0) + 1);

                        Post post = ForumLogic.GetPostFromNumber(topic, postNumber);

                        if (post != null)
                        {
                            link.SetAttributeValue("href", baseUrl + ForumLogic.GetPostUrl(forum, topic, post));
                        }
                    }
                }
            }
            else if (focus.HasValue)
            {
                int postId = focus.Value + vanillaPostOffset;

                Post post = DB.Instance.GetRecordById<Post>(postId);

                string postUrl = ForumLogic.GetPostUrl(post);

                if (!String.IsNullOrEmpty(postUrl))
                {
                    link.SetAttributeValue("href", baseUrl + postUrl);
                }

            }
        }

        private const int vanillaTopicOffset = 78301;
        private const int vanillaPostOffset = 3250431;
        private const int VANILLA_POSTS_PER_PAGE = 20;

        private static void RemoveAttributes(HtmlNode node, List<string> attributes)
        {
            foreach(string att in attributes)
            {
                if(node.Attributes.Any(a => a.Name == att))
                {
                    node.Attributes[att].Remove();
                }
            }
        }

        private static void MakeEmbedsResponseive(HtmlNode node)
        {
            var embeds = node.SelectNodes($".//div[{ContainsClass("ipsEmbeddedVideo")}]");

            if (embeds != null)
            {
                foreach (var embed in embeds)
                {
                    embed.AddClass("embed-responsive embed-responsive-16by9");

                    var iframe = embed.SelectSingleNode(".//iframe");

                    if (iframe != null)
                    {
                        iframe.AddClass("embed-responsive-item");
                    }
                }
            }
        }

        private static string ContainsClass(string @class)
        {
            return $"contains(concat(' ', @class, ' '), ' {@class} ')";
        }

        private static void ConvertQuotes(HtmlNode node, List<Post> posts, int? pmMember, string baseUrl)
        {
            HtmlNode quote;

            baseUrl = GetBaseUrl(pmMember, baseUrl);

            while ((quote = node.SelectSingleNode($".//blockquote[{ContainsClass("ipsQuote")}]")) != null)
            {
                quote.RemoveClass("ipsQuote");
                quote.AddClass("yc-quote");

                string html = "";

                if (quote.Attributes.Any(a => a.Name == "data-ipsquote-contentapp" || a.Name == "data-ipsquote-contenttype"))
                {
                    int memberId = quote.GetAttributeValue("data-ipsquote-userid", 0);
                    string name = quote.GetAttributeValue("data-ipsquote-username", null);
                    int timestamp = quote.GetAttributeValue("data-ipsquote-timestamp", 0);
                    string app = quote.GetAttributeValue("data-ipsquote-contentapp", quote.GetAttributeValue("data-ipsquote-contenttype", ""));

                    if(String.IsNullOrEmpty(name))
                    {
                        name = MemberLogic.GetMember(memberId).name;
                    }

                    string timeStr = "";

                    if(timestamp != 0)
                    {
                        timeStr = $"On {LangLogic.FormatDate(timestamp, LangLogic.LongDateFormat)}, ";
                    }

                    string quoteLinkHtml = "";

                    if(app == "forums")
                    {
                        int pid = quote.GetAttributeValue("data-ipsquote-contentcommentid", -1);

                        if(pid > -1)
                        {
                            Post post = DB.Instance.GetRecordById<Post>(pid);

                            if(post != null)
                            {
                                Topic topic = DB.Instance.GetRecordById<Topic>(post.Item);

                                if(topic != null)
                                {
                                    Forum forum = ForumLogic.GetForum(topic.Container);

                                    if(forum != null)
                                    {
                                        string postUrl = ForumLogic.GetPostUrl(forum, topic, post, posts);
                                        quoteLinkHtml = $"<div class='pull-right'><a href='{baseUrl}{postUrl}'><span class='glyphicon glyphicon-share-alt'></span></a></div>";
                                    }
                                }
                            }

                            
                        }
                    }
                    html += "<blockquote class='yc-quote'>";

                    html += $"<div class='yc-quote-cite'>{timeStr}{name} said:{quoteLinkHtml}</div>";

                    HtmlNode content = quote.SelectSingleNode($"./div[{ContainsClass("ipsQuote_contents")}]");
                    string contentStr = quote.InnerHtml;

                    if (content != null)
                    {
                        contentStr = content.InnerHtml;
                    }

                    html += $"<div class='yc-quote-content'>{contentStr}</div></blockquote>";

                    quote.ParentNode.ReplaceChild(HtmlNode.CreateNode(html), quote);
                }
                else
                {
                    html += "<blockquote class='yc-quote'>";

                    string cite = quote.Attributes.FirstOrDefault(a => a.Name == "data-cite")?.Value;
                    if (String.IsNullOrEmpty(cite))
                    {
                        cite = quote.Attributes.FirstOrDefault(a => a.Name == "data-ipsquote-username")?.Value;
                    }

                    if(!String.IsNullOrEmpty(cite))
                    {
                        html += $"<div class='yc-quote-cite'>{cite} said:</div>";
                    }
                    else
                    {
                        html += "<div class='yc-quote-cite'>Quote</div>";
                    }

                    html += $"<div class='yc-quote-content'>{quote.InnerHtml}</div></blockquote>";

                    quote.ParentNode.ReplaceChild(HtmlNode.CreateNode(html), quote);
                }
            }
        }

        private static void ConvertSpoilers(HtmlNode node)
        {
            HtmlNode spoiler;

            int spoilerIndex = 0;

            while ((spoiler = GetSpoilerNode(node)) != null)
            {
                spoiler.RemoveClass("ipsSpoiler");
                spoiler.AddClass("yc-spoiler");

                HtmlNode contents = spoiler.SelectSingleNode($"./div[{ContainsClass("ipsSpoiler_contents")}]");

                string contentHtml = contents?.InnerHtml ?? spoiler.InnerHtml;

                string spoilerId = $"spoiler{spoilerIndex}";

                string spoilerHtml = $"<div class='yc-spoiler'><div class='yc-spoiler-header' data-toggle='collapse' data-target='#{spoilerId}'><button class='btn btn-sm btn-primary' type='button'>Spoiler</button></div>"
                                   + $"<div class='yc-spoiler-content collapse' id='{spoilerId}'>{contentHtml}</div></div>";

                spoiler.ParentNode.ReplaceChild(HtmlNode.CreateNode(spoilerHtml), spoiler);
                


                spoilerIndex++;
            }
        }

        private static HtmlNode GetSpoilerNode(HtmlNode node)
        {
            HtmlNode spoiler = node.SelectSingleNode($".//div[{ContainsClass("ipsSpoiler")}]");

            if(spoiler == null)
            {
                spoiler = node.SelectSingleNode($".//blockquote[{ContainsClass("ipsStyle_spoiler")}]");
            }

            return spoiler;
        }

        private static string baseUrlRegex = @"(?:(?:\<|&lt;)___base_url___(?:\>|&gt;)|https?\:\/\/youchewpoop\.com\/forum|https?\:\/\/(?:i\.)?youchew\.net\/forum)";
        private static Regex baseSiteRegex = new Regex(@"(?:(?:\<|&lt;)___base_url___(?:\>|&gt;)|https?\:\/\/youchewpoop\.com|https?\:\/\/(?:i\.)?youchew\.net)");
        private static Regex embedRegex = new Regex($@"{baseUrlRegex}\/index\.php\??\/(.+?)\/(\d+?)-");
        private static Regex embedCommentRegex = new Regex(@"embedComment=(\d+)");
        private static Regex forumRegex = new Regex($@"{baseUrlRegex}\/index\.php\??\/forum\/(\d+)-[^\/]*?\/?");
        private static Regex topicRegex = new Regex($@"{baseUrlRegex}\/index\.php\??(?:\/topic\/(\d+)-[^\/]*?\/?|showtopic=(\d+))");
        private static Regex postRegex = new Regex($@"{baseUrlRegex}\/index\.php\?findpost=(\d+)");
        private static Regex postRegex2 = new Regex($@"{baseUrlRegex}\/index\.php\?app=forums(?:&amp;|&)module=forums(?:&amp;|&)section=findpost(?:&amp;|&)pid=(\d+)");
        private static Regex memberRegex = new Regex($@"(?:{baseUrlRegex}\/index\.php\??(?:\/(?:user|profile)\/(\d+)-|showuser=(\d+))|{baseUrlRegex}\/(?:account|memberlist).php\?.*?u=(\d+))");
        private static Regex statusRegex = new Regex($@"{baseUrlRegex}.*?(?:status_id=|status=|statuses\/id\/)(\d+)");
        private static Regex newsRegex = new Regex($@"{baseSiteRegex.ToString()}(?:\/~youch1)?\/news\/([^\/]*)\/?");
        private static string vanillaBaseUrlRegex = @"(?:(?:\<|&lt;)___base_url___(?:\>|&gt;)|https?\:\/\/youchewpoop\.com\/(?:forum|archive)|https?\:\/\/youchew\.net\/archive|https?\:\/\/youtubepoop\.com\/(?:forum|archive)|https?\:\/\/pooparchive\.com\/forum)";
        private static Regex vanillaTopicRegex = new Regex($@"{vanillaBaseUrlRegex}\/comments\.php");
        private static Regex discussionIdRegex = new Regex(@"DiscussionID=(\d+)");
        private static Regex pageRegex = new Regex(@"page=(\d+)");
        private static Regex itemRegex = new Regex(@"#Item_(\d+)");
        private static Regex focusRegex = new Regex(@"Focus=(\d+)");
        private static Regex fileStoreRegex = new Regex(@"(?:\<|&lt;).*?(?:\>|&gt;)\/?(.*)");
        private static Regex findPostRegex = new Regex(@"findpost__p__(\d+)");
        private static Regex pageStartRegex = new Regex(@"page__st__(\d+)");
        private static Regex videoCategoryRegex = new Regex($@"{baseUrlRegex}\/index\.php\??\/videos\/category[\/\-](\d+)-");
        private static Regex videoRegex = new Regex($@"{baseUrlRegex}\/index\.php\??\/videos\/view-(\d+)-");
        private static Regex blogRegex = new Regex($@"{baseUrlRegex}\/index\.php\??\/blogs\/blog\/(\d+)-");
        private static Regex blogEntryRegex = new Regex($@"{baseUrlRegex}\/index\.php\??\/(?:blogs\/entry\/(\d+)-|blog\/\d+\/entry-(\d+)-)");
        private static Regex blogEntryCommentRegex = new Regex($@"{baseUrlRegex}\/index\.php\??\/blogs\/entry\/\d+-.*?comment-(\d+)");
        private static Regex externalEmbedRegex = new Regex($@"{baseUrlRegex}\/(?:index\.php)?\?app=core(?:&amp;|&)module=system(?:&amp;|&)controller=embed(?:&amp;|&)url=(.*)");
        private static Regex articleRegex = new Regex($@"(?:{baseUrlRegex}|{baseSiteRegex.ToString()})\/index\.php\??\/(?:page\/(?:Articles|index\.html)\/_|articles).*?-r(\d+)");
        private static Regex articleRegex2 = new Regex($@"{baseSiteRegex.ToString()}\/index\.php\?app=ccs(?:&amp;|&)module=pages(?:&amp;|&)section=pages.*-r(\d+)");
        private static Regex commentRegex = new Regex($@"#?comment[\-=](\d+)");
        private static Regex mediaRegex = new Regex($@"(?:{baseSiteRegex.ToString()}|(?:\<|&lt;).*?(?:\>|&gt;)).*\.(?:jpg|png|gif|jpeg|mp3|wav|ogg|swf)", RegexOptions.IgnoreCase);

        private static string GetBaseUrl(int? pmMember, string baseUrl)
        {
            if(!String.IsNullOrEmpty(baseUrl))
            {
                return baseUrl;
            }

            if(pmMember.HasValue)
            {
                return MessageLogic.MessageBaseUrl;
            }

            return "";
        }

        private static void ConvertInternalEmbedsToLinks(HtmlNode node, int? pmMember, string baseUrl)
        {
            var iframes = node.SelectNodes(".//iframe");

            if(iframes != null)
            {
                baseUrl = GetBaseUrl(pmMember, baseUrl);

                foreach(var iframe in iframes.Where(i => i.Attributes.Any(a => a.Name == "src")))
                {
                    string src = iframe.GetAttributeValue("src", "");

                    string forumTitle;
                    string forumUrl = ConvertForumLinks(src, out forumTitle);

                    if(!String.IsNullOrEmpty(forumUrl))
                    {
                        HtmlNode newNode = HtmlNode.CreateNode($"<a href='{baseUrl}{forumUrl}'>{forumTitle}</a>");
                        iframe.ParentNode.ReplaceChild(newNode, iframe);

                        continue;
                    }

                    Match videoMatch = videoRegex.Match(src);

                    if(videoMatch.Success)
                    {
                        int videoId = Int32.Parse(videoMatch.Groups[1].Value);

                        Video video = DB.Instance.GetRecordById<Video>(videoId);

                        string href = src;
                        string title = "Video Link";

                        if (video != null)
                        {
                            string videoUrl = VideoLogic.GetVideoUrl(video);
                            title = video.Title;

                            if(!String.IsNullOrEmpty(videoUrl))
                            {
                                href = baseUrl + videoUrl;
                            }
                        }

                        HtmlNode newNode = HtmlNode.CreateNode($"<a href='{href}'>{title}</a>");

                        iframe.ParentNode.ReplaceChild(newNode, iframe);

                        continue;
                    }

                    Match blogMatch = blogRegex.Match(src);

                    if(blogMatch.Success)
                    {
                        int blogId = Int32.Parse(blogMatch.Groups[1].Value);

                        Blog blog = DB.Instance.GetRecordById<Blog>(blogId);
                        string url = src;
                        string title = "Blog Link";


                        if(blog != null)
                        {
                            string blogUrl = BlogLogic.GetBlogUrl(blog);
                            title = blog.Title;

                            if(!String.IsNullOrEmpty(blogUrl))
                            {
                                url = baseUrl + blogUrl;
                            }
                        }

                        HtmlNode newNode = HtmlNode.CreateNode($"<a href='{url}'>{title}</a>");

                        iframe.ParentNode.ReplaceChild(newNode, iframe);

                        continue;
                    }

                    Match blogEntryMatch = blogEntryRegex.Match(src);

                    if (blogEntryMatch.Success)
                    {
                        int entryId = Int32.Parse(GetFirstNotNullGroup(blogEntryMatch.Groups, 1, 2));

                        BlogEntry entry = DB.Instance.GetRecordById<BlogEntry>(entryId);

                        string url = src;
                        string title = "Blog Entry Link";

                        if(entry != null)
                        {
                            string entryUrl = BlogLogic.GetBlogEntryUrl(entry);
                            title = entry.Title;

                            if(!String.IsNullOrEmpty(entryUrl))
                            {
                                url = baseUrl + entryUrl;
                            }
                        }

                        HtmlNode newNode = HtmlNode.CreateNode($"<a href='{url}'>{title}</a>");

                        iframe.ParentNode.ReplaceChild(newNode, iframe);

                        continue;
                    }

                    Match externalEmbedMatch = externalEmbedRegex.Match(src);

                    if (externalEmbedMatch.Success)
                    {
                        string url = externalEmbedMatch.Groups[1].Value;

                        if (url.StartsWith("http%3A") || url.StartsWith("https%3A"))
                        {
                            url = System.Net.WebUtility.UrlDecode(url);
                        }

                        HtmlNode parent = iframe.ParentNode;
                        HtmlNode newNode = HtmlNode.CreateNode($"<a href='{url}'>{url}</a>");

                        if(parent.HasClass("ipsEmbeddedOther"))
                        {
                            parent.ParentNode.ReplaceChild(newNode, parent);
                        }
                        else
                        {
                            iframe.ParentNode.ReplaceChild(newNode, iframe);
                        }

                        continue;
                    }

                    Match articleMatch = articleRegex.Match(src);
                    Match articleMatch2 = articleRegex2.Match(src);

                    if (articleMatch.Success || articleMatch2.Success)
                    {
                        string value = articleMatch.Success ? articleMatch.Groups[1].Value : articleMatch2.Groups[1].Value;

                        int articleId = Int32.Parse(value);

                        Topic topic = GetArticleTopic(articleId);

                        string url = src;
                        string title = "Article Link";

                        if (topic != null)
                        {
                            Forum forum = ForumLogic.GetForum(topic);
                            title = topic.Title;

                            if (forum != null)
                            {
                                url = ForumLogic.GetTopicUrl(forum, topic);
                            }
                        }

                        if (!String.IsNullOrEmpty(url))
                        {
                            HtmlNode newNode = HtmlNode.CreateNode($"<a href='{baseUrl}{url}'>{title}</a>");
                            iframe.ParentNode.ReplaceChild(newNode, iframe);
                        }

                        continue;
                    }

                    if (src.Contains("/bugtracker/"))
                    {
                        HtmlNode newNode = HtmlNode.CreateNode($"<a href='{src}'>Bugtracker Link</a>");
                        iframe.ParentNode.ReplaceChild(newNode, iframe);

                        continue;
                    }

                    Match match = embedRegex.Match(src);

                    if (match.Success)
                    {
                        string furlModule = match.Groups[1].Value;
                        int id = Int32.Parse(match.Groups[2].Value);

                        if (furlModule == "topic")
                        {
                            Topic topic = DB.Instance.GetRecordById<Topic>(id);

                            if (topic != null)
                            {
                                Forum forum = ForumLogic.GetForum(topic.Container);
                                Match commentMatch = embedCommentRegex.Match(src);

                                string url = ForumLogic.GetTopicUrl(forum, topic);
                                string title = topic.Title;

                                int? pid = null;

                                if (commentMatch.Success)
                                {
                                    pid = Int32.Parse(commentMatch.Groups[1].Value);
                                }
                                else
                                {
                                    Match findPostMatch = findPostRegex.Match(src);

                                    if (findPostMatch.Success)
                                    {
                                        pid = Int32.Parse(findPostMatch.Groups[1].Value);
                                    }
                                    else
                                    {
                                        Match pageStartMatch = pageStartRegex.Match(src);

                                        if (pageStartMatch.Success)
                                        {
                                            int pageSt = Int32.Parse(pageStartMatch.Groups[1].Value);

                                            Post post = ItemLogic.GetCommentsWithPermission<Post>(topic.Id, 1, pageSt).FirstOrDefault();

                                            if (post != null)
                                            {
                                                pid = post.Id;
                                            }

                                        }
                                    }
                                }

                                if (pid.HasValue)
                                {
                                    Post post = DB.Instance.GetRecordById<Post>(pid.Value);

                                    if (post != null)
                                    {
                                        url = ForumLogic.GetPostUrl(forum, topic, post);
                                        title = $"{MemberLogic.GetMember(post.Author).name} replied to {title}";
                                    }
                                }

                                HtmlNode newNode = HtmlNode.CreateNode($"<a href='{baseUrl}{url}'>{title}</a>");
                                iframe.ParentNode.ReplaceChild(newNode, iframe);

                            }
                            else
                            {
                                iframe.ParentNode.RemoveChild(iframe);
                            }
                        }

                        continue;
                    }

                    if (baseSiteRegex.IsMatch(src))
                    {
                        LogLogic.Log(LogLogic.IFrame, src);
                        continue;
                    }

                    //<___base_url___>/index.php?/topic/118626-how-to-get-a-member-title/&do=embed
                }
            }
        }

        private static Topic GetArticleTopic(int articleId)
        {
            string query = $"SELECT record_topicid FROM cms_custom_database_1 WHERE primary_id_field = {articleId}";

            int? topicId = DB.Instance.ExecuteScalar<int?>(query);

            if(topicId.HasValue)
            {
                return DB.Instance.GetRecordById<Topic>(topicId.Value);
            }

            return null;
        }

        private static void ConvertImages(HtmlNode node, int? pmMember, bool downloadImages)
        {
            var imgs = node.SelectNodes(".//img");

            if (imgs != null)
            {
                foreach (var img in imgs)
                {
                    if (!img.HasClass("ipsEmoji") && !HasParentWithClass(img, "non-responsive") && !img.Attributes.Any(a => a.Name == "data-emoticon"))
                    {
                        img.AddClass("img-responsive");
                    }

                    string src = img.GetAttributeValue("src", "");

                    src = ConvertImage(src, pmMember, downloadImages);

                    if(!String.IsNullOrEmpty(src))
                    {
                        img.SetAttributeValue("src", src);
                    }
                }
            }
        }

        private static bool HasParentWithClass(HtmlNode img, string @class)
        {
            HtmlNode currentNode = img;
            
            while(true)
            {
                if(currentNode.Id == "content")
                {
                    return false;
                }

                if(currentNode.HasClass(@class))
                {
                    return true;
                }

                currentNode = currentNode.ParentNode;
            }

        }

        private static string ConvertImage(string src, int? pmMember, bool downloadImages)
        {
            if (!String.IsNullOrEmpty(src))
            {
                string origSrc = src;

                src = src.Replace("<___base_url___>", "http://youchew.net")
                         .Replace("&lt;___base_url___&gt;", "http://youchew.net");

                Match fileStoreMatch = fileStoreRegex.Match(src);

                if (fileStoreMatch.Success)
                {
                    string fileName = fileStoreMatch.Groups[1].Value;

                    src = GetFileStoreSrc(fileName);

                    if(String.IsNullOrEmpty(src))
                    {
                        LogLogic.Log(LogLogic.Image, origSrc);
                        return origSrc;
                    }
                }

                if (baseSiteRegex.IsMatch(src))
                {
                    src = src.Replace("forum/public/style_emoticons/default", "uploads/emoticons")
                             .Replace("forum/public/style_emoticons/#EMO_DIR%23", "uploads/emoticons")
                             .Replace("public/style_emoticons/default", "uploads/emoticons")
                             .Replace("gallery/wp-content/uploads", "images")
                             .Replace("news/wp-content/uploads", "images")
                             .Replace("forum/uploads", "uploads")
                             .Replace("forum/public", "publicOLD")
                             .Replace("/public/", "/publicOLD/")
                             .Replace("forum/flags", "flags")
                             .Replace("wiki/images", "images");

                    Uri url = new Uri(src);

                    string path = url.LocalPath.TrimStart('/', '\\');

                    if (url.Host == "i.youchew.net")
                    {
                        path = $@"uploads\i\{path}";
                    }
                    
                    ImageLogic.IncludeImage(src, path, pmMember);

                    src = path;
                }
                else if(!pmMember.HasValue)
                {
                    Uri url = null;

                    try
                    {
                        url = new Uri(src);
                    }
                    catch { }

                    if (url != null)
                    {
                        src = ImageLogic.DownloadImage(src, downloadImages);
                    }
                }
            }

            return src;
        }

        private static List<string> FileStores = new List<string>()
        {
            "uploads",
            "uploads/blogs",
            "uploads/videos/thumbnails",
            "uploads/videos",
            "uploads/videos/cat_images",
            "uploads/awards",
            "uploads/i",
        };

        private static string GetFileStoreSrc(string fileName)
        {
            foreach (string fileStore in FileStores)
            {
                string path = $@"{fileStore}/{fileName}";

                string imageFile = $@"{ImageLogic.ImageRepository}\{path.Replace("/", "\\")}";

                if(File.Exists(imageFile))
                {
                    return $"http://youchew.net/{path}";
                }
            }

            return null;
        }
    }
}
