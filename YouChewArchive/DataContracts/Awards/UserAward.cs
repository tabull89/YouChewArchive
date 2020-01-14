using System;
using System.Collections.Generic;

namespace YouChewArchive.DataContracts
{
	public class UserAward
	{
		public static string DatabaseColumnId = "row_id";
		public static string TableName = "awards_awarded";
		public int row_id { get; set; }
		public int award_id { get; set; }
		public int user_id { get; set; }
		public bool is_active { get; set; }
		public string notes { get; set; }
		public int date { get; set; }
		public int awarded_by { get; set; }
		public bool approved { get; set; }
		
		[Ignore]
		public int Id
		{
			get
			{
				return row_id;
			}
		}
	}
}
