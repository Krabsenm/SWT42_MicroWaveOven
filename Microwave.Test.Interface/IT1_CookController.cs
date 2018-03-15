using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices.ComTypes;
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
    public class IT1_CookController
    {
        private IOutput _output;
        private ITimer _timer;
        private IPowerTube _powerTube;
        private ILight _light;
        private IDisplay _display;
        private ICookController _cookController;
        private IUserInterface _userInterface;

        [SetUp]
        public void Setup()
        {
            _output = Substitute.For<IOutput>();
            _userInterface = Substitute.For<IUserInterface>();
            _timer = new Timer();
            _powerTube = new PowerTube(_output);
            _light = new Light(_output);
            _display = new Display(_output);
            _cookController = new CookController(_timer,_display,_powerTube, _userInterface);
        }

        [TestCase(5, 1000)]
        [TestCase(5, 3000)]
        [TestCase(5, 20000)]
        public void ControllerStartCooking_TimerStarted_Test(int power, int timer)
        {
            _cookController.StartCooking(power,timer);
            Assert.That(_timer.TimeRemaining, Is.EqualTo(timer));
        }

        [TestCase(5, 1000)]
        [TestCase(5, 3000)]
        [TestCase(5, 20000)]
        public void ControllerStartCooking_PowerTurnOn_Test(int power, int timer)
        {
            _cookController.StartCooking(power, timer);
            _output.Received().OutputLine($"PowerTube works with {power} %");
        }

        [TestCase(5, 2000, 1300, 1)]
        [TestCase(5, 5000, 3300, 3)]
        public void ControllerStartCooking_TimerTicksCorrectly_Test(int power, int timer, int waitTime, int expectedTicks)
        {
            ManualResetEvent pause = new ManualResetEvent(false);
            int ticks = 0;
            _timer.TimerTick += (sender, args) => ticks++;

            _cookController.StartCooking(power, timer);
            pause.WaitOne(waitTime);
            Assert.That(ticks, Is.EqualTo(expectedTicks));
        }

        [TestCase(5, 2000, 1300, 1)]
        [TestCase(5, 5000, 3300, 3)]
        public void ControllerStartCooking_displayShowtime_Test(int power, int timer, int waitTime, int expectedTicks)
        {
            ManualResetEvent pause = new ManualResetEvent(false);
            int ticks = 0;
            _timer.TimerTick += (sender, args) => ticks++;

            _cookController.StartCooking(power, timer);
            pause.WaitOne(waitTime);
            _output.Received().OutputLine(Arg.Is<string>(str => str.Contains("00:0" + ((timer/1000)-ticks).ToString())));
        }

        [Test]
        public void ControllerStartCooking_TimerExpired_Test()
        {
            ManualResetEvent pause = new ManualResetEvent(false);
            bool expired = false;
            _timer.Expired += (sender, args) => expired = true;

            _cookController.StartCooking(5, 2000);
            pause.WaitOne(2100);
            Assert.That(expired, Is.EqualTo(true));
        }

        [Test]
        public void ControllerStartCooking_CookingIsDoneUI_Test()
        {
            ManualResetEvent pause = new ManualResetEvent(false);

            _cookController.StartCooking(5, 2000);
            pause.WaitOne(2100);
            _userInterface.Received().CookingIsDone();
        }

        [Test]
        public void ControllerStartCooking_PowerTubeTurnOn_Test()
        {
            _cookController.StartCooking(5, 2000);
            _output.Received().OutputLine(Arg.Is<string>(str => str.Contains($"PowerTube works with {5} %")));
        }

        [Test]
        public void ControllerStartCooking_PowerTubeTurnOff_Test()
        {
            ManualResetEvent pause = new ManualResetEvent(false);

            _cookController.StartCooking(5, 2000);
            pause.WaitOne(2100);
            _output.Received().OutputLine(Arg.Is<string>(str => str.Contains($"PowerTube turned off")));
        }

        [Test]
        public void ControllerStop_TimeExpired_PowerTubeTurnedOff_Test()
        {
            ManualResetEvent pause = new ManualResetEvent(false);

            _cookController.StartCooking(5, 2000);
            _cookController.Stop();
            _output.Received().OutputLine(Arg.Is<string>(str => str.Contains($"PowerTube turned off")));
        }

    }
}

