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
    public class IT1_ButtonDoor
    {
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
            _light = Substitute.For<ILight>();
            _display = Substitute.For<IDisplay>();
            _cookController = Substitute.For<ICookController>();

            _powerButton = new Button();
            _timeButton = new Button();
            _startCancelButton = new Button();
            _door = new Door();

            _userInterface = new UserInterface(_powerButton,_timeButton,_startCancelButton,_door,_display,_light,_cookController);
        }

        [Test]
        public void Door_OpenDoor_LightOn()
        {
            _door.Open();

            _light.Received().TurnOn();
        }

        [Test]
        public void Door_OpenThenCloseDoor_LightOff()
        {
            _door.Open();
            _door.Close();

            _light.Received().TurnOff();
        }

        [TestCase(1, 50)]
        [TestCase(3, 150)]
        [TestCase(14, 700)]
        [TestCase(15, 50)]
        public void PowerButton_PowerButtonPressedNumberOfTimes_DisplayCorrectPower(int presses, int power)
        {
            for (int i = 0; i < presses; ++i)
                _powerButton.Press();

            _display.ReceivedWithAnyArgs(presses).ShowPower(0);
            _display.Received().ShowPower(power);
        }

        [TestCase(1, 1)]
        [TestCase(3, 3)]
        [TestCase(14, 14)]
        public void TimerButton_TimerButtonPressedNumberOfTimes_DisplayCorrectTime(int presses, int time)
        {
            _powerButton.Press();

            for (int i = 0; i < presses; ++i)
                _timeButton.Press();

            _display.ReceivedWithAnyArgs(presses).ShowTime(0,0);
            _display.Received().ShowTime(time, 0);
        }

        [Test]
        public void StartCancelButton_StartPressed_LightOn()
        {
            _powerButton.Press();
            _timeButton.Press();
            _startCancelButton.Press();
            
            _light.Received().TurnOn();
        }

        [Test]
        public void StartCancelButton_StartPressed_StartCooking()
        {
            _powerButton.Press();
            _timeButton.Press();
            _startCancelButton.Press();

            _cookController.Received().StartCooking(50, 60);
        }

        [Test]
        public void StartCancelButton_CancelPressed_StopCooking()
        {
            _powerButton.Press();
            _timeButton.Press();
            _startCancelButton.Press();
            _startCancelButton.Press();

            _cookController.Received().Stop();
        }

        [Test]
        public void StartCancelButton_CancelPressed_ClearDisplay()
        {
            _powerButton.Press();
            _timeButton.Press();
            _startCancelButton.Press();
            _startCancelButton.Press();

            _display.Received().Clear();
        }

        [Test]
        public void StartCancelButton_CancelPressed_LightOff()
        {
            _powerButton.Press();
            _timeButton.Press();
            _startCancelButton.Press();
            _startCancelButton.Press();

            _light.Received().TurnOff();
        }

        [Test]
        public void Door_OpenDoorWhileCooking_StopCooking()
        {
            _powerButton.Press();
            _timeButton.Press();
            _startCancelButton.Press();
            _door.Open();

            _cookController.Received().Stop();
        }

        [Test]
        public void Door_OpenDoorWhileCooking_ClearDisplay()
        {
            _powerButton.Press();
            _timeButton.Press();
            _startCancelButton.Press();
            _door.Open();

            _display.Received().Clear();
        }
    }
}

