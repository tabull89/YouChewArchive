using System;
using System.Collections.Generic;

namespace YouChewArchive.DataContracts
{
	public class Group
	{
		public static string DatabaseColumnId = "g_id";
		public static string TableName = "core_groups";
		public int g_id { get; set; }
		public bool? g_view_board { get; set; }
		public bool? g_mem_info { get; set; }
		public bool? g_other_topics { get; set; }
		public bool? g_use_search { get; set; }
		public bool? g_edit_profile { get; set; }
		public bool? g_post_new_topics { get; set; }
		public bool? g_reply_own_topics { get; set; }
		public bool? g_reply_other_topics { get; set; }
		public string g_edit_posts { get; set; }
		public string g_delete_own_posts { get; set; }
		public bool? g_open_close_posts { get; set; }
		public bool? g_delete_own_topics { get; set; }
		public bool? g_post_polls { get; set; }
		public bool? g_vote_polls { get; set; }
		public bool? g_use_pm { get; set; }
		public bool? g_append_edit { get; set; }
		public bool? g_access_offline { get; set; }
		public bool? g_avoid_q { get; set; }
		public bool? g_avoid_flood { get; set; }
		public string g_icon { get; set; }
		public long? g_attach_max { get; set; }
		public string prefix { get; set; }
		public string suffix { get; set; }
		public int? g_max_messages { get; set; }
		public int? g_max_mass_pm { get; set; }
		public int? g_search_flood { get; set; }
		public int? g_edit_cutoff { get; set; }
		public bool? g_hide_from_list { get; set; }
		public bool? g_post_closed { get; set; }
		public string g_photo_max_vars { get; set; }
		public bool g_dohtml { get; set; }
		public bool g_edit_topic { get; set; }
		public bool g_bypass_badwords { get; set; }
		public bool g_can_msg_attach { get; set; }
		public int g_attach_per_post { get; set; }
		public int g_topic_rate_setting { get; set; }
		public int g_dname_changes { get; set; }
		public int g_dname_date { get; set; }
		public bool g_mod_preview { get; set; }
		public int g_rep_max_positive { get; set; }
		public int g_rep_max_negative { get; set; }
		public string g_signature_limits { get; set; }
		public bool g_hide_online_list { get; set; }
		public int g_bitoptions { get; set; }
		public int g_pm_perday { get; set; }
		public int g_mod_post_unit { get; set; }
		public int g_ppd_limit { get; set; }
		public int g_ppd_unit { get; set; }
		public int g_displayname_unit { get; set; }
		public int g_sig_unit { get; set; }
		public int g_pm_flood_mins { get; set; }
		public int g_max_bgimg_upload { get; set; }
		public bool? g_vs_view { get; set; }
		public bool g_vs_view_offline { get; set; }
		public bool? g_vs_add_video { get; set; }
		public bool? g_vs_edit_video { get; set; }
		public bool? g_vs_delete_video { get; set; }
		public bool? g_vs_rate_video { get; set; }
		public bool? g_vs_rate_video_change { get; set; }
		public bool? g_vs_report_video { get; set; }
		public bool? g_vs_view_comments { get; set; }
		public bool? g_vs_add_comments { get; set; }
		public bool? g_vs_edit_comments { get; set; }
		public bool? g_vs_delete_comments { get; set; }
		public int? g_vs_comments_per_member { get; set; }
		public bool? g_vs_m_edit_videos { get; set; }
		public bool? g_vs_m_delete_videos { get; set; }
		public bool? g_vs_m_edit_comments { get; set; }
		public bool? g_vs_m_delete_comments { get; set; }
		public bool? g_vs_m_manage { get; set; }
		public bool g_award_mod { get; set; }
		public int? g_display { get; set; }
		public bool? g_vs_embed_video { get; set; }
		public bool g_can_give_awards { get; set; }
		public bool g_can_remove_awards { get; set; }
		public bool g_can_receive_awards { get; set; }
		public bool g_tracker_view_offline { get; set; }
		public int g_tracker_attach_max { get; set; }
		public int g_tracker_attach_per_post { get; set; }
		public int g_blog_attach_max { get; set; }
		public int g_blog_attach_per_entry { get; set; }
		public bool g_blog_do_html { get; set; }
		public bool g_blog_do_commenthtml { get; set; }
		public bool g_blog_allowpoll { get; set; }
		public bool? g_blog_allowprivate { get; set; }
		public bool g_blog_allowprivclub { get; set; }
		public bool g_blog_alloweditors { get; set; }
		public bool g_blog_allowskinchoose { get; set; }
		public bool g_blog_preventpublish { get; set; }
		public bool? g_blog_allowlocal { get; set; }
		public int? g_blog_maxblogs { get; set; }
		public bool? g_blog_allowownmod { get; set; }
		public bool? g_blog_allowdelete { get; set; }
		public bool? g_blog_allowcomment { get; set; }
		public string g_membermap_markerColour { get; set; }
		public int g_bitoptions2 { get; set; }
		public bool g_upload_animated_photos { get; set; }
		public bool? g_view_displaynamehistory { get; set; }
		public string g_hide_own_posts { get; set; }
		public string g_lock_unlock_own { get; set; }
		public string g_can_report { get; set; }
		public string g_create_clubs { get; set; }
		public string g_club_allowed_nodes { get; set; }
		public bool g_close_polls { get; set; }
		public bool g_promote_exclude { get; set; }
		public int? g_club_limit { get; set; }
		
		[Ignore]
		public string Description
		{
			get
			{
				return LangLogic.GetValue($"core_group_{g_id}");
			}
		}
		
		[Ignore]
		public int Id
		{
			get
			{
				return g_id;
			}
		}
	}
}
