using System;
using System.Diagnostics;
using System.Threading;

namespace WaitingQueue
{
    class Program
    {
        static void Main(string[] args)
        {
            var wq = new WaitingQueue<int>(10);

            Console.WriteLine($"wq is empty: {wq.IsEmpty()}");
            Console.WriteLine($"can dequeue: {wq.CanDequeue()}");

            for (int i = 0; i < 10; i++)
            {
                wq.Enqueue(i);
            }

            Console.WriteLine($"wq is full: {wq.IsFull()}");

            Thread thread1 = new Thread(()=> {
                Thread.Sleep(1000);
                var i = wq.Dequeue();
                Console.WriteLine($"thread1 deque: {i}");
            });

            var stopwatch = new Stopwatch();

            thread1.Start();
            
            stopwatch.Start();
            Console.WriteLine($"can enqueue: {wq.CanEnqueue()}");
            wq.EnqueueWaitAvailable(11);
            Console.WriteLine($"main thread enqueue: 11");
            Console.WriteLine($"time elapsed: {stopwatch.Elapsed}");
            Console.WriteLine($"wq is full: {wq.IsFull()}");
            for (int i = 9; i > 0; i--)
                wq.Dequeue();
            Console.WriteLine($"last item: {wq.Dequeue()}");
            Console.WriteLine($"wq is empty: {wq.IsEmpty()}");

            Thread thread2 = new Thread(() =>
            {
                Thread.Sleep(1000);
                wq.Enqueue(12);
                Console.WriteLine($"thread2 enquque: 12");
            });

            stopwatch = new Stopwatch();

            thread2.Start();

            stopwatch.Start();
            Console.WriteLine($"can dequeue: {wq.CanDequeue()}");
            var x = wq.DequeueWaitAvailable();
            Console.WriteLine($"main thread dequeue: {x}");
            Console.WriteLine($"time elapsed: {stopwatch.Elapsed}");
            Console.WriteLine($"wq is empty: {wq.IsEmpty()}");

            Console.ReadLine();
        }
    }
}
