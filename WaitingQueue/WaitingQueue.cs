using System;
using System.Collections.Generic;
using System.Threading;

namespace WaitingQueue
{
    public class WaitingQueue<T> : IDisposable
    {
        private readonly Queue<T> items;
        private readonly int cap;
        private static readonly object locker = new object();
        private static readonly AutoResetEvent newItemEvt = new AutoResetEvent(false);
        private static readonly ManualResetEvent queueNotFullEvt = new ManualResetEvent(true);

        public WaitingQueue(int capacity)
        {
            cap = capacity;
            items = new Queue<T>(capacity);
        }

        public void Dispose()
        {
            items.Clear();
        }

        public void Clear()
        {
            items.Clear();
        }

        public bool IsEmpty()
        {
            lock(locker)
            {
                return items.Count == 0;
            }
        }

        public bool IsFull()
        {
            lock(locker)
            {
                return items.Count == cap;
            }
        }

        // Flush old item if full
        public void Enqueue(T t)
        {
            lock(locker)
            {
                items.Enqueue(t);
                newItemEvt.Set();
                // if full, reset flag
                if (items.Count == cap)
                    queueNotFullEvt.Reset();
            }
        }

        // Throw exception if empty
        public T Dequeue()
        {
            lock(locker)
            {
                queueNotFullEvt.Set();
                return items.Dequeue();
            }
        }

        // dequeue until item inside
        public T DequeueWaitAvailable()
        {
        OUT:
            try
            {
                return Dequeue();
            }
            catch (InvalidOperationException)
            {
                while (newItemEvt.WaitOne())
                {
                    goto OUT;
                }
                throw;
            }
        }

        // enqueue without flush
        public void EnqueueWaitAvailable(T t)
        {
        IN:
            // check flag, if not full(set), enqueue
            if (queueNotFullEvt.WaitOne(0))
            {
                Enqueue(t);
                return;
            }
            // if full(unset), wait
            queueNotFullEvt.WaitOne();
            goto IN;
        }

        public bool CanDequeue()
        {
            return newItemEvt.WaitOne(0);
        }

        public bool CanEnqueue()
        {
            return queueNotFullEvt.WaitOne(0);
        }
    }
}
