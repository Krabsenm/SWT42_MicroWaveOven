using MicrowaveOvenClasses.Boundary;
using MicrowaveOvenClasses.Controllers;
using MicrowaveOvenClasses.Interfaces;
using NSubstitute;
using NUnit.Framework;

namespace Microwave.Test.Interface
{
    [TestFixture]
    public class IT2_UserInterfaceStubbedCookController
    {
        private IOutput _output;
        private IDoor _door;
        private ILight _light;
        private IDisplay _display;

        private IButton _powerButton;
        private IButton _timeButton;
        private IButton _startCancelButton;

        private ICookController _cookController;

        private IUserInterface _uut;

        [SetUp]
        public void Setup()
        {
            _output = Substitute.For<IOutput>();
            _cookController = Substitute.For<ICookController>();
            _door = new Door();
            _light = new Light(_output);
            _display = new Display(_output);

            _powerButton = new Button();
            _timeButton = new Button();
            _startCancelButton = new Button();

            _uut = new UserInterface(_powerButton, _timeButton, _startCancelButton, _door, _display,_light,_cookController);
        }

        [Test]
        public void DoorTest_DoorIsOpened_LightIsTurnedOn()
        {
            _door.Open();

            _output.Received().OutputLine(Arg.Is<string>(str => 
                str.ToLower().Contains("light") &&
                str.ToLower().Contains("on")
                ));
        }

        [Test]
        public void DoorTest_DoorIsOpenedThenClosed_LightIsTurnedOff()
        {
            _door.Open();
            _output.ClearReceivedCalls();
            _door.Close();

            _output.Received().OutputLine(Arg.Is<string>(str =>
                str.ToLower().Contains("light") &&
                str.ToLower().Contains("off")
            ));
        }

        [TestCase(1,50)]
        [TestCase(3, 150)]
        [TestCase(14, 700)]
        [TestCase(15, 50)]
        public void PowerButtonTest_PowerButtonPressedNumberOfTimes_DisplayCorrectPower(int presses, int power)
        {
            for (int i = 0; i < presses; ++i)
                _powerButton.Press();

            _output.Received().OutputLine(Arg.Is<string>(str =>
                str.ToLower().Contains("display") &&
                str.ToLower().Contains(power.ToString()) &&
                str.ToLower().Contains("w")
            ));
        }

        [TestCase(1, 1)]
        [TestCase(3, 3)]
        [TestCase(14, 14)]
        public void TimerButtonTest_TimerButtonPressedNumberOfTimes_DisplayCorrectTime(int presses, int time)
        {
            _powerButton.Press();

            for (int i = 0; i < presses; ++i)
                _timeButton.Press();

            _output.Received().OutputLine(Arg.Is<string>(str =>
                str.ToLower().Contains("display") &&
                str.ToLower().Contains($"{time:D2}:00")
            ));
        }

        [Test]
        public void PowerSetupTest_CancelPressedDuringSetup_DisplayCleared()
        {
            _powerButton.Press();
            _startCancelButton.Press();

            _output.Received().OutputLine(Arg.Is<string>(str =>
                str.ToLower().Contains("display") &&
                str.ToLower().Contains("cleared")
            ));
        }

        [Test]
        public void PowerSetupTest_DoorOpenedDuringSetup_DisplayCleared()
        {
            _powerButton.Press();
            _door.Open();

            _output.Received().OutputLine(Arg.Is<string>(str =>
                str.ToLower().Contains("display") &&
                str.ToLower().Contains("cleared")
            ));
        }

        [Test]
        public void PowerSetupTest_DoorOpenedDuringSetup_LightTurnedOn()
        {
            _powerButton.Press();
            _door.Open();

            _output.Received().OutputLine(Arg.Is<string>(str =>
                str.ToLower().Contains("light") &&
                str.ToLower().Contains("on")
            ));
        }

        [Test]
        public void TimerSetupTest_DoorOpenedDuringSetup_DisplayCleared()
        {
            _powerButton.Press();
            _timeButton.Press();
            _door.Open();

            _output.Received().OutputLine(Arg.Is<string>(str =>
                str.ToLower().Contains("display") &&
                str.ToLower().Contains("cleared")
            ));
        }

        [Test]
        public void TimerSetupTest_DoorOpenedDuringSetup_LightTurnedOn()
        {
            _powerButton.Press();
            _timeButton.Press();
            _door.Open();

            _output.Received().OutputLine(Arg.Is<string>(str =>
                str.ToLower().Contains("light") &&
                str.ToLower().Contains("on")
            ));
        }

        [Test]
        public void UserInterfaceTest_TurnOnMicrowave_LightTurnedOn()
        {
            _powerButton.Press();
            _timeButton.Press();
            _startCancelButton.Press();

            _output.Received().OutputLine(Arg.Is<string>(str =>
                str.ToLower().Contains("light") &&
                str.ToLower().Contains("on")
            ));
        }

        [Test]
        public void UserInterfaceTest_TurnOnMicrowave_CookingStarted()
        {
            _powerButton.Press();
            _timeButton.Press();
            _startCancelButton.Press();

            _cookController.ReceivedWithAnyArgs().StartCooking(50, 60);
        }

        [Test]
        public void UserInterfaceTest_CookingIsDone_DisplayCleared()
        {
            _powerButton.Press();
            _timeButton.Press();
            _startCancelButton.Press();

            _output.ClearReceivedCalls();
            _uut.CookingIsDone();
            
            _output.Received().OutputLine(Arg.Is<string>(str =>
                str.ToLower().Contains("display") &&
                str.ToLower().Contains("cleared")
            ));
        }

        [Test]
        public void UserInterfaceTest_CookingIsDone_LightTurnedOff()
        {
            _powerButton.Press();
            _timeButton.Press();
            _startCancelButton.Press();

            _output.ClearReceivedCalls();
            _uut.CookingIsDone();

            _output.Received().OutputLine(Arg.Is<string>(str =>
                str.ToLower().Contains("light") &&
                str.ToLower().Contains("off")
            ));
        }

        [Test]
        public void UserInterfaceTest_DoorOpenedDuringCooking_ControllerStopped()
        {
            _powerButton.Press();
            _timeButton.Press();
            _startCancelButton.Press();
            _door.Open();

            _cookController.Received().Stop();
        }

        [Test]
        public void UserInterfaceTest_DoorOpenedDuringCooking_DisplayCleared()
        {
            _powerButton.Press();
            _timeButton.Press();
            _startCancelButton.Press();

            _output.ClearReceivedCalls();
            _door.Open();

            _output.Received().OutputLine(Arg.Is<string>(str =>
                str.ToLower().Contains("display") &&
                str.ToLower().Contains("cleared")
            ));
        }

        [Test]
        public void UserInterfaceTest_CancelPressedDuringCooking_ControllerStopped()
        {
            _powerButton.Press();
            _timeButton.Press();
            _startCancelButton.Press();
            _startCancelButton.Press();

            _cookController.Received().Stop();
        }

        [Test]
        public void UserInterfaceTest_CancelPressedDuringCooking_DisplayCleared()
        {
            _powerButton.Press();
            _timeButton.Press();
            _startCancelButton.Press();

            _output.ClearReceivedCalls();
            _startCancelButton.Press();

            _output.Received().OutputLine(Arg.Is<string>(str =>
                str.ToLower().Contains("display") &&
                str.ToLower().Contains("cleared")
            ));
        }

        [Test]
        public void UserInterfaceTest_CancelPressedDuringCooking_LightTurnedOff()
        {
            _powerButton.Press();
            _timeButton.Press();
            _startCancelButton.Press();

            _output.ClearReceivedCalls();
            _startCancelButton.Press();

            _output.Received().OutputLine(Arg.Is<string>(str =>
                str.ToLower().Contains("light") &&
                str.ToLower().Contains("off")
            ));
        }
    }
}
