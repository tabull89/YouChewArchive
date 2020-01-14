using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YouChewArchive.Logic;

namespace YouChewArchive.CSV
{
    public class BannedMemberWarnLogCSV
    {
        public int Id { get; set; }
        public int? MemberId { get; set; }
        public string Member
        {
            get
            {
                return MemberLogic.GetMember(MemberId.GetValueOrDefault(0)).name;
            }
        }
        public int? ModeratorId { get; set; }
        public string Moderator
        {
            get
            {
                return MemberLogic.GetMember(ModeratorId.GetValueOrDefault(0)).name;
            }
        }
        public int? DateUnix { get; set; }
        public DateTime? Date
        {
            get
            {
                if(DateUnix.HasValue)
                {
                    return LangLogic.ConvertFromUnixtime(DateUnix.Value);
                }

                return null;
            }
        }
        public int? ReasonId { get; set; }
        public string Reason
        {
            get
            {
                return ReasonId.HasValue ? MemberLogic.GetWarnReason(ReasonId.Value) : "Other";
            }
        }
        public int? Points { get; set; }
        public string MemberNote { get; set; }
        public string ModeratorNote { get; set; }
        public bool? Acknowledged { get; set; }
        public int? ExpireDateUnix { get; set; }
        public DateTime? ExpireDate
        {
            get
            {
                return ExpireDateUnix.HasValue ? (DateTime?)LangLogic.ConvertFromUnixtime(ExpireDateUnix.Value) : null;
            }
        }
    }
}
