using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YouChewArchive.DataContracts
{
    public class Image
    {
        public int id { get; set; }
        public int group_id { get; set; }
        public string image { get; set; }
        public int? field_1 { get; set; }
        public string field_2 { get; set; }
        public bool? field_3 { get; set; }
        public bool? field_4 { get; set; }
        public string field_5 { get; set; }
        public string thumbnail { get; set; }

    }
}
