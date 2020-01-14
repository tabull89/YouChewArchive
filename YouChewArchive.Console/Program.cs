using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YouChewArchive.Classes;
using YouChewArchive.Data;
using YouChewArchive.DataContracts;
using YouChewArchive.Logic;

namespace YouChewArchive.Console
{
    class Program
    {
        static private Progress progress = new Progress();

        static string UnixToString(int? unix)
        {
            if (!unix.HasValue)
            {
                return null;
            }

            return new DateTime(1970, 1, 1).AddSeconds(unix.Value).ToString();
        }

        static string LogMembers(IEnumerable<Member> members, string title)
        {
            string text = title + Environment.NewLine;

            foreach(var m in members.Where(m => m.ShowPreviousStaffs()))
            {
                text += m.Id + ": " + m.name + Environment.NewLine;
            }

            return text;
        }

        static void LogMembers()
        {
            List<Member> members = DB.Instance.GetData<Member>($"SELECT * FROM {Member.TableName}");


            string owners = LogMembers(members.Where(m => m.WasOwner()), "Owners");
            string admins = LogMembers(members.Where(m => m.WasAdmin()), "Admin");
            string mods = LogMembers(members.Where(m => m.WasModerator()), "Mods");
            string cafe = LogMembers(members.Where(m => m.Awards.Any(a => a.award_id == 79)), "Cafe");
            string janitor = LogMembers(members.Where(m => m.Awards.Any(a => a.award_id == 67)), "Janitor");
            string editorincheif = LogMembers(members.Where(m => m.WasEditorInChief()), "Editor In Cheif");
            string editor = LogMembers(members.Where(m => m.WasEditor()), "Editor");
            string writing = LogMembers(members.Where(m => m.IsInGroup(9) || m.WasWritingStaff()), "Writing");
            string tech = LogMembers(members.Where(m => m.WasTechStaff()), "Tech");
            string design = LogMembers(members.Where(m => m.IsInGroup(60) || m.WasDesignStaff()), "design");
            string art = LogMembers(members.Where(m => m.WasStaffArtist()), "Staff Artist");
            string free = LogMembers(members.Where(m => m.WasFreelancer()), "Freelancer");
            string review = LogMembers(members.Where(m => m.WasReviewCrew()), "Review Crew");

            string total = owners + Environment.NewLine + admins + Environment.NewLine + mods + Environment.NewLine + cafe + Environment.NewLine + janitor + Environment.NewLine 
                + editorincheif + Environment.NewLine + editor + Environment.NewLine + writing + Environment.NewLine 
                + tech + Environment.NewLine + design + Environment.NewLine + art + Environment.NewLine + free + Environment.NewLine + review + Environment.NewLine;
        }

        static void Main(string[] args)
        {
            LogLogic.DisableLog();


            try
            {
                EventLogic.OnProgressEvent += OnProgessEvent;
                LogLogic.ResetAll();

                GenerateAll();

                //MemberLogic.GenerateMembers();

                //BlogLogic.GenerateBlogs();

                //MemberLogic.GenerateMembers(MemberLogic.GetMember(260));

                //DeleteEmptyDirs(@"C:\YC\images\external");

                //ClassGenerator.GenerateFromXMLFile(@"C:\Users\tabull\source\repos\YouChewArchive\YouChewArchive\tables.xml");
                //ClassGenerator.GenerateFromXMLFile(@"C:\Users\tabull\source\repos\YouChewArchive\YouChewArchive\Tables2.xml");
                //ClassGenerator.GenerateFromXMLFile(@"C:\Users\tabull\source\repos\YouChewArchive\YouChewArchive\Tables3.xml");
                // GenerateAll();

                //ForumLogic.GeneratePosts(DB.Instance.GetRecordById<Topic>(120348));

                //AwardLogic.GenerateAwardPage();

                //MemberLogic.GenerateMembers(MemberLogic.GetMember(160));
                //MemberLogic.GenerateMembers(MemberLogic.GetMember(41));
                //MemberLogic.GenerateMembers(MemberLogic.GetMember(605));

                //ForumLogic.GenerateTopicList(ForumLogic.GetForum(73));
                //ForumLogic.GenerateTopicList(DB.Instance.GetRecordById<Forum>(24));
                //ForumLogic.GeneratePosts(DB.Instance.GetRecordById<Topic>(3));


            }
            finally
            {
                DB.Instance.Dispose();
                LogLogic.Dispose();
                ImageLogic.SaveInvalidImages();
            }

            LogLogic.SortAndDistinctAll(); 
        }

        private static void GenerateAll()
        {
            using (StopWatch totalSw = new StopWatch("Total"))
            {

                //using (StopWatch sw = new StopWatch("Private Messages"))
                //{
                //    MessageLogic.GeneratePMs();
                //}

                using (StopWatch sw = new StopWatch("Home and Search"))
                {
                    ContentLogic.GenerateHomePage();
                    ContentLogic.GenerateSearchPage();
                }

                using (StopWatch sw = new StopWatch("Forum"))
                {
                    ForumLogic.GenerateTopicList();
                }

                using (StopWatch sw = new StopWatch("Blog"))
                {
                    BlogLogic.GenerateBlogs();
                }

                using (StopWatch sw = new StopWatch("Status Updates"))
                {
                    StatusLogic.GenerateStatusUpdates();
                }

                using (StopWatch sw = new StopWatch("Videos"))
                {
                    VideoLogic.GenerateVideos();
                }

                using (StopWatch sw = new StopWatch("Extra"))
                {
                    ForumLogic.GenerateHighestReputation();
                    ImageLogic.GenerateBanners();
                    ImageLogic.GenerateTradingCardGame();
                    AwardLogic.GenerateAwardPage();
                }

                using (StopWatch sw = new StopWatch("Members"))
                {
                    MemberLogic.GenerateMembers();
                    MemberLogic.GenerateMemberList();
                    //MemberLogic.GenerateBannedMembers();
                    //MemberLogic.WriteBannedMembersCSV();
                }

                using (StopWatch sw = new StopWatch("Board Index"))
                {
                    ForumLogic.GenerateBoardIndex();
                }
            }
        }

        private static void Fluff()
        {
            string emotes = @"C:\Users\tabull\source\repos\YouChewArchive\YouChewArchive\Output\uploads\emoticons";

            List<string> files = System.IO.Directory.GetFiles(emotes).Where(e => !e.Contains("default")).ToList();

            foreach(string file in files)
            {
                System.IO.FileInfo fi = new System.IO.FileInfo(file);

                string defaultFileName = emotes + "\\default_" + fi.Name;

                if(!System.IO.File.Exists(defaultFileName))
                {
                    System.IO.File.Copy(file, defaultFileName);
                }
            }
        }

        static void DeleteEmptyDirs(string dir)
        {
            if (String.IsNullOrEmpty(dir))
                throw new ArgumentException(
                    "Starting directory is a null reference or an empty string",
                    "dir");

            try
            {
                foreach (var d in Directory.EnumerateDirectories(dir))
                {
                    DeleteEmptyDirs(d);
                }

                var entries = Directory.EnumerateFileSystemEntries(dir);

                if (!entries.Any())
                {
                    try
                    {
                        Directory.Delete(dir);
                    }
                    catch (UnauthorizedAccessException) { }
                    catch (DirectoryNotFoundException) { }
                }
            }
            catch (UnauthorizedAccessException) { }
        }

        static private Stopwatch sw = new Stopwatch();

        static void OutputConsole()
        {
            if(!sw.IsRunning)
            {
                sw.Start();
            }

            if(sw.ElapsedMilliseconds > 50 || progress.Important)
            {

                System.Console.Clear();

                StringBuilder sb = new StringBuilder();

                sb.AppendLine($"App: {progress.App}");

                if (!String.IsNullOrEmpty(progress.CurrentContainer))
                {
                    sb.AppendLine(progress.CurrentContainer);
                }

                if(!String.IsNullOrEmpty(progress.CurrentItem))
                {
                    sb.AppendLine(progress.CurrentItem);
                }

                sb.AppendLine($"Containers: {progress.ContainersCompleted}/{progress.Containers}")
                  .AppendLine($"Items: {progress.ItemsCompleted}/{progress.Items}")
                  .AppendLine($"Comments: {progress.CommentsCompleted}/{progress.Comments}");

                System.Console.Write(sb.ToString());

                sw.Reset();
                progress.Important = false;
            }

        }

        private static void OnProgessEvent(object sender, List<ProgressEventArgs> args)
        {
            progress.ProcessEvent(args);
            OutputConsole();
        }

        static void DisplayForums(List<ForumContainer> fcs, int indent = 0)
        {
            foreach(var fc in fcs)
            {
                System.Console.WriteLine(new string('\t', indent) + fc.Forum.Description);

                DisplayForums(fc.ChildForums, indent + 1);
                
            }
        }
    }
}
