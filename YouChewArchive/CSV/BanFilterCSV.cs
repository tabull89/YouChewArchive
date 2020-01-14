using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YouChewArchive.CSV
{
    public class BanFilterCSV
    {
        public int ban_id { get; set; }
        public string ban_type { get; set; }
        public string ban_content { get; set; }
        public int ban_date { get; set; }
        [Ignore]
        public DateTime? BanDate
        {
            get
            {
                return LangLogic.ConvertFromUnixtime(ban_date);
            }
        }
        public string ban_reason { get; set; }
    }
}
