using NUnit.Framework;

namespace Applivery.Desktop.Comics.ViewModels.Test
{
    public class DisplayerViewModelTest
    {
        private DisplayerViewModel DisplayerViewModel;

        [SetUp]
        public void Setup()
        {
            DisplayerViewModel = new DisplayerViewModel();
        }

        [Test]
        public void GivenViewModelThenGetTheDisplayerModelHasBeenCreated()
        {
            Assert.IsNotNull(DisplayerViewModel.DisplayerModel);
        }
    }
}
