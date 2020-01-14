using System;
using System.Collections.Generic;

namespace YouChewArchive.DataContracts
{
	public partial class Member
	{
		public static string DatabaseColumnId = "member_id";
		public static string TableName = "core_members";
		public int member_id { get; set; }
		public string name { get; set; }
		public int member_group_id { get; set; }
		public string email { get; set; }
		public int joined { get; set; }
		public string ip_address { get; set; }
		public bool? allow_admin_mails { get; set; }
		public int? skin { get; set; }
		public int? warn_level { get; set; }
		public int warn_lastwarn { get; set; }
		public int? language { get; set; }
		public int restrict_post { get; set; }
		public int? bday_day { get; set; }
		public int? bday_month { get; set; }
		public int? bday_year { get; set; }
		public int msg_count_new { get; set; }
		public int msg_count_total { get; set; }
		public bool msg_count_reset { get; set; }
		public bool msg_show_notification { get; set; }
		public string misc { get; set; }
		public int? last_visit { get; set; }
		public int? last_activity { get; set; }
		public int mod_posts { get; set; }
		public string auto_track { get; set; }
		public int? temp_ban { get; set; }
		public string mgroup_others { get; set; }
		public int member_login_key_expire { get; set; }
		public string has_blog { get; set; }
		public bool has_gallery { get; set; }
		public string members_seo_name { get; set; }
		public string members_cache { get; set; }
		public bool members_disable_pm { get; set; }
		public string failed_logins { get; set; }
		public int failed_login_count { get; set; }
		public int members_profile_views { get; set; }
		public string members_pass_hash { get; set; }
		public string members_pass_salt { get; set; }
		public int members_bitoptions { get; set; }
		public string members_day_posts { get; set; }
		public int notification_cnt { get; set; }
		public string conv_password { get; set; }
		public bool inactive_notified { get; set; }
		public int inactive_lastNotified { get; set; }
		public int inactive_oldGroup { get; set; }
		public int inactive_moved { get; set; }
		public bool? blogs_recache { get; set; }
		public string pp_last_visitors { get; set; }
		public string pp_main_photo { get; set; }
		public int? pp_main_width { get; set; }
		public int? pp_main_height { get; set; }
		public string pp_thumb_photo { get; set; }
		public int? pp_thumb_width { get; set; }
		public int? pp_thumb_height { get; set; }
		public int? pp_setting_count_comments { get; set; }
		public int? pp_reputation_points { get; set; }
		public string pp_gravatar { get; set; }
		public string pp_photo_type { get; set; }
		public string signature { get; set; }
		public string pconversation_filters { get; set; }
		public string pp_customization { get; set; }
		public string timezone { get; set; }
		public string pp_cover_photo { get; set; }
		public string profilesync { get; set; }
		public int profilesync_lastsync { get; set; }
		public int members_bitoptions2 { get; set; }
		public string create_menu { get; set; }
		public int? marked_site_read { get; set; }
		public int pp_cover_offset { get; set; }
		public int? acp_skin { get; set; }
		public int? acp_language { get; set; }
		public string member_title { get; set; }
		public int member_posts { get; set; }
		public int? member_last_post { get; set; }
		public bool pp_setting_count_visitors { get; set; }
		public string notes { get; set; }
		public decimal eco_points { get; set; }
		public decimal eco_worth { get; set; }
		public bool? is_tapatalk_member { get; set; }
		public int top10_positive_votes { get; set; }
		public int top10_negative_votes { get; set; }
		public string member_streams { get; set; }
		public int? photo_last_update { get; set; }
		public int? donation_group { get; set; }
		public int? donation_group_expires { get; set; }
		public decimal? donation_amount { get; set; }
		public int? donation_disable_ads { get; set; }
		public bool? membermap_location_synced { get; set; }
		public int? failed_mfa_attempts { get; set; }
		public string mfa_details { get; set; }
		public string permission_array { get; set; }
		
		[Ignore]
		public int Id
		{
			get
			{
				return member_id;
			}
		}
		
		[Ignore]
		public string Url
		{
			get
			{
				return $"member/{member_id}-{Logic.MemberLogic.LegalifyMemberSeo(this)}.html";
			}
		}
	}
}
