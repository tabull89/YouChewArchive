using CsvHelper;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Reflection = System.Reflection;
using System.Text;
using System.Threading.Tasks;
using YouChewArchive.Classes;
using YouChewArchive.CSV;
using YouChewArchive.Data;
using YouChewArchive.DataContracts;

namespace YouChewArchive.Logic
{
    public static class MemberLogic
    {
        static MemberLogic()
        {
            GetAllMembers();
            SetMemberFilter();

            WarnReasons = DB.Instance.GetData<WarnReason>($"SELECT * FROM {WarnReason.TableName}")
                                     .ToDictionary(w => w.id, w => w.Description);
        }

        private static void SetMemberFilter()
        {
            MemberFilter dpy = new MemberFilter()
            {
                MemberId = 605,
                minDate = (int)(new DateTime(2010, 1, 1) - new DateTime(1970, 1, 1)).TotalSeconds,
            };

            MemberFilter babilonio = new MemberFilter()
            {
                MemberId = 10425,
            };
            babilonio.AddExcludedIds<Forum>(74); //General Topic

            MemberFilters = new List<MemberFilter>()
            {
                dpy,
                babilonio,
            };            
        }

        public static List<int> GetExcludedMembers<T>(int id)
        {
            return MemberFilters.Where(f => f.GetExcludedIds<T>().Contains(id))
                                .Select(f => f.MemberId)
                                .ToList();
        }

        private static Dictionary<int, Member> memberMap = new Dictionary<int, Member>();
        private static List<MemberFilter> MemberFilters = new List<MemberFilter>();

        public static List<MemberFilter> GetMemberFilters()
        {
            return MemberFilters;
        }

        private static Member guest = new Member()
        {
            member_id = 0,
            name = "Guest",
            members_seo_name = "guest",
            member_group_id = 2,
        };

        private static Dictionary<int, Group> groupMap = new Dictionary<int, Group>();


        private static void GetAllMembers()
        {
            memberMap = DB.Instance.GetData<Member>($"SELECT * FROM {Member.TableName}").ToDictionary(m => m.Id, m => m);
            groupMap = DB.Instance.GetData<Group>($"SELECT * FROM {Group.TableName}").ToDictionary(g => g.Id, g => g);
        }

        public static Member GetMember(int memberId)
        {
            Member member;

            if(!memberMap.TryGetValue(memberId, out member))
            {
                member = guest;
            }
            else
            {
                member.Contributed = true;
            }

            return member;
        }

        public static Group GetGroup(int groupId)
        {
            Group group;

            if(!groupMap.TryGetValue(groupId, out group))
            {
                group = groupMap[2];
            }

            return group;
        }

        public static string GetUrlHtml(int memberId, bool withStyle=true, string baseUrl="")
        {
            Member member = GetMember(memberId);

            if(member.Id == 0)
            {
                return member.name;
            }

            string preStyle = "";
            string postStyle = "";

            if(withStyle)
            {
                Group group = GetGroup(member.member_group_id);

                if (group.prefix != "<span style='color:#'>")
                {
                    preStyle = group.prefix;
                    postStyle = group.suffix;
                }
            }

            return $"<a href='{baseUrl}{member.Url}'>{preStyle}{member.name}{postStyle}</a>";

        }

        public static string LegalifyMemberSeo(Member member)
        {
            if (member.Id == 20886)
            {
                return "boundless";
            }

            return member.members_seo_name;
        }

        public static string GetGroupName(int groupId, bool withStyle=true)
        {
            Group group = GetGroup(groupId);

            string preStyle = "";
            string postStyle = "";

            if (withStyle)
            {
                preStyle = group.prefix;
                postStyle = group.suffix;
            }

            return $"{preStyle}{group.Description}{postStyle}";
        }

        public static void GenerateMemberList()
        {
            GenerateMemberListByJoined();
            GenerateMemberListByName();
        }

        private static void GenerateMemberListByName()
        {
            GenerateMemberList(GetContributedMembers().OrderBy(m => m.name.ToLower()).ToList(), "member/list/name/{id}.html");
        }

        private static void GenerateMemberListByJoined()
        {
            GenerateMemberList(GetContributedMembers().OrderBy(m => m.joined).ToList(), "member/list/joined/{id}.html");
        }

        private static IEnumerable<Member> GetContributedMembers()
        {
            return memberMap.Values.Where(m => m.Contributed || m.member_posts > 0);
        }

        public static int GetContributedMemberCount()
        {
            return GetContributedMembers().Count();
        }

        private const int MEMBERS_PER_PAGE = 100;

        private static void GenerateMemberList(List<Member> members, string urlTemplate)
        {
            int page = 1;
            int totalPages = (int)Math.Ceiling((decimal)members.Count / MEMBERS_PER_PAGE);

            while (true)
            {
                List<Member> currentMembers = members.Skip((page - 1) * MEMBERS_PER_PAGE).Take(MEMBERS_PER_PAGE).ToList();

                if (currentMembers.Count == 0 && page != 1)
                {
                    break;
                }

                GenerateMemberList(currentMembers, urlTemplate, page, totalPages);

                page++;
            }
        }

        private static void GenerateMemberList(List<Member> currentMembers, string urlTemplate, int page, int totalPages)
        {
            string html = $"<div class='page-header'><h1>Members</h1></div>";

            string buttons = "<div class='btn-group' role='group'>";

            if(urlTemplate.Contains("joined"))
            {
                buttons += $"<button type='button' class='btn btn-primary active'>Joined</button>" +
                    $"<a href='member/list/name/{page}.html' class='btn btn-primary' role='button'>Name</a>";
            }
            else
            {
                buttons += $"<a href='member/list/joined/{page}.html' class='btn btn-primary'>Joined</a>" +
                    $"<button type='button' class='btn btn-primary active'>Name</button>";
                    
            }

            buttons += "</div>";

            html += $"<div class='pull-right'>Sort by: {buttons}</div>";

            html += Output.GeneratePagination(urlTemplate, page, totalPages, true);

            html += "<ul class='list-group'>";

            foreach(Member member in currentMembers)
            {
                html += "<li class='list-group-item'><div class='media'>" +
                        $"<div class='media-left media-middle image-icon'>" +
                        $"<img class='media-object' src='http://youchew.net/forum/uploads/{member.pp_thumb_photo ?? member.pp_main_photo}' />" +
                        $"</div>" +
                        $"<div class='media-body'><div class='row'>" +
                        $"<div class='col-md-10'><h4 class='media-heading'>{GetUrlHtml(member.Id)}</h4>" +
                        $"<p>{GetGroupName(member.member_group_id, false)}</p>" +
                        $"<p>Joined: {LangLogic.FormatDate(member.joined, LangLogic.LongDateFormat)}</p></div>" +
                        $"<div class='col-md-2'>{LangLogic.FormatNumber(member.member_posts, "post", "posts")}</div>" +
                        $"</div>" +
                        $"</div>" +
                        "</div></li>" + Environment.NewLine;
            }

            html += "</ul>" + Environment.NewLine;
            
            html += Output.GeneratePagination(urlTemplate, page, totalPages, false);

            Output output = new Output()
            {
                Content = html,
                CleanupContent = true,
                DownloadImages = true,
                FileName = urlTemplate.Replace("{id}", page.ToString()),
                FoldersDeep = urlTemplate.Count(c => c == '/'),
                Title = "Members",
            };

            output.Generate();
        }

        private static Dictionary<int, string> WarnReasons = new Dictionary<int, string>();

        public static string GetWarnReason(int reasonId)
        {
            string reason;

            if(!WarnReasons.TryGetValue(reasonId, out reason))
            {
                return "Other";
            }

            return reason;
        }

        private const int BANNED_MEMBERS_PER_PAGE = 50;

        public static void WriteBannedMemberIPs()
        {
            List<Type> types = Reflection.Assembly.GetExecutingAssembly().GetTypes()
                                    .Where(t =>
                                    {
                                        if(t.IsClass && t.Namespace == "YouChewArchive.DataContracts")
                                        {
                                            var columnMap = ContentLogic.GetDatabaseColumnMap(t);

                                            return columnMap.ContainsKey("Author") && columnMap.ContainsKey("IpAddress");
                                        }

                                        return false;
                                    })
                                    .ToList();

            Dictionary<Tuple<int, string>, BannedMemberIPCSV> ipMap = new Dictionary<Tuple<int, string>, BannedMemberIPCSV>();

            foreach(Type type in types)
            {
                var ips = GetBannedMemberIpAddresses(type)
                            .ToDictionary(m => Tuple.Create(m.MemberId, m.IpAddress), m => m);

                foreach (var ip in ips)
                {
                    BannedMemberIPCSV bannedIp;

                    if (!ipMap.TryGetValue(ip.Key, out bannedIp))
                    {
                        ipMap.Add(ip.Key, ip.Value);
                    }
                    else
                    {
                        bannedIp.Count += ip.Value.Count;
                    }
                }

                
            }
            using (StreamWriter sw = new StreamWriter(@"C:\YC\BannedMemberIpAddresses.csv", false, Encoding.UTF8))
            using (CsvWriter csv = new CsvWriter(sw))
            {
                csv.WriteRecords(ipMap.Values);
            }
        }

        private static List<BannedMemberIPCSV> GetBannedMemberIpAddresses(Type type)
        {
            var columnMap = ContentLogic.GetDatabaseColumnMap(type);
            string tableName = AppLogic.GetStaticField<string>(type, "TableName");
            string databaseprefix = DB.GetDatabasePrefix(type);

            if(!columnMap.ContainsKey("Author") && !columnMap.ContainsKey("IpAddress"))
            {
                throw new Exception($"Type {type.Name} does not have the Author and IpAddress column map");
            }

            string author = databaseprefix + columnMap["Author"];
            string ipAddress = databaseprefix + columnMap["IpAddress"];

            string query = $"SELECT c.{author} MemberId, c.{ipAddress} IpAddress, COUNT(*) 'Count' FROM {tableName} c JOIN {Member.TableName} m ON m.member_id = c.{author} AND m.temp_ban <> 0 GROUP BY c.{author}, c.{ipAddress}";

            return DB.Instance.GetData<BannedMemberIPCSV>(query);

        }

        public static void WriteBannedMembersCSV()
        {
            List<BannedMemberCSV> banned = memberMap.Values
                .Where(m => m.IsBanned())
                .Select(m => new BannedMemberCSV()
            {
                Id = m.Id,
                Name = m.name,
                Email = m.email,
                BanDate = (m.BannedDate.HasValue ? (DateTime?)LangLogic.ConvertFromUnixtime(m.BannedDate.Value) : null),
                BanDateUnix = m.BannedDate,
                PermanentBan = m.IsPermBanned(),
                TempBan = m.IsTempBanned(),
                Warnings = m.WarnLogs.Count(),
                BannedNotes = m.BannedInfo?.member_banned_notes,
            }).ToList();

            using(StreamWriter sw = new StreamWriter(@"C:\YC\banned.csv", false, Encoding.UTF8))
            using (CsvWriter csv = new CsvWriter(sw))
            {
                csv.WriteRecords(banned);
            }

            List<BannedMemberWarnLogCSV> warnlog = DB.Instance.GetData<WarnLog>($"SELECT * FROM {WarnLog.TableName}")
                                                     .Select(wl => new BannedMemberWarnLogCSV()
                                                     {
                                                         Acknowledged = wl.acknowledged,
                                                         DateUnix = wl.date,
                                                         ExpireDateUnix = wl.expire_date,
                                                         Id = wl.id,
                                                         MemberId = wl.member,
                                                         MemberNote = wl.note_member,
                                                         ModeratorNote = wl.note_mods,
                                                         ModeratorId = wl.moderator,
                                                         Points = wl.points,
                                                         ReasonId = wl.reason,
                                                     }).ToList();

            using (StreamWriter sw = new StreamWriter(@"C:\YC\warnlog.csv", false, Encoding.UTF8))
            using (CsvWriter csv = new CsvWriter(sw))
            {
                csv.WriteRecords(warnlog);
            }

            using (StreamWriter sw = new StreamWriter(@"C:\YC\banfilter.csv", false, Encoding.UTF8))
            using (CsvWriter csv = new CsvWriter(sw))
            {
                csv.WriteRecords(DB.Instance.GetData<BanFilterCSV>("SELECT * FROM core_banfilters"));
            }

            WriteBannedMemberIPs();
        }

        public static void GenerateBannedMembers()
        {
            List<Member> bannedMembers = memberMap.Values
                                                  .Where(m => m.IsPermBanned() && m.member_posts > 15)
                                                  .OrderByDescending(m => m.BannedDate)
                                                  .ToList();

            List<Member> suspendedMembers = memberMap.Values
                                                     .Where(m => m.IsTempBanned())
                                                     .OrderByDescending(m => m.BannedDate)
                                                     .ToList();
            int page = 1;

            int totalPages = (int)Math.Ceiling((decimal)(bannedMembers.Count + suspendedMembers.Count) / BANNED_MEMBERS_PER_PAGE);

            while (true)
            {
                List<Member> currentBannedMembers;

                if(page == 1)
                {
                    currentBannedMembers = bannedMembers.Take(BANNED_MEMBERS_PER_PAGE - suspendedMembers.Count).ToList();
                }
                else
                {
                    currentBannedMembers = bannedMembers.Skip((BANNED_MEMBERS_PER_PAGE - suspendedMembers.Count) + ((page - 2) * BANNED_MEMBERS_PER_PAGE))
                                                        .Take(BANNED_MEMBERS_PER_PAGE)
                                                        .ToList();
                }

                if (currentBannedMembers.Count == 0 && page != 1)
                {
                    break;
                }

                GenerateBannedMembers(currentBannedMembers, page == 1 ? suspendedMembers : null, page, totalPages);

                page++;
            }
        }

        private static void GenerateBannedMembers(List<Member> currentBannedMembers, List<Member> suspendedMembers, int page, int totalPages)
        {
            string html = "<div class='page-header'><h1>Banned Members</h1></div>";

            html += Output.GeneratePagination("banned/{id}.html", page, totalPages, true);

            if (suspendedMembers != null)
            {
                string suspendedListHtml = "<ul class='list-group'>";

                foreach(Member member in suspendedMembers)
                {
                    string suspendedDate = member.BannedDate.HasValue ? LangLogic.ConvertFromUnixtime(member.BannedDate.Value).ToString("MMMM dd, yyyy") : "Unknown";
                    string unsuspendedDate = LangLogic.ConvertFromUnixtime(member.temp_ban.Value).ToString("MMMM dd, yyyy");

                    suspendedListHtml += "<li class='list-group-item'><div class='row'>" +
                        $"<div class='col-md-3'><p>{GetUrlHtml(member.Id, false, "http://youchew.net/")}</p><p>Email: {member.email}</p><p>Suspended: {suspendedDate}</p><p>Ususpension Date: {unsuspendedDate}</p></div>" +
                        $"<div class='col-md-9'>{GenerateWarnLog(member)}</div>" +
                        "</div></li>";
                    
                }

                suspendedListHtml += "</ul>";

                html += Output.CreatePanel("Suspended Members", null, suspendedListHtml);

            }

            if(currentBannedMembers.Count > 0)
            {
                string bannedListHtml = "<ul class='list-group'>";

                foreach (Member member in currentBannedMembers)
                {
                    string bannedDate = member.BannedDate.HasValue ? LangLogic.ConvertFromUnixtime(member.BannedDate.Value).ToString("MMMM dd, yyyy") : "Unknown";

                    bannedListHtml += "<li class='list-group-item'><div class='row'>" +
                        $"<div class='col-md-3'><p>{GetUrlHtml(member.Id, false, "http://youchew.net/")}</p><p>Email: {member.email}</p><p>Banned: {bannedDate}</p></div>" +
                        $"<div class='col-md-9'>{GenerateWarnLog(member)}</div>" +
                        "</div></li>";

                }

                bannedListHtml += "</ul>";

                html += Output.CreatePanel("Banned Members", null, bannedListHtml);
            }


            html += Output.GeneratePagination("banned/{id}.html", page, totalPages, false);

            Output output = new Output()
            {
                Content = html,
                CleanupContent = true,
                DownloadImages = true,
                FoldersDeep = 1,
                FileName = $"banned/{page}.html",
                Title = "Banned Members",
                BaseUrl = "http://youchew.net/",
                NoNav = true,
                
            };

            output.Generate();
            

        }

        private static string GenerateWarnLog(Member member)
        {
            string html = "<ul class='list-group'>";

            foreach(WarnLog warning in member.WarnLogs)
            {
                int notes = 0;
                bool hasModNote = !String.IsNullOrEmpty(warning.note_mods);
                bool hasMemberNote = !String.IsNullOrEmpty(warning.note_member);

                if(!hasModNote)
                {
                    notes++;
                }

                if(hasMemberNote)
                {
                    notes++;
                }

                int modCol = 4;
                int memberCol = 4;

                if(hasModNote && !hasMemberNote)
                {
                    modCol = 8;
                }

                if(hasMemberNote && !hasModNote)
                {
                    memberCol = 4;
                }



                html += "<li class='list-group-item'>" +
                    $"<div class='row post'>" +
                    $"<div class='col-md-4'><p>Warned: {LangLogic.FormatDate(warning.date, LangLogic.ShortDateNoTime)}</p><p>{GetWarnReason(warning.reason.GetValueOrDefault(0))}</p></div>" +
                    (hasMemberNote ? $"<div class='col-md-{memberCol}'>{Output.CreatePanel("Member Note", warning.note_member)}</div>" : "") +
                    (hasModNote ? $"<div class='col-md-{modCol}'>{Output.CreatePanel("Moderator Note", warning.note_mods)}</div>" : "") +
                    "</div></li>";
            }

            html += "</ul>";

            return html;
        }

        public static void GenerateMembers()
        {
            int completed = 0;

            EventLogic.RaiseEvent(ProgressEventArgs.App, "members");


            EventLogic.RaiseEvent(ProgressEventArgs.Containers, memberMap.Count.ToString());
            EventLogic.RaiseEvent(ProgressEventArgs.ContainersCompleted, completed.ToString());

            foreach (Member member in memberMap.Values)
            {
                GenerateMembers(member);

                EventLogic.RaiseEvent(ProgressEventArgs.ContainersCompleted, (++completed).ToString());
            }
        }

        public static void GenerateMembers(Member member)
        {

            EventLogic.RaiseEvent(ProgressEventArgs.CurrentContainer, $"{member.Id}: {member.name}");

            string html = $"<div class='page-header'><h1>{member.name}</h1><h4>{GetGroupName(member.member_group_id)}</h4></div>";

            html += "<div>"
                  + $"<div class='col-md-3'><div><img src='http://youchew.net/uploads/{member.pp_main_photo ?? member.pp_thumb_photo}' class='avatar' /></div>";

            if(!String.IsNullOrEmpty(member.member_title))
            {
                html += $"<div><p>{member.member_title}</p></div>";
            }

            html += $"<div><p>Joined: {LangLogic.FormatDate(member.joined, LangLogic.ShortDateFormat)}</p>"
                  + $"<p>{LangLogic.FormatNumber(member.member_posts, "post", "posts")}</p></div>";


            if(member.HasPreviousStaffs())
            {
                string prevListGroup = "<ul class='list-group'>";

                foreach (string group in member.PreviousStaffs)
                {
                    prevListGroup += $"<li class='list-group-item'>{group}</li>" + Environment.NewLine;
                }

                prevListGroup += "</ul>" + Environment.NewLine;

                html += Output.CreatePanel("Staff Positions Held", null, prevListGroup);
            }

            if(member.HasPreviousNames())
            {
                string prevListGroup = "<ul class='list-group'>";

                foreach(string name in member.PreviousNames)
                {
                    prevListGroup += $"<li class='list-group-item'>{name}</li>" + Environment.NewLine;
                }

                prevListGroup += "</ul>" + Environment.NewLine;

                html += Output.CreatePanel("Previous Names", null, prevListGroup);
            }

            html += "</div>"
                  + $"<div class='col-md-9'>";

            if(!String.IsNullOrEmpty(member.ProfileFields.AboutMe))
            {
                html += Output.CreatePanel("About Me", member.ProfileFields.AboutMe);
            }

            if(!String.IsNullOrEmpty(member.signature))
            {
                html += Output.CreatePanel("Signature", member.signature, null, true);
            }

            if(member.HasBlogs())
            {
                string blogListGroup = "<div class='list-group'>";

                foreach(Blog blog in member.Blogs)
                {
                    blogListGroup += $"<a href='{BlogLogic.GetBlogUrl(blog)}' class='list-group-item'>{blog.Description}</a>" + Environment.NewLine;
                }

                blogListGroup += "</div>" + Environment.NewLine;

                html += Output.CreatePanel("Blogs", null, blogListGroup);
            }

            if(member.HasAwards())
            {
                string awardhtml = "<div class='row flex-row'>";

                foreach (UserAward ua in member.Awards)
                {

                    Award award = AwardLogic.GetAward(ua.award_id);
                    if (award != null)
                    {
                        awardhtml += $"<div class='col-md-4 col-sm-6 user-award'><div class='thumbnail'><div class='caption'><h4><img src='http://youchew.net/forum/uploads/{award.icon_thumb ?? award.icon}' />{award.name}</h4>" +
                            $"<p>Given on {LangLogic.FormatDate(ua.date, LangLogic.ShortDateNoTime)} by {GetUrlHtml(ua.awarded_by)}</p>" +
                            $"<p>{ua.notes}</p></div></div></div>";
                    }
                }

                awardhtml += "</div>";
                

                html += Output.CreatePanel("Awards", awardhtml, null, true);
            }

            html += "</div>";

            html += "</div>";

            Output output = new Output()
            {
                Content = html,
                CleanupContent = true,
                DownloadImages = true,
                FileName = member.Url,
                FoldersDeep = 1,
                Title = member.name,
                ActiveNav = "Members",
            };

            output.Generate();
            
        }

        
    }
}
