using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YouChewArchive.DataContracts;

namespace YouChewArchive
{
    public class ForumContainer
    {
        public Forum Forum { get; set; }
        public List<ForumContainer> ChildForums { get; set; } = new List<ForumContainer>();
    }
}
