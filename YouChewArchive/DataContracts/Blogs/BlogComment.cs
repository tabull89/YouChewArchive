using System;
using System.Collections.Generic;

namespace YouChewArchive.DataContracts
{
	public class BlogComment
	{
		public static string DatabaseColumnId = "id";
		public static string DatabasePrefix = "comment_";
		public static string TableName = "blog_comments";
		public static string Application = "blog";
		public int id { get; set; }
		public int entry_id { get; set; }
		public int? member_id { get; set; }
		public string member_name { get; set; }
		public string ip_address { get; set; }
		public int? date { get; set; }
		public int? edit_time { get; set; }
		public string text { get; set; }
		public bool? approved { get; set; }
		public string edit_member_name { get; set; }
		public bool edit_show { get; set; }
		
		[Ignore]
		public int Id
		{
			get
			{
				return id;
			}
		}
		
		[Ignore]
		public int Item
		{
			get
			{
				return entry_id;
			}
		}
		
		[Ignore]
		public int? Author
		{
			get
			{
				return member_id;
			}
		}
		
		[Ignore]
		public string AuthorName
		{
			get
			{
				return member_name;
			}
		}
		
		[Ignore]
		public string Content
		{
			get
			{
				return text;
			}
		}
		
		[Ignore]
		public int? Date
		{
			get
			{
				return date;
			}
		}
		
		[Ignore]
		public string IpAddress
		{
			get
			{
				return ip_address;
			}
		}
		
		[Ignore]
		public int? EditTime
		{
			get
			{
				return edit_time;
			}
		}
		
		[Ignore]
		public string EditMemberName
		{
			get
			{
				return edit_member_name;
			}
		}
		
		[Ignore]
		public bool EditShow
		{
			get
			{
				return edit_show;
			}
		}
		
		[Ignore]
		public bool? Approved
		{
			get
			{
				return approved;
			}
		}
		
		public static Dictionary<string, string> DatabaseColumnMap = new Dictionary<string, string>()
		{
			{ "Item", "entry_id" },
			{ "Author", "member_id" },
			{ "AuthorName", "member_name" },
			{ "Content", "text" },
			{ "Date", "date" },
			{ "IpAddress", "ip_address" },
			{ "EditTime", "edit_time" },
			{ "EditMemberName", "edit_member_name" },
			{ "EditShow", "edit_show" },
			{ "Approved", "approved" },
		};
	}
}
