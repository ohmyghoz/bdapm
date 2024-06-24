using BDA.DataModel;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using BDA.BusinessLayer;

namespace ConsoleSyncUser
{
    class Program
    {
        static void Main(string[] args)
        {
            DataEntities db = new DataEntities();
            db.Database.CommandTimeout = 1800;
            
            var start = new TimeSpan(3, 0, 0);
            var end = new TimeSpan(7, 0, 0);
            if (DateTime.Now.TimeOfDay >= start && DateTime.Now.TimeOfDay <= end)
            {
                Console.WriteLine("Mulai Sync User Pengawas");
                var m = new syncUser(db);
                m.AddUser();
                m.DeleteUser();
                Console.WriteLine("Selesai Sync User Pengawas");
            }
        }

    }
}
