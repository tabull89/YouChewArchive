using System;
using System.Collections.Generic;

namespace YouChewArchive.DataContracts
{
	public class VideoComment
	{
		public static string DatabaseColumnId = "id";
		public static string DatabasePrefix = "comment_";
		public static string TableName = "videos_comments";
		public static string Application = "videos";
		public int id { get; set; }
		public int? member_id { get; set; }
		public string member_name { get; set; }
		public int video_id { get; set; }
		public int? date_added { get; set; }
		public string text { get; set; }
		public string ip_address { get; set; }
		public int edit_time { get; set; }
		public string edit_name { get; set; }
		public bool append_edit { get; set; }
		public bool approved { get; set; }
		
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
				return video_id;
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
				return date_added;
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
		public int EditTime
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
				return edit_name;
			}
		}
		
		[Ignore]
		public bool EditShow
		{
			get
			{
				return append_edit;
			}
		}
		
		[Ignore]
		public bool Approved
		{
			get
			{
				return approved;
			}
		}
		
		public static Dictionary<string, string> DatabaseColumnMap = new Dictionary<string, string>()
		{
			{ "Item", "video_id" },
			{ "Author", "member_id" },
			{ "AuthorName", "member_name" },
			{ "Content", "text" },
			{ "Date", "date_added" },
			{ "IpAddress", "ip_address" },
			{ "EditTime", "edit_time" },
			{ "EditMemberName", "edit_name" },
			{ "EditShow", "append_edit" },
			{ "Approved", "approved" },
		};
	}
}
