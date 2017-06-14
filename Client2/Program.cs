using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client2
{
    class Program
    {
        public static void Main()
        {
            Console.WriteLine(@"This project exists to launch another copy of the client in debug mode. To do this right click the project, click properties. In the debug section choose ""Start External Program"" rather than start project. Find the client exe that'll have been produced once you build the solution for the first time. It'll be somewhere like OperationalTransform\Client\bin\Debug\Client.exe.");
            Console.ReadKey();
        }
    }
}
