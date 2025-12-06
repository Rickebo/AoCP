using System.Collections.Generic;
using System.Numerics;
using Lib.Geometry;

namespace Lib.Grids;

/// <summary>
/// Finite 2D grid backed by a rectangular array with origin at the bottom-left corner.
/// </summary>
/// <typeparam name="TValue">Cell value type.</typeparam>
public class ArrayGrid<TValue> : IGrid<TValue, IntegerCoordinate<int>, int>
{
    /// <summary>
    /// Gets the grid width.
    /// </summary>
    public int Width => _values.GetLength(0);

    /// <summary>
    /// Gets the grid height.
    /// </summary>
    public int Height => _values.GetLength(1);

    /// <summary>
    /// Gets the total number of cells.
    /// </summary>
    public int Size => Width * Height;

    /// <summary>
    /// Coordinate of the bottom-left cell.
    /// </summary>
    public IntegerCoordinate<int> BottomLeft => new(0, 0);

    /// <summary>
    /// Coordinate of the bottom-right cell.
    /// </summary>
    public IntegerCoordinate<int> BottomRight => new(Width - 1, 0);

    /// <summary>
    /// Coordinate of the top-left cell.
    /// </summary>
    public IntegerCoordinate<int> TopLeft => new(0, Height - 1);

    /// <summary>
    /// Coordinate of the top-right cell.
    /// </summary>
    public IntegerCoordinate<int> TopRight => new(Width - 1, Height - 1);

    /// <summary>
    /// Coordinates along the left edge from bottom to top (exclusive of the final point).
    /// </summary>
    public IEnumerable<IntegerCoordinate<int>> LeftSide => BottomLeft.MoveTo(TopLeft);

    /// <summary>
    /// Coordinates along the top edge from left to right (exclusive of the final point).
    /// </summary>
    public IEnumerable<IntegerCoordinate<int>> TopSide => TopLeft.MoveTo(TopRight);

    /// <summary>
    /// Coordinates along the right edge from top to bottom (exclusive of the final point).
    /// </summary>
    public IEnumerable<IntegerCoordinate<int>> RightSide => TopRight.MoveTo(BottomRight);

    /// <summary>
    /// Coordinates along the bottom edge from right to left (exclusive of the final point).
    /// </summary>
    public IEnumerable<IntegerCoordinate<int>> BottomSide => BottomRight.MoveTo(BottomLeft);

    private readonly TValue[,] _values;

    /// <summary>
    /// Initializes a grid of the given size with default values.
    /// </summary>
    /// <param name="width">Grid width.</param>
    /// <param name="height">Grid height.</param>
    public ArrayGrid(int width, int height)
    {
        _values = new TValue[width, height];
    }

    /// <summary>
    /// Initializes a grid using an existing value array.
    /// </summary>
    /// <param name="values">Backing array; dimensions determine grid size.</param>
    public ArrayGrid(TValue[,] values)
    {
        _values = values;
    }

    /// <summary>
    /// Initializes a grid with all cells set to <paramref name="initialValue"/>.
    /// </summary>
    /// <param name="width">Grid width.</param>
    /// <param name="height">Grid height.</param>
    /// <param name="initialValue">Initial value for every cell.</param>
    public ArrayGrid(int width, int height, TValue initialValue)
    {
        _values = new TValue[width, height];
        for (var y = 0; y < height; y++)
            for (var x = 0; x < width; x++)
                _values[x, y] = initialValue;
    }

    /// <summary>
    /// Initializes a grid from enumerable rows.
    /// </summary>
    /// <param name="rows">Rows of values starting at the bottom.</param>
    /// <param name="width">Expected grid width.</param>
    /// <param name="height">Expected grid height.</param>
    public ArrayGrid(IEnumerable<IEnumerable<TValue>> rows, int width, int height)
    {
        _values = new TValue[width, height];
        int y = 0;
        foreach (var row in rows)
        {
            if (y >= Height)
                break;

            int x = 0;
            foreach (var cell in row)
            {
                if (x >= Width)
                    break;

                _values[x, y] = cell;
                x++;
            }
            y++;
        }
    }

    /// <summary>
    /// Enumerates values within the specified range selection.
    /// </summary>
    public IEnumerable<TValue> this[Range x, Range y]
    {
        get
        {
            var (startX, lengthX) = x.GetOffsetAndLength(Width);
            var (startY, lengthY) = y.GetOffsetAndLength(Height);
            var endX = startX + lengthX;
            var endY = startY + lengthY;

            for (var cx = startX; cx < endX; cx++)
                for (var cy = startY; cy < endY; cy++)
                    yield return _values[cx, cy];
        }
    }

    /// <summary>
    /// Gets or sets the value at the given coordinate.
    /// </summary>
    public TValue this[IntegerCoordinate<int> coordinate]
    {
        get => _values[coordinate.X, coordinate.Y];
        set => _values[coordinate.X, coordinate.Y] = value;
    }

    /// <summary>
    /// Gets or sets the value at the given indices.
    /// </summary>
    public TValue this[int x, int y]
    {
        get => _values[x, y];
        set => _values[x, y] = value;
    }

    /// <summary>
    /// Checks whether a coordinate lies on the outline of a square ring at the specified radius from the grid center.
    /// </summary>
    /// <param name="coordinate">Coordinate to check.</param>
    /// <param name="radius">Radius measured from the center cell.</param>
    /// <returns><see langword="true"/> if the coordinate lies on the ring.</returns>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when the radius extends outside the grid.</exception>
    public bool OnRadius(IntegerCoordinate<int> coordinate, int radius)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(radius, nameof(radius));

        var midWidth = Width / 2;
        var midHeight = Height / 2;

        var minX = midWidth - radius;
        var maxX = midWidth + radius;
        var minY = midHeight - radius;
        var maxY = midHeight + radius;

        if (minX < 0 || maxX >= Width || minY < 0 || maxY >= Height)
            throw new ArgumentOutOfRangeException(nameof(radius), "Radius can not extend outside of grid.");

        var onVerticalEdge = (coordinate.X == minX || coordinate.X == maxX) &&
                             coordinate.Y >= minY && coordinate.Y <= maxY;
        var onHorizontalEdge = (coordinate.Y == minY || coordinate.Y == maxY) &&
                               coordinate.X >= minX && coordinate.X <= maxX;

        return onVerticalEdge || onHorizontalEdge;
    }

    /// <summary>
    /// Checks whether a coordinate is on the outer border of the grid.
    /// </summary>
    public bool OnOutline(IntegerCoordinate<int> coordinate) =>
        (coordinate.X == 0 || coordinate.X == Width - 1) || (coordinate.Y == 0 || coordinate.Y == Height - 1);

    /// <summary>
    /// Applies a modifier to every cell in the grid.
    /// </summary>
    /// <param name="modifier">Function that receives the current value and returns the replacement.</param>
    public void Apply(Func<TValue, TValue> modifier)
    {
        foreach (var coordinate in Coordinates)
            this[coordinate] = modifier(this[coordinate]);
    }

    /// <summary>
    /// Replaces all instances of <paramref name="from"/> with <paramref name="to"/>.
    /// </summary>
    public void Replace(TValue from, TValue to) =>
        Replace(x => EqualityComparer<TValue>.Default.Equals(x, from), to);

    /// <summary>
    /// Replaces all values matching <paramref name="predicate"/> with <paramref name="value"/>.
    /// </summary>
    public void Replace(Func<TValue, bool> predicate, TValue value) =>
        Apply(v => predicate(v) ? value : v);

    /// <summary>
    /// Enumerates the values in a given row (bottom = 0).
    /// </summary>
    public IEnumerable<TValue> Row(int y)
    {
        for (var x = 0; x < Width; x++)
            yield return _values[x, y];
    }

    /// <summary>
    /// Enumerates the values in a given column (left = 0).
    /// </summary>
    public IEnumerable<TValue> Column(int x)
    {
        for (var y = 0; y < Height; y++)
            yield return _values[x, y];
    }

    /// <summary>
    /// Enumerates values starting at <paramref name="pos"/> and moving in <paramref name="dir"/> until outside the grid or count exhausted.
    /// </summary>
    public IEnumerable<TValue> RetrieveDirection(IntegerCoordinate<int> pos, Direction dir, int count = int.MaxValue)
    {
        while (Contains(pos) && count-- > 0)
        {
            yield return _values[pos.X, pos.Y];
            pos = pos.Move(dir);
        }
    }

    /// <summary>
    /// Enumerates values within a rectangular section anchored at <paramref name="pos"/>.
    /// </summary>
    public IEnumerable<TValue> RetrieveSection(IntegerCoordinate<int> pos, int width, int height)
    {
        var startX = System.Math.Max(0, pos.X);
        var startY = System.Math.Max(0, pos.Y);
        var endX = System.Math.Min(Width, pos.X + width);
        var endY = System.Math.Min(Height, pos.Y + height);

        for (int y = startY; y < endY; y++)
            for (int x = startX; x < endX; x++)
                yield return _values[x, y];
    }

    /// <summary>
    /// Determines whether the specified coordinate is inside grid bounds.
    /// </summary>
    public bool Contains(int x, int y) =>
        x >= 0 && x < Width && y >= 0 && y < Height;

    /// <summary>
    /// Determines whether the specified coordinate is inside grid bounds.
    /// </summary>
    public bool Contains(IntegerCoordinate<int> coordinate) =>
        Contains(coordinate.X, coordinate.Y);

    /// <summary>
    /// Fills the entire grid with the provided value.
    /// </summary>
    public void Fill(TValue value) =>
        Fill(IntegerCoordinate<int>.Zero, Width, Height, value);

    /// <summary>
    /// Fills a rectangular region with a value, clamped to grid bounds.
    /// </summary>
    public void Fill(IntegerCoordinate<int> coordinate, int width, int height, TValue value)
    {
        var maxY = System.Math.Min(coordinate.Y + height, Height);
        var maxX = System.Math.Min(coordinate.X + width, Width);

        // Fill within the requested rectangle without spilling outside the grid bounds.
        for (var y = coordinate.Y; y < maxY; y++)
            for (var x = coordinate.X; x < maxX; x++)
                _values[x, y] = value;
    }

    /// <summary>
    /// Finds the first coordinate whose value matches the predicate or throws if none exist.
    /// </summary>
    /// <exception cref="Exception">Thrown when no match is found.</exception>
    public IntegerCoordinate<int> Find(Func<TValue, bool> predicate) =>
        FindOrNull(predicate) ??
        throw new Exception("Found no grid cell matching predicate.");

    /// <summary>
    /// Finds the first coordinate whose value matches the predicate or returns <see langword="null"/>.
    /// </summary>
    public IntegerCoordinate<int>? FindOrNull(Func<TValue, bool> predicate)
    {
        foreach (var coordinate in Coordinates)
            if (predicate(this[coordinate]))
                return coordinate;

        return null;
    }

    /// <summary>
    /// Finds all coordinates matching the predicate.
    /// </summary>
    public IEnumerable<IntegerCoordinate<int>> FindAll(Func<TValue, bool> predicate) =>
        Coordinates.Where(coordinate => predicate(this[coordinate]));

    /// <summary>
    /// Returns a flipped copy of the grid across the specified axes.
    /// </summary>
    public virtual ArrayGrid<TValue> Flip(Axis axis) =>
        Flip(values => new ArrayGrid<TValue>(values), axis);

    /// <summary>
    /// Flips the grid using a custom constructor for derived types.
    /// </summary>
    protected virtual TGrid Flip<TGrid>(Func<TValue[,], TGrid> constructor, Axis axis) where TGrid : ArrayGrid<TValue>
    {
        // Flip and return new grid
        var newGrid = new TValue[Width, Height];
        for (var y = 0; y < Height; y++)
        {
            for (var x = 0; x < Width; x++)
            {
                var mappedX = axis.HasFlag(Axis.X) ? Width - 1 - x : x;
                var mappedY = axis.HasFlag(Axis.Y) ? Height - 1 - y : y;
                newGrid[x, y] = _values[mappedX, mappedY];
            }
        }

        return constructor(newGrid);
    }

    /// <summary>
    /// All coordinates along the grid outline.
    /// </summary>
    public IEnumerable<IntegerCoordinate<int>> Outline =>
        LeftSide.Concat(RightSide).Concat(TopSide).Concat(BottomSide);

    /// <summary>
    /// All coordinates in the grid (row-major from bottom to top).
    /// </summary>
    public IEnumerable<IntegerCoordinate<int>> Coordinates =>
        Enumerable.Range(0, Height).SelectMany(
            y => Enumerable.Range(0, Width).Select(
                x => new IntegerCoordinate<int>(x, y)
            )
        );

    /// <summary>
    /// Coordinates within a rectangular section limited by grid bounds.
    /// </summary>
    public IEnumerable<IntegerCoordinate<int>> SectionCoordinates(IntegerCoordinate<int> origin, int width, int height)
    {
        var widthCount = System.Math.Max(0, System.Math.Min(Width - origin.X, width));
        var heightCount = System.Math.Max(0, System.Math.Min(Height - origin.Y, height));

        if (widthCount == 0 || heightCount == 0)
            return [];

        return Enumerable.Range(origin.Y, heightCount).SelectMany(
            y => Enumerable.Range(origin.X, widthCount).Select(
                x => new IntegerCoordinate<int>(x, y)
            )
        );
    }
        
    /// <summary>
    /// Creates an empty grid with the same dimensions.
    /// </summary>
    public ArrayGrid<TValue> OfSameSize() => new(Width, Height);

    /// <summary>
    /// Creates a shallow copy of the grid.
    /// </summary>
    public ArrayGrid<TValue> Copy() => Resize(Width, Height);

    /// <summary>
    /// Resizes the grid by taking a section from the origin.
    /// </summary>
    public ArrayGrid<TValue> Resize(int width, int height) => Section(IntegerCoordinate<int>.Zero, width, height);

    /// <summary>
    /// Extracts a section of the grid starting at <paramref name="origin"/>.
    /// </summary>
    public ArrayGrid<TValue> Section(IntegerCoordinate<int> origin, int width, int height)
    {
        var sectionWidth = System.Math.Min(Width - origin.X, width);
        var sectionHeight = System.Math.Min(Height - origin.Y, height);
        var sectionValues = new TValue[sectionWidth, sectionHeight];

        for (var y = 0; y < sectionHeight; y++)
            for (var x = 0; x < sectionWidth; x++)
                sectionValues[x, y] = _values[origin.X + x, origin.Y + y];

        return new ArrayGrid<TValue>(sectionValues);
    }

    /// <summary>
    /// Creates a search source that wraps this grid for pathfinding algorithms.
    /// </summary>
    /// <typeparam name="TCost">Numeric type for traversal cost.</typeparam>
    /// <param name="isWalkable">Predicate to determine if a cell is traversable.</param>
    /// <param name="costSelector">Function to compute edge cost between two coordinates.</param>
    /// <param name="includeDiagonals">Whether diagonal movement is allowed.</param>
    /// <returns>A configured <see cref="GridSearchSource{TValue, TCost}"/>.</returns>
    public GridSearchSource<TValue, TCost> AsSearchSource<TCost>(
        Func<IntegerCoordinate<int>, TValue, bool>? isWalkable = null,
        Func<IntegerCoordinate<int>, IntegerCoordinate<int>, TValue, TValue, TCost>? costSelector = null,
        bool includeDiagonals = false
    ) where TCost : INumber<TCost> =>
        new(
            this,
            isWalkable ?? ((_, _) => true),
            costSelector ?? ((_, _, _, _) => TCost.One),
            includeDiagonals
        );
}


