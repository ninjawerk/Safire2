using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;
using Safire.Library.TableModels;

namespace Safire.SQLite
{
    public enum Lock
    {
        Exclusive,
        Shared
    }
    public class LockManager
    {
        public Guid myKey { get; set; }
        public static Dictionary<Guid, Lock> Locks = new Dictionary<Guid, Lock>();

        public LockManager()
        {
            myKey = Guid.NewGuid();
        }

        public void PrepareForRead()
        {
          //  while (Locks.ContainsValue(Lock.Exclusive))
            {
            }
            Locks.Add(myKey, Lock.Shared);
        }
        public void PrepareForWrite()
        {
           // while (Locks.Count > 0)
            {

            }
           // Locks.Add(myKey, Lock.Exclusive);
        }

        public void RemoveLock()
        {
            Locks.Remove(myKey);
        }
    }
}
