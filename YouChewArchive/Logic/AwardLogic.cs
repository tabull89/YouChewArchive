using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YouChewArchive.Data;
using YouChewArchive.DataContracts;

namespace YouChewArchive.Logic
{
    public static class AwardLogic
    {
        private static Dictionary<int, Award> awardMap;

        static AwardLogic()
        {
            awardMap = DB.Instance.GetData<Award>($"SELECT * FROM {Award.TableName} WHERE visible = 1").ToDictionary(a => a.Id, a => a);
        }

        public static Award GetAward(int id)
        {
            Award award;

            awardMap.TryGetValue(id, out award);

            return award;
        }

        public static List<AwardCategory> GetCategories()
        {
            return DB.Instance.GetData<AwardCategory>($"SELECT * FROM {AwardCategory.TableName} WHERE visible = 1 ORDER BY placement");
        }

        public static List<Award> GetAwards(AwardCategory category)
        {
            return awardMap.Values
                    .Where(a => a.parent == category.Id)
                    .OrderBy(a => a.placement)
                    .ToList();
        }

        public static int GetAwardedCount(Award award)
        {
            return DB.Instance.ExecuteScalar<int>($"SELECT COUNT(DISTINCT user_id) FROM {UserAward.TableName} WHERE award_id = {award.Id} AND is_active = 1 AND approved = 1");
        }

        public static void GenerateAwardPage()
        {
            List<AwardCategory> categories = GetCategories();

            string html = $"<div class='page-header'><h1>Awards</h1></div>";

            foreach(AwardCategory category in categories)
            {
                string listGroup = "<ul class='list-group'>";

                List<Award> awards = GetAwards(category);

                foreach(Award award in awards)
                {
                    listGroup += "<li class='list-group-item'><div class='media'>" +
                        $"<div class='media-left media-middle image-icon'>" +
                        $"<img class='media-object' src='http://youchew.net/forum/uploads/{award.icon ?? award.icon_thumb}' />" +
                        $"</div>" +
                        $"<div class='media-body'>" +
                        $"<h4 class='media-heading'>{award.name}</h4>" +
                        $"<p>{award.desc}</p>" +
                        $"</div>" +
                        $"<div class='media-right award-awarded'>" +
                        LangLogic.FormatNumber(GetAwardedCount(award), "awarded", "awarded") +
                        $"</div>" +
                        "</div></li>" + Environment.NewLine;
                }

                listGroup += "</ul>" + Environment.NewLine;

                html += Output.CreatePanel(category.title, null, listGroup, true);
            }

            Output output = new Output()
            {
                Content = html,
                CleanupContent = true,
                DownloadImages = true,
                FileName = "extra/awards.html",
                Title = "Awards",
                FoldersDeep = 1,
            };

            output.Generate();
            
        }
    }
}
