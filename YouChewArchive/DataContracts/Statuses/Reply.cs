using System;
using System.Collections.Generic;

namespace YouChewArchive.DataContracts
{
	public class Reply
	{
		public static string DatabaseColumnId = "id";
		public static string DatabasePrefix = "reply_";
		public static string TableName = "core_member_status_replies";
		public static string Application = "core";
        public static string RepuationTypeId = "status_reply_id";
        public int id { get; set; }
		public int status_id { get; set; }
		public int member_id { get; set; }
		public int date { get; set; }
		public string content { get; set; }
		public bool approved { get; set; }
		public string ip_address { get; set; }
		
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
				return status_id;
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
		public string Content
		{
			get
			{
				return content;
			}
		}
		
		[Ignore]
		public int Author
		{
			get
			{
				return member_id;
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
				return ip_address;
			}
		}
		
		public static Dictionary<string, string> DatabaseColumnMap = new Dictionary<string, string>()
		{
			{ "Item", "status_id" },
			{ "Date", "date" },
			{ "Content", "content" },
			{ "Author", "member_id" },
			{ "Approved", "approved" },
			{ "IpAddress", "ip_address" },
		};
	}
}
