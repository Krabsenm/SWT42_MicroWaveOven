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
            // fakes!!
            _output = Substitute.For<IOutput>();
            _timer = Substitute.For<ITimer>();
            _light = Substitute.For<ILight>();

            // reals!!!
            _display = new Display(_output);
            _powerTube = new PowerTube(_output);


            // drivers!!!
            _door = new Door();
            _powerButton = new Button();
            _timeButton = new Button();
            _startCancelButton = new Button();

            //also reals!!!
            _cookController = new CookController(_timer, _display, _powerTube);
            _userInterface = new UserInterface(_powerButton, _timeButton, _startCancelButton, _door, _display, _light, _cookController);
            ((CookController) _cookController).UI = _userInterface;
        }


        /******************************************************************
         *                Door Opens Test
         * 
         ******************************************************************/
        [Test]
        public void DoorOpens_CookingState_PowerTube_TurnOff()
        {

            //assign
            _powerButton.Press(); // go to setpower state
            _timeButton.Press(); // go to settime state
            _startCancelButton.Press(); // go to cooking state.
            _output.ClearReceivedCalls();

            //act
            _door.Open();

            //assert
            _output.Received().OutputLine(Arg.Is<string>(str =>
                str.Contains("PowerTube turned off")));
        }

        [Test]
        public void DoorOpens_CookingState_Display_Cleared()
        {

            //assign
            _powerButton.Press(); // go to setpower state
            _timeButton.Press(); // go to settime state
            _startCancelButton.Press(); // go to cooking state.
            _output.ClearReceivedCalls();

            //act
            _door.Open();

            //assert
            _output.Received().OutputLine(Arg.Is<string>(str =>
                str.Contains("Display cleared")));
        }

        /******************************************************************
         *                Display Tests
         * 
         ******************************************************************/

        [Test]
        public void PowerButtonPressed_ShowPower_DisplaysPowerLevel()
        {
            //assign
            _powerButton.Press();
            _output.ClearReceivedCalls();

            //act
            _powerButton.Press(); //sets power to 100  


            //assert
            _output.Received().OutputLine(Arg.Is<string>(str =>
                str.Contains("Display shows: 100 W")));
        }


        [Test]
        public void OnTimeButtonPressed_ShowTime_DisplaysTime()
        {

            //assign
            _powerButton.Press(); // go to setpower state
            _timeButton.Press(); // go to settime state
            _output.ClearReceivedCalls();


            //act
            _timeButton.Press(); // set time to 02:00 mins. 


            //assert
            _output.Received().OutputLine(Arg.Is<string>(str =>
                str.Contains("Display shows: 02:00")));
        }


        [Test]
        public void OnStartCancelPressed_CookingState_Display_cleared()
        {

            //assign
            _powerButton.Press(); // go to setpower state
            _timeButton.Press(); // go to settime state
            _startCancelButton.Press(); // go to cooking state. 
            _output.ClearReceivedCalls();


            //act
            _startCancelButton.Press(); // go to ready state. 

            //assert
            _output.Received().OutputLine(Arg.Is<string>(str =>
                str.Contains("Display cleared")));
        }

        /******************************************************************
         *                PowerTube Tests
         * 
         ******************************************************************/

        [Test]
        public void OnStartCancelPressed_OnStartCancelPressed_PowerTube_TurnOn()
        {

            //assign
            _powerButton.Press(); // go to setpower state
            _timeButton.Press(); // go to settime state
            _output.ClearReceivedCalls();


            //act
            _startCancelButton.Press(); // go to cooking state. 

            //assert
            _output.Received().OutputLine(Arg.Is<string>(str =>
                str.Contains("PowerTube works with 50 %")));
        }


        [Test]
        public void OnStartCancelPressed_CookingState_PowerTube_TurnOff()
        {

            //assign
            _powerButton.Press(); // go to setpower state
            _timeButton.Press(); // go to settime state
            _startCancelButton.Press(); // go to cooking state. 
            _output.ClearReceivedCalls();


            //act
            _startCancelButton.Press(); // go to ready state. 

            //assert
            _output.Received().OutputLine(Arg.Is<string>(str =>
                str.Contains("PowerTube turned off")));
        }


/********************************************************************************/



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
