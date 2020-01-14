using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YouChewArchive
{
    public class Progress
    {
        public bool Important { get; set; }

        public string App { get; private set; }
        public string Containers { get; private set; }
        public string CurrentContainer { get; private set; }
        public string ContainersCompleted { get; private set; }
        public string Items { get; private set; }
        public string ItemsCompleted { get; private set; }
        public string CurrentItem { get; private set; }
        public string Comments { get; private set; }
        public string CommentsCompleted { get; private set; }



        public Progress()
        {
            Reset();
            
        }

        private void Reset()
        {
            Containers = "0";
            CurrentContainer = "";
            ContainersCompleted = "0";

            Items = "0";
            ItemsCompleted = "0";
            CurrentItem = "";

            Comments = "0";
            CommentsCompleted = "0";
        }

        public void ProcessEvent(List<ProgressEventArgs> args)
        {
            args.ForEach(arg =>
            {

                switch (arg.Type)
                { 
                    case ProgressEventArgs.Important:
                        Important = true;
                        break;

                    case ProgressEventArgs.App:
                        App = arg.Value;
                        break;

                    case ProgressEventArgs.Containers:
                        Containers = arg.Value;
                        break;

                    case ProgressEventArgs.CurrentContainer:
                        CurrentContainer = arg.Value;
                        break;

                    case ProgressEventArgs.ContainersCompleted:
                        ContainersCompleted = arg.Value;
                        break;

                    case ProgressEventArgs.Items:
                        Items = arg.Value;
                        break;

                    case ProgressEventArgs.ItemsCompleted:
                        ItemsCompleted = arg.Value;
                        break;

                    case ProgressEventArgs.CurrentItem:
                        CurrentItem = arg.Value;
                        break;

                    case ProgressEventArgs.Comments:
                        Comments = arg.Value;
                        break;

                    case ProgressEventArgs.CommentsCompleted:
                        CommentsCompleted = arg.Value;
                        break;
                }
            });
        }
    }
}
