using System;
using MicrowaveOvenClasses.Boundary;
using MicrowaveOvenClasses.Controllers;
using MicrowaveOvenClasses.Interfaces;
using NSubstitute;
using NUnit.Framework;

namespace Microwave.Test.Interface
{
    [TestFixture]
    public class IT2_LightCookController
    {
        private IOutput _output;
        private ITimer _timer;
        private IPowerTube _powerTube;
        private ILight _light;
        private IDisplay _display;
        private ICookController _cookController;
        private IButton _powerButton;
        private IButton _timeButton;
        private IButton _startCancelButton;
        private IDoor _door;
        private IUserInterface _userInterface;

        [SetUp]
        public void Setup()
        {
            _output = Substitute.For<IOutput>();
            _display = Substitute.For<IDisplay>();
            _timer = Substitute.For<ITimer>();
            _powerTube = Substitute.For<IPowerTube>();

            _powerButton = new Button();
            _timeButton = new Button();
            _startCancelButton = new Button();
            _door = new Door();

            _light = new Light(_output);
            _cookController = new CookController(_timer, _display, _powerTube);
            _userInterface = new UserInterface(_powerButton, _timeButton, _startCancelButton, _door, _display, _light, _cookController);

            ((CookController) _cookController).UI = _userInterface;
        }

        [Test]
        public void Light_OpenDoor_LogLightOn()
        {
            _door.Open();

            _output.Received().OutputLine(Arg.Is<string>(str => 
                str.ToLower().Contains("light") &&
                str.ToLower().Contains("on")
                ));
        }

        [Test]
        public void Light_OpenDoorThenClose_LogLightOff()
        {
            _door.Open();
            _output.ClearReceivedCalls();

            _door.Close();

            _output.Received().OutputLine(Arg.Is<string>(str =>
                str.ToLower().Contains("light") &&
                str.ToLower().Contains("off")
            ));
        }

        [Test]
        public void Light_TurnOnMicrowave_LogLightOn()
        {
            _powerButton.Press();
            _timeButton.Press();
            _startCancelButton.Press();

            _output.Received().OutputLine(Arg.Is<string>(str =>
                str.ToLower().Contains("light") &&
                str.ToLower().Contains("on")
            ));
        }

        [TestCase(1, 50)]
        [TestCase(3, 150)]
        [TestCase(14, 700)]
        [TestCase(15, 50)]
        public void CookController_PowerButtonPressedNumberOfTimes_PowerTubeTurnedOnWithCorrectPower(int presses, int power)
        {
            for (int i = 0; i < presses; ++i)
                _powerButton.Press();

            _timeButton.Press();
            _startCancelButton.Press();

            _powerTube.Received().TurnOn(power);
        }

        [TestCase(1, 60)]
        [TestCase(3, 180)]
        [TestCase(14, 840)]
        public void CookController_TimerButtonPressedNumberOfTimes_TimerStartedWithCorrectTime(int presses, int timeInSec)
        {
            _powerButton.Press();

            for (int i = 0; i < presses; ++i)
                _timeButton.Press();

            _startCancelButton.Press();

            _timer.Received().Start(timeInSec);
        }

        [TestCase(1,0)]
        [TestCase(0, 59)]
        [TestCase(0, 1)]
        public void CookController_TimerTick_ShowTime(int minRemaining, int secRemaining)
        {
            _powerButton.Press();
            _timeButton.Press();
            _startCancelButton.Press();

            _timer.TimeRemaining.Returns(minRemaining*60+secRemaining);
            _timer.TimerTick += Raise.EventWith(this, EventArgs.Empty);

            _display.Received().ShowTime(minRemaining, secRemaining);
        }

        [Test]
        public void CookController_TimerExpired_PowerTubeOff()
        {
            _powerButton.Press();
            _timeButton.Press();
            _startCancelButton.Press();

            _timer.Expired += Raise.EventWith(this, EventArgs.Empty);

            _powerTube.Received().TurnOff();
        }

        [Test]
        public void CookController_TimerExpired_CookingDone()
        {
            _powerButton.Press();
            _timeButton.Press();
            _startCancelButton.Press();

            _timer.Expired += Raise.EventWith(this, EventArgs.Empty);

            _timer.DidNotReceive().Stop();
            _display.Received().Clear();
        }

        [Test]
        public void Light_TimerExpired_LogLightOff()
        {
            _powerButton.Press();
            _timeButton.Press();
            _startCancelButton.Press();

            _timer.Expired += Raise.EventWith(this, EventArgs.Empty);

            _timer.DidNotReceive().Stop();
            _output.Received().OutputLine(Arg.Is<string>(str =>
                str.ToLower().Contains("light") &&
                str.ToLower().Contains("off")
            ));
        }

        [Test]
        public void CookController_DoorOpenWhileCooking_StopTimer()
        {
            _powerButton.Press();
            _timeButton.Press();
            _startCancelButton.Press();

            _door.Open();

            _timer.Received().Stop();
        }

        [Test]
        public void CookController_DoorOpenWhileCooking_PowerTubeOff()
        {
            _powerButton.Press();
            _timeButton.Press();
            _startCancelButton.Press();

            _door.Open();

            _powerTube.Received().TurnOff();
        }

        [Test]
        public void CookController_CancelWhileCooking_StopTimer()
        {
            _powerButton.Press();
            _timeButton.Press();
            _startCancelButton.Press();

            _startCancelButton.Press();

            _timer.Received().Stop();
        }

        [Test]
        public void CookController_CancelWhileCooking_PowerTubeOff()
        {
            _powerButton.Press();
            _timeButton.Press();
            _startCancelButton.Press();

            _startCancelButton.Press();

            _powerTube.Received().TurnOff();
        }

        [Test]
        public void CookController_CancelWhileCooking_LogLightOff()
        {
            _powerButton.Press();
            _timeButton.Press();
            _startCancelButton.Press();

            _startCancelButton.Press();

            _output.Received().OutputLine(Arg.Is<string>(str =>
                str.ToLower().Contains("light") &&
                str.ToLower().Contains("off")
            ));
        }
    }
}
