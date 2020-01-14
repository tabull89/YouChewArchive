using System;
using System.Collections.Generic;

namespace YouChewArchive.DataContracts
{
	public class PermissionIndex
	{
		public static string DatabaseColumnId = "perm_id";
		public static string TableName = "core_permission_index";
		public int perm_id { get; set; }
		public string app { get; set; }
		public string perm_type { get; set; }
		public int perm_type_id { get; set; }
		public string perm_view { get; set; }
		public string perm_2 { get; set; }
		public string perm_3 { get; set; }
		public string perm_4 { get; set; }
		public string perm_5 { get; set; }
		public string perm_6 { get; set; }
		public string perm_7 { get; set; }
		public bool owner_only { get; set; }
		public bool friend_only { get; set; }
		public string authorized_users { get; set; }
		
		[Ignore]
		public int Id
		{
			get
			{
				return perm_id;
			}
		}
	}
}
