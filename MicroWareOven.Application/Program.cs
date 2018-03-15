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
            var microwave = new MicrowaveOven();
            ManualResetEvent pause = new ManualResetEvent(false);

            // The user opens the door

            microwave._door.Open();

            //The user closes the door
            microwave._door.Close();

            //user sets power
            
           microwave._powerButton.Press();
           

            //user sets time
            microwave._timeButton.Press();
            
            //user press start/cancel button
            microwave._startCancelButton.Press();

            pause.WaitOne(10000);

            // user opens door while running
            microwave._door.Open();

            // user opens door while running
            microwave._startCancelButton.Press();
        }
    }
}
