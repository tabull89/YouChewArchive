using System;
using System.Collections.Generic;

namespace YouChewArchive.DataContracts
{
	public class VideoCategory
	{
		public static string DatabaseColumnId = "id";
		public static string TableName = "videos_cat";
		public int id { get; set; }
		public int views { get; set; }
		public int fid { get; set; }
		public int parent_id { get; set; }
		public int position { get; set; }
		public string options { get; set; }
		public string seo_name { get; set; }
		public bool cat_only { get; set; }
		public bool? tags_enable { get; set; }
		public bool? tags_prefixes { get; set; }
		public string tags_predefined { get; set; }
		public string cat_image { get; set; }
		public int last_video_id { get; set; }
		public int last_video_date { get; set; }
		public long? club_id { get; set; }
		
		[Ignore]
		public string Description
		{
			get
			{
				return LangLogic.GetValue($"videos_category_{Id}");
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
	}
}
