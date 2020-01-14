using System;
using System.Collections.Generic;

namespace YouChewArchive.DataContracts
{
	public partial class Blog
	{
		public static string DatabaseColumnId = "id";
		public static string DatabasePrefix = "blog_";
		public static string TableName = "blog_blogs";
		public int id { get; set; }
		public int member_id { get; set; }
		public int? num_views { get; set; }
		public bool pinned { get; set; }
		public bool disabled { get; set; }
		public bool allowguests { get; set; }
		public int? rating_total { get; set; }
		public int? rating_count { get; set; }
		public string settings { get; set; }
		public string last_visitors { get; set; }
		public string seo_name { get; set; }
		public int? editors { get; set; }
		public string groupblog_ids { get; set; }
		public int last_edate { get; set; }
		public int count_entries { get; set; }
		public int count_comments { get; set; }
		public int count_entries_hidden { get; set; }
		public int count_comments_hidden { get; set; }
		public decimal rating_average { get; set; }
		public string cover_photo { get; set; }
		public int? cover_photo_offset { get; set; }
		public int? social_group { get; set; }
		public int count_entries_future { get; set; }
		public string name { get; set; }
		public string desc { get; set; }
		public long? club_id { get; set; }
		public string sidebar { get; set; }
		
		[Ignore]
		public string Description
		{
			get
			{
				return LangLogic.GetValue($"blogs_blog_{Id}");
			}
		}
		
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
				return member_id;
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
		public int? Views
		{
			get
			{
				return num_views;
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
		public int Date
		{
			get
			{
				return last_edate;
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
		public int? CoverPhotoOffset
		{
			get
			{
				return cover_photo_offset;
			}
		}
		
		public static Dictionary<string, string> DatabaseColumnMap = new Dictionary<string, string>()
		{
			{ "Author", "member_id" },
			{ "Title", "name" },
			{ "Views", "num_views" },
			{ "Pinned", "pinned" },
			{ "Date", "last_edate" },
			{ "CoverPhoto", "cover_photo" },
			{ "CoverPhotoOffset", "cover_photo_offset" },
		};
	}
}
