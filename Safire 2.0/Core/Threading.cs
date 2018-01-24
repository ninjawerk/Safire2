using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;

namespace Safire.Core
{
    class KeyedWorker : BackgroundWorker
    {
        public int KeyTag { get; set; }
        private Thread workerThread;

        public int CreateKey()
        {
            KeyTag =  StringCode.TimeKey();
            return KeyTag;
        }
    }
    class ThreadWorker : BackgroundWorker
    {
        public int KeyTag { get; set; }
        private Thread workerThread;

        protected override void OnDoWork(DoWorkEventArgs e)
        {
            workerThread = Thread.CurrentThread;
            try
            {
                base.OnDoWork(e);
            }
            catch (ThreadAbortException)
            {
                e.Cancel = true; //We must set Cancel property to true!
                Thread.ResetAbort(); //Prevents ThreadAbortException propagation
            }
        }
        public void Abort()
        {
            if (workerThread != null)
            {
                workerThread.Abort();
                workerThread = null;
            }
        }
        public void CreateKey()
        {
            KeyTag =  StringCode.TimeKey();
        }
    }
    public static class StringCode
    {

        public static string GetCommand(string s)
        {
            try
            {
                return s.Substring(0, s.IndexOf("ComSplit", System.StringComparison.Ordinal));
            }
            catch
            {
                return null;
            }
        }
        public static string GetValue(string s)
        {
            string ss = s.Substring(s.IndexOf("ComSplit", System.StringComparison.Ordinal), s.Length - s.IndexOf("ComSplit", System.StringComparison.Ordinal));
            return ss.Replace("ComSplit", "");

        }
        public static string Bind(string Command, string Value)
        {
            return Command + "ComSplit" + Value;
        }
        public static int TimeKey()
        {
            return DateTime.Now.Second + DateTime.Now.Minute + DateTime.Now.Hour + DateTime.Now.Millisecond;
        }
        public static string BindCommands(List<string> commands)
        {
            string ret = null;
            foreach (string str in commands)
            {
                ret = ret + "BindSplit" + str;
            }
            return ret;
        }
        public static string GetValue(string command, string str)
        {
            string[] lines = Regex.Split(str, "BindSplit");
            foreach (string stri in lines)
            {
                if (stri != "")
                    if (GetCommand(stri) == command)
                    {
                        return GetValue(stri);
                    }
            }
            return null;
        }
    }
}
