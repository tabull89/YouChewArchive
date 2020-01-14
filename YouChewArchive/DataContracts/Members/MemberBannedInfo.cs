using System;
using System.Collections.Generic;

namespace YouChewArchive.DataContracts
{
	public class MemberBannedInfo
	{
		public static string DatabaseColumnId = "member_id";
		public static string TableName = "members_banned_info";
		public int member_id { get; set; }
		public int? member_banned_date { get; set; }
		public string member_banned_notes { get; set; }
		public int? last_moderator { get; set; }
		public int? last_moderator_date { get; set; }
		
		[Ignore]
		public int Id
		{
			get
			{
				return member_id;
			}
		}
	}
}
