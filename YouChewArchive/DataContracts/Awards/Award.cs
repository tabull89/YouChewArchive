using System;
using System.Collections.Generic;

namespace YouChewArchive.DataContracts
{
	public class Award
	{
		public static string DatabaseColumnId = "id";
		public static string TableName = "awards_awards";
		public int id { get; set; }
		public string name { get; set; }
		public string desc { get; set; }
		public string icon { get; set; }
		public int placement { get; set; }
		public int parent { get; set; }
		public bool visible { get; set; }
		public string public_perms { get; set; }
		public string icon_thumb { get; set; }
		
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
