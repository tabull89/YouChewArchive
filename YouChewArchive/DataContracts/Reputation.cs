using System;
using System.Collections.Generic;

namespace YouChewArchive.DataContracts
{
	public class Reputation
	{
		public static string DatabaseColumnId = "id";
		public static string TableName = "core_reputation_index";
		public long id { get; set; }
		public int member_id { get; set; }
		public string app { get; set; }
		public string type { get; set; }
		public int type_id { get; set; }
		public int rep_date { get; set; }
		public bool rep_rating { get; set; }
		public int member_received { get; set; }
		public string rep_class { get; set; }
		public int item_id { get; set; }
		public string class_type_id_hash { get; set; }
		public long reaction { get; set; }
		
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
