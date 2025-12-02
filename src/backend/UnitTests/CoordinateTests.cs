using Lib.Coordinate;
using Lib.Enums;

namespace LibUnitTests;

[TestFixture]
internal class CoordinateTests
{
    [Test]
    public void Coordinate_Int_BasicMethods_And_Operators()
    {
        var a = new Coordinate<int>(1, -2);
        var b = new Coordinate<int>(3, 4);

        Assert.Multiple(() =>
        {
            Assert.That(a.X, Is.EqualTo(1));
            Assert.That(a.Y, Is.EqualTo(-2));
            Assert.That(Coordinate<int>.Zero, Is.EqualTo(new Coordinate<int>(0, 0)));
            Assert.That(Coordinate<int>.One, Is.EqualTo(new Coordinate<int>(1, 1)));
            Assert.That(Coordinate<int>.UnitX, Is.EqualTo(new Coordinate<int>(1, 0)));
            Assert.That(Coordinate<int>.UnitY, Is.EqualTo(new Coordinate<int>(0, 1)));
            Assert.That(a.GetStringX(), Is.EqualTo("1"));
            Assert.That(a.GetStringY(), Is.EqualTo("-2"));
        });

        var min = a.Min(b);
        var max = a.Max(b);
        Assert.Multiple(() =>
        {
            Assert.That(min, Is.EqualTo(new Coordinate<int>(1, -2)));
            Assert.That(max, Is.EqualTo(new Coordinate<int>(3, 4)));
        });

        var clamped = new Coordinate<int>(5, -5).Clamp(
            new Coordinate<int>(0, -2),
            new Coordinate<int>(4, 10));
        Assert.That(clamped, Is.EqualTo(new Coordinate<int>(4, -2)));

        var copySign = new Coordinate<int>(-2, 3)
            .CopySign(new Coordinate<int>(1, -1));
        Assert.That(copySign, Is.EqualTo(new Coordinate<int>(2, -3)));

        var abs = new Coordinate<int>(-2, -3).Abs();
        Assert.That(abs, Is.EqualTo(new Coordinate<int>(2, 3)));

        Assert.Multiple(() =>
        {
            Assert.That(a + b, Is.EqualTo(new Coordinate<int>(4, 2)));
            Assert.That(b - a, Is.EqualTo(new Coordinate<int>(2, 6)));
            Assert.That(a * 2, Is.EqualTo(new Coordinate<int>(2, -4)));
            Assert.That(b / 2, Is.EqualTo(new Coordinate<int>(1, 2)));
            Assert.That(a * b, Is.EqualTo(new Coordinate<int>(3, -8)));
            Assert.That(
                b / new Coordinate<int>(1, 2),
                Is.EqualTo(new Coordinate<int>(3, 2))
            );
        });
    }

    [Test]
    public void Coordinate_Int_ManhattanLength_DefaultInterfaceImplementation()
    {
        var coord = new Coordinate<int>(-2, 3);
        ICoordinate<Coordinate<int>, int> asInterface = coord;

        var length = asInterface.ManhattanLength();
        Assert.That(length, Is.EqualTo(1));
    }

    [Test]
    public void Coordinate_Int_Equality_Members()
    {
        var a = new Coordinate<int>(2, 3);
        var same = new Coordinate<int>(2, 3);
        var diff = new Coordinate<int>(2, 4);

        Assert.Multiple(() =>
        {
            Assert.That(a.Equals(same), Is.True);
            Assert.That(a.Equals((object)same), Is.True);
            Assert.That(a.Equals(diff), Is.False);
            Assert.That(a.Equals((object)diff), Is.False);
            Assert.That(a == same, Is.True);
            Assert.That(a != same, Is.False);
            Assert.That(a == diff, Is.False);
            Assert.That(a != diff, Is.True);
            Assert.That(a.GetHashCode(), Is.EqualTo(same.GetHashCode()));
        });
    }

    [Test]
    public void Distance_Int_BasicMethods_And_Operators()
    {
        var a = new Distance<int>(3, -4);
        var b = new Distance<int>(1, 2);

        Assert.Multiple(() =>
        {
            Assert.That(a.X, Is.EqualTo(3));
            Assert.That(a.Y, Is.EqualTo(-4));
            Assert.That(Distance<int>.Zero, Is.EqualTo(new Distance<int>(0, 0)));
            Assert.That(a.Manhattan(), Is.EqualTo(7));
            Assert.That(a.Abs(), Is.EqualTo(new Distance<int>(3, 4)));
        });

        Assert.Multiple(() =>
        {
            Assert.That(a + b, Is.EqualTo(new Distance<int>(4, -2)));
            Assert.That(a - b, Is.EqualTo(new Distance<int>(2, -6)));
            Assert.That(a * 2, Is.EqualTo(new Distance<int>(6, -8)));
            Assert.That(a / 2, Is.EqualTo(new Distance<int>(1, -2)));
            Assert.That(a * b, Is.EqualTo(new Distance<int>(3, -8)));
            Assert.That(
                a / new Distance<int>(3, -2),
                Is.EqualTo(new Distance<int>(1, 2))
            );
        });

        var same = new Distance<int>(3, -4);
        var diff = new Distance<int>(3, -5);
        Assert.Multiple(() =>
        {
            Assert.That(a.Equals(same), Is.True);
            Assert.That(a.Equals((object)same), Is.True);
            Assert.That(a.Equals(diff), Is.False);
            Assert.That(a.Equals((object)diff), Is.False);
            Assert.That(a == same, Is.True);
            Assert.That(a != same, Is.False);
            Assert.That(a == diff, Is.False);
            Assert.That(a != diff, Is.True);
            Assert.That(a.GetHashCode(), Is.EqualTo(same.GetHashCode()));
        });
    }

    [Test]
    public void FloatingCoordinate_Double_BasicMethods_And_Operators()
    {
        var a = new FloatingCoordinate<double>(3.0, 4.0);
        var b = new FloatingCoordinate<double>(-1.0, 2.0);

        Assert.Multiple(() =>
        {
            Assert.That(a.X, Is.EqualTo(3.0));
            Assert.That(a.Y, Is.EqualTo(4.0));
            Assert.That(FloatingCoordinate<double>.Zero,
                Is.EqualTo(new FloatingCoordinate<double>(0.0, 0.0)));
            Assert.That(FloatingCoordinate<double>.One,
                Is.EqualTo(new FloatingCoordinate<double>(1.0, 1.0)));
            Assert.That(FloatingCoordinate<double>.UnitX,
                Is.EqualTo(new FloatingCoordinate<double>(1.0, 0.0)));
            Assert.That(FloatingCoordinate<double>.UnitY,
                Is.EqualTo(new FloatingCoordinate<double>(0.0, 1.0)));
            Assert.That(a.Length, Is.EqualTo(5.0).Within(1e-10));
            Assert.That(a.GetStringX(), Is.EqualTo("3"));
            Assert.That(a.GetStringY(), Is.EqualTo("4"));
        });

        var min = a.Min(b);
        var max = a.Max(b);
        Assert.Multiple(() =>
        {
            Assert.That(min, Is.EqualTo(new FloatingCoordinate<double>(-1.0, 2.0)));
            Assert.That(max, Is.EqualTo(new FloatingCoordinate<double>(3.0, 4.0)));
        });

        var clamped = new FloatingCoordinate<double>(5.0, -5.0).Clamp(
            new FloatingCoordinate<double>(0.0, -2.0),
            new FloatingCoordinate<double>(4.0, 10.0));
        Assert.That(clamped, Is.EqualTo(new FloatingCoordinate<double>(4.0, -2.0)));

        var copySign = new FloatingCoordinate<double>(-2.0, 3.0)
            .CopySign(new FloatingCoordinate<double>(1.0, -1.0));
        Assert.That(copySign, Is.EqualTo(new FloatingCoordinate<double>(2.0, -3.0)));

        var abs = new FloatingCoordinate<double>(-2.0, -3.0).Abs();
        Assert.That(abs, Is.EqualTo(new FloatingCoordinate<double>(2.0, 3.0)));

        Assert.Multiple(() =>
        {
            Assert.That(-a, Is.EqualTo(new FloatingCoordinate<double>(-3.0, -4.0)));
            Assert.That(a + b, Is.EqualTo(new FloatingCoordinate<double>(2.0, 6.0)));
            Assert.That(a - b, Is.EqualTo(new FloatingCoordinate<double>(4.0, 2.0)));
            Assert.That(a * 2.0, Is.EqualTo(new FloatingCoordinate<double>(6.0, 8.0)));
            Assert.That(a / 2.0, Is.EqualTo(new FloatingCoordinate<double>(1.5, 2.0)));
            Assert.That(a * b, Is.EqualTo(new FloatingCoordinate<double>(-3.0, 8.0)));
            Assert.That(
                a / new FloatingCoordinate<double>(3.0, 2.0),
                Is.EqualTo(new FloatingCoordinate<double>(1.0, 2.0))
            );
        });

        var same = new FloatingCoordinate<double>(3.0, 4.0);
        var diff = new FloatingCoordinate<double>(3.0, 5.0);
        Assert.Multiple(() =>
        {
            Assert.That(a.Equals(same), Is.True);
            Assert.That(a.Equals((object)same), Is.True);
            Assert.That(a.Equals(diff), Is.False);
            Assert.That(a.Equals((object)diff), Is.False);
            Assert.That(a == same, Is.True);
            Assert.That(a != same, Is.False);
            Assert.That(a == diff, Is.False);
            Assert.That(a != diff, Is.True);
            Assert.That(a.GetHashCode(), Is.EqualTo(same.GetHashCode()));
        });
    }

    [Test]
    public void IntegerCoordinate_Int_StaticConstants()
    {
        Assert.Multiple(() =>
        {
            Assert.That(IntegerCoordinate<int>.Zero,
                Is.EqualTo(new IntegerCoordinate<int>(0, 0)));
            Assert.That(IntegerCoordinate<int>.One,
                Is.EqualTo(new IntegerCoordinate<int>(1, 1)));
            Assert.That(IntegerCoordinate<int>.UnitX,
                Is.EqualTo(new IntegerCoordinate<int>(1, 0)));
            Assert.That(IntegerCoordinate<int>.UnitY,
                Is.EqualTo(new IntegerCoordinate<int>(0, 1)));
        });
    }

    [Test]
    public void IntegerCoordinate_Int_Neighbours_Horizontal_Vertical()
    {
        var c = new IntegerCoordinate<int>(10, 20);

        var neighbours = c.Neighbours.ToArray();
        var expectedNeighbours = new[]
        {
            new IntegerCoordinate<int>(10, 21),
            new IntegerCoordinate<int>(11, 20),
            new IntegerCoordinate<int>(10, 19),
            new IntegerCoordinate<int>(9, 20)
        };
        Assert.That(neighbours, Is.EquivalentTo(expectedNeighbours));

        var horizontal = c.HorizontalNeighbours.ToArray();
        var expectedHorizontal = new[]
        {
            new IntegerCoordinate<int>(11, 20),
            new IntegerCoordinate<int>(9, 20)
        };
        Assert.That(horizontal, Is.EquivalentTo(expectedHorizontal));

        var vertical = c.VerticalNeighbours.ToArray();
        var expectedVertical = new[]
        {
            new IntegerCoordinate<int>(10, 21),
            new IntegerCoordinate<int>(10, 19)
        };
        Assert.That(vertical, Is.EquivalentTo(expectedVertical));
    }

    [Test]
    public void IntegerCoordinate_Int_Min_Max_Clamp_CopySign_Abs()
    {
        var a = new IntegerCoordinate<int>(-2, 3);
        var b = new IntegerCoordinate<int>(1, -4);

        Assert.Multiple(() =>
        {
            Assert.That(a.Min(b), Is.EqualTo(new IntegerCoordinate<int>(-2, -4)));
            Assert.That(a.Max(b), Is.EqualTo(new IntegerCoordinate<int>(1, 3)));
        });

        var clamped = new IntegerCoordinate<int>(5, -5)
            .Clamp(new IntegerCoordinate<int>(0, -2),
                   new IntegerCoordinate<int>(4, 10));
        Assert.That(clamped, Is.EqualTo(new IntegerCoordinate<int>(4, -2)));

        var copySign = a.CopySign(new IntegerCoordinate<int>(1, -1));
        Assert.That(copySign, Is.EqualTo(new IntegerCoordinate<int>(2, -3)));

        var abs = new IntegerCoordinate<int>(-2, -3).Abs();
        Assert.That(abs, Is.EqualTo(new IntegerCoordinate<int>(2, 3)));
    }

    [Test]
    public void IntegerCoordinate_Int_Distance_Move_Modulo_And_DirectionOf()
    {
        var a = new IntegerCoordinate<int>(1, 2);
        var b = new IntegerCoordinate<int>(4, -2);

        Assert.That(a.ManhattanLength(), Is.EqualTo(3));

        var distance = a.Distance(b);
        Assert.That(distance, Is.EqualTo(new Distance<int>(3, -4)));

        var moved = a.Move(distance);
        Assert.That(moved, Is.EqualTo(b));

        var movedByDirection = a.Move(Direction.NorthEast);
        Assert.That(movedByDirection, Is.EqualTo(new IntegerCoordinate<int>(2, 3)));

        var dirOf = a.DirectionOf(b);
        Assert.That(dirOf, Is.EqualTo(new IntegerCoordinate<int>(1, -1)));

        var path = a.MoveTo(b).ToList();
        Assert.Multiple(() =>
        {
            Assert.That(path[0], Is.EqualTo(a));
            Assert.That(path[^1], Is.EqualTo(new IntegerCoordinate<int>(4, -1)));
            Assert.That(path.Count, Is.EqualTo(4));
        });

        var mod = new IntegerCoordinate<int>(-1, -11)
            .Modulo(new IntegerCoordinate<int>(10, 10));
        Assert.That(mod, Is.EqualTo(new IntegerCoordinate<int>(9, 9)));
    }

    [Test]
    public void IntegerCoordinate_Int_Arithmetic_And_Bitwise_Operators()
    {
        var a = new IntegerCoordinate<int>(2, 3);
        var b = new IntegerCoordinate<int>(5, 7);

        Assert.Multiple(() =>
        {
            Assert.That(-a, Is.EqualTo(new IntegerCoordinate<int>(-2, -3)));
            Assert.That(a + b, Is.EqualTo(new IntegerCoordinate<int>(7, 10)));
            Assert.That(b - a, Is.EqualTo(new IntegerCoordinate<int>(3, 4)));
            Assert.That(a * b, Is.EqualTo(new IntegerCoordinate<int>(10, 21)));
            Assert.That(b / a, Is.EqualTo(new IntegerCoordinate<int>(2, 2)));
            Assert.That(a * 2, Is.EqualTo(new IntegerCoordinate<int>(4, 6)));
            Assert.That(b / 2, Is.EqualTo(new IntegerCoordinate<int>(2, 3)));
            Assert.That(a % b, Is.EqualTo(new IntegerCoordinate<int>(2 % 5, 3 % 7)));
            Assert.That(a & b,
                Is.EqualTo(new IntegerCoordinate<int>(2 & 5, 3 & 7)));
            Assert.That(a | b,
                Is.EqualTo(new IntegerCoordinate<int>(2 | 5, 3 | 7)));
            Assert.That(a ^ b,
                Is.EqualTo(new IntegerCoordinate<int>(2 ^ 5, 3 ^ 7)));
            Assert.That(~a, Is.EqualTo(new IntegerCoordinate<int>(~2, ~3)));
        });
    }

    [Test]
    public void IntegerCoordinate_Int_Equality_ToString_And_StringProperties()
    {
        var a = new IntegerCoordinate<int>(2, 3);
        var same = new IntegerCoordinate<int>(2, 3);
        var diff = new IntegerCoordinate<int>(2, 4);

        Assert.Multiple(() =>
        {
            Assert.That(a.Equals(same), Is.True);
            Assert.That(a.Equals((object)same), Is.True);
            Assert.That(a.Equals(diff), Is.False);
            Assert.That(a.Equals((object)diff), Is.False);
            Assert.That(a == same, Is.True);
            Assert.That(a != same, Is.False);
            Assert.That(a == diff, Is.False);
            Assert.That(a != diff, Is.True);
            Assert.That(a.GetHashCode(), Is.EqualTo(same.GetHashCode()));
            Assert.That(a.GetStringX(), Is.EqualTo("2"));
            Assert.That(a.GetStringY(), Is.EqualTo("3"));
            Assert.That(a.ToString(), Is.EqualTo("<2 3>"));
        });
    }
}
