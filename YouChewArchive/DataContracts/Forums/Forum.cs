using System;
using System.Collections.Generic;

namespace YouChewArchive.DataContracts
{
	public class Forum
	{
		public static string DatabaseColumnId = "id";
		public static string TableName = "forums_forums";
		public static string PermissionApp = "forums";
		public static string PermissionType = "forum";
		public int id { get; set; }
		public int topics { get; set; }
		public int posts { get; set; }
		public int? last_post { get; set; }
		public int last_poster_id { get; set; }
		public string last_poster_name { get; set; }
		public int? position { get; set; }
		public bool? use_ibc { get; set; }
		public bool? use_html { get; set; }
		public string password { get; set; }
		public string password_override { get; set; }
		public string last_title { get; set; }
		public int? last_id { get; set; }
		public string sort_key { get; set; }
		public string sort_order { get; set; }
		public int? prune { get; set; }
		public string topicfilter { get; set; }
		public bool? show_rules { get; set; }
		public bool? preview_posts { get; set; }
		public bool allow_poll { get; set; }
		public bool allow_pollbump { get; set; }
		public bool inc_postcount { get; set; }
		public int? skin_id { get; set; }
		public int? parent_id { get; set; }
		public string redirect_url { get; set; }
		public bool redirect_on { get; set; }
		public int redirect_hits { get; set; }
		public string notify_modq_emails { get; set; }
		public bool? sub_can_post { get; set; }
		public bool permission_showtopic { get; set; }
		public int queued_topics { get; set; }
		public int queued_posts { get; set; }
		public bool forum_allow_rating { get; set; }
		public int forum_last_deletion { get; set; }
		public string newest_title { get; set; }
		public int newest_id { get; set; }
		public int min_posts_post { get; set; }
		public int min_posts_view { get; set; }
		public bool can_view_others { get; set; }
		public bool hide_last_info { get; set; }
		public string name_seo { get; set; }
		public string seo_last_title { get; set; }
		public string seo_last_name { get; set; }
		public string last_x_topic_ids { get; set; }
		public int forums_bitoptions { get; set; }
		public bool disable_sharelinks { get; set; }
		public int deleted_posts { get; set; }
		public int deleted_topics { get; set; }
		public string conv_parent { get; set; }
		public string icon { get; set; }
		public decimal eco_tpc_pts { get; set; }
		public decimal eco_rply_pts { get; set; }
		public decimal eco_get_rply_pts { get; set; }
		public string tag_predefined { get; set; }
		public int archived_topics { get; set; }
		public int archived_posts { get; set; }
		public bool viglink { get; set; }
		public string ipseo_priority { get; set; }
		public string qa_rate_questions { get; set; }
		public string qa_rate_answers { get; set; }
		public long? club_id { get; set; }
		public string feature_color { get; set; }
		
		[Ignore]
		public string Description
		{
			get
			{
				return LangLogic.GetValue($"forums_forum_{id}");
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
		public string Url
		{
			get
			{
				return $"forum/{Id}-{name_seo}/1.html";
			}
		}
	}
}
