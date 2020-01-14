using System;
using System.Collections.Generic;

namespace YouChewArchive.DataContracts
{
	public class ProfileFieldData
	{
		public static string DatabaseColumnId = "id";
		public static string DatabasePrefix = "pf_";
		public static string TableName = "core_pfields_data";
		public int id { get; set; }
		public string content { get; set; }
		public string type { get; set; }
		public bool not_null { get; set; }
		public bool member_hide { get; set; }
		public int max_input { get; set; }
		public bool member_edit { get; set; }
		public int position { get; set; }
		public bool show_on_reg { get; set; }
		public string input_format { get; set; }
		public bool admin_only { get; set; }
		public string format { get; set; }
		public int group_id { get; set; }
		public string search_type { get; set; }
		public bool filtering { get; set; }
		public bool multiple { get; set; }
		public bool allow_attachments { get; set; }
		
		[Ignore]
		public string Description
		{
			get
			{
				return LangLogic.GetValue($"core_pfield_{Id}");
			}
		}
		
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
