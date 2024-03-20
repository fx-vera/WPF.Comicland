using NUnit.Framework;

namespace Applivery.Desktop.Comics.Logic.Test
{
    public class ApiReaderTest
    {
        [SetUp]
        public void Setup()
        {

        }

        [Test]
        public void ReadFromAPIAndGetResults()
        {
            var result = ApiReader.ReadComicsFromAPI().Result;
            Assert.IsNotEmpty(result.data.results);
        }
    }
}