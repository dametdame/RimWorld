using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;
using System.Threading;

namespace DTimeControl
{


    public static class LockUtility
    {
        public static Dictionary<object, LockWrapper> locks = new Dictionary<object, LockWrapper>();

        public static void LockObject(ref object obj)
        {
            //if (Thread.CurrentThread.ManagedThreadId == TimeControlBase.mainThreadId)
           //     return;
            
                
           //lock (locks)
            //{
                //if (locks.Count > 10)
                //Log.Message("Locking " + obj.ToString() + " with " + locks.Count + " other objs");
                //if (!locks.ContainsKey(obj))
                locks.SetOrAdd(obj, new LockWrapper(obj));
            lock (locks[obj])
            {
                locks[obj].Lock(ref obj);
            }
            //}
        }

        public static void UnLockObject(ref object obj)
        {
            // if (Thread.CurrentThread.ManagedThreadId == TimeControlBase.mainThreadId)
            //     return;
            //lock (locks)
           // {
                //Log.Message("Unlocking " + obj.ToString() + " with " + locks.Count + " other objs");
                LockWrapper locked = locks.TryGetValue(obj);
            lock (locked)
            {
                locked?.Unlock(ref obj);
                if (locked.count <= 0)
                {
                    locks.Remove(obj);
                }
                else
                {
                    //Log.Message("Unlock, still > 0 locks, count is " + locked.count);
                }
            }
            //}
        }
    }

    public class LockWrapper
    {
        object __lockObj;
        bool __lockWasTaken;
        public int count = 0;

        public LockWrapper(object obj)
        {
            __lockObj = obj;
            __lockWasTaken = false;
        }

        public void Lock(ref object obj)
        {
            System.Threading.Monitor.Enter(__lockObj, ref __lockWasTaken);
            count++;
        }
        public void Unlock(ref object obj)
        {
            if (__lockWasTaken) 
            {
                System.Threading.Monitor.Exit(__lockObj);
                count--;
            }
            else
            {
                Log.Message("Lock not taken, not decrementing");
            }
        }
    }
}
