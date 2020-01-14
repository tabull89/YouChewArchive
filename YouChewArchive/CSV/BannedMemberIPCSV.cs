using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YouChewArchive.Logic;

namespace YouChewArchive.CSV
{
    public class BannedMemberIPCSV
    {
        public int MemberId { get; set; }

        [Ignore]
        public string Name
        {
            get
            {
                return MemberLogic.GetMember(MemberId).name;
            }
        }
        public string IpAddress { get; set; }
        public int Count { get; set; }
    }
}
