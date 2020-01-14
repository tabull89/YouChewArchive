using System;
using System.Collections.Generic;

namespace YouChewArchive.DataContracts
{
	public class ProfileFields
	{
		public static string DatabaseColumnId = "member_id";
		public static string TableName = "core_pfields_content";
		public int member_id { get; set; }
		public string field_1 { get; set; }
		public string field_2 { get; set; }
		public string field_3 { get; set; }
		public string field_6 { get; set; }
		public string field_7 { get; set; }
		public string field_10 { get; set; }
		public string field_13 { get; set; }
		public string field_18 { get; set; }
		public string field_19 { get; set; }
		public string sfsMemInfo { get; set; }
		public int sfsNextCheck { get; set; }
		public string field_20 { get; set; }
		public string field_21 { get; set; }
		public string field_22 { get; set; }
		public string field_23 { get; set; }
		public string field_24 { get; set; }
		public string field_25 { get; set; }
		
		[Ignore]
		public int Id
		{
			get
			{
				return member_id;
			}
		}
		
		[Ignore]
		public string AIM
		{
			get
			{
				return field_1;
			}
		}
		
		[Ignore]
		public string MSN
		{
			get
			{
				return field_2;
			}
		}
		
		[Ignore]
		public string WebsiteURL
		{
			get
			{
				return field_3;
			}
		}
		
		[Ignore]
		public string Location
		{
			get
			{
				return field_6;
			}
		}
		
		[Ignore]
		public string Interests
		{
			get
			{
				return field_7;
			}
		}
		
		[Ignore]
		public string Skype
		{
			get
			{
				return field_10;
			}
		}
		
		[Ignore]
		public string Country
		{
			get
			{
				return field_13;
			}
		}
		
		[Ignore]
		public string Gender
		{
			get
			{
				return field_18;
			}
		}
		
		[Ignore]
		public string DisplayBanner
		{
			get
			{
				return field_19;
			}
		}
		
		[Ignore]
		public string AboutMe
		{
			get
			{
				return field_20;
			}
		}
		
		[Ignore]
		public string Tumblr
		{
			get
			{
				return field_21;
			}
		}
		
		[Ignore]
		public string Twitter
		{
			get
			{
				return field_22;
			}
		}
		
		[Ignore]
		public string Facebook
		{
			get
			{
				return field_23;
			}
		}
		
		[Ignore]
		public string Steam
		{
			get
			{
				return field_24;
			}
		}
		
		[Ignore]
		public string Discord
		{
			get
			{
				return field_25;
			}
		}
		
		public static Dictionary<string, string> DatabaseColumnMap = new Dictionary<string, string>()
		{
			{ "AIM", "field_1" },
			{ "MSN", "field_2" },
			{ "WebsiteURL", "field_3" },
			{ "Location", "field_6" },
			{ "Interests", "field_7" },
			{ "Skype", "field_10" },
			{ "Country", "field_13" },
			{ "Gender", "field_18" },
			{ "DisplayBanner", "field_19" },
			{ "AboutMe", "field_20" },
			{ "Tumblr", "field_21" },
			{ "Twitter", "field_22" },
			{ "Facebook", "field_23" },
			{ "Steam", "field_24" },
			{ "Discord", "field_25" },
		};
	}
}
