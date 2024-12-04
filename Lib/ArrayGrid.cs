namespace Lib;

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
        int y = 0;
        foreach (var row in rows)
        {
            int x = 0;
            foreach (var cell in row)
            {
                _values[x, y] = cell;
                x++;
            }

            y++;
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

    public bool Contains(IntegerCoordinate<int> coordinate)
    {
        return coordinate.X >= 0 && coordinate.X < Width &&
        coordinate.Y >= 0 && coordinate.Y < Height;
    }

    public void Fill(
        IntegerCoordinate<int> coordinate,
        int width,
        int height,
        TValue value
    )
    {
        for (int y = coordinate.Y; y < height; y++)
        {
            for (int x = coordinate.X; x < width; x++)
            {
                _values[x, y] = value;
            }
        }
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
        int newHeight = Math.Min(Height, height) - origin.Y;
        int newWidth = Math.Min(Width, width) - origin.X;
        var newValues = new TValue[newHeight, newWidth];

        for (int y = origin.Y; y < newHeight; y++)
        {
            for (int x = origin.X; x < newWidth; x++)
            {
                newValues[y, x] = _values[y, x];
            }
        }

        return new ArrayGrid<TValue>(newValues);
    }
}