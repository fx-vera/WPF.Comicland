using Applivery.Desktop.Core.Utils;
using NUnit.Framework;

namespace Applivery.Desktop.Core.Test.Utils
{
    internal class UnhandledExceptionHandlerTest
    {
        [SetUp]
        public void Setup()
        {
            UnhandledExceptionHandler.Init();
        }

        [Test]
        public void Test1()
        {
            throw new System.Exception("Checking Exception Manager");

            //Assert.Pass();
        }
    }
}
