using CarRental.Business.Contracts.Service_Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SM = System.ServiceModel;

namespace CarRental.ServiceHost
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Starting up services...");
            Console.WriteLine("");

            SM.ServiceHost host = new SM.ServiceHost(typeof(IInventoryService));
            host.Open();

            Console.WriteLine("");
            Console.WriteLine("Press [Enter] to exit");
            Console.ReadLine();
        }
    }
}
