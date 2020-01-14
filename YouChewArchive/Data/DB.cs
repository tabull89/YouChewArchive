using MySql.Data;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace YouChewArchive.Data
{
    public class DB : IDisposable
    {
        private MySqlConnection connection;

        private DB()
        {
            stopwatch.Start();
        }

        private static DB _db = null;
        public static DB Instance
        {
            get
            {
                if(_db == null)
                {
                    _db = new DB();
                    _db.Connect();
                }

                return _db;
            }
        }

        private void Connect()
        {
            MySqlConnectionStringBuilder builder = new MySqlConnectionStringBuilder()
            {
                Server = "localhost",
                Database = "The Real Database name",
                Port = 3306,
                UserID = "The Real User Id",
                Password = "The Real Password"
            };

            connection = new MySqlConnection(builder.ToString());

            connection.Open();

        }
        
        private MySqlCommand CreateCommand()
        {
            MySqlCommand command = new MySqlCommand();
            command.Connection = connection;

            return command;
        }

        private MySqlCommand CreateCommand(string cmdTxt)
        {
            MySqlCommand command = CreateCommand();
            command.CommandType = CommandType.Text;
            command.CommandText = cmdTxt;

            return command;
        }

        //Also includes string and nullable
        private bool IsPrimitive(Type type)
        {
            if(type.IsPrimitive || Nullable.GetUnderlyingType(type) != null)
            {
                return true;
            }

            if(type == typeof(string) || type == typeof(DateTime))
            {
                return true;
            }

            return false;
        }

        public List<T> GetData<T>(string query, List<MySqlParameter> parameters = null, int? timeout = null)
        {
            if(parameters == null)
            {
                parameters = new List<MySqlParameter>();
            }

            using (MySqlCommand command = CreateCommand(query))
            {
                if(timeout.HasValue)
                {
                    command.CommandTimeout = timeout.Value;
                }
                else
                {
                    command.CommandTimeout = 300;
                }

                command.Parameters.AddRange(parameters.ToArray());

                
                MySqlDataReader reader = command.ExecuteReader();

                //Type type = typeof(T);

                //if (IsPrimitive(type))
                //{
                //    List<T> ret = new List<T>();

                //    while(reader.Read())
                //    {
                //        ret.Add((T)AppLogic.ChangeType(reader[0], type));
                //    }

                //    return ret;
                //}
                //else
                //{
                    return MapReaderToList<T>(reader);
                //}

            }
        }

        public int ExecuteNonQuery(string query, List<MySqlParameter> parameters = null)
        {
            if (parameters == null)
            {
                parameters = new List<MySqlParameter>();
            }

            using (MySqlCommand command = CreateCommand(query))
            {
                command.Parameters.AddRange(parameters.ToArray());

                return command.ExecuteNonQuery();
            }
        }

        private Dictionary<Tuple<Type, int>, object> RecordMap = new Dictionary<Tuple<Type, int>, object>();

        private Stopwatch stopwatch = new Stopwatch();

        public T GetRecordById<T>(int id)
        {
            Type type = typeof(T);

            object record;

            if(RecordMap.TryGetValue(Tuple.Create(type, id), out record))
            {
                return (T)record;
            }

            if(stopwatch.Elapsed.TotalMinutes > 5)
            {
                RecordMap.Clear();
                stopwatch.Restart();
            }

            string databaseColumnId = type.GetField("DatabaseColumnId")?.GetValue(null).ToString();
            string databaseTable = type.GetField("TableName")?.GetValue(null).ToString();

            if(databaseColumnId == null || databaseTable == null)
            {
                throw new Exception($"{type.Name} does not have a DatabaseColumnId or TableName public static member");
            }

            string query = $"SELECT * FROM {databaseTable} WHERE {GetDatabasePrefix<T>()}{databaseColumnId}={id}";

            record = GetData<T>(query).FirstOrDefault();

            RecordMap.Add(Tuple.Create(type, id), record);

            return (T)record;
        }

        public List<T> GetRecordsByIds<T>(IEnumerable<int> ids)
        {
            if(!ids.Any())
            {
                throw new Exception("ids must have at least one value");
            }

            Type type = typeof(T);

            string databaseColumnId = type.GetField("DatabaseColumnId")?.GetValue(null).ToString();
            string databaseTable = type.GetField("TableName")?.GetValue(null).ToString();

            if (databaseColumnId == null || databaseTable == null)
            {
                throw new Exception($"{type.Name} does not have a DatabaseColumnId or TableName public static member");
            }

            string idStr = String.Join(",", ids);

            string query = $"SELECT * FROM {databaseTable} WHERE {GetDatabasePrefix<T>()}{databaseColumnId} IN ({idStr})";

            return GetData<T>(query);
        }

        public T ExecuteScalar<T>(string query, List<MySqlParameter> parameters = null, int? timeout = null)
        {
            if(parameters == null)
            {
                parameters = new List<MySqlParameter>();
            }

            using (MySqlCommand command = CreateCommand(query))
            {
                command.Parameters.AddRange(parameters.ToArray());

                if(timeout.HasValue)
                {
                    command.CommandTimeout = timeout.Value;
                }

                object obj = command.ExecuteScalar();

                return (T)AppLogic.ChangeType(obj, typeof(T));
            }
        }

        private static List<T> MapReaderToList<T>(MySqlDataReader reader)
        {
            List<T> list = new List<T>();

            T obj = default(T);

            PropertyInfo[] properties = GetProperties(typeof(T));
            string databasePrefix = GetDatabasePrefix<T>();

            while (reader.Read())
            {
                obj = Activator.CreateInstance<T>();

                HashSet<string> columns = new HashSet<string>();

                foreach (DataRow row in reader.GetSchemaTable().Rows)
                {
                    columns.Add(row["ColumnName"].ToString());
                }

                foreach (PropertyInfo property in properties)
                {
                    string columnName = databasePrefix + property.Name;

                    if (!columns.Contains(columnName))
                    {
                        property.SetValue(obj, null);
                    }
                    else if (!Object.Equals(reader[columnName], DBNull.Value))
                    {
                        object value = AppLogic.ChangeType(reader[columnName], property.PropertyType);

                        property.SetValue(obj, value, null);
                    }
                }

                list.Add(obj);
            }

            return list;

        }

        private static Dictionary<Type, string> DatabasePrefixMap = new Dictionary<Type, string>();

        public static string GetDatabasePrefix(Type type)
        {
            string databasePrefix;

            if (!DatabasePrefixMap.TryGetValue(type, out databasePrefix))
            {
                databasePrefix = "";

                if (AppLogic.HasStaticField(type, "DatabasePrefix"))
                {
                    databasePrefix = AppLogic.GetStaticField<string>(type, "DatabasePrefix");
                }
            }

            return databasePrefix;
        }

        public static string GetDatabasePrefix<T>()
        {
            return GetDatabasePrefix(typeof(T));
            
        }

        private static Dictionary<Type, PropertyInfo[]> propertyCache = new Dictionary<Type, PropertyInfo[]>();
        private static PropertyInfo[] GetProperties(Type type)
        {
            PropertyInfo[] info;

            if (propertyCache.TryGetValue(type, out info))
            {
                return info;
            }

            info = type.GetProperties(BindingFlags.Public | BindingFlags.Instance)
                       .Where(p => p.GetCustomAttribute<IgnoreAttribute>() == null)
                       .ToArray();


            propertyCache[type] = info;

            return info;


        }

        

        public void Close()
        {
            connection.Close();
        }

        public void Dispose()
        {
            connection?.Close();
            connection?.Dispose();
            connection = null;
        }
    }
}
