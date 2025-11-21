using Lib.Coordinate;

namespace Lib.Grid;

public class ArrayGrid<TValue> : IGrid<TValue, IntegerCoordinate<int>, int>
{
    private readonly TValue[,] _values;

    public int Width => _values.GetLength(0);
    public int Height => _values.GetLength(1);

    public ArrayGrid(int width, int height)
    {
        _values = new TValue[width, height];
    } 

    public ArrayGrid(TValue[,] values)
    {
        _values = values;
    }

    public ArrayGrid(IEnumerable<IEnumerable<TValue>> rows, int width, int height)
    {
        _values = new TValue[width, height];
        var y = 0;
        foreach (var row in rows)
        {
            var x = 0;
            foreach (var cell in row)
            {
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
            var xDir = Math.Sign(x.End.Value - x.Start.Value);
            var yDir = Math.Sign(y.End.Value - y.Start.Value);
            for (var cx = x.Start.Value; cx < x.End.Value; cx += xDir)
            {
                for (var cy = y.Start.Value; cy < y.End.Value; cy += yDir)
                {
                    yield return _values[cx, cy];
                }
            }
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

    public void Apply(Func<TValue, TValue> modifier)
    {
        foreach (var coordinate in Coordinates)
            this[coordinate] = modifier(this[coordinate]);
    }

    public void Replace(TValue from, TValue to) => Replace(x => x != null ? x.Equals(from) : to == null, to);

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

    public IEnumerable<TValue> RetrieveDirection(IntegerCoordinate<int> pos, Direction dir, int count)
    {
        while (Contains(pos) && count-- > 0)
        {
            yield return _values[pos.X, pos.Y];
            pos = pos.Move(dir);
        }
    }

    public bool Contains(IntegerCoordinate<int> coordinate) =>
        coordinate.X >= 0 &&
        coordinate.X < Width &&
        coordinate.Y >= 0 &&
        coordinate.Y < Height;

    public void Fill(
        IntegerCoordinate<int> coordinate,
        int width,
        int height,
        TValue value
    )
    {
        for (var y = coordinate.Y; y < height; y++)
        {
            for (var x = coordinate.X; x < width; x++)
            {
                _values[x, y] = value;
            }
        }
    }

    public IntegerCoordinate<int> Find(Func<TValue, bool> predicate) =>
        FindOrNull(predicate) ??
        throw new Exception("Found no grid cell matching predicate.");

    public IntegerCoordinate<int>? FindOrNull(Func<TValue, bool> predicate)
    {
        foreach (var coordinate in Coordinates)
        {
            if (predicate(this[coordinate]))
                return coordinate;
        }

        return null;
    }

    public IEnumerable<IntegerCoordinate<int>> FindAll(Func<TValue, bool> predicate) =>
        Coordinates.Where(coordinate => predicate(this[coordinate]));

    public virtual ArrayGrid<TValue> FlipX() =>
        FlipX(values => new ArrayGrid<TValue>(values));

    public virtual ArrayGrid<TValue> FlipY() =>
        FlipY(values => new ArrayGrid<TValue>(values));

    protected virtual TGrid FlipX<TGrid>(Func<TValue[,], TGrid> constructor)
        where TGrid : ArrayGrid<TValue>
    {
        var newGrid = new TValue[Width, Height];
        var mx = Width - 1;

        for (var x = 0; x < Width; x++)
        {
            for (var y = 0; y < Height; y++)
            {
                newGrid[x, y] = _values[mx - x, y];
            }
        }

        return constructor(newGrid);
    }

    protected virtual TGrid FlipY<TGrid>(Func<TValue[,], TGrid> constructor)
        where TGrid : ArrayGrid<TValue>
    {
        var newGrid = new TValue[Width, Height];
        var my = Height - 1;

        for (var x = 0; x < Width; x++)
        {
            for (var y = 0; y < Height; y++)
            {
                newGrid[x, y] = _values[x, my - y];
            }
        }

        return constructor(newGrid);
    }

    public IntegerCoordinate<int> BottomLeft => new(0, 0);
    public IntegerCoordinate<int> BottomRight => new(Width - 1, 0);
    public IntegerCoordinate<int> TopLeft => new(0, Height - 1);
    public IntegerCoordinate<int> TopRight => new(Width - 1, Height - 1);
    public int Size => Width * Height;
    public IEnumerable<IntegerCoordinate<int>> Left => BottomLeft.MoveTo(TopLeft);
    public IEnumerable<IntegerCoordinate<int>> Top => TopLeft.MoveTo(TopRight);
    public IEnumerable<IntegerCoordinate<int>> Right => TopRight.MoveTo(BottomRight);
    public IEnumerable<IntegerCoordinate<int>> Bottom => BottomRight.MoveTo(BottomLeft);

    public IEnumerable<IntegerCoordinate<int>> Outline =>
        Left.Concat(Right).Concat(Top).Concat(Bottom);

    public IEnumerable<IntegerCoordinate<int>> Coordinates =>
        Enumerable.Range(0, Height)
            .SelectMany(
                y => Enumerable.Range(0, Width)
                    .Select(x => new IntegerCoordinate<int>(x, y))
            );

    public ArrayGrid<TValue> OfSameSize() => new(Width, Height);

    public ArrayGrid<TValue> Copy() => Resize(Width, Height);

    public ArrayGrid<TValue> Resize(int width, int height) =>
        Section(
            IntegerCoordinate<int>.Zero,
            width,
            height
        );

    public ArrayGrid<TValue> Section(IntegerCoordinate<int> origin, int width, int height)
    {
        var newHeight = Math.Min(Height, height) - origin.Y;
        var newWidth = Math.Min(Width, width) - origin.X;
        var newValues = new TValue[newHeight, newWidth];

        for (var y = origin.Y; y < newHeight; y++)
        {
            for (var x = origin.X; x < newWidth; x++)
            {
                newValues[y, x] = _values[y, x];
            }
        }

        return new ArrayGrid<TValue>(newValues);
    }
}