using System;
using System.Collections.Generic;

namespace YouChewArchive.DataContracts
{
	public class BlogEntry
	{
		public static string DatabaseColumnId = "id";
		public static string DatabasePrefix = "entry_";
		public static string TableName = "blog_entries";
		public static string Application = "blog";
		public int id { get; set; }
		public int blog_id { get; set; }
		public int author_id { get; set; }
		public string author_name { get; set; }
		public int? date { get; set; }
		public string name { get; set; }
		public string content { get; set; }
		public string status { get; set; }
		public bool? locked { get; set; }
		public int? num_comments { get; set; }
		public int? last_comment_mid { get; set; }
		public int queued_comments { get; set; }
		public string post_key { get; set; }
		public int? edit_time { get; set; }
		public string edit_name { get; set; }
		public int? last_update { get; set; }
		public int? gallery_album { get; set; }
		public int? poll_state { get; set; }
		public bool featured { get; set; }
		public string name_seo { get; set; }
		public int publish_date { get; set; }
		public string image { get; set; }
		public int views { get; set; }
		public bool hidden { get; set; }
		public bool pinned { get; set; }
		public string ip_address { get; set; }
		public string cover_photo { get; set; }
		public int? cover_photo_offset { get; set; }
		public bool future_date { get; set; }
		public int? hidden_comments { get; set; }
		public bool is_future_entry { get; set; }
		public bool meta_data { get; set; }
		
		[Ignore]
		public int Id
		{
			get
			{
				return id;
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
				return content;
			}
		}
		
		[Ignore]
		public int Container
		{
			get
			{
				return blog_id;
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
		public int? Updated
		{
			get
			{
				return last_update;
			}
		}
		
		[Ignore]
		public string Title
		{
			get
			{
				return name;
			}
		}
		
		[Ignore]
		public int? NumComments
		{
			get
			{
				return num_comments;
			}
		}
		
		[Ignore]
		public int UnapprovedComments
		{
			get
			{
				return queued_comments;
			}
		}
		
		[Ignore]
		public int? HiddenComments
		{
			get
			{
				return hidden_comments;
			}
		}
		
		[Ignore]
		public int? LastCommentBy
		{
			get
			{
				return last_comment_mid;
			}
		}
		
		[Ignore]
		public int? LastComment
		{
			get
			{
				return last_update;
			}
		}
		
		[Ignore]
		public int Views
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
				return hidden;
			}
		}
		
		[Ignore]
		public bool Pinned
		{
			get
			{
				return pinned;
			}
		}
		
		[Ignore]
		public int? Poll
		{
			get
			{
				return poll_state;
			}
		}
		
		[Ignore]
		public bool Featured
		{
			get
			{
				return featured;
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
		public bool? Locked
		{
			get
			{
				return locked;
			}
		}
		
		[Ignore]
		public string CoverPhoto
		{
			get
			{
				return cover_photo;
			}
		}
		
		[Ignore]
		public bool IsFutureEntry
		{
			get
			{
				return is_future_entry;
			}
		}
		
		[Ignore]
		public int FutureDate
		{
			get
			{
				return publish_date;
			}
		}
		
		[Ignore]
		public string Status
		{
			get
			{
				return status;
			}
		}
		
		[Ignore]
		public bool MetaData
		{
			get
			{
				return meta_data;
			}
		}
		
		public static Dictionary<string, string> DatabaseColumnMap = new Dictionary<string, string>()
		{
			{ "Author", "author_id" },
			{ "AuthorName", "author_name" },
			{ "Content", "content" },
			{ "Container", "blog_id" },
			{ "Date", "date" },
			{ "Updated", "last_update" },
			{ "Title", "name" },
			{ "NumComments", "num_comments" },
			{ "UnapprovedComments", "queued_comments" },
			{ "HiddenComments", "hidden_comments" },
			{ "LastCommentBy", "last_comment_mid" },
			{ "LastComment", "last_update" },
			{ "Views", "views" },
			{ "Approved", "hidden" },
			{ "Pinned", "pinned" },
			{ "Poll", "poll_state" },
			{ "Featured", "featured" },
			{ "IpAddress", "ip_address" },
			{ "Locked", "locked" },
			{ "CoverPhoto", "cover_photo" },
			{ "IsFutureEntry", "is_future_entry" },
			{ "FutureDate", "publish_date" },
			{ "Status", "status" },
			{ "MetaData", "meta_data" },
		};
	}
}
