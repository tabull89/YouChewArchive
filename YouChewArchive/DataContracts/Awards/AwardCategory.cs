using System;
using System.Collections.Generic;

namespace YouChewArchive.DataContracts
{
	public class AwardCategory
	{
		public static string DatabaseColumnId = "cat_id";
		public static string TableName = "awards_categories";
		public int cat_id { get; set; }
		public string title { get; set; }
		public int placement { get; set; }
		public bool visible { get; set; }
		
		[Ignore]
		public int Id
		{
			get
			{
				return cat_id;
			}
		}
	}
}
