using System;
using System.Collections.Generic;

namespace YouChewArchive.DataContracts
{
	public class Status
	{
		public static string DatabaseColumnId = "id";
		public static string DatabasePrefix = "status_";
		public static string TableName = "core_member_status_updates";
		public static string Application = "core";
		public int id { get; set; }
		public int member_id { get; set; }
		public int date { get; set; }
		public string content { get; set; }
		public int replies { get; set; }
		public string last_ids { get; set; }
		public bool is_latest { get; set; }
		public bool is_locked { get; set; }
		public string hash { get; set; }
		public bool imported { get; set; }
		public int author_id { get; set; }
		public string author_ip { get; set; }
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
		public int Date
		{
			get
			{
				return date;
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
		public int NumComments
		{
			get
			{
				return replies;
			}
		}
		
		[Ignore]
		public bool Locked
		{
			get
			{
				return is_locked;
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
		
		[Ignore]
		public string IpAddress
		{
			get
			{
				return author_ip;
			}
		}
		
		[Ignore]
		public string Content
		{
			get
			{
				return content;
			}
		}
		
		[Ignore]
		public string Title
		{
			get
			{
				return content;
			}
		}
		
		public static Dictionary<string, string> DatabaseColumnMap = new Dictionary<string, string>()
		{
			{ "Date", "date" },
			{ "Author", "author_id" },
			{ "NumComments", "replies" },
			{ "Locked", "is_locked" },
			{ "Approved", "approved" },
			{ "IpAddress", "author_ip" },
			{ "Content", "content" },
			{ "Title", "content" },
		};
	}
}
