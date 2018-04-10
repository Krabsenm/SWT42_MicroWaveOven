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
    public class IT5_Output
    {
        private IOutput _output;
        private ILight _light;
        private IDisplay _display;
        private IPowerTube _powerTube;


        [SetUp]
        public void Setup()
        {
            _output = new Output();
            _light = new Light(_output);
            _display = new Display(_output);
            _powerTube = new PowerTube(_output);
            
        }

        [Test]
        public void LightToOutputTest_TurnOn_OutputLine()
        {
            //assign
            //Used to redirect Console output
            var sw = new StringWriter();
            Console.SetOut(sw); //Redirect


            //act
            _light.TurnOn();


            //assert
            Assert.That(sw.ToString(), Is.EqualTo("Light is turned on\r\n"));
        }


        [Test]
        public void LightToOutputTest_TurnOff_OutputLine()
        {
            //assign
            //Used to redirect Console output
            var sw = new StringWriter();
            Console.SetOut(sw); //Redirect

            //act
            _light.TurnOff();

            //assert
            Assert.That(sw.ToString(), Is.EqualTo("Light is turned off\r\n"));
        }

        [Test]
        public void PowerTubeToOutputTest_TurnOn_OutputLine()
        {
            //assign
            //Used to redirect Console output
            var sw = new StringWriter();
            Console.SetOut(sw); //Redirect

            //act
            _powerTube.TurnOn(50);

            //assert
            Assert.That(sw.ToString(), Is.EqualTo("PowerTube works with 50 %\r\n"));
        }

        [Test]
        public void PowerTubeToOutputTest_TurnOff_OutputLine()
        {
            //assign
            //Used to redirect Console output
            var sw = new StringWriter();
            Console.SetOut(sw); //Redirect

            //act
            _powerTube.TurnOff();

            //assert
            Assert.That(sw.ToString(), Is.EqualTo("PowerTube turned offf\r\n"));
        }


        [Test]
        public void DisplayToOutput_DisplayCleared_OutputLine()
        {

            //assign
           
            //Used to redirect Console output
            var sw = new StringWriter(); 
            Console.SetOut(sw); //Redirect


            //act
            _display.Clear(); // go to ready state. 

            //assert
            Assert.That(sw.ToString(), Is.EqualTo("Display cleared\r\n"));
        }

    }
}
