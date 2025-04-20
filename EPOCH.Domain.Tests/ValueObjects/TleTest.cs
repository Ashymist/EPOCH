using EPOCH.Domain.ValueObjects;
namespace EPOCH.Domain.Tests.ValueObjects
{
    public class TleTest
    {
        [Fact]
        public void Tle_Should_Throw_When_TleLine1_Is_Null()
        {
            Assert.Throws<ArgumentException>(() => new Tle(null!, new string('B', 69)));
        }

        [Fact]
        public void Tle_Should_Throw_When_TleLine2_Is_Null()
        {
            Assert.Throws<ArgumentException>(() => new Tle(new string('A', 69), null!));
        }

        [Fact]
        public void Tle_Should_Throw_When_TleLine1_Is_Less_Than_69_Characters()
        {
            Assert.Throws<ArgumentException>(() => new Tle(new string('A', 1), new string('B', 69)));
        }

        [Fact]
        public void Tle_Should_Throw_When_TleLine1_Is_More_Than_69_Characters()
        {
            Assert.Throws<ArgumentException>(() => new Tle(new string('A', 100), new string('B', 69)));
        }

        [Fact]
        public void Tle_Should_Throw_When_TleLine2_Is_Less_Than_69_Characters()
        {
            Assert.Throws<ArgumentException>(() => new Tle(new string('A', 69), new string('B', 1)));
        }

        [Fact]
        public void Tle_Should_Throw_When_TleLine2_Is_More_Than_69_Characters()
        {
            Assert.Throws<ArgumentException>(() => new Tle(new string('A', 69), new string('B', 100)));
        }

        [Fact] 
        public void Tle_Should_Not_Throw_When_Passed_Valid_Tle_Lines() 
        {
            string tle1 = new string('A', 69);
            string tle2 = new string('B', 69);

            var exception = Record.Exception(() => new Tle(tle1, tle2));

            Assert.Null(exception);
        }
    }
}