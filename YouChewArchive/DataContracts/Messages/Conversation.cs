using System;
using System.Collections.Generic;

namespace YouChewArchive.DataContracts
{
	public class Conversation
	{
		public static string DatabaseColumnId = "id";
		public static string DatabasePrefix = "mt_";
		public static string TableName = "core_message_topics";
		public static string Application = "core";
		public int id { get; set; }
		public int date { get; set; }
		public string title { get; set; }
		public int hasattach { get; set; }
		public int starter_id { get; set; }
		public int start_time { get; set; }
		public int last_post_time { get; set; }
		public int to_count { get; set; }
		public int to_member_id { get; set; }
		public int replies { get; set; }
		public int first_msg_id { get; set; }
		public bool is_draft { get; set; }
		public bool is_deleted { get; set; }
		public bool is_system { get; set; }
		
		[Ignore]
		public int Id
		{
			get
			{
				return id;
			}
		}
		
		[Ignore]
		public string Title
		{
			get
			{
				return title;
			}
		}
		
		[Ignore]
		public int Date
		{
			get
			{
				return last_post_time;
			}
		}
		
		[Ignore]
		public int Author
		{
			get
			{
				return starter_id;
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
		public int LastComment
		{
			get
			{
				return last_post_time;
			}
		}
		
		[Ignore]
		public int FirstCommentId
		{
			get
			{
				return first_msg_id;
			}
		}
		
		public static Dictionary<string, string> DatabaseColumnMap = new Dictionary<string, string>()
		{
			{ "Title", "title" },
			{ "Date", "last_post_time" },
			{ "Author", "starter_id" },
			{ "NumComments", "replies" },
			{ "LastComment", "last_post_time" },
			{ "FirstCommentId", "first_msg_id" },
		};
	}
}
