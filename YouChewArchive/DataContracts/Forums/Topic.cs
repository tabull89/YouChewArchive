using System;
using System.Collections.Generic;

namespace YouChewArchive.DataContracts
{
	public class Topic
	{
		public static string DatabaseColumnId = "tid";
		public static string TableName = "forums_topics";
		public int tid { get; set; }
		public string title { get; set; }
		public string state { get; set; }
		public int? posts { get; set; }
		public int starter_id { get; set; }
		public int? start_date { get; set; }
		public int last_poster_id { get; set; }
		public int last_post { get; set; }
		public int? icon_id { get; set; }
		public string starter_name { get; set; }
		public string last_poster_name { get; set; }
		public string poll_state { get; set; }
		public int? last_vote { get; set; }
		public int? views { get; set; }
		public int forum_id { get; set; }
		public bool approved { get; set; }
		public bool? author_mode { get; set; }
		public bool? pinned { get; set; }
		public string moved_to { get; set; }
		public int topic_firstpost { get; set; }
		public int topic_queuedposts { get; set; }
		public int topic_open_time { get; set; }
		public int topic_close_time { get; set; }
		public int topic_rating_total { get; set; }
		public int topic_rating_hits { get; set; }
		public string title_seo { get; set; }
		public int moved_on { get; set; }
		public int last_real_post { get; set; }
		public bool topic_archive_status { get; set; }
		public int topic_answered_pid { get; set; }
		public int? popular_time { get; set; }
		public bool? featured { get; set; }
		public int? question_rating { get; set; }
		public int? topic_hiddenposts { get; set; }
		public string topic_description { get; set; }
		public int? locked_override { get; set; }
		public bool topic_meta_data { get; set; }
		
		[Ignore]
		public int Id
		{
			get
			{
				return tid;
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
		public string AuthorName
		{
			get
			{
				return starter_name;
			}
		}
		
		[Ignore]
		public int Container
		{
			get
			{
				return forum_id;
			}
		}
		
		[Ignore]
		public int? Date
		{
			get
			{
				return start_date;
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
		public int? NumComments
		{
			get
			{
				return posts;
			}
		}
		
		[Ignore]
		public int UnapprovedComments
		{
			get
			{
				return topic_queuedposts;
			}
		}
		
		[Ignore]
		public int? HiddenComments
		{
			get
			{
				return topic_hiddenposts;
			}
		}
		
		[Ignore]
		public int FirstCommentId
		{
			get
			{
				return topic_firstpost;
			}
		}
		
		[Ignore]
		public int LastComment
		{
			get
			{
				return last_post;
			}
		}
		
		[Ignore]
		public int LastCommentBy
		{
			get
			{
				return last_poster_id;
			}
		}
		
		[Ignore]
		public string LastCommentName
		{
			get
			{
				return last_poster_name;
			}
		}
		
		[Ignore]
		public int? Views
		{
			get
			{
				return views;
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
		public bool? Pinned
		{
			get
			{
				return pinned;
			}
		}
		
		[Ignore]
		public string Poll
		{
			get
			{
				return poll_state;
			}
		}
		
		[Ignore]
		public string Status
		{
			get
			{
				return state;
			}
		}
		
		[Ignore]
		public string MovedTo
		{
			get
			{
				return moved_to;
			}
		}
		
		[Ignore]
		public int MovedOn
		{
			get
			{
				return moved_on;
			}
		}
		
		[Ignore]
		public bool? Featured
		{
			get
			{
				return featured;
			}
		}
		
		[Ignore]
		public string State
		{
			get
			{
				return state;
			}
		}
		
		[Ignore]
		public int Updated
		{
			get
			{
				return last_post;
			}
		}
		
		[Ignore]
		public bool MetaData
		{
			get
			{
				return topic_meta_data;
			}
		}
		
		public static Dictionary<string, string> DatabaseColumnMap = new Dictionary<string, string>()
		{
			{ "Author", "starter_id" },
			{ "AuthorName", "starter_name" },
			{ "Container", "forum_id" },
			{ "Date", "start_date" },
			{ "Title", "title" },
			{ "NumComments", "posts" },
			{ "UnapprovedComments", "topic_queuedposts" },
			{ "HiddenComments", "topic_hiddenposts" },
			{ "FirstCommentId", "topic_firstpost" },
			{ "LastComment", "last_post" },
			{ "LastCommentBy", "last_poster_id" },
			{ "LastCommentName", "last_poster_name" },
			{ "Views", "views" },
			{ "Approved", "approved" },
			{ "Pinned", "pinned" },
			{ "Poll", "poll_state" },
			{ "Status", "state" },
			{ "MovedTo", "moved_to" },
			{ "MovedOn", "moved_on" },
			{ "Featured", "featured" },
			{ "State", "state" },
			{ "Updated", "last_post" },
			{ "MetaData", "topic_meta_data" },
		};
	}
}
