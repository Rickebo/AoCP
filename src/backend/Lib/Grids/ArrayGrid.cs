using System.Numerics;
using Lib.Geometry;

namespace Lib.Grids;

/// <summary>
/// Finite two-dimensional grid backed by a rectangular array.
/// </summary>
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
    /// Gets the total number of cells in the grid.
    /// </summary>
    public int Size => Width * Height;

    /// <summary>
    /// Gets the coordinate of the bottom-left corner.
    /// </summary>
    public IntegerCoordinate<int> BottomLeft => new(0, 0);

    /// <summary>
    /// Gets the coordinate of the bottom-right corner.
    /// </summary>
    public IntegerCoordinate<int> BottomRight => new(Width - 1, 0);

    /// <summary>
    /// Gets the coordinate of the top-left corner.
    /// </summary>
    public IntegerCoordinate<int> TopLeft => new(0, Height - 1);

    /// <summary>
    /// Gets the coordinate of the top-right corner.
    /// </summary>
    public IntegerCoordinate<int> TopRight => new(Width - 1, Height - 1);

    /// <summary>
    /// Enumerates the coordinates along the left grid edge from bottom to top.
    /// </summary>
    public IEnumerable<IntegerCoordinate<int>> LeftSide => BottomLeft.MoveTo(TopLeft);

    /// <summary>
    /// Enumerates the coordinates along the top grid edge from left to right.
    /// </summary>
    public IEnumerable<IntegerCoordinate<int>> TopSide => TopLeft.MoveTo(TopRight);

    /// <summary>
    /// Enumerates the coordinates along the right grid edge from top to bottom.
    /// </summary>
    public IEnumerable<IntegerCoordinate<int>> RightSide => TopRight.MoveTo(BottomRight);

    /// <summary>
    /// Enumerates the coordinates along the bottom grid edge from right to left.
    /// </summary>
    public IEnumerable<IntegerCoordinate<int>> BottomSide => BottomRight.MoveTo(BottomLeft);

    private readonly TValue[,] _values;

    /// <summary>
    /// Initializes an empty grid with the specified dimensions.
    /// </summary>
    /// <param name="width">Grid width.</param>
    /// <param name="height">Grid height.</param>
    public ArrayGrid(int width, int height)
    {
        _values = new TValue[width, height];
    } 

    /// <summary>
    /// Initializes a grid backed by an existing value array.
    /// </summary>
    /// <param name="values">Backing 2D array. Ownership is retained by the grid.</param>
    public ArrayGrid(TValue[,] values)
    {
        _values = values;
    }

    /// <summary>
    /// Initializes a grid with all cells set to the provided initial value.
    /// </summary>
    /// <param name="width">Grid width.</param>
    /// <param name="height">Grid height.</param>
    /// <param name="initialValue">Value used to populate each cell.</param>
    public ArrayGrid(int width, int height, TValue initialValue)
    {
        _values = new TValue[width, height];
        for (var y = 0; y < height; y++)
            for (var x = 0; x < width; x++)
                _values[x, y] = initialValue;
    }

    /// <summary>
    /// Initializes a grid using a set of row sequences, truncating to the specified dimensions.
    /// </summary>
    /// <param name="rows">Row values, enumerated from top to bottom.</param>
    /// <param name="width">Target width; extra values are ignored.</param>
    /// <param name="height">Target height; extra rows are ignored.</param>
    public ArrayGrid(IEnumerable<IEnumerable<TValue>> rows, int width, int height)
    {
        _values = new TValue[width, height];
        int x = 0, y = 0;
        foreach (var row in rows)
        {
            if (y >= Height)
                break;

            x = 0;
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
    /// Enumerates values within the specified x and y ranges in order.
    /// </summary>
    /// <param name="x">Inclusive/exclusive range for x-coordinates.</param>
    /// <param name="y">Inclusive/exclusive range for y-coordinates.</param>
    /// <returns>An enumerable over the selected cells.</returns>
    public IEnumerable<TValue> this[Range x, Range y]
    {
        get
        {
            var xDir = System.Math.Sign(x.End.Value - x.Start.Value);
            var yDir = System.Math.Sign(y.End.Value - y.Start.Value);
            for (var cx = x.Start.Value; cx < x.End.Value; cx += xDir)
                for (var cy = y.Start.Value; cy < y.End.Value; cy += yDir)
                    if (Contains(cx, cy))
                        yield return _values[cx, cy];
        }
    }

    /// <summary>
    /// Gets or sets a value by integer coordinate.
    /// </summary>
    /// <param name="coordinate">Coordinate to access.</param>
    public TValue this[IntegerCoordinate<int> coordinate]
    {
        get => _values[coordinate.X, coordinate.Y];
        set => _values[coordinate.X, coordinate.Y] = value;
    }

    /// <summary>
    /// Gets or sets a value by x and y indices.
    /// </summary>
    /// <param name="x">X coordinate.</param>
    /// <param name="y">Y coordinate.</param>
    public TValue this[int x, int y]
    {
        get => _values[x, y];
        set => _values[x, y] = value;
    }

    /// <summary>
    /// Determines whether a coordinate lies exactly on a square outline at the given radius from grid center.
    /// </summary>
    /// <param name="coordinate">Coordinate to test.</param>
    /// <param name="radius">Radius from the grid center.</param>
    /// <returns><c>true</c> when the coordinate is on the outline; otherwise <c>false</c>.</returns>
    /// <exception cref="ArgumentException">Thrown when the radius exceeds the grid bounds.</exception>
    public bool OnRadius(IntegerCoordinate<int> coordinate, int radius)
    {
        // Radius must be within grid
        if (radius > System.Math.Min(Width, Height))
            throw new ArgumentException("Radius can not extend outside of grid.");

        // Grid mid points
        int midWidth = Width / 2;
        int midHeight= Height / 2;

        return (coordinate.X == midWidth - radius || coordinate.X == midWidth + radius) 
            && (coordinate.Y == midHeight - radius || coordinate.Y == midHeight + radius);
    }

    /// <summary>
    /// Determines whether a coordinate is on the outer border of the grid.
    /// </summary>
    /// <param name="coordinate">Coordinate to evaluate.</param>
    /// <returns><c>true</c> if the coordinate lies on any edge; otherwise <c>false</c>.</returns>
    public bool OnOutline(IntegerCoordinate<int> coordinate) =>
        (coordinate.X == 0 || coordinate.X == Width - 1) || (coordinate.Y == 0 || coordinate.Y == Height - 1);

    /// <summary>
    /// Applies a transformation to every cell value in the grid.
    /// </summary>
    /// <param name="modifier">Function that returns the new value for the existing one.</param>
    public void Apply(Func<TValue, TValue> modifier)
    {
        foreach (var coordinate in Coordinates)
            this[coordinate] = modifier(this[coordinate]);
    }

    /// <summary>
    /// Replaces all instances of <paramref name="from"/> with <paramref name="to"/>.
    /// </summary>
    /// <param name="from">Value to replace.</param>
    /// <param name="to">Replacement value.</param>
    public void Replace(TValue from, TValue to) =>
        Replace(x => x != null ? x.Equals(from) : to == null, to);

    /// <summary>
    /// Replaces all values matching the predicate with the supplied value.
    /// </summary>
    /// <param name="predicate">Predicate indicating which cells to replace.</param>
    /// <param name="value">Replacement value.</param>
    public void Replace(Func<TValue, bool> predicate, TValue value) =>
        Apply(v => predicate(v) ? value : v);

    /// <summary>
    /// Enumerates a single row of values.
    /// </summary>
    /// <param name="y">Row index.</param>
    /// <returns>Values from left to right on the given row.</returns>
    public IEnumerable<TValue> Row(int y)
    {
        for (var x = 0; x < Width; x++)
            yield return _values[x, y];
    }

    /// <summary>
    /// Enumerates a single column of values.
    /// </summary>
    /// <param name="x">Column index.</param>
    /// <returns>Values from bottom to top on the given column.</returns>
    public IEnumerable<TValue> Column(int x)
    {
        for (var y = 0; y < Height; y++)
            yield return _values[x, y];
    }

    /// <summary>
    /// Retrieves values starting at a coordinate and stepping in the specified direction until out of bounds or a limit is reached.
    /// </summary>
    /// <param name="pos">Starting coordinate.</param>
    /// <param name="dir">Direction to move.</param>
    /// <param name="count">Maximum number of cells to return.</param>
    /// <returns>Sequence of values along the path.</returns>
    public IEnumerable<TValue> RetrieveDirection(IntegerCoordinate<int> pos, Direction dir, int count = int.MaxValue)
    {
        while (Contains(pos) && count-- > 0)
        {
            yield return _values[pos.X, pos.Y];
            pos = pos.Move(dir);
        }
    }

    /// <summary>
    /// Retrieves a rectangular subsection of values bounded by width and height starting at the supplied origin.
    /// </summary>
    /// <param name="pos">Bottom-left coordinate of the section.</param>
    /// <param name="width">Width of the section.</param>
    /// <param name="height">Height of the section.</param>
    /// <returns>Values inside the section in row-major order.</returns>
    public IEnumerable<TValue> RetrieveSection(IntegerCoordinate<int> pos, int width, int height)
    {
        for (int y = pos.Y; y < pos.Y + height && y < Height; y++)
            for (int x = pos.X; x < pos.X + width && x < Width; x++)
                yield return _values[x, y];
    }

    /// <summary>
    /// Determines whether the specified coordinate is inside the grid bounds.
    /// </summary>
    /// <param name="x">X coordinate.</param>
    /// <param name="y">Y coordinate.</param>
    /// <returns><c>true</c> if the coordinate is in bounds; otherwise <c>false</c>.</returns>
    public bool Contains(int x, int y) =>
        x >= 0 && x < Width && y >= 0 && y < Height;

    /// <summary>
    /// Determines whether the specified coordinate is inside the grid bounds.
    /// </summary>
    /// <param name="coordinate">Coordinate to check.</param>
    /// <returns><c>true</c> if the coordinate is in bounds; otherwise <c>false</c>.</returns>
    public bool Contains(IntegerCoordinate<int> coordinate) =>
        Contains(coordinate.X, coordinate.Y);

    /// <summary>
    /// Fills the entire grid with the given value.
    /// </summary>
    /// <param name="value">Value to assign to every cell.</param>
    public void Fill(TValue value) =>
        Fill(IntegerCoordinate<int>.Zero, Width, Height, value);

    /// <summary>
    /// Fills a rectangular region of the grid with the given value.
    /// </summary>
    /// <param name="coordinate">Bottom-left coordinate of the region.</param>
    /// <param name="width">Width of the region.</param>
    /// <param name="height">Height of the region.</param>
    /// <param name="value">Value to assign.</param>
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
    /// Finds the first coordinate that satisfies the predicate or throws if none exist.
    /// </summary>
    /// <param name="predicate">Predicate to test each cell value.</param>
    /// <returns>Coordinate of the first matching cell.</returns>
    /// <exception cref="Exception">Thrown when no matching cell is found.</exception>
    public IntegerCoordinate<int> Find(Func<TValue, bool> predicate) =>
        FindOrNull(predicate) ??
        throw new Exception("Found no grid cell matching predicate.");

    /// <summary>
    /// Finds the first coordinate that satisfies the predicate or returns <c>null</c> if none exist.
    /// </summary>
    /// <param name="predicate">Predicate to test each cell value.</param>
    /// <returns>The coordinate of the first matching cell, or <c>null</c> when not found.</returns>
    public IntegerCoordinate<int>? FindOrNull(Func<TValue, bool> predicate)
    {
        foreach (var coordinate in Coordinates)
            if (predicate(this[coordinate]))
                return coordinate;

        return null;
    }

    /// <summary>
    /// Returns all coordinates whose values satisfy the predicate.
    /// </summary>
    /// <param name="predicate">Predicate to filter cell values.</param>
    /// <returns>Coordinates of all matching cells.</returns>
    public IEnumerable<IntegerCoordinate<int>> FindAll(Func<TValue, bool> predicate) =>
        Coordinates.Where(coordinate => predicate(this[coordinate]));

    /// <summary>
    /// Creates a flipped copy of the grid along the specified axes.
    /// </summary>
    /// <param name="axis">Axis or axes to flip around.</param>
    /// <returns>A new grid containing mirrored values.</returns>
    public virtual ArrayGrid<TValue> Flip(Axis axis) =>
        Flip(values => new ArrayGrid<TValue>(values), axis);

    /// <summary>
    /// Flips the grid along the specified axes using a custom constructor for the resulting type.
    /// </summary>
    /// <param name="constructor">Function that constructs the concrete grid type.</param>
    /// <param name="axis">Axis or axes to flip around.</param>
    /// <typeparam name="TGrid">Concrete grid type to return.</typeparam>
    /// <returns>A new grid instance containing mirrored values.</returns>
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
    /// Enumerates all coordinates along the grid outline.
    /// </summary>
    public IEnumerable<IntegerCoordinate<int>> Outline =>
        LeftSide.Concat(RightSide).Concat(TopSide).Concat(BottomSide);

    /// <summary>
    /// Enumerates all coordinates within the grid in row-major order.
    /// </summary>
    public IEnumerable<IntegerCoordinate<int>> Coordinates =>
        Enumerable.Range(0, Height).SelectMany(
            y => Enumerable.Range(0, Width).Select(
                x => new IntegerCoordinate<int>(x, y)
            )
        );

    /// <summary>
    /// Enumerates coordinates inside a subsection defined by origin and dimensions.
    /// </summary>
    /// <param name="origin">Bottom-left coordinate of the subsection.</param>
    /// <param name="width">Width of the subsection.</param>
    /// <param name="height">Height of the subsection.</param>
    /// <returns>All coordinates inside the subsection that remain within bounds.</returns>
    public IEnumerable<IntegerCoordinate<int>> SectionCoordinates(IntegerCoordinate<int> origin, int width, int height)
    {
        return Enumerable.Range(origin.Y, System.Math.Min(Height - origin.Y, height)).SelectMany(
            y => Enumerable.Range(origin.X, System.Math.Min(Width - origin.X, width)).Select(
                x => new IntegerCoordinate<int>(x, y)
            )
        );
    }
        
    /// <summary>
    /// Creates an empty grid with the same dimensions.
    /// </summary>
    /// <returns>A new grid with identical width and height.</returns>
    public ArrayGrid<TValue> OfSameSize() => new(Width, Height);

    /// <summary>
    /// Creates a full copy of the grid values.
    /// </summary>
    /// <returns>A new grid containing all cell values.</returns>
    public ArrayGrid<TValue> Copy() => Resize(Width, Height);

    /// <summary>
    /// Copies the grid into a new instance resized to the provided dimensions.
    /// </summary>
    /// <param name="width">Target width.</param>
    /// <param name="height">Target height.</param>
    /// <returns>A new grid containing values from the original up to the requested size.</returns>
    public ArrayGrid<TValue> Resize(int width, int height) => Section(IntegerCoordinate<int>.Zero, width, height);

    /// <summary>
    /// Copies a subsection of the grid into a new grid of the given size.
    /// </summary>
    /// <param name="origin">Bottom-left coordinate of the subsection.</param>
    /// <param name="width">Width of the subsection.</param>
    /// <param name="height">Height of the subsection.</param>
    /// <returns>A new grid populated with the selected region.</returns>
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
    /// Wraps the grid in a search source suitable for graph algorithms.
    /// </summary>
    /// <typeparam name="TCost">Numeric type for traversal cost.</typeparam>
    /// <param name="isWalkable">Predicate determining if a cell can be visited.</param>
    /// <param name="costSelector">Selector that produces the cost between two adjacent cells.</param>
    /// <param name="includeDiagonals">When <c>true</c>, diagonal neighbours are considered.</param>
    /// <returns>A <see cref="GridSearchSource{TValue, TCost}"/> adapter.</returns>
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


