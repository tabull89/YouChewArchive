using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YouChewArchive.CSV
{
    public class BannedMemberCSV
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public int Warnings { get; set; }
        public int? BanDateUnix { get; set; }
        public DateTime? BanDate { get; set; }
        public bool TempBan { get; set; }
        public bool PermanentBan { get; set; }
        public string BannedNotes { get; set; }
    }
}
