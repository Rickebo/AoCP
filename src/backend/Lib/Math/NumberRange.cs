using System.Diagnostics.CodeAnalysis;
using System.Numerics;

namespace Lib.Math;

/// <summary>
/// Represents an inclusive numeric range with helper operations.
/// </summary>
public class NumberRange<T> where T : INumber<T>, IMinMaxValue<T>
{
    /// <summary>
    /// Gets the inclusive start of the range.
    /// </summary>
    public T Start { get; }

    /// <summary>
    /// Gets the inclusive end of the range.
    /// </summary>
    public T Stop { get; }

    /// <summary>
    /// Gets the number of values contained in the range.
    /// </summary>
    public T Length => T.One + Stop - Start;

    /// <summary>
    /// Initializes a new <see cref="NumberRange{T}"/>.
    /// </summary>
    /// <param name="start">Inclusive start of the range.</param>
    /// <param name="stop">Inclusive end of the range.</param>
    /// <exception cref="InvalidOperationException">Thrown when <paramref name="stop"/> is before <paramref name="start"/>.</exception>
    public NumberRange(T start, T stop)
    {
        if (stop < start)
            throw new InvalidOperationException();

        Start = start;
        Stop = stop;
    }

    /// <summary>
    /// Gets a value indicating whether the range contains more than one value.
    /// </summary>
    public bool IsNonEmpty => Stop > Start;

    /// <summary>
    /// Gets a value indicating whether the range represents a single value.
    /// </summary>
    public bool IsEmpty => Stop == Start;

    /// <summary>
    /// Gets a value indicating whether the range bounds are in ascending order.
    /// </summary>
    public bool IsValid => Stop >= Start;

    /// <summary>
    /// Checks whether the given coordinate lies inside the range (inclusive).
    /// </summary>
    /// <param name="coordinate">Value to test.</param>
    /// <returns><c>true</c> when the value lies within the range.</returns>
    public bool Contains(T coordinate) =>
        coordinate >= Start && coordinate <= Stop;

    /// <summary>
    /// Returns a range consisting of values strictly greater than the threshold.
    /// </summary>
    /// <param name="threshold">Threshold to compare against.</param>
    /// <returns>A new range truncated below the threshold.</returns>
    public NumberRange<T> GreaterThan(T threshold) =>
        new(
            T.Max(threshold + T.One, Start),
            T.Max(threshold + T.One, Stop)
        );

    /// <summary>
    /// Returns a range consisting of values strictly less than the threshold.
    /// </summary>
    /// <param name="threshold">Threshold to compare against.</param>
    /// <returns>A new range truncated above the threshold.</returns>
    public NumberRange<T> LessThan(T threshold) =>
        new(
            T.Min(threshold, Start),
            T.Min(threshold, Stop)
        );

    /// <summary>
    /// Computes the intersection of two ranges.
    /// </summary>
    /// <param name="left">Left operand.</param>
    /// <param name="right">Right operand.</param>
    /// <returns>The overlapping portion of the ranges.</returns>
    public static NumberRange<T> operator |(NumberRange<T> left, NumberRange<T> right) =>
        left.Intersection(right);

    /// <summary>
    /// Computes the union of two overlapping ranges.
    /// </summary>
    /// <param name="left">Left operand.</param>
    /// <param name="right">Right operand.</param>
    /// <returns>The combined range.</returns>
    public static NumberRange<T> operator &(NumberRange<T> left, NumberRange<T> right) =>
        left.Union(right);

    /// <summary>
    /// Removes the specified subrange from this range when it aligns with an edge.
    /// </summary>
    /// <param name="other">Range to remove.</param>
    /// <returns>The remaining range after removal.</returns>
    /// <exception cref="InvalidOperationException">Thrown when <paramref name="other"/> is not aligned with an edge.</exception>
    public NumberRange<T> Without(NumberRange<T> other)
    {
        if (other.Start == Start && other.Stop < Stop)
            return new NumberRange<T>(other.Stop + T.One, Stop);

        if (other.Stop == Stop && other.Start > Start)
            return new NumberRange<T>(Start, other.Start - T.One);

        throw new InvalidOperationException();
    }

    /// <summary>
    /// Returns a copy of the range starting at the specified coordinate.
    /// </summary>
    /// <param name="coordinate">New start value.</param>
    /// <returns>A new range with updated start.</returns>
    public NumberRange<T> StartAt(T coordinate) => new(coordinate, Stop);

    /// <summary>
    /// Returns a copy of the range ending at the specified coordinate.
    /// </summary>
    /// <param name="coordinate">New stop value.</param>
    /// <returns>A new range with updated stop.</returns>
    public NumberRange<T> StopAt(T coordinate) => new(Start, coordinate);

    /// <summary>
    /// Checks if this range is entirely before another.
    /// </summary>
    /// <param name="other">Range to compare to.</param>
    /// <returns><c>true</c> when this range ends before <paramref name="other"/> ends.</returns>
    public bool IsBefore(NumberRange<T> other) =>
        Start < other.Start && Stop < other.Stop;

    /// <summary>
    /// Checks if this range is entirely after another.
    /// </summary>
    /// <param name="other">Range to compare to.</param>
    /// <returns><c>true</c> when this range starts after <paramref name="other"/> starts.</returns>
    public bool IsAfter(NumberRange<T> other) =>
        Start > other.Start && Stop > other.Stop;

    /// <summary>
    /// Computes the complement of the range relative to the numeric limits.
    /// </summary>
    /// <param name="range">Range whose complement to compute.</param>
    /// <returns>The complement range.</returns>
    /// <exception cref="InvalidOperationException">Thrown when the complement cannot be represented.</exception>
    public static NumberRange<T> operator ~(NumberRange<T> range) =>
        range.Start == T.MinValue
            ? new NumberRange<T>(range.Stop, T.MaxValue)
            : range.Stop == T.MaxValue
                ? new NumberRange<T>(T.MinValue, range.Start)
                : throw new InvalidOperationException();

    /// <summary>
    /// Determines whether this range intersects another.
    /// </summary>
    /// <param name="other">Range to test.</param>
    /// <returns><c>true</c> when the ranges overlap.</returns>
    public bool Intersects(NumberRange<T> other) =>
        Contains(other.Start) || other.Contains(Start) ||
        Contains(other.Stop) || other.Contains(Stop);

    /// <summary>
    /// Returns the union of two intersecting ranges.
    /// </summary>
    /// <param name="other">Range to merge with.</param>
    /// <returns>The merged range.</returns>
    /// <exception cref="InvalidOperationException">Thrown when the ranges do not intersect.</exception>
    public NumberRange<T> Union(NumberRange<T> other) =>
        Intersects(other)
            ? new NumberRange<T>(T.Min(Start, other.Start), T.Max(Stop, other.Stop))
            : throw new InvalidOperationException();

    /// <summary>
    /// Determines whether this range fully contains another.
    /// </summary>
    /// <param name="other">Range to test.</param>
    /// <returns><c>true</c> when <paramref name="other"/> lies entirely within this range.</returns>
    public bool Contains(NumberRange<T> other) =>
        Start <= other.Start && Stop >= other.Stop;

    /// <summary>
    /// Determines whether this range is contained by another.
    /// </summary>
    /// <param name="other">Range that may contain this range.</param>
    /// <returns><c>true</c> when this range lies entirely within <paramref name="other"/>.</returns>
    public bool ContainedBy(NumberRange<T> other) =>
        other.Contains(this);

    /// <summary>
    /// Computes the intersection of two ranges.
    /// </summary>
    /// <param name="other">Range to intersect with.</param>
    /// <returns>The overlapping portion of the two ranges.</returns>
    public NumberRange<T> Intersection(NumberRange<T> other) =>
        new(
            T.Max(Start, other.Start),
            T.Min(Stop, other.Stop)
        );

    /// <summary>
    /// Splits the range into two at the given coordinate when contained.
    /// </summary>
    /// <param name="coordinate">Coordinate at which to split.</param>
    /// <returns>One or two ranges depending on whether the coordinate lies within the range.</returns>
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
    /// Splits the range around another range, yielding the surrounding portions.
    /// </summary>
    /// <param name="other">Range to split around.</param>
    /// <returns>Ranges representing the non-overlapping portions and optionally the overlapping portion.</returns>
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

    /// <summary>
    /// Determines equality with another object.
    /// </summary>
    /// <param name="obj">Object to compare.</param>
    /// <returns><c>true</c> when the other object is a range with matching bounds.</returns>
    public override bool Equals([NotNullWhen(true)] object? obj) =>
        obj is NumberRange<T> other &&
        Start == other.Start &&
        Stop == other.Stop;

    /// <summary>
    /// Generates a hash code from the range bounds.
    /// </summary>
    /// <returns>Hash code for the range.</returns>
    public override int GetHashCode() =>
        HashCode.Combine(Start, Stop);

    /// <summary>
    /// Returns a string representation of the range.
    /// </summary>
    /// <returns>String in the format "[start,stop]".</returns>
    public override string ToString() => $"[{Start},{Stop}]";
}
