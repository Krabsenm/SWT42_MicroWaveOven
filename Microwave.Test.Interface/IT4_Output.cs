using System;
using System.IO;
using MicrowaveOvenClasses.Boundary;
using MicrowaveOvenClasses.Controllers;
using MicrowaveOvenClasses.Interfaces;
using NSubstitute;
using NUnit.Framework;

namespace Microwave.Test.Interface
{
    [TestFixture]
    public class IT4_Output
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

        [Test]
        public void LightToOutputTest_OpenDoor_LightWritesToOutput()
        {
            //Used to redirect Console output
            var sw = new StringWriter();
            Console.SetOut(sw); //Redirect
            _door.Open();

            Assert.That(sw.ToString(), Is.EqualTo("Light is turned on\r\n"));
        }

        [Test]
        public void PowerTubeToOutputTest_StartCooking_DisplayIsClearedPowerTubeWritesToOutput()
        {
            //Used to redirect Console output
            var sw = new StringWriter();
            _powerButton.Press();
            _timeButton.Press();
            
            Console.SetOut(sw); //Redirect
            _startCancelButton.Press();

            Assert.That(sw.ToString(), Is.EqualTo("Display cleared\r\nLight is turned on\r\nPowerTube works with 50 %\r\n"));
        }
    }
}
