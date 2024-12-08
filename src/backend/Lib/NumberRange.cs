using System.Diagnostics.CodeAnalysis;
using System.Numerics;

namespace Lib;

public class NumberRange<T> where T : INumber<T>, IMinMaxValue<T>
{
    public T Start { get; }
    public T Stop { get; }
    public T Length => T.One + Stop - Start;

    public NumberRange(T start, T stop)
    {
        if (Stop < Start)
            throw new InvalidOperationException();

        Start = start;
        Stop = stop;
    }

    public bool IsNonEmpty => Stop > Start;
    public bool IsEmpty => Stop == Start;
    public bool IsValid => Stop >= Start;

    public bool Contains(T coordinate) =>
        coordinate >= Start && coordinate <= Stop;

    public NumberRange<T> GreaterThan(T threshold) =>
        new NumberRange<T>(
            T.Max(threshold + T.One, Start),
            T.Max(threshold + T.One, Stop)
        );

    public NumberRange<T> LessThan(T threshold) =>
        new NumberRange<T>(
            T.Min(threshold, Start),
            T.Min(threshold, Stop)
        );

    public static NumberRange<T> operator |(NumberRange<T> left, NumberRange<T> right) =>
        left.Intersection(right);

    public static NumberRange<T> operator &(NumberRange<T> left, NumberRange<T> right) =>
        left.Union(right);

    public NumberRange<T> Without(NumberRange<T> other)
    {
        if (other.Start == Start && other.Stop < Stop)
            return new NumberRange<T>(other.Stop + T.One, Stop);

        if (other.Stop == Stop && other.Start > Start)
            return new NumberRange<T>(Start, other.Start - T.One);

        throw new InvalidOperationException();
    }

    public NumberRange<T> StartAt(T coordinate) => new(coordinate, Stop);

    public NumberRange<T> StopAt(T coordinate) => new(Start, coordinate);

    public bool IsBefore(NumberRange<T> other) =>
        Start < other.Start && Stop < other.Stop;

    public bool IsAfter(NumberRange<T> other) =>
        Start > other.Start && Stop > other.Stop;

    public static NumberRange<T> operator ~(NumberRange<T> range) =>
        range.Start == T.MinValue
            ? new NumberRange<T>(range.Stop, T.MaxValue)
            : range.Stop == T.MaxValue
                ? new NumberRange<T>(T.MinValue, range.Start)
                : throw new InvalidOperationException();

    public bool Intersects(NumberRange<T> other) =>
        Contains(other.Start) || other.Contains(Start) ||
        Contains(other.Stop) || other.Contains(Stop);

    public NumberRange<T> Union(NumberRange<T> other) =>
        Intersects(other)
            ? new NumberRange<T>(T.Min(Start, other.Start), T.Max(Stop, other.Stop))
            : throw new InvalidOperationException();

    public bool Contains(NumberRange<T> other) =>
        Start <= other.Start && Stop >= other.Stop;

    public bool ContainedBy(NumberRange<T> other) =>
        other.Contains(this);

    public NumberRange<T> Intersection(NumberRange<T> other) =>
        new NumberRange<T>(
            T.Max(Start, other.Start),
            T.Min(Stop, other.Stop)
        );

    public IEnumerable<NumberRange<T>> Split(T coordinate)
    {
        if (!Contains(coordinate))
        {
            yield return this;
            yield break;
        }
        else
        {
            yield return StopAt(coordinate);
            yield return StartAt(coordinate);
        }
    }

    public IEnumerable<NumberRange<T>> Split(NumberRange<T> other)
    {
        if (!Intersects(other) || other.Contains(this))
        {
            yield return this;
            yield break;
        }

        if (IsBefore(other))
        {
            yield return StopAt(other.Start);
            yield return StartAt(other.Start);
        }
        else if (IsAfter(other))
        {
            yield return StopAt(other.Stop);
            yield return StartAt(other.Stop);
        }
        else
        {
            yield return StopAt(other.Start);
            yield return other;
            yield return StartAt(other.Start);
        }
    }

    public override bool Equals([NotNullWhen(true)] object? obj) =>
        obj is NumberRange<T> other &&
        Start == other.Start &&
        Stop == other.Stop;

    public override int GetHashCode() =>
        HashCode.Combine(Start, Stop);

    public override string ToString() => $"[{Start},{Stop}]";
}