using System;
using System.Collections.Generic;

namespace YouChewArchive.DataContracts
{
	public class Video
	{
		public static string DatabaseColumnId = "tid";
		public static string TableName = "videos_videos";
		public static string Application = "videos";
		public static string RepuationTypeId = "video_id";
		public int tid { get; set; }
		public int cid { get; set; }
		public int video_author_id { get; set; }
		public int date { get; set; }
		public string title { get; set; }
		public string description { get; set; }
		public string short_desc { get; set; }
		public bool video_status { get; set; }
		public int topic_id { get; set; }
		public int views { get; set; }
		public string embed { get; set; }
		public int last_updated { get; set; }
		public string video_type { get; set; }
		public string video_data { get; set; }
		public string seo_title { get; set; }
		public string thumbnail { get; set; }
		public int num_comments { get; set; }
		public int num_votes { get; set; }
		public double video_rating { get; set; }
		public bool isLatestVideo { get; set; }
		public bool thumbnail_type { get; set; }
		public bool featured { get; set; }
		public string extra_videos { get; set; }
		public string extra_videos_cached { get; set; }
		
		[Ignore]
		public int Id
		{
			get
			{
				return tid;
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
				return date;
			}
		}
		
		[Ignore]
		public int Updated
		{
			get
			{
				return last_updated;
			}
		}
		
		[Ignore]
		public int Author
		{
			get
			{
				return video_author_id;
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
		public string Content
		{
			get
			{
				return description;
			}
		}
		
		[Ignore]
		public int Container
		{
			get
			{
				return cid;
			}
		}
		
		[Ignore]
		public int NumComments
		{
			get
			{
				return num_comments;
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
		public bool Approved
		{
			get
			{
				return video_status;
			}
		}
		
		[Ignore]
		public double RatingAverage
		{
			get
			{
				return video_rating;
			}
		}
		
		[Ignore]
		public int RatingHits
		{
			get
			{
				return num_votes;
			}
		}
		
		public static Dictionary<string, string> DatabaseColumnMap = new Dictionary<string, string>()
		{
			{ "Title", "title" },
			{ "Date", "date" },
			{ "Updated", "last_updated" },
			{ "Author", "video_author_id" },
			{ "Views", "views" },
			{ "Content", "description" },
			{ "Container", "cid" },
			{ "NumComments", "num_comments" },
			{ "Featured", "featured" },
			{ "Approved", "video_status" },
			{ "RatingAverage", "video_rating" },
			{ "RatingHits", "num_votes" },
		};
	}
}
