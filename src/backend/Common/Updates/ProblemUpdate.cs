using System.Text.Json.Serialization;

namespace Common.Updates;

/// <summary>
/// Base type for updates emitted while running a problem.
/// </summary>
[JsonDerivedType(typeof(OngoingProblemUpdate))]
[JsonDerivedType(typeof(FinishedProblemUpdate))]
[JsonDerivedType(typeof(TextProblemUpdate))]
[JsonDerivedType(typeof(StartProblemUpdate))]
[JsonDerivedType(typeof(GlyphGridUpdate))]
[JsonDerivedType(typeof(StringGridUpdate))]
public abstract class ProblemUpdate
{
    /// <summary>
    /// Gets the discriminator used by clients to process the update.
    /// </summary>
    public abstract string Type { get; }

    /// <summary>
    /// Gets or sets the identifier for the problem that produced the update.
    /// </summary>
    public ProblemId Id { get; set; } = new();
}

