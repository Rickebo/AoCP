using Lib.Geometry;
using Lib.Text;

namespace Lib.Text.Tests;

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
            CollectionAssert.AreEqual(new[] { 1, -2, 3 }, ints);
            CollectionAssert.AreEqual(new uint[] { 4, 5 }, unsigned);
            CollectionAssert.AreEqual(new[] { 1.5, -2.5 }, doubles);
            CollectionAssert.AreEqual(new[] { 1.5m, -2.5m }, decimals);
        });

        Assert.Throws<NotSupportedException>(() => Parser.GetValues<DateTime>("1"));
    }

    [Test]
    public void GetValuesFromArrayAndValueArrays_FlattenInput()
    {
        var values = Parser.GetValues<int>(new[] { "1 2", "3" });
        var arrays = Parser.GetValueArrays<int>(new[] { "1 2", "3 4" });

        CollectionAssert.AreEqual(new[] { 1, 2, 3 }, values);
        CollectionAssert.AreEqual(new[] { new[] { 1, 2 }, new[] { 3, 4 } }, arrays);
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

