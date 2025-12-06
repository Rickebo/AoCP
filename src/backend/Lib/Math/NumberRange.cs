using System.Diagnostics.CodeAnalysis;
using System.Numerics;

namespace Lib.Math;

/// <summary>
/// Represents an inclusive numeric interval.
/// </summary>
/// <typeparam name="T">Numeric type.</typeparam>
public class NumberRange<T> where T : INumber<T>, IMinMaxValue<T>
{
    /// <summary>
    /// Inclusive start of the range.
    /// </summary>
    public T Start { get; }

    /// <summary>
    /// Inclusive end of the range.
    /// </summary>
    public T Stop { get; }

    /// <summary>
    /// Length of the range (Stop - Start + 1).
    /// </summary>
    public T Length => T.One + Stop - Start;

    /// <summary>
    /// Creates a new range.
    /// </summary>
    /// <param name="start">Inclusive start.</param>
    /// <param name="stop">Inclusive end.</param>
    /// <exception cref="InvalidOperationException">Thrown when stop is less than start.</exception>
    public NumberRange(T start, T stop)
    {
        if (stop < start)
            throw new InvalidOperationException();

        Start = start;
        Stop = stop;
    }

    /// <summary>
    /// Indicates whether the range has positive length.
    /// </summary>
    public bool IsNonEmpty => Length > T.Zero;

    /// <summary>
    /// Indicates whether the range is empty.
    /// </summary>
    public bool IsEmpty => !IsNonEmpty;

    /// <summary>
    /// Indicates whether the range is valid (stop is greater than or equal to start).
    /// </summary>
    public bool IsValid => Stop >= Start;

    /// <summary>
    /// Checks if the range contains a value.
    /// </summary>
    public bool Contains(T coordinate) =>
        coordinate >= Start && coordinate <= Stop;

    /// <summary>
    /// Returns a range of values strictly greater than the threshold.
    /// </summary>
    public NumberRange<T> GreaterThan(T threshold) =>
        new(
            T.Max(threshold + T.One, Start),
            T.Max(threshold + T.One, Stop)
        );

    /// <summary>
    /// Returns a range of values strictly less than the threshold.
    /// </summary>
    public NumberRange<T> LessThan(T threshold) =>
        new(
            T.Min(threshold, Start),
            T.Min(threshold, Stop)
        );

    /// <summary>
    /// Computes the intersection of two ranges.
    /// </summary>
    public static NumberRange<T> operator |(NumberRange<T> left, NumberRange<T> right) =>
        left.Intersection(right);

    /// <summary>
    /// Computes the union of two overlapping ranges.
    /// </summary>
    public static NumberRange<T> operator &(NumberRange<T> left, NumberRange<T> right) =>
        left.Union(right);

    /// <summary>
    /// Returns the range after removing <paramref name="other"/> when it aligns with one side.
    /// </summary>
    public NumberRange<T> Without(NumberRange<T> other)
    {
        if (other.Start == Start && other.Stop < Stop)
            return new NumberRange<T>(other.Stop + T.One, Stop);

        if (other.Stop == Stop && other.Start > Start)
            return new NumberRange<T>(Start, other.Start - T.One);

        throw new InvalidOperationException();
    }

    /// <summary>
    /// Returns a range that starts at the specified coordinate.
    /// </summary>
    public NumberRange<T> StartAt(T coordinate) => new(coordinate, Stop);

    /// <summary>
    /// Returns a range that stops at the specified coordinate.
    /// </summary>
    public NumberRange<T> StopAt(T coordinate) => new(Start, coordinate);

    /// <summary>
    /// Checks whether this range lies entirely before another.
    /// </summary>
    public bool IsBefore(NumberRange<T> other) =>
        Start < other.Start && Stop < other.Stop;

    /// <summary>
    /// Checks whether this range lies entirely after another.
    /// </summary>
    public bool IsAfter(NumberRange<T> other) =>
        Start > other.Start && Stop > other.Stop;

    /// <summary>
    /// Returns the complement when the range spans to the numeric bounds on one side.
    /// </summary>
    public static NumberRange<T> operator ~(NumberRange<T> range) =>
        range.Start == T.MinValue
            ? new NumberRange<T>(range.Stop, T.MaxValue)
            : range.Stop == T.MaxValue
                ? new NumberRange<T>(T.MinValue, range.Start)
                : throw new InvalidOperationException();

    /// <summary>
    /// Checks whether this range intersects another.
    /// </summary>
    public bool Intersects(NumberRange<T> other) =>
        Contains(other.Start) || other.Contains(Start) ||
        Contains(other.Stop) || other.Contains(Stop);

    /// <summary>
    /// Returns the union of two intersecting ranges.
    /// </summary>
    public NumberRange<T> Union(NumberRange<T> other) =>
        Intersects(other)
            ? new NumberRange<T>(T.Min(Start, other.Start), T.Max(Stop, other.Stop))
            : throw new InvalidOperationException();

    /// <summary>
    /// Checks whether this range fully contains another.
    /// </summary>
    public bool Contains(NumberRange<T> other) =>
        Start <= other.Start && Stop >= other.Stop;

    /// <summary>
    /// Checks whether this range is contained within another.
    /// </summary>
    public bool ContainedBy(NumberRange<T> other) =>
        other.Contains(this);

    /// <summary>
    /// Returns the intersection of two ranges.
    /// </summary>
    public NumberRange<T> Intersection(NumberRange<T> other) =>
        new(
            T.Max(Start, other.Start),
            T.Min(Stop, other.Stop)
        );

    /// <summary>
    /// Splits the range at a coordinate if it lies within.
    /// </summary>
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

    /// <summary>
    /// Splits the range based on overlap with another range, yielding sub-ranges.
    /// </summary>
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

    /// <inheritdoc />
    public override bool Equals([NotNullWhen(true)] object? obj) =>
        obj is NumberRange<T> other &&
        Start == other.Start &&
        Stop == other.Stop;

    /// <inheritdoc />
    public override int GetHashCode() =>
        HashCode.Combine(Start, Stop);

    /// <summary>
    /// Returns a string representation of the range.
    /// </summary>
    public override string ToString() => $"[{Start},{Stop}]";
}
