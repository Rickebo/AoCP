using Lib.Geometry;
using Lib.Text;

namespace Lib.Tests.Text;

public class ParserTests
{
    [Test]
    public void GetValues_ParsesVariousNumericTypes()
    {
        var ints = Parser.GetValues<int>("a1 b-2 c3");
        var unsigned = Parser.GetValues<uint>("4 5");
        var doubles = Parser.GetValues<double>("1.5 -2.5");
        var decimals = Parser.GetValues<decimal>("1,5 -2,5", ",");

        Assert.Multiple(() =>
        {
            Assert.That(ints, Is.EqualTo(new[] { 1, -2, 3 }).AsCollection);
            Assert.That(unsigned, Is.EqualTo(new uint[] { 4, 5 }).AsCollection);
            Assert.That(doubles, Is.EqualTo(new[] { 1.5, -2.5 }).AsCollection);
            Assert.That(decimals, Is.EqualTo(new[] { 1.5m, -2.5m }).AsCollection);
        });

        Assert.Throws<NotSupportedException>(() => Parser.GetValues<DateTime>("1"));
    }

    [Test]
    public void GetValuesFromArrayAndValueArrays_FlattenInput()
    {
        var values = Parser.GetValues<int>(["1 2", "3"]);
        var arrays = Parser.GetValueArrays<int>(["1 2", "3 4"]);

        Assert.That(values, Is.EqualTo(new[] { 1, 2, 3 }).AsCollection);
        Assert.That(arrays, Is.EqualTo(new[] { [1, 2], new[] { 3, 4 } }).AsCollection);
    }

    [Test]
    public void ParseDirectionGrid_MapsCharactersToDirections()
    {
        var grid = Parser.ParseDirectionGrid(">\n<");

        Assert.Multiple(() =>
        {
            Assert.That(grid.Width, Is.EqualTo(1));
            Assert.That(grid.Height, Is.EqualTo(2));
            Assert.That(grid[0, 0], Is.EqualTo(Direction.West));
            Assert.That(grid[0, 1], Is.EqualTo(Direction.East));
        });
    }
}


