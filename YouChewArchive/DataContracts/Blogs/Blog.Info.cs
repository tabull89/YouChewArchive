using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YouChewArchive.Data;

namespace YouChewArchive.DataContracts
{
    public partial class Blog
    {
        private int? lastEntry;

        [Ignore]
        public int LastEntry
        {
            get
            {
                if(!lastEntry.HasValue)
                {
                    lastEntry = DB.Instance.ExecuteScalar<int?>($"SELECT MAX(entry_date) FROM {BlogEntry.TableName} WHERE entry_blog_id = {Id}");

                    if(!lastEntry.HasValue)
                    {
                        lastEntry = 0;
                    }
                }

                return lastEntry.Value;
            }
        }
    }
}
