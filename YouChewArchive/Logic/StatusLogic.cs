using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YouChewArchive.Data;
using YouChewArchive.DataContracts;

namespace YouChewArchive.Logic
{
    public static class StatusLogic
    {
        private static Dictionary<int, List<Reply>> Replies = new Dictionary<int, List<Reply>>();
        private static List<Status> Statuses = new List<Status>();
        private static Dictionary<int, Status> StatusMap = new Dictionary<int, Status>();

        static StatusLogic()
        {
            GetData();
        }

        private static void GetData()
        {
            GetStatuses();
            GetReplies();
        }

        public static void GetStatuses()
        {
            string permissionWhere = ContentLogic.GeneratePermissionWhere<Status>();
            string query = $"SELECT * FROM {Status.TableName} WHERE {permissionWhere} ORDER BY status_date DESC";

            Statuses = DB.Instance.GetData<Status>(query);

            StatusMap = Statuses.ToDictionary(s => s.Id, s => s);
        }

        private static void GetReplies()
        {
            string permissionWhere = ContentLogic.GeneratePermissionWhere<Reply>();
            string query = $"SELECT * FROM {Reply.TableName} WHERE {permissionWhere}";

            Replies = DB.Instance.GetData<Reply>(query)
                        .GroupBy(r => r.Item)
                        .ToDictionary(g => g.Key, g => g.OrderBy(r => r.Date).ToList());
        }

        private const int STATUSES_PER_PAGE = 100;
        private const int REPLIES_FOR_PAGE = 10;

        private static Dictionary<int, int> StatusPageMap = new Dictionary<int, int>();

        public static int GetStatusPage(Status status)
        {
            int page;

            if(!StatusPageMap.TryGetValue(status.Id, out page))
            {

                int count = Statuses.Where(s => s.Date >= status.Date).Count();

                page = (int)Math.Ceiling(count / (decimal)STATUSES_PER_PAGE);

                StatusPageMap.Add(status.Id, page);
            }

            return page;

        }

        public static Status GetStatus(int id)
        {
            Status status;

            StatusMap.TryGetValue(id, out status);

            return status;
            
        }

        public static List<Reply> GetReplies(Status status)
        {
            List<Reply> replies;

            if(!Replies.TryGetValue(status.Id, out replies))
            {
                replies = new List<Reply>();
            }

            return replies;
        }

        public static string GetStatusUrl(Status status, int? page = null)
        {
            if(page == null)
            {
                page = GetStatusPage(status);
            }

             return $"status/{page.Value}.html#status-{status.Id}";   
        }

        public static string GetFullStatusUrl(Status status)
        {
            return $"status/full/{status.Id}.html";
        }

        public static string GetReplyUrl(Reply reply, int? page = null)
        {
            Status status = GetStatus(reply.Item);

            if(status != null)
            {
                int replies = GetReplies(status).Count;

                if(replies > REPLIES_FOR_PAGE)
                {
                    return $"status/full/{status.Id}.html#reply-{reply.Id}";
                }
                else
                {
                    if(page == null)
                    {
                        page = GetStatusPage(status);
                    }

                    return $"status/{page.Value}.html#reply-{reply.Id}";
                }

            }

            return "";
        }

        public static void GenerateStatusUpdates()
        {
            int totalPages = (int)Math.Ceiling((decimal)Statuses.Count / STATUSES_PER_PAGE);

            EventLogic.RaiseEvent(ProgressEventArgs.App, "Status Updates");
            EventLogic.RaiseEvent(ProgressEventArgs.Containers, "0");
            EventLogic.RaiseEvent(ProgressEventArgs.ContainersCompleted, "0");
            EventLogic.RaiseEvent(ProgressEventArgs.CurrentContainer, "");
            EventLogic.RaiseEvent(ProgressEventArgs.Items, Statuses.Count.ToString());
            EventLogic.RaiseEvent(ProgressEventArgs.CurrentItem, "");
            EventLogic.RaiseEvent(ProgressEventArgs.Comments, "0");
            EventLogic.RaiseEvent(ProgressEventArgs.CommentsCompleted, "0");

            int page = 1;
            int statusesCompleted = 0;
            EventLogic.RaiseEvent(ProgressEventArgs.ItemsCompleted, statusesCompleted.ToString());

            while (true)
            {
                List<Status> currentStatuses = Statuses.Skip((page - 1) * STATUSES_PER_PAGE).Take(STATUSES_PER_PAGE).ToList();

                if (currentStatuses.Count == 0 && page != 1)
                {
                    break;
                }

                GenerateStatusUpdates(currentStatuses, page, totalPages, ref statusesCompleted);

                page++;

               
            }
        }

        private static void GenerateStatusUpdates(List<Status> currentStatuses, int page, int totalPages, ref int statusesCompleted)
        {
            string html = $"<div class='page-header'><h1>Status Updates</h1></div>";

            html += Output.GeneratePagination("status/{id}.html", page, totalPages, true);

            html += "<ul class='list-group'>";

            foreach (Status status in currentStatuses)
            {
                string repHtml = "";

                int rep = ReputationLogic.GetTotalRepuation<Status>(status.Id);

                if (rep != 0)
                {
                    string repClass = rep > 0 ? "rep-positive" : "rep-negative";

                    repHtml = $"<div class='col-md-12 text-right'><span class='rep {repClass}'>{rep}</span></div>";
                }

                string toMemberHtml = "";

                if(status.member_id != status.Author)
                {
                    toMemberHtml = $" -> {MemberLogic.GetUrlHtml(status.member_id)}";
                }

                html += $"<li class='list-group-item status'><div class='row' id='status-{status.Id}'>";
                html += $"<div class='col-md-2'>{MemberLogic.GetUrlHtml(status.Author)}{toMemberHtml}</div>";
                html += $"<div class='col-md-10'><div class='row'>" +
                    $"<div class='col-md-12 post-info-bar'>{LangLogic.FormatDate(status.Date, LangLogic.LongDateFormat)}<a class='pull-right' title='Link to this status' href='{GetStatusUrl(status, page)}'>#</a></div>" +
                    $"<div class='col-md-12'>{status.Content}</div>" +
                    repHtml + "</div>";

                List<Reply> replies = GetReplies(status);

                if(replies.Count > 0)
                {
                    html += "<ul class='list-group'>";

                    if(replies.Count > REPLIES_FOR_PAGE)
                    {
                        html += $"<a class='list-group-item reply' href='{GetFullStatusUrl(status)}'>{replies.Count} Replies</a>";

                        GenerateFullStatusUpdate(status, page, rep);
                    }
                    else
                    {
                        foreach(Reply reply in replies)
                        {
                            repHtml = "";

                            rep = ReputationLogic.GetTotalRepuation<Reply>(reply.Id);

                            if (rep != 0)
                            {
                                string repClass = rep > 0 ? "rep-positive" : "rep-negative";

                                repHtml = $"<div class='col-md-12 text-right'><span class='rep {repClass}'>{rep}</span></div>";
                            }

                            html += $"<li class='list-group-item reply'><div class='row' id='reply-{reply.Id}'>";
                            html += $"<div class='col-md-2'>{MemberLogic.GetUrlHtml(reply.Author)}</div>";
                            html += $"<div class='col-md-10'>" +
                                $"<div class='col-md-12 post-info-bar'>{LangLogic.FormatDate(reply.Date, LangLogic.LongDateFormat)}<a class='pull-right' title='Link to this reply' href='{GetReplyUrl(reply)}'>#</a></div>" +
                                $"<div class='col-md-12'>{reply.Content}</div>" +
                                repHtml +
                                "</div></div></li>" + Environment.NewLine;
                        }
                    }

                    html += "</ul>" + Environment.NewLine;
                }

                html += $"</div>";

                html += "</div></li>" + Environment.NewLine;

                EventLogic.RaiseEvent(ProgressEventArgs.ItemsCompleted, (++statusesCompleted).ToString());
            }

            html += "</ul>" + Environment.NewLine;

            html += Output.GeneratePagination("status/{id}.html", page, totalPages, false);

            Output output = new Output()
            {
                Content = html,
                CleanupContent = true,
                DownloadImages = true,
                FileName = $"status/{page}.html",
                FoldersDeep = 1,
                Title = "Status Updates",
            };

            output.Generate();
        }

        private static void GenerateFullStatusUpdate(Status status, int page, int rep)
        {
            string html = $"<div class='page-header'><h1>Status Update From {MemberLogic.GetMember(status.Author).name}</h1></div>";

            html += "<ul class='list-group'>";

            string repHtml = "";


            if (rep != 0)
            {
                string repClass = rep > 0 ? "rep-positive" : "rep-negative";

                repHtml = $"<div class='col-md-12 text-right'><span class='rep {repClass}'>{rep}</span></div>";
            }

            string toMemberHtml = "";

            if (status.member_id != status.Author)
            {
                toMemberHtml = $" -> {MemberLogic.GetUrlHtml(status.member_id)}";
            }

            html += $"<li class='list-group-item'><div class='row status' id='status-{status.Id}'>";
            html += $"<div class='col-md-2'>{MemberLogic.GetUrlHtml(status.Author)}{toMemberHtml}</div>";
            html += $"<div class='col-md-10'><div class='row'>" +
                $"<div class='col-md-12 post-info-bar'>{LangLogic.FormatDate(status.Date, LangLogic.LongDateFormat)}<a class='pull-right' title='Link to this status' href='{GetStatusUrl(status, page)}'>#</a></div>" +
                $"<div class='col-md-12'>{status.Content}</div>" +
                repHtml + "</div>";

            List<Reply> replies = GetReplies(status);


            html += "<ul class='list-group'>";


            foreach (Reply reply in replies)
            {
                repHtml = "";

                rep = ReputationLogic.GetTotalRepuation<Reply>(reply.Id);

                if (rep != 0)
                {
                    string repClass = rep > 0 ? "rep-positive" : "rep-negative";

                    repHtml = $"<div class='col-md-12 text-right'><span class='rep {repClass}'>{rep}</span></div>";
                }

                html += $"<li class='list-group-item reply'><div class='row' id='reply-{reply.Id}'>";
                html += $"<div class='col-md-2'>{MemberLogic.GetUrlHtml(reply.Author)}</div>";
                html += $"<div class='col-md-10'>" +
                    $"<div class='col-md-12 post-info-bar'>{LangLogic.FormatDate(reply.Date, LangLogic.LongDateFormat)}<a class='pull-right' title='Link to this reply' href='{GetReplyUrl(reply)}'>#</a></div>" +
                    $"<div class='col-md-12'>{reply.Content}</div>" +
                    repHtml +
                    "</div></div></li>" + Environment.NewLine;
            }


            html += "</ul>" + Environment.NewLine;


            html += $"</div></li>";

            html += "</div></ul>" + Environment.NewLine;

            Output output = new Output()
            {
                Content = html,
                CleanupContent = true,
                DownloadImages = true,
                FileName = GetFullStatusUrl(status),
                FoldersDeep = 2,
                Title = $"Status Update From {MemberLogic.GetMember(status.Author).name}",
                ActiveNav = "Status Updates",
            };

            output.Generate();

        }
    }
}
