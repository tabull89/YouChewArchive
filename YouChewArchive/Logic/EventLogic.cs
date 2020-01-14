using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YouChewArchive
{
    public class EventLogic
    {

        public delegate void ProgressEvent(object sender, List<ProgressEventArgs> args);
        public static event ProgressEvent OnProgressEvent;

        public static void RaiseEvent(string type, string value)
        {
            OnProgressEvent?.Invoke(null, new List<ProgressEventArgs> { new ProgressEventArgs(type, value) });
        }

        public static void RaiseEvent(List<ProgressEventArgs> args)
        {
            OnProgressEvent?.Invoke(null, args);
        }

        public static void RaiseImportantEvent()
        {
            RaiseEvent(ProgressEventArgs.Important, null);
        }

    }
}
