using System;
using System.Collections.Generic;

namespace YouChewArchive.DataContracts
{
	public class MemberHistory
	{
		public static string DatabaseColumnId = "id";
		public static string DatabasePrefix = "log_";
		public static string TableName = "core_member_history";
		public long id { get; set; }
		public string app { get; set; }
		public long member { get; set; }
		public long? by { get; set; }
		public string type { get; set; }
		public string data { get; set; }
		public decimal date { get; set; }
		public string ip_address { get; set; }
		
		[Ignore]
		public long Id
		{
			get
			{
				return id;
			}
		}
	}
}
