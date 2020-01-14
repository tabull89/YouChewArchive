using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml;
using YouChewArchive.DataContracts;

namespace YouChewArchive.Data
{
    public static class ClassGenerator
    {
        private static int currentIndent = 0;
        private static void ResetIndent()
        {
            currentIndent = 0;
        }

        private static void IncreaseIndent()
        {
            currentIndent++;
        }

        private static void DecreaseIndent()
        {
            currentIndent--;
        }

        private static StringBuilder AppendWithIndent(StringBuilder sb, string str)
        {
            if (currentIndent > 0)
            {
                sb.Append(new string('\t', currentIndent));
            }

            return sb.Append(str);
        }

        private static StringBuilder AppendLineWithIndent(StringBuilder sb, string str)
        {
            if (currentIndent > 0)
            {
                sb.Append(new string('\t', currentIndent));
            }

            return sb.AppendLine(str);
        }

        public static void GenerateDataContract(Table table)
        {
            ResetIndent();

            StringBuilder sb = new StringBuilder();

            AppendLineWithIndent(sb, "using System;");
            AppendLineWithIndent(sb, "using System.Collections.Generic;");
            AppendLineWithIndent(sb, "");
            AppendLineWithIndent(sb, "namespace YouChewArchive.DataContracts");
            OpenCurlyBrace(sb);


            AppendLineWithIndent(sb, $"public class {table.DataContractName}");

            OpenCurlyBrace(sb);

            if (!String.IsNullOrEmpty(table.DataBaseColumnId))
            {
                AddStaticProperty(sb, "string", "DatabaseColumnId", $"\"{table.DataBaseColumnId}\"");
            }

            if(!String.IsNullOrEmpty(table.DatabasePrefix))
            {
                AddStaticProperty(sb, "string", "DatabasePrefix", $"\"{table.DatabasePrefix}\"");
            }

            if(!String.IsNullOrEmpty(table.DatabaseColumnParent))
            {
                AddStaticProperty(sb, "string", "DatabaseColumnParent", $"\"{table.DatabaseColumnParent}\"");
            }

            AddStaticProperty(sb, "string", "TableName", $"\"{table.Name}\"");

            if (!String.IsNullOrEmpty(table.Application))
            {
                AddStaticProperty(sb, "string", "Application", $"\"{table.Application}\"");
            }

            if(!String.IsNullOrEmpty(table.PermissionApp))
            {
                AddStaticProperty(sb, "string", "PermissionApp", $"\"{table.PermissionApp}\"");
            }

            if(!String.IsNullOrEmpty(table.PermissionType))
            {
                AddStaticProperty(sb, "string", "PermissionType", $"\"{table.PermissionType}\"");
            }

            if(!String.IsNullOrEmpty(table.RepuationTypeId))
            {
                AddStaticProperty(sb, "string", "RepuationTypeId", $"\"{table.RepuationTypeId}\"");
            }

            List<ISColumn> columns = GetColumns(table);

            foreach (ISColumn column in columns)
            {
                AppendLineWithIndent(sb, ConvertToProperty(column));
            }

            if (!String.IsNullOrEmpty(table.Description))
            {
                AddGetter(sb, "string", "Description", $"return LangLogic.GetValue($\"{table.Description}\");");
            }

            if(!String.IsNullOrEmpty(table.DataBaseColumnId))
            {
                ISColumn idCol = columns.First(c => c.COLUMN_NAME == table.DataBaseColumnId);

                AddGetter(sb, GetType(idCol), "Id", $"return {idCol.COLUMN_NAME};");
            }

            if(!String.IsNullOrEmpty(table.Url))
            {
                AddGetter(sb, "string", "Url", $"return $\"{table.Url}\";");
            }

            AddDatabaseColumnMap(sb, table, columns);

            CloseCurlyBrace(sb);

            CloseCurlyBrace(sb);

            string @class = sb.ToString();

            WriteToFile(table.DataContractName, @class);
        }

        private static void AddDatabaseColumnMap(StringBuilder sb, Table table, List<ISColumn> columns)
        {
            Dictionary<string, string> map = new Dictionary<string, string>();

            foreach (var column in table.DatabaseColumnMap)
            {
                ISColumn iscolumn = columns.FirstOrDefault(c => c.COLUMN_NAME == column.Value);

                if(iscolumn == null)
                {
                    continue;
                }

                char[] nameArr = column.Key.ToCharArray();

                nameArr[0] = nameArr[0].ToString().ToUpper().First();

                for(int i = 1; i < nameArr.Length; i++)
                {
                    if (nameArr[i] == '_' && (i + 1) < nameArr.Length)
                    {
                        char nextLetter = nameArr[i + 1];
                        if('a' <= nextLetter && nextLetter <= 'z')
                        {
                            nameArr[i + 1] = nextLetter.ToString().ToUpper().First();
                        }
                    }
                }

                string name = new string(nameArr).Replace("_", "");

                AddGetter(sb, GetType(iscolumn), name, $"return {column.Value};");

                map.Add(name, column.Value);

            }

            if (map.Count > 0)
            {
                AppendLineWithIndent(sb, "");
                AppendLineWithIndent(sb, "public static Dictionary<string, string> DatabaseColumnMap = new Dictionary<string, string>()");
                OpenCurlyBrace(sb);

                foreach (var column in map)
                {
                    AppendLineWithIndent(sb, $"{{ \"{column.Key}\", \"{column.Value}\" }},");
                }

                CloseCurlyBrace(sb, ";");
            }

        }

        private static StringBuilder AddStaticProperty(StringBuilder sb, string type, string name, string defaultValue = null)
        {
            AppendWithIndent(sb, $"public static {type} {name}");

            if(defaultValue != null)
            {
                sb.Append($" = {defaultValue}");
            }

            sb.AppendLine(";");

            return sb;
        }

        private static StringBuilder AddGetter(StringBuilder sb, string type, string getterName, string getterCode, bool addIgnoreAttribute = true)
        {
            AppendLineWithIndent(sb, "");

            if (addIgnoreAttribute)
            {
                AddIgnore(sb);
            }

            AppendLineWithIndent(sb, $"public {type} {getterName}");

            OpenCurlyBrace(sb);

            AppendLineWithIndent(sb, "get");

            OpenCurlyBrace(sb);

            AppendLineWithIndent(sb, getterCode);

            CloseCurlyBrace(sb);

            CloseCurlyBrace(sb);

            return sb;
        }

        private static StringBuilder OpenCurlyBrace(StringBuilder sb)
        {
            AppendLineWithIndent(sb, "{");
            IncreaseIndent();

            return sb;
        }

        private static StringBuilder CloseCurlyBrace(StringBuilder sb, string afterCurlyBraceText = "")
        {
            DecreaseIndent();
            AppendLineWithIndent(sb, "}" + afterCurlyBraceText);

            return sb;
        }

        private static StringBuilder AddIgnore(StringBuilder sb)
        {
            return AppendLineWithIndent(sb, "[Ignore]");
        }

        private static void WriteToFile(string dcName, string @class)
        {
            string directory = @"C:\Users\tabull\source\repos\YouChewArchive\YouChewArchive\DataContracts";

            using (StreamWriter sw = new StreamWriter($"{directory}\\{dcName}.cs", false))
            {
                sw.Write(@class);
            }
        }

        public static void GenerateFromXMLFile(string file)
        {
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load(file);

            XmlNodeList tables = xmlDoc.GetElementsByTagName("table");

            foreach(XmlNode tableNode in tables)
            {
                Table table = new Table();

                table.Name = tableNode.SelectSingleNode("./name")?.InnerText;
                table.DataContractName = tableNode.SelectSingleNode("./datacontract")?.InnerText;
                table.Description= tableNode.SelectSingleNode("./description")?.InnerText;
                table.Application = tableNode.SelectSingleNode("./application")?.InnerText;
                table.DataBaseColumnId = tableNode.SelectSingleNode("./databasecolumnid")?.InnerText;
                table.PermissionApp = tableNode.SelectSingleNode("./permapp")?.InnerText;
                table.PermissionType = tableNode.SelectSingleNode("./permtype")?.InnerText;
                table.DatabaseColumnParent = tableNode.SelectSingleNode("./databasecolumnparent")?.InnerText;
                table.Url = tableNode.SelectSingleNode("./url")?.InnerText;
                table.DatabasePrefix = tableNode.SelectSingleNode("./databaseprefix")?.InnerText;
                table.RepuationTypeId = tableNode.SelectSingleNode("./reputationtypeid")?.InnerText;
                table.FieldMap = tableNode.SelectSingleNode("./fieldmap")?.InnerText;
                table.FieldMapNameFrom = tableNode.SelectSingleNode("./fieldmapnamefrom")?.InnerText;
                table.FieldMapNameTo = tableNode.SelectSingleNode("./fieldmapnameto")?.InnerText;

                XmlNode columnMapNode = tableNode.SelectSingleNode("./databasecolumnmap");

                if(columnMapNode != null)
                {
                    XmlNodeList columnNodes = columnMapNode.SelectNodes("./column");

                    foreach(XmlNode columnNode in columnNodes)
                    {
                        table.DatabaseColumnMap.Add(columnNode.Attributes["key"].Value, columnNode.Attributes["value"].Value);                        
                    }
                }

                if(!String.IsNullOrEmpty(table.FieldMap))
                {
                    Type type = Type.GetType("YouChewArchive.DataContracts." + table.FieldMap);

                    string tableName = AppLogic.GetStaticField<string>(type, "TableName");

                    var method = DB.Instance.GetType().GetMethod("GetData");
                    var generic = method.MakeGenericMethod(type);
                    object obj = generic.Invoke(DB.Instance, new object[] { $"SELECT * FROM {tableName}", null, null });

                    Regex mappingRegex = new Regex("{(.*?)}");

                    foreach(object o in (IEnumerable<object>)obj)
                    {
                        string from = ConvertFieldMapping(table.FieldMapNameFrom, type, o, mappingRegex).Replace(" ", "");
                        string to = ConvertFieldMapping(table.FieldMapNameTo, type, o, mappingRegex);

                        table.DatabaseColumnMap.Add(from, to);
                    }


                }

                GenerateDataContract(table); 
            }
        }

        private static string ConvertFieldMapping(string fieldMapping, Type type, object o, Regex mappingRegex)
        {
            return mappingRegex.Replace(fieldMapping, match =>
            {
                string fieldName = match.Groups[1].Value;

                return AppLogic.GetPropertyValue<string>(type, o, fieldName);
            });

            
        }

        private static List<ISColumn> GetColumns(Table table)
        {
            string query = "SELECT COLUMN_NAME, COLUMN_TYPE, DATA_TYPE, IS_NULLABLE, NUMERIC_PRECISION FROM information_schema.COLUMNS WHERE TABLE_SCHEMA = 'board' AND TABLE_NAME = @table ORDER BY ORDINAL_POSITION";
            List<MySqlParameter> parameters = new List<MySqlParameter>()
            {
                new MySqlParameter("@table", MySqlDbType.String) {Value = table.Name },
            };

            List<ISColumn> columns = DB.Instance.GetData<ISColumn>(query, parameters);

            if (!String.IsNullOrEmpty(table.DatabasePrefix))
            {
                foreach(ISColumn column in columns.Where(c => c.COLUMN_NAME.StartsWith(table.DatabasePrefix)))
                {
                    column.COLUMN_NAME = column.COLUMN_NAME.Substring(table.DatabasePrefix.Length);
                }
            }

            return columns;
        }

        private static string GetType(ISColumn column)
        {
            string type = "";

            switch (column.DATA_TYPE.ToLower())
            {
                case "bigint":
                case "int":
                case "smallint":
                case "mediumint":
                case "tinyint":
                    type = ConvertInt(column);
                    break;

                case "bit":
                    type = IsNullable(column) ? "bool?" : "bool";
                    break;

                case "blob":
                case "binary":
                case "longblob":
                case "mediumblob":
                case "varbinary":
                    type = "byte[]";
                    break;

                case "datetime":
                    type = IsNullable(column) ? "DateTime?" : "DateTime";
                    break;

                case "char":
                case "longtext":
                case "mediumtext":
                case "text":
                case "tinytext":
                case "varchar":
                case "set":
                case "enum":
                    type = "string";
                    break;

                case "decimal":
                    type = IsNullable(column) ? "decimal?" : "decimal";
                    break;

                case "double":
                case "float":
                    type = IsNullable(column) ? "double?" : "double";
                    break;

                default:
                    throw new Exception($"Unknown Type: {column.DATA_TYPE}");

            }

            return type;
        }

        private static string ConvertToProperty(ISColumn column)
        {
            string type = GetType(column);

            return $"public {type} {column.COLUMN_NAME} {{ get; set; }}";
        }

        private static string ConvertInt(ISColumn column)
        {
            string type = "int";

            if(column.COLUMN_TYPE.Contains("(1)"))
            {
                type = "bool";
            }
            else if(column.DATA_TYPE.ToLower() == "bigint")
            {
                type = "long";
            }

            if(IsNullable(column))
            {
                type += "?";
            }

            return type;
        }

        private static bool IsNullable(ISColumn column)
        {
            return column.IS_NULLABLE.ToLower() == "yes";
        }
    }
}
