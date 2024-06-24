using BDA.DataModel;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using BDA.BusinessLayer;

namespace ConsoleQueue
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {

                var start = new TimeSpan(0, 0, 0);
                var end = new TimeSpan(23, 0, 0);
                if (DateTime.Now.TimeOfDay >= start && DateTime.Now.TimeOfDay <= end)
                {
                    var m = new QueueExportData();
                    while (true)
                    {
                        m.CreateCSV();
                    }
                }
                

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                System.Threading.Thread.Sleep(20000);
            }

        }

    }
}
