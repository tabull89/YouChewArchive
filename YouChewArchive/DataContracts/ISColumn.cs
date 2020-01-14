using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YouChewArchive.DataContracts
{
    public class ISColumn
    {
        public string COLUMN_NAME { get; set; }
        public string COLUMN_TYPE { get; set; }
        public string DATA_TYPE { get; set; }
        public string IS_NULLABLE { get; set; }
        public int? NUMERIC_PRECISION { get; set; }
    }
}
