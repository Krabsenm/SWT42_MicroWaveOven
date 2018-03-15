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
       public  IOutput _output;
       public  IDoor _door;
       public  ILight _light;
       public  IDisplay _display;
       public  ITimer _timer;
       public  IPowerTube _powerTube;
 
       public  IButton _powerButton;
       public  IButton _timeButton;
       public  IButton _startCancelButton;
 
       public  ICookController _cookController;
       public  IUserInterface _userInterface;

        public MicrowaveOven()
        {
            _output = new Output();
            _door = new Door();
            _light = new Light(_output);
            _display = new Display(_output);
            _timer = new Timer();
            _powerTube = new PowerTube(_output);

            _powerButton = new Button();
            _timeButton = new Button();
            _startCancelButton = new Button();

            _cookController = new CookController(_timer, _display, _powerTube);
            _userInterface = new UserInterface(_powerButton, _timeButton, _startCancelButton, _door, _display, _light, _cookController);
            ((CookController)_cookController).UI = _userInterface;
        }

    }
}
