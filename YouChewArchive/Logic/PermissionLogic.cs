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
    public static class PermissionLogic
    {
        private static Dictionary<Type, HashSet<int>> permissionMap = new Dictionary<Type, HashSet<int>>();

        public static HashSet<int> GetPermissions<T>()
        {
            Type type = typeof(T);

            HashSet<int> validIds = new HashSet<int>();

            if(!permissionMap.TryGetValue(type, out validIds))
            {
                validIds = new HashSet<int>();

                string permApp = AppLogic.GetStaticField<T, string>("PermissionApp");
                string permType = AppLogic.GetStaticField<T, string>("PermissionType");

                StringBuilder query = new StringBuilder();

                query.Append("SELECT * FROM ")
                     .Append(PermissionIndex.TableName)
                     .Append($" WHERE (app = @permApp AND perm_type = @permType) AND (perm_view = '*' OR ")
                     .Append(String.Join(" OR ", Settings.Groups.Select(g => $"FIND_IN_SET('{g}', perm_view)")))
                     .Append(")");

                List<MySqlParameter> parameters = new List<MySqlParameter>()
                {
                    new MySqlParameter("@permApp", MySqlDbType.String) {Value = permApp},
                    new MySqlParameter("@permType", MySqlDbType.String) { Value = permType },
                };

                List<PermissionIndex> pis = DB.Instance.GetData<PermissionIndex>(query.ToString(), parameters);

                foreach (PermissionIndex pi in pis)
                {
                    validIds.Add(pi.perm_type_id);
                }

                permissionMap.Add(type, validIds);
            }

            return validIds;
        }
    }
}
