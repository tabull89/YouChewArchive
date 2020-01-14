using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YouChewArchive.Data;
using YouChewArchive.DataContracts;

namespace YouChewArchive.Logic
{
    public static class ReputationLogic
    {
        private static string GetTypeIdName<T>()
        {
            if(AppLogic.HasStaticField<T>("RepuationTypeId"))
            {
                return AppLogic.GetStaticField<T, string>("RepuationTypeId");
            }

            return DB.GetDatabasePrefix<T>() + AppLogic.GetStaticField<T, string>("DatabaseColumnId"); 
        }

        public static int GetTotalRepuation<T>(int id)
        {
            string app = AppLogic.GetStaticField<T, string>("Application");
            string columnId = GetTypeIdName<T>();          

            List<MySqlParameter> parameters = new List<MySqlParameter>()
            {
                new MySqlParameter("@app", MySqlDbType.String) { Value = app },
                new MySqlParameter("@type", MySqlDbType.String) { Value = columnId },
                new MySqlParameter("@id", MySqlDbType.Int32) { Value = id },
            };


            int? repRating = DB.Instance.ExecuteScalar<int?>($"SELECT SUM(rep_rating) FROM {Reputation.TableName} WHERE app=@app AND type=@type AND type_id=@id", parameters);

            return repRating.GetValueOrDefault();
        }

        public static List<HighestReputation> GetHighestReputationPosts(int count)
        {
            List<int> ids = ForumLogic.GetAllForumIds();

            string app = Post.Application;
            string columnId = GetTypeIdName<Post>();

            List<MySqlParameter> parameters = new List<MySqlParameter>()
            {
                new MySqlParameter("@app", MySqlDbType.String) { Value = app },
                new MySqlParameter("@type", MySqlDbType.String) { Value = columnId },
            };


            string query = $@"SELECT type_id, SUM(rep_rating) rep FROM {Reputation.TableName} 
                              JOIN {Post.TableName} p ON p.pid = type_id 
                              JOIN {Topic.TableName} t ON t.tid = p.topic_id
                              JOIN {Forum.TableName} f ON t.forum_id = f.id
                              WHERE ({ContentLogic.GeneratePermissionWhere<Post>("p")}) AND ({ContentLogic.GeneratePermissionWhere<Topic>("t")}) 
                                  AND f.id IN ({String.Join(",", ids.Select(id => id.ToString()))}) AND t.tid <> 119942
                                  AND app=@app AND type=@type 
                              GROUP BY type_id                              
                              ORDER BY SUM(rep_rating) DESC LIMIT {count}";

            return DB.Instance.GetData<HighestReputation>(query, parameters, 600);

        }
    }
}
