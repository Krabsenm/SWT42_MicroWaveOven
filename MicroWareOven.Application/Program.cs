using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MicroWareOven.Application
{
    class Program
    {
        static void Main(string[] args)
        {
            var Microwave = new MicrowaveOven();
            ConsoleKeyInfo cki;

            Console.Write("Microwave key guide:\n" +
                          "O - Open door\n" +
                          "C - Close door\n" +
                          "P - Power button press\n" +
                          "T - Time button press\n" +
                          "S - Start/cancel button press\n" +
                          "H - Hey guide\n" +
                          "Esc - Close program\r\n");

            do
            {
                cki = Console.ReadKey(true);

                switch (cki.Key)
                {
                    case ConsoleKey.O:
                        Console.WriteLine("\nOpening door");
                        Microwave.Door.Open();
                        break;

                    case ConsoleKey.C:
                        Console.WriteLine("\nClosing door");
                        Microwave.Door.Close();
                        break;

                    case ConsoleKey.P:
                        Console.WriteLine("\nPower button pressed");
                        Microwave.PowerButton.Press();
                        break;

                    case ConsoleKey.T:
                        Console.WriteLine("\nTime button pressed");
                        Microwave.TimeButton.Press();
                        break;

                    case ConsoleKey.S:
                        Console.WriteLine("\nStart/cancel button pressed");
                        Microwave.StartCancelButton.Press();
                        break;

                    case ConsoleKey.H:
                        Console.Write("\nMicrowave key guide:\n" +
                                      "O - Open door\n" +
                                      "C - Close door\n" +
                                      "P - Power button press\n" +
                                      "T - Time button press\n" +
                                      "S - Start/cancel button press\n" +
                                      "Esc - Close program\r\n");
                        break;

                    case ConsoleKey.Escape:
                        break;

                    default:
                        Console.WriteLine("Invalid Input - Press H to open Key Guide");
                        break;
                }
            } while (cki.Key != ConsoleKey.Escape);
        }
    }
}
