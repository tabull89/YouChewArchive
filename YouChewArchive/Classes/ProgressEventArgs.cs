using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YouChewArchive
{
    public class ProgressEventArgs : EventArgs
    {
        public const string App = "App";
        public const string Important = "Important";
        public const string Containers = "Containers";
        public const string CurrentContainer = "CurrentContainer";
        public const string ContainersCompleted = "ContainersCompleted";
        public const string Items = "Items";
        public const string ItemsCompleted = "ItemsCompleted";
        public const string CurrentItem = "CurrentItem";
        public const string Comments = "Comments";
        public const string CommentsCompleted = "CommentsCompleted";
        

        public string Type { get; private set; }
        public string Value { get; private set; }

        public ProgressEventArgs(string type, string value)
        {
            Type = type;
            Value = value;
        }

    }
}
