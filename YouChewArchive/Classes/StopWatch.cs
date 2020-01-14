using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YouChewArchive.Logic;

namespace YouChewArchive.Classes
{
    public class StopWatch : IDisposable
    {
        private string Text;

        private System.Diagnostics.Stopwatch sw;

        public StopWatch(string text)
        {
            Text = text;
            sw = new System.Diagnostics.Stopwatch();

            sw.Start();

        }

        public void Dispose()
        {
            sw.Stop();

            string elapsedTime = sw.Elapsed.ToString("c").Split('.').First();

            LogLogic.Log(LogLogic.Time, $"{Text}: {elapsedTime}");
        }
    }
}
