using Lib.Coordinate;
using Lib.Enums;
using Lib.Grid;

namespace Lib.UnitTests.Grids;

public class ArrayGridTests
{
    private static ArrayGrid<int> CreateSampleGrid()
    {
        var grid = new ArrayGrid<int>(3, 3);
        var counter = 1;
        foreach (var coord in grid.Coordinates)
            grid[coord] = counter++;
        return grid;
    }

    [Test]
    public void Constructors_SetupDimensionsAndInitialValues()
    {
        var filled = new ArrayGrid<int>(2, 2, 7);
        var copied = new ArrayGrid<int>(new[,] { { 1, 2 }, { 3, 4 } });

        Assert.Multiple(() =>
        {
            Assert.That(filled.Width, Is.EqualTo(2));
            Assert.That(filled.Height, Is.EqualTo(2));
            Assert.That(filled.Size, Is.EqualTo(4));
            Assert.That(filled[0, 0], Is.EqualTo(7));

            Assert.That(copied[1, 0], Is.EqualTo(3));
        });

        var fromRows = new ArrayGrid<int>(new[] { new[] { 1, 2 }, new[] { 3, 4 } }, 2, 2);
        Assert.That(fromRows[1, 1], Is.EqualTo(4));

        Assert.Multiple(() =>
        {
            Assert.That(filled.BottomLeft, Is.EqualTo(new IntegerCoordinate<int>(0, 0)));
            Assert.That(filled.TopRight, Is.EqualTo(new IntegerCoordinate<int>(1, 1)));
        });
    }

    [Test]
    public void Indexers_SupportCoordinatesAndRanges()
    {
        var grid = CreateSampleGrid();
        var coord = new IntegerCoordinate<int>(1, 1);

        Assert.Multiple(() =>
        {
            Assert.That(grid[coord], Is.EqualTo(5));
            grid[coord] = 99;
            Assert.That(grid[1, 1], Is.EqualTo(99));
        });

        var rangeValues = grid[0..2, 0..2].ToArray();
        CollectionAssert.AreEquivalent(new[] { 1, 2, 4, 99 }, rangeValues);
    }

    [Test]
    public void OutlineAndOnRadius_OnOutline_DetectBorders()
    {
        var grid = new ArrayGrid<int>(5, 5);
        var center = new IntegerCoordinate<int>(2, 2);
        var corner = new IntegerCoordinate<int>(0, 0);

        Assert.Multiple(() =>
        {
            Assert.That(grid.OnOutline(corner), Is.True);
            Assert.That(grid.OnOutline(center), Is.False);
            Assert.That(grid.OnRadius(new IntegerCoordinate<int>(0, 0), 2), Is.True);
            Assert.That(grid.OnRadius(center, 1), Is.False);
        });

        Assert.That(grid.Outline.Distinct().Count(), Is.EqualTo(16));
    }

    [Test]
    public void ApplyAndReplace_ModifyValues()
    {
        var grid = new ArrayGrid<int>(2, 2);
        grid[0, 0] = 1;
        grid[1, 0] = 2;
        grid[0, 1] = 3;
        grid[1, 1] = 4;

        grid.Apply(v => v + 1);
        grid.Replace(3, 0);
        grid.Replace(v => v > 4, -1);

        Assert.Multiple(() =>
        {
            Assert.That(grid[0, 0], Is.EqualTo(2));
            Assert.That(grid[1, 0], Is.EqualTo(0));
            Assert.That(grid[1, 1], Is.EqualTo(-1));
        });
    }

    [Test]
    public void RowColumnAndRetrieveDirection_ReturnSequences()
    {
        var grid = CreateSampleGrid();

        CollectionAssert.AreEqual(new[] { 1, 2, 3 }, grid.Row(0));
        CollectionAssert.AreEqual(new[] { 1, 4, 7 }, grid.Column(0));

        var fromCenterNorth = grid.RetrieveDirection(new IntegerCoordinate<int>(1, 1), Direction.North).ToArray();
        CollectionAssert.AreEqual(new[] { 5, 8 }, fromCenterNorth);

        var section = grid.RetrieveSection(new IntegerCoordinate<int>(1, 1), 2, 2).ToArray();
        CollectionAssert.AreEqual(new[] { 5, 6, 8, 9 }, section);
    }

    [Test]
    public void ContainsAndFill_SetValuesWithinBounds()
    {
        var grid = new ArrayGrid<int>(3, 3, 0);
        Assert.Multiple(() =>
        {
            Assert.That(grid.Contains(1, 1), Is.True);
            Assert.That(grid.Contains(new IntegerCoordinate<int>(-1, 0)), Is.False);
        });

        grid.Fill(2);
        Assert.That(grid[2, 2], Is.EqualTo(2));

        grid.Fill(new IntegerCoordinate<int>(1, 1), 2, 2, 5);
        Assert.Multiple(() =>
        {
            Assert.That(grid[1, 1], Is.EqualTo(5));
            Assert.That(grid[2, 2], Is.EqualTo(5));
            Assert.That(grid[0, 0], Is.EqualTo(2));
        });
    }

    [Test]
    public void FindHelpers_LocateCells()
    {
        var grid = CreateSampleGrid();

        Assert.Multiple(() =>
        {
            Assert.That(grid.Find(v => v == 5), Is.EqualTo(new IntegerCoordinate<int>(1, 1)));
            Assert.That(grid.FindOrNull(v => v == 99), Is.Null);
            CollectionAssert.AreEquivalent(
                new[] { new IntegerCoordinate<int>(0, 0), new IntegerCoordinate<int>(1, 0) },
                grid.FindAll(v => v <= 2));
        });
    }

    [Test]
    public void Flip_ReturnsMirroredGrid()
    {
        var grid = CreateSampleGrid();

        var flippedX = grid.Flip(Axis.X);
        var flippedY = grid.Flip(Axis.Y);

        Assert.Multiple(() =>
        {
            Assert.That(flippedX[0, 0], Is.EqualTo(grid[2, 0]));
            Assert.That(flippedY[0, 0], Is.EqualTo(grid[0, 2]));
            Assert.That(grid[0, 0], Is.EqualTo(1)); // original untouched
        });
    }

    [Test]
    public void CoordinatesAndSections_EnumerateCorrectly()
    {
        var grid = new ArrayGrid<int>(2, 2);
        Assert.That(grid.Coordinates.Count(), Is.EqualTo(4));

        var sectionCoords = grid.SectionCoordinates(new IntegerCoordinate<int>(1, 0), 2, 2).ToArray();
        CollectionAssert.AreEqual(new[] { new IntegerCoordinate<int>(1, 0), new IntegerCoordinate<int>(1, 1) }, sectionCoords);

        var outline = grid.Outline.ToArray();
        CollectionAssert.AreEquivalent(
            new[]
            {
                new IntegerCoordinate<int>(0, 0),
                new IntegerCoordinate<int>(0, 1),
                new IntegerCoordinate<int>(1, 0),
                new IntegerCoordinate<int>(1, 1)
            },
            outline);
    }

    [Test]
    public void ResizeCopyAndSection_CreateIndependentGrids()
    {
        var grid = CreateSampleGrid();

        var copy = grid.Copy();
        var resized = grid.Resize(2, 2);
        var section = grid.Section(new IntegerCoordinate<int>(1, 1), 2, 2);
        var sameSize = grid.OfSameSize();

        Assert.Multiple(() =>
        {
            Assert.That(copy[1, 1], Is.EqualTo(grid[1, 1]));
            Assert.That(resized.Width, Is.EqualTo(2));
            Assert.That(resized.Height, Is.EqualTo(2));
            Assert.That(section[0, 0], Is.EqualTo(grid[1, 1]));
            Assert.That(sameSize.Width, Is.EqualTo(grid.Width));
            Assert.That(sameSize.Height, Is.EqualTo(grid.Height));
        });
    }

    [Test]
    public void AsSearchSource_ProvidesNeighbours()
    {
        var grid = new ArrayGrid<int>(2, 1);
        grid[0, 0] = 1;
        grid[1, 0] = 2;

        var source = grid.AsSearchSource<int>();
        var element = source.ToElement(new IntegerCoordinate<int>(0, 0));
        var neighbours = source.GetNeighbours(element).ToArray();

        Assert.Multiple(() =>
        {
            Assert.That(neighbours.Length, Is.EqualTo(1));
            Assert.That(neighbours[0].Cost, Is.EqualTo(1));
            Assert.That(neighbours[0].Element.Value, Is.EqualTo(2));
        });
    }
}
