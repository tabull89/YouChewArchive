using System;
using System.Collections.Generic;

namespace YouChewArchive.DataContracts
{
	public class Message
	{
		public static string DatabaseColumnId = "id";
		public static string DatabasePrefix = "msg_";
		public static string TableName = "core_message_posts";
		public static string Application = "core";
		public int id { get; set; }
		public int topic_id { get; set; }
		public int? date { get; set; }
		public string post { get; set; }
		public string post_key { get; set; }
		public int author_id { get; set; }
		public string ip_address { get; set; }
		public bool is_first_post { get; set; }
		
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
				return topic_id;
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
		public string Content
		{
			get
			{
				return post;
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
		public string IpAddress
		{
			get
			{
				return ip_address;
			}
		}
		
		[Ignore]
		public bool First
		{
			get
			{
				return is_first_post;
			}
		}
		
		public static Dictionary<string, string> DatabaseColumnMap = new Dictionary<string, string>()
		{
			{ "Item", "topic_id" },
			{ "Date", "date" },
			{ "Content", "post" },
			{ "Author", "author_id" },
			{ "IpAddress", "ip_address" },
			{ "First", "is_first_post" },
		};
	}
}
