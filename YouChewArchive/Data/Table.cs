using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YouChewArchive.Data
{
    public class Table
    {
        public string Name { get; set; }
        public string DataContractName { get; set; }
        public string Description { get; set; }
        public Dictionary<string, string> DatabaseColumnMap { get; set; } = new Dictionary<string, string>();
        public string Application { get; set; }
        public string DataBaseColumnId { get; set; }
        public string PermissionApp { get; set; }
        public string PermissionType { get; set; }
        public string DatabaseColumnParent { get; set; }
        public string Url { get; set; }
        public string DatabasePrefix { get; set; }
        public string RepuationTypeId { get; set; }
        public string FieldMap { get; set; }
        public string FieldMapNameFrom { get; set; }
        public string FieldMapNameTo { get; set; }
    }
}
