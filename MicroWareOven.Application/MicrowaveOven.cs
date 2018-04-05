using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MicrowaveOvenClasses.Boundary;
using MicrowaveOvenClasses.Controllers;
using MicrowaveOvenClasses.Interfaces;

namespace MicroWareOven.Application
{
    class MicrowaveOven
    {
       public  IOutput Output;
       public  IDoor Door;
       public  ILight Light;
       public  IDisplay Display;
       public  ITimer Timer;
       public  IPowerTube PowerTube;
 
       public  IButton PowerButton;
       public  IButton TimeButton;
       public  IButton StartCancelButton;
 
       public  ICookController CookController;
       public  IUserInterface UserInterface;

        public MicrowaveOven()
        {
            Output = new Output();
            Door = new Door();
            Light = new Light(Output);
            Display = new Display(Output);
            Timer = new Timer();
            PowerTube = new PowerTube(Output);

            PowerButton = new Button();
            TimeButton = new Button();
            StartCancelButton = new Button();

            CookController = new CookController(Timer, Display, PowerTube);
            UserInterface = new UserInterface(PowerButton, TimeButton, StartCancelButton, Door, Display, Light, CookController);
            ((CookController)CookController).UI = UserInterface;
        }

    }
}
