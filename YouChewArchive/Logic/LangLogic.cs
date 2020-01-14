using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YouChewArchive.Data;

namespace YouChewArchive
{
    public static class LangLogic
    {
        private static Dictionary<string, string> langMap = new Dictionary<string, string>();

        public static string LongDateFormat = "LLL";
        public static string ShortDateFormat = "l LT";
        public static string ShortDateNoTime = "l";

        public static string GetValue(string key)
        {
            string value = null;

            if(!langMap.TryGetValue(key, out value))
            {
                string query = "SELECT IFNULL(word_custom, word_default) FROM core_sys_lang_words WHERE lang_id = @langId AND word_key = @key";
                List<MySqlParameter> parameters = new List<MySqlParameter>()
                {
                    new MySqlParameter("@langId", MySqlDbType.UInt32) { Value = Settings.Language },
                    new MySqlParameter("@key", MySqlDbType.String) {Value = key},
                };

                value = DB.Instance.ExecuteScalar<string>(query, parameters);

                langMap.Add(key, value);
            }

            return value;
             
        }

        public static string FormatNumber(int number, string singular = null, string plural = null)
        {
            string ret = number.ToString("N0");

            if(singular != null && plural != null)
            {
                if(number == 1)
                {
                    ret += $" {singular}";
                }
                else
                {
                    ret += $" {plural}";
                }
            }

            return ret;
        }

        public static string FormatDate(int? date, string momentFormat)
        {
            if (!date.HasValue)
            {
                return "";
            }

            string toStringFormat = "";

            if (momentFormat == LongDateFormat)
            {
                toStringFormat = "MMMM dd, yyyy h:mm tt";
            }
            else if(momentFormat == ShortDateNoTime)
            {
                toStringFormat = "MM/dd/yyyy";
            }
            else if(momentFormat == ShortDateFormat)
            {
                toStringFormat = "g";
            }

            DateTime datetime = ConvertFromUnixtime(date.Value);

            return $"<time datetime='{datetime.ToString("s")}' data-format='{momentFormat}' data-unixtime={date}>{datetime.ToString(toStringFormat)}</time>";
        }

        public static DateTime ConvertFromUnixtime(int date)
        {
            return new DateTime(1970, 1, 1).AddSeconds(date);
        }
    }
}
