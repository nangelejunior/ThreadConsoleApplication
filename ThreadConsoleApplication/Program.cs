using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace ThreadConsoleApplication
{
    class Program
    {
        static void Main(string[] args)
        {
            IList<IList<int>> lists = new List<IList<int>>();
            for (int i = 0; i < 6; i++)
            {
                lists.Add(new List<int>());

                for (int j = 0; j < 5000; j++)
                {
                    lists[i].Add(j);
                }
            }

            int nroThreads = 5;

            Console.WriteLine("Started: " + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss"));

            for (int i = 0; i < lists.Count(); i++)
            {
                Console.WriteLine();

                StringBuilder sb = new StringBuilder();

                IList<int> list = lists[i];

                int toProcess = nroThreads;
                int nroItems = list.Count / nroThreads;

                Thread[] threads = new Thread[nroThreads];

                for (int j = 0; j < threads.Count(); j++)
                {
                    IList<int> l = list.Skip(nroItems * j).Take(nroItems).ToList();

                    threads[j] = new Thread(delegate()
                        {
                            toProcess = Proc(sb, list, l, ref toProcess);
                        });
                    threads[j].Name = "Thread_" + i + "-" + j;
                    threads[j].Start();
                }

                lock (list)
                {
                    if (toProcess > 0)
                    {
                        Monitor.Wait(list);
                    }
                }

                Console.WriteLine(sb.ToString());
            }

            Console.WriteLine();

            Console.WriteLine("Finished: " + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss"));

            System.Console.ReadKey();
        }

        private static int Proc(StringBuilder sb, IList<int> list, IList<int> l, ref int toProcess)
        {
            if (Interlocked.Decrement(ref toProcess) >= 0)
            {
                lock (list)
                {
                    sb.AppendLine("ThreadName [" + Thread.CurrentThread.Name + "]. Start at: " + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss.fff"));

                    int sum = 0;
                    foreach (int item in l)
                    {
                        sum++;

                        Thread.Sleep(60000);
                    }

                    sb.AppendLine("ThreadName [" + Thread.CurrentThread.Name + "]. End at: " + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss.fff"));

                    sb.AppendLine("ThreadName [" + Thread.CurrentThread.Name + "]  " + l.First() + " - " + l.Last() + ". Processados: " + sum);

                    Monitor.Pulse(list);
                }
            }
            return toProcess;
        }
    }
}
