using BDA.BusinessLayer;
using BDA.DataModel;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace ConsoleApp1
{
    class Program
    {
        static void Main(string[] args)
        {

            DataEntities db = new DataEntities();
            db.Database.CommandTimeout = 1800;
            //kirim email
            
            
            var m = new EmailHelper(db);
            Console.WriteLine("Create Daily Alert Email");
            m.GeneraterAlertEmailDaily();
            Console.WriteLine("Finish Daily Alert Email");
            Console.WriteLine("Create Monthly Alert Email");
            m.GeneraterAlertEmailMonthly();
            Console.WriteLine("Finish Monthly Alert Email");
            Console.WriteLine("Start sending email");
            m.SendEmail();
            Console.WriteLine("Finish sending email");

        }
        
    }
}
