using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YouChewArchive.Data;

namespace YouChewArchive
{
    public static class ContainerLogic
    {
        public static List<T> GetItemsWithPermission<T>(int containerId, int? limitCount = null, int? limitOffset = null)
        {
            string tableName = AppLogic.GetStaticField<string>(typeof(T), "TableName");
            Dictionary<string, string> databaseColumnMap = ContentLogic.GetDatabaseColumnMap<T>();
            string databasePrefix = DB.GetDatabasePrefix<T>();


            if (!databaseColumnMap.ContainsKey("Container"))
            {
                throw new Exception("DatabaseColumnMap does not have Container");
            }


            string query = $"SELECT * FROM {tableName} WHERE {databasePrefix}{databaseColumnMap["Container"]} = {containerId}";

            string permissionWhere = ContentLogic.GeneratePermissionWhere<T>();

            if (permissionWhere.Length > 0)
            {
                query += " AND " + permissionWhere;
            }

  
            string orderby = "";
            if (databaseColumnMap.ContainsKey("Pinned"))
            {
                orderby += $"{databasePrefix}{databaseColumnMap["Pinned"]} DESC";
            }

            if (databaseColumnMap.ContainsKey("LastComment"))
            {
                if (orderby.Length > 0)
                {
                    orderby += ", ";
                }

                orderby += $"{databasePrefix}{databaseColumnMap["LastComment"]} DESC";
            }

            if (orderby.Length > 0)
            {
                query += $" ORDER BY {orderby}";
            }

            if(limitCount.HasValue && limitOffset.HasValue)
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
