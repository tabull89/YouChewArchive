using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YouChewArchive.Data;
using YouChewArchive.DataContracts;
using YouChewArchive.Logic;

namespace YouChewArchive
{
    public static class ItemLogic
    {
        public static List<T> GetCommentsWithPermission<T>(int itemId, int? limitCount = null, int? limitOffset = null)
        {
            string tableName = AppLogic.GetStaticField<string>(typeof(T), "TableName");
            Dictionary<string, string> databaseColumnMap = ContentLogic.GetDatabaseColumnMap<T>();
            string databasePrefix = DB.GetDatabasePrefix<T>();

            if (!databaseColumnMap.ContainsKey("Item"))
            {
                throw new Exception("DatabaseColumnMap does not have Item");
            }


            string query = $"SELECT * FROM {tableName} WHERE {databasePrefix}{databaseColumnMap["Item"]} = {itemId}";

            string permissionWhere = ContentLogic.GeneratePermissionWhere<T>();

            if (permissionWhere.Length > 0)
            {
                query += " AND " + permissionWhere;
            }

            if(!AppLogic.CanViewHidden())
            {
                if(typeof(T) == typeof(Post))
                {
                    Topic topic = DB.Instance.GetRecordById<Topic>(itemId);

                    List<int> excludeMembers = MemberLogic.GetExcludedMembers<Forum>(topic.Container);

                    if(excludeMembers.Any())
                    {
                        query += $" AND {databasePrefix}{databaseColumnMap["Author"]} NOT IN ({String.Join(",", excludeMembers)})";
                    }
                }
            }

            if (databaseColumnMap.ContainsKey("Date"))
            {
                query += $" ORDER BY {databasePrefix}{databaseColumnMap["Date"]}";
            }

            if (limitCount.HasValue && limitOffset.HasValue)
            {
                query += $" LIMIT {limitOffset.Value}, {limitCount.Value}";
            }
            else if (limitCount.HasValue)
            {
                query += $" LIMIT {limitCount.Value}";
            }

            return DB.Instance.GetData<T>(query);
        }
    }
}
