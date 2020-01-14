using System;
using System.Collections.Generic;

namespace YouChewArchive.DataContracts
{
	public class Lang
	{
		public static string DatabaseColumnId = "word_id";
		public static string TableName = "core_sys_lang_words";
		public int word_id { get; set; }
		public int lang_id { get; set; }
		public string word_app { get; set; }
		public string word_key { get; set; }
		public string word_default { get; set; }
		public string word_custom { get; set; }
		public string word_default_version { get; set; }
		public string word_custom_version { get; set; }
		public bool word_js { get; set; }
		public bool word_export { get; set; }
		public long? word_plugin { get; set; }
		public int? word_theme { get; set; }
		
		[Ignore]
		public int Id
		{
			get
			{
				return word_id;
			}
		}
	}
}
