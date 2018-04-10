using System;
using System.Threading;
using MicrowaveOvenClasses.Boundary;
using MicrowaveOvenClasses.Controllers;
using MicrowaveOvenClasses.Interfaces;
using NSubstitute;
using NUnit.Framework;
using Timer = MicrowaveOvenClasses.Boundary.Timer;

namespace Microwave.Test.Interface
{
    [TestFixture]
    public class IT4_Timer
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

            _powerButton = new Button();
            _timeButton = new Button();
            _startCancelButton = new Button();
            _door = new Door();

            _powerTube = new PowerTube(_output);
            _light = new Light(_output);
            _display = new Display(_output);
            _timer = new Timer();

            _cookController = new CookController(_timer, _display, _powerTube);
            _userInterface = new UserInterface(_powerButton, _timeButton, _startCancelButton, _door, _display, _light, _cookController);
            ((CookController) _cookController).UI = _userInterface;
        }

        [Test]
        public void Timer_TimerTick_LogTimeRemaining()
        {
            ManualResetEvent pause = new ManualResetEvent(false);

            _powerButton.Press();
            _timeButton.Press();
            _startCancelButton.Press();

            pause.WaitOne(1100);

            _output.Received().OutputLine(Arg.Is<string>(str => str.Contains("00:59")));
        }

        [Test]
        public void Timer_TwoTimerTicks_LogTimeRemaining()
        {
            ManualResetEvent pause = new ManualResetEvent(false);

            _powerButton.Press();
            _timeButton.Press();
            _startCancelButton.Press();

            pause.WaitOne(2100);

            _output.Received().OutputLine(Arg.Is<string>(str => str.Contains("00:58")));
        }

        [Test]
        public void Timer_StartMicrowaveWaitOneMinute_TimerExpired()
        {
            ManualResetEvent pause = new ManualResetEvent(false);

            _powerButton.Press();
            _timeButton.Press();
            _startCancelButton.Press();

            pause.WaitOne(61000);

            _output.Received().OutputLine(Arg.Is<string>(str => str.Contains("Display cleared")));
        }

        [Test]
        public void Timer_DoorOpenToStopTimer_TimerStopped()
        {
            ManualResetEvent pause = new ManualResetEvent(false);

            _powerButton.Press();
            _timeButton.Press();
            _startCancelButton.Press();

            pause.WaitOne(1100);
            _door.Open();
            pause.WaitOne(1100);

            _output.Received().OutputLine(Arg.Is<string>(str => str.Contains("00:59")));
            _output.DidNotReceive().OutputLine(Arg.Is<string>(str => str.Contains("00:58")));
        }

        [Test]
        public void Timer_CancelPressedToStopTimer_TimerStopped()
        {
            ManualResetEvent pause = new ManualResetEvent(false);

            _powerButton.Press();
            _timeButton.Press();
            _startCancelButton.Press();

            pause.WaitOne(1100);
            _startCancelButton.Press();
            pause.WaitOne(1100);

            _output.Received().OutputLine(Arg.Is<string>(str => str.Contains("00:59")));
            _output.DidNotReceive().OutputLine(Arg.Is<string>(str => str.Contains("00:58")));
        }
    }
}
