using System;
using System.Collections.Generic;

namespace YouChewArchive.DataContracts
{
	public class WarnLog
	{
		public static string DatabaseColumnId = "id";
		public static string DatabasePrefix = "wl_";
		public static string TableName = "core_members_warn_logs";
		public int id { get; set; }
		public int? member { get; set; }
		public int? moderator { get; set; }
		public int? date { get; set; }
		public int? reason { get; set; }
		public int? points { get; set; }
		public string note_member { get; set; }
		public string note_mods { get; set; }
		public string mq { get; set; }
		public string rpa { get; set; }
		public string suspend { get; set; }
		public bool? acknowledged { get; set; }
		public string content_app { get; set; }
		public string content_id1 { get; set; }
		public string content_id2 { get; set; }
		public int? expire_date { get; set; }
		public string content_module { get; set; }
		
		[Ignore]
		public int Id
		{
			get
			{
				return id;
			}
		}
	}
}
