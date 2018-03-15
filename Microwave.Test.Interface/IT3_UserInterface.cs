using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using MicrowaveOvenClasses.Boundary;
using MicrowaveOvenClasses.Controllers;
using MicrowaveOvenClasses.Interfaces;
using NSubstitute;
using NUnit.Framework;
using Timer = MicrowaveOvenClasses.Boundary.Timer;

namespace Microwave.Test.Interface
{
    [TestFixture]
    public class IT3_UserInterface
    {
        private IOutput _output;
        private IDoor _door;
        private ILight _light;
        private IDisplay _display;
        private ITimer _timer;
        private IPowerTube _powerTube;

        private IButton _powerButton;
        private IButton _timeButton;
        private IButton _startCancelButton;

        private ICookController _cookController;
        private IUserInterface _userInterface;

        [SetUp]
        public void Setup()
        {
            _output = Substitute.For<IOutput>();
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
            ((CookController) _cookController).UI = _userInterface;
        }

        [Test]
        public void StartCookingTest_StartCooking_PowerTubeTurnOn()
        {
            _powerButton.Press();
            _timeButton.Press();
            _startCancelButton.Press();

            _output.Received().OutputLine(Arg.Is<string>(str =>
                str.ToLower().Contains("powertube") &&
                str.ToLower().Contains("50 %")
            ));
        }

        [Test]
        public void StartCookingTest_StartCookingAndWaitCookingIsDoneCalled_PowerTubeTurnOff()
        {
            ManualResetEvent pause = new ManualResetEvent(false);
            _powerButton.Press();
            _timeButton.Press();
            _startCancelButton.Press();

            pause.WaitOne(62000);

            _output.Received().OutputLine(Arg.Is<string>(str =>
                str.ToLower().Contains("powertube") &&
                str.ToLower().Contains("off")
            ));
        }

        [Test]
        public void StopCookingTest_StartCookingOpenDoor_PowerTubeTurnOff()
        {
            _powerButton.Press();
            _timeButton.Press();
            _startCancelButton.Press();

            _door.Open();

            _output.Received().OutputLine(Arg.Is<string>(str =>
                str.ToLower().Contains("powertube") &&
                str.ToLower().Contains("off")
            ));
        }

        [Test]
        public void StopCookingTest_StartCookingPressCancel_PowerTubeTurnOff()
        {
            _powerButton.Press();
            _timeButton.Press();
            _startCancelButton.Press();

            _startCancelButton.Press();

            _output.Received().OutputLine(Arg.Is<string>(str =>
                str.ToLower().Contains("powertube") &&
                str.ToLower().Contains("off")
            ));
        }
    }
}
