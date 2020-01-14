using System;
using System.Collections.Generic;

namespace YouChewArchive.DataContracts
{
	public class Post
	{
		public static string DatabaseColumnId = "pid";
		public static string TableName = "forums_posts";
		public static string Application = "forums";
		public int pid { get; set; }
		public bool? append_edit { get; set; }
		public int? edit_time { get; set; }
		public int author_id { get; set; }
		public string author_name { get; set; }
		public string ip_address { get; set; }
		public int? post_date { get; set; }
		public int? icon_id { get; set; }
		public string post { get; set; }
		public bool queued { get; set; }
		public int topic_id { get; set; }
		public string post_title { get; set; }
		public bool? new_topic { get; set; }
		public string edit_name { get; set; }
		public string post_key { get; set; }
		public bool post_htmlstate { get; set; }
		public string post_edit_reason { get; set; }
		public int post_bwoptions { get; set; }
		public int pdelete_time { get; set; }
		public int? post_field_int { get; set; }
		public string post_field_t1 { get; set; }
		public string post_field_t2 { get; set; }
		public int? page_number { get; set; }
		
		[Ignore]
		public int Id
		{
			get
			{
				return pid;
			}
		}
		
		[Ignore]
		public int Item
		{
			get
			{
				return topic_id;
			}
		}
		
		[Ignore]
		public int Author
		{
			get
			{
				return author_id;
			}
		}
		
		[Ignore]
		public string AuthorName
		{
			get
			{
				return author_name;
			}
		}
		
		[Ignore]
		public string Content
		{
			get
			{
				return post;
			}
		}
		
		[Ignore]
		public int? Date
		{
			get
			{
				return post_date;
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
		public bool? EditShow
		{
			get
			{
				return append_edit;
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
		public string EditReason
		{
			get
			{
				return post_edit_reason;
			}
		}
		
		[Ignore]
		public bool Hidden
		{
			get
			{
				return queued;
			}
		}
		
		[Ignore]
		public bool? First
		{
			get
			{
				return new_topic;
			}
		}
		
		public static Dictionary<string, string> DatabaseColumnMap = new Dictionary<string, string>()
		{
			{ "Item", "topic_id" },
			{ "Author", "author_id" },
			{ "AuthorName", "author_name" },
			{ "Content", "post" },
			{ "Date", "post_date" },
			{ "IpAddress", "ip_address" },
			{ "EditTime", "edit_time" },
			{ "EditShow", "append_edit" },
			{ "EditMemberName", "edit_name" },
			{ "EditReason", "post_edit_reason" },
			{ "Hidden", "queued" },
			{ "First", "new_topic" },
		};
	}
}
