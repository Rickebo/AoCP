using System.Numerics;
using Lib.Geometry;

namespace Lib.Grids;

public class ArrayGrid<TValue> : IGrid<TValue, IntegerCoordinate<int>, int>
{
    public int Width => _values.GetLength(0);
    public int Height => _values.GetLength(1);
    public int Size => Width * Height;

    public IntegerCoordinate<int> BottomLeft => new(0, 0);
    public IntegerCoordinate<int> BottomRight => new(Width - 1, 0);
    public IntegerCoordinate<int> TopLeft => new(0, Height - 1);
    public IntegerCoordinate<int> TopRight => new(Width - 1, Height - 1);

    public IEnumerable<IntegerCoordinate<int>> LeftSide => BottomLeft.MoveTo(TopLeft);
    public IEnumerable<IntegerCoordinate<int>> TopSide => TopLeft.MoveTo(TopRight);
    public IEnumerable<IntegerCoordinate<int>> RightSide => TopRight.MoveTo(BottomRight);
    public IEnumerable<IntegerCoordinate<int>> BottomSide => BottomRight.MoveTo(BottomLeft);

    private readonly TValue[,] _values;

    public ArrayGrid(int width, int height)
    {
        _values = new TValue[width, height];
    } 

    public ArrayGrid(TValue[,] values)
    {
        _values = values;
    }

    public ArrayGrid(int width, int height, TValue initialValue)
    {
        _values = new TValue[width, height];
        for (var y = 0; y < height; y++)
            for (var x = 0; x < width; x++)
                _values[x, y] = initialValue;
    }

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

    public TValue this[IntegerCoordinate<int> coordinate]
    {
        get => _values[coordinate.X, coordinate.Y];
        set => _values[coordinate.X, coordinate.Y] = value;
    }

    public TValue this[int x, int y]
    {
        get => _values[x, y];
        set => _values[x, y] = value;
    }

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

    public bool OnOutline(IntegerCoordinate<int> coordinate) =>
        (coordinate.X == 0 || coordinate.X == Width - 1) || (coordinate.Y == 0 || coordinate.Y == Height - 1);

    public void Apply(Func<TValue, TValue> modifier)
    {
        foreach (var coordinate in Coordinates)
            this[coordinate] = modifier(this[coordinate]);
    }

    public void Replace(TValue from, TValue to) =>
        Replace(x => x != null ? x.Equals(from) : to == null, to);

    public void Replace(Func<TValue, bool> predicate, TValue value) =>
        Apply(v => predicate(v) ? value : v);

    public IEnumerable<TValue> Row(int y)
    {
        for (var x = 0; x < Width; x++)
            yield return _values[x, y];
    }

    public IEnumerable<TValue> Column(int x)
    {
        for (var y = 0; y < Height; y++)
            yield return _values[x, y];
    }

    public IEnumerable<TValue> RetrieveDirection(IntegerCoordinate<int> pos, Direction dir, int count = int.MaxValue)
    {
        while (Contains(pos) && count-- > 0)
        {
            yield return _values[pos.X, pos.Y];
            pos = pos.Move(dir);
        }
    }

    public IEnumerable<TValue> RetrieveSection(IntegerCoordinate<int> pos, int width, int height)
    {
        for (int y = pos.Y; y < pos.Y + height && y < Height; y++)
            for (int x = pos.X; x < pos.X + width && x < Width; x++)
                yield return _values[x, y];
    }

    public bool Contains(int x, int y) =>
        x >= 0 && x < Width && y >= 0 && y < Height;

    public bool Contains(IntegerCoordinate<int> coordinate) =>
        Contains(coordinate.X, coordinate.Y);

    public void Fill(TValue value) =>
        Fill(IntegerCoordinate<int>.Zero, Width, Height, value);

    public void Fill(IntegerCoordinate<int> coordinate, int width, int height, TValue value)
    {
        var maxY = System.Math.Min(coordinate.Y + height, Height);
        var maxX = System.Math.Min(coordinate.X + width, Width);

        // Fill within the requested rectangle without spilling outside the grid bounds.
        for (var y = coordinate.Y; y < maxY; y++)
            for (var x = coordinate.X; x < maxX; x++)
                _values[x, y] = value;
    }

    public IntegerCoordinate<int> Find(Func<TValue, bool> predicate) =>
        FindOrNull(predicate) ??
        throw new Exception("Found no grid cell matching predicate.");

    public IntegerCoordinate<int>? FindOrNull(Func<TValue, bool> predicate)
    {
        foreach (var coordinate in Coordinates)
            if (predicate(this[coordinate]))
                return coordinate;

        return null;
    }

    public IEnumerable<IntegerCoordinate<int>> FindAll(Func<TValue, bool> predicate) =>
        Coordinates.Where(coordinate => predicate(this[coordinate]));

    public virtual ArrayGrid<TValue> Flip(Axis axis) =>
        Flip(values => new ArrayGrid<TValue>(values), axis);

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

    public IEnumerable<IntegerCoordinate<int>> Outline =>
        LeftSide.Concat(RightSide).Concat(TopSide).Concat(BottomSide);

    public IEnumerable<IntegerCoordinate<int>> Coordinates =>
        Enumerable.Range(0, Height).SelectMany(
            y => Enumerable.Range(0, Width).Select(
                x => new IntegerCoordinate<int>(x, y)
            )
        );

    public IEnumerable<IntegerCoordinate<int>> SectionCoordinates(IntegerCoordinate<int> origin, int width, int height)
    {
        return Enumerable.Range(origin.Y, System.Math.Min(Height - origin.Y, height)).SelectMany(
            y => Enumerable.Range(origin.X, System.Math.Min(Width - origin.X, width)).Select(
                x => new IntegerCoordinate<int>(x, y)
            )
        );
    }
        
    public ArrayGrid<TValue> OfSameSize() => new(Width, Height);

    public ArrayGrid<TValue> Copy() => Resize(Width, Height);

    public ArrayGrid<TValue> Resize(int width, int height) => Section(IntegerCoordinate<int>.Zero, width, height);

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


