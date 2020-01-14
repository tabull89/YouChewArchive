using System;
using System.Collections.Generic;

namespace YouChewArchive.DataContracts
{
	public class MessageTopicUserMap
	{
		public static string DatabaseColumnId = "id";
		public static string DatabasePrefix = "map_";
		public static string TableName = "core_message_topic_user_map";
		public static string Application = "core";
		public int id { get; set; }
		public int user_id { get; set; }
		public int topic_id { get; set; }
		public string folder_id { get; set; }
		public int read_time { get; set; }
		public bool user_active { get; set; }
		public bool user_banned { get; set; }
		public bool has_unread { get; set; }
		public bool is_system { get; set; }
		public bool is_starter { get; set; }
		public int left_time { get; set; }
		public bool ignore_notification { get; set; }
		public int last_topic_reply { get; set; }
		
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
