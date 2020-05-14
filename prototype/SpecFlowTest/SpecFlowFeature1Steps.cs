using System;
using Icarus.Sensors.ObjectDetection;
using TechTalk.SpecFlow;

namespace SpecFlowTest
{
    [Binding]
    public class SpecFlowFeature1Steps
    {
        private readonly IObjectDetectionController objectDetectionController;

        public SpecFlowFeature1Steps()
        {
            this.objectDetectionController = new ObjectDetectionController(new ObjectDetectionSensor());
        }

        [Given(@"I have entered (.*) into the calculator")]
        public void GivenIHaveEnteredIntoTheCalculator(int p0)
        {
            
        }

        [When(@"I press add")]
        public void WhenIPressAdd()
        {

        }

        [Then(@"the result should be (.*) on the screen")]
        public void ThenTheResultShouldBeOnTheScreen(int p0)
        {

        }
    }
}
