using System;
using System.Collections.Generic;

namespace YouChewArchive.DataContracts
{
	public class WarnReason
	{
		public static string DatabaseColumnId = "id";
		public static string DatabasePrefix = "wr_";
		public static string TableName = "core_members_warn_reasons";
		public int id { get; set; }
		public double? points { get; set; }
		public bool? points_override { get; set; }
		public int? remove { get; set; }
		public string remove_unit { get; set; }
		public bool? remove_override { get; set; }
		public int? order { get; set; }
		public string name { get; set; }
		public string notes { get; set; }
		
		[Ignore]
		public string Description
		{
			get
			{
				return LangLogic.GetValue($"core_warn_reason_{Id}");
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
