using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YouChewArchive.Data;
using YouChewArchive.DataContracts;
using YouChewArchive.Logic;

namespace YouChewArchive
{
    public static class MessageLogic
    {
        private static string MapWhere = "map_user_active=1 AND map_is_system=0";
        private static string TopicWhere = "mt_is_deleted=0 AND mt_is_draft=0";

        public static string MessageBaseUrl = "http://youchew.net/";

        private static Dictionary<Tuple<int, int>, HashSet<int>> ConvoMap = new Dictionary<Tuple<int, int>, HashSet<int>>();

        public static List<Conversation> GetConversations(int memberId)
        {
            string query = $"SELECT t.* FROM {Conversation.TableName} t JOIN {MessageTopicUserMap.TableName} ON map_topic_id = mt_id WHERE map_user_id = {memberId} AND {MapWhere} AND {TopicWhere} ORDER BY mt_last_post_time DESC";

            return DB.Instance.GetData<Conversation>(query);
        }

        public static List<MessageTopicUserMap> GetMessageMembers(Conversation convo)
        {
            string query = $"SELECT * FROM {MessageTopicUserMap.TableName} WHERE map_topic_id = {convo.Id}";

            return DB.Instance.GetData<MessageTopicUserMap>(query);
        }

        public static List<int> GetMembersWithMessages()
        {
            string query = $"SELECT DISTINCT map_user_id FROM {MessageTopicUserMap.TableName} WHERE {MapWhere}";

            return DB.Instance.GetData<MessageTopicUserMap>(query).Select(m => m.user_id).ToList();

        }

        public static List<Message> GetMessages(Conversation convo)
        {
            string query = $"SELECT * FROM {Message.TableName} WHERE msg_topic_id = {convo.Id} ORDER BY msg_date";

            return DB.Instance.GetData<Message>(query);
        }

        private static bool AlreadyGenerated(Conversation convo, Member member, int page)
        {
            HashSet<int> hash;

            if(ConvoMap.TryGetValue(Tuple.Create(convo.id, page), out hash))
            {
                return hash.Contains(member.Id);
            }

            return false;
        }

        public static void GeneratePMs()
        {
            EventLogic.RaiseEvent(ProgressEventArgs.App, "core");
            List<int> membersToGenerate = GetMembersWithMessages();

            EventLogic.RaiseEvent(ProgressEventArgs.Containers, membersToGenerate.Count.ToString());

            int membersCompleted = 0;

            foreach (int memberId in membersToGenerate)
            {
                PrepareMemberDirectory(memberId);

                GenerateTopicList(memberId);

                EventLogic.RaiseEvent(ProgressEventArgs.ContainersCompleted, (++membersCompleted).ToString());
            }
        }

        public static string GetMemberDirectory(Member member)
        {
            return $"{Output.OutputDirectory}\\Messages\\{member.Id}-{MemberLogic.LegalifyMemberSeo(member)}";
        }

        private static void PrepareMemberDirectory(int memberId)
        {
            Member member = MemberLogic.GetMember(memberId);

            string directory = GetMemberDirectory(member);

            Output.CreateDirectoryIfNotExists(directory);

            
            Output.Copy($@"{Output.OutputDirectory}\css", $@"{directory}\css");
            Output.Copy($@"{Output.OutputDirectory}\fonts", $@"{directory}\fonts");
            Output.Copy($@"{Output.OutputDirectory}\js", $@"{directory}\js");
            File.Copy($@"{Output.OutputDirectory}\README.txt", $@"{directory}\README.txt", true);

            GenerateStartPage(member);
        }

        private static void GenerateStartPage(Member member)
        {
            string html = $"<div class='page-header'><h1>The Private Messages of {member.name}</h1></div>"
                        + $"<div class='col-md-12'><p>Welcome to your private message archive.  This has every private message your have sent or recieved on YouChew as of August 26, 2018</p>" +
                        "<p><a href='message/1.html'>Click here to view your archived inbox</p></div>";

            Output output = new Output()
            {
                Content = html,
                FileName = "index.html",
                FoldersDeep = 0,
                Title = member.name,
                PMMember = member,
                ActiveNav = "Home",
            };

            output.Generate();

        }

        private const int PM_TOPICS_PER_PAGE = 100;
        private const int PMS_PER_PAGE = 50;

        public static void GenerateTopicList(int memberId)
        {
            Member member = MemberLogic.GetMember(memberId);
            EventLogic.RaiseEvent(ProgressEventArgs.CurrentContainer, member.name);

            List<Conversation> convos = GetConversations(memberId);

            EventLogic.RaiseEvent(ProgressEventArgs.Items, convos.Count.ToString());

            int convosCompleted = 0;
            int page = 1;
            int totalPages = (int)Math.Ceiling((decimal)convos.Count / PM_TOPICS_PER_PAGE);

            while(true)
            {
                List<Conversation> currentConvos = convos.Skip((page - 1) * PM_TOPICS_PER_PAGE).Take(PM_TOPICS_PER_PAGE).ToList();

                if (currentConvos.Count == 0 && page != 1)
                {
                    break;
                }

                GenerateTopicList(member, currentConvos, page, totalPages, ref convosCompleted);

                page++;
            }

            Output.Zip(member);
        }

        private static void GenerateTopicList(Member member, List<Conversation> currentConvos, int page, int totalPages, ref int convosCompleted)
        {
            EventLogic.RaiseEvent(ProgressEventArgs.CurrentContainer, member.name);

            string html = $"<div class='page-header'><h1>Inbox</h1></div>"
                        + Output.GeneratePagination($"message/{{id}}.html", page, totalPages, true);

            foreach(Conversation convo in currentConvos)
            {
                html += "<div class='row topic'>"
                     + $"<div class='col-md-8'><p><a href='{GetConverstationUrl(convo)}'>{convo.Title}</a></p>"
                     + $"<p>{GetMemberNamesInTitle(convo, member)}</p>"
                     + "</div>"
                     + $"<div class='col-md-2 text-right'>{LangLogic.FormatNumber(convo.NumComments, "message", "messages")}</div>"
                     + $"<div class='col-md-2'><p>{LangLogic.FormatDate(convo.LastComment, LangLogic.ShortDateFormat)}</p></div>"
                     + "</div>";
            }

            html += Output.GeneratePagination($"message/{{id}}.html", page, totalPages, false);

            Output output = new Output()
            {
                Content = html,
                FoldersDeep = 1,
                Title = "Inbox",
                FileName = $"message/{page}.html",
                PMMember = member,
            };

            output.Generate();

            foreach (Conversation convo in currentConvos)
            {
                EventLogic.RaiseEvent(ProgressEventArgs.CurrentItem, convo.Title);

                if (!AlreadyGenerated(convo, member, 1))
                {
                    GenerateMessages(convo, member);
                }

                EventLogic.RaiseEvent(ProgressEventArgs.ItemsCompleted, (++convosCompleted).ToString());
            }

            
        }

        private static string GetMemberNamesInTitle(Conversation convo, Member currentMember)
        {
            int to_member_id = convo.to_member_id;

            if(to_member_id == 0)
            {
                string query = $"SELECT map_user_id FROM {MessageTopicUserMap.TableName} WHERE map_topic_id = {convo.Id} AND map_user_id <> {convo.Author} LIMIT 1";

                to_member_id = DB.Instance.ExecuteScalar<int?>(query) ?? 0;
            }

            List<string> members = new List<string>()
            {
                currentMember.Id == convo.Author ? "You" : MemberLogic.GetMember(convo.Author).name,
                currentMember.Id == to_member_id ? "You" : MemberLogic.GetMember(to_member_id).name
            };

            int otherCount = convo.to_count - 2;

            if(otherCount > 0)
            {
                members.Add(LangLogic.FormatNumber(otherCount, "other", "others"));
            }

            if(members.Count == 2)
            {
                return $"{members[0]} and {members[1]}";
            }

            return $"{members[0]}, {members[1]}, and {members[2]}";
            
        }

        public static string GetConverstationUrl(Conversation convo)
        {
            return $"message/{convo.Id}/1.html";
        }

        private static void GenerateMessages(Conversation convo, Member member)
        {
            List<MessageTopicUserMap> map = GetMessageMembers(convo);

            List<Member> membersInConvo = map.Select(m => MemberLogic.GetMember(m.user_id)).ToList();
            List<Message> messages = GetMessages(convo);

            int totalPages = (int)Math.Ceiling((decimal)messages.Count / PMS_PER_PAGE);

            EventLogic.RaiseEvent(ProgressEventArgs.Comments, messages.Count.ToString());

            int page = 1;

            while (true)
            {
                List<Message> currentMessages = messages.Skip((page - 1) * PMS_PER_PAGE).Take(PMS_PER_PAGE).ToList();

                if (currentMessages.Count == 0 && page != 1)
                {
                    break;
                }

                GeneratePosts(currentMessages, membersInConvo, map, member, convo, page, totalPages);

                EventLogic.RaiseEvent(ProgressEventArgs.CommentsCompleted, (((page - 1) * PMS_PER_PAGE) + currentMessages.Count).ToString());

                page++;
            }
        }

        private static void GeneratePosts(List<Message> currentMessages, List<Member> membersInConvo, List<MessageTopicUserMap> map, Member member, Conversation convo, int page, int totalPages)
        {
            string html = $"<div class='page-header'><h1>{convo.Title}</h1></div>";

            html += $"<div class='row convo-members'><h4>Members in Converstation</h4>";

            foreach(Member m in membersInConvo)
            {
                html += $"<div class='col-md-2 col-sm-4 col-xs-6'>{MemberLogic.GetUrlHtml(m.Id, true, MessageBaseUrl)}</div>";
            }

            html += "</div>";

            html += Output.GeneratePagination($"message/{convo.Id}/{{id}}.html", page, totalPages, true);

            foreach(Message message in currentMessages)
            {
                html += $"<div class='row post' id='post-{message.Id}'>";
                html += $"<div class='col-md-2'><p>{MemberLogic.GetUrlHtml(message.Author, true, MessageBaseUrl)}</p><p>{MemberLogic.GetGroupName(MemberLogic.GetMember(message.Author).member_group_id, false)}</p></div>";
                html += $"<div class='col-md-10'>" +
                    $"<div class='col-md-12 post-info-bar'>{LangLogic.FormatDate(message.Date, LangLogic.LongDateFormat)}</div>" +
                    $"<div class='col-md-12'>{message.Content}</div>" +
                    $"</div>";

                html += "</div>";
            }

            html += Output.GeneratePagination($"message/{convo.Id}/{{id}}.html", page, totalPages, false);

            foreach (MessageTopicUserMap m in map.Where(m => m.user_active))
            {
                Member memberToGenerateFrom = MemberLogic.GetMember(m.user_id);

                if (!AlreadyGenerated(convo, memberToGenerateFrom, page))
                {
                    Output output = new Output()
                    {
                        Content = html,
                        FileName = $"message/{convo.Id}/{page}.html",
                        FoldersDeep = 2,
                        Title = convo.Title,
                        CleanupContent = true,
                        PMMember = memberToGenerateFrom,
                        ActiveNav = "Inbox",
                    };

                    output.Generate();
                }

                SetGenerated(convo, memberToGenerateFrom, page);
            }
        }

        private static void SetGenerated(Conversation convo, Member memberToGenerateFrom, int page)
        {
            HashSet<int> hash;

            Tuple<int, int> tuple = Tuple.Create(convo.id, page);

            if (!ConvoMap.TryGetValue(tuple, out hash))
            {
                hash = new HashSet<int>();

                ConvoMap.Add(tuple, hash);
            }

            hash.Add(memberToGenerateFrom.Id);
        }
    }
}
