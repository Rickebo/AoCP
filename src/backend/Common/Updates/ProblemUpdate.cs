using System.Text.Json.Serialization;

namespace Common.Updates;

[JsonDerivedType(typeof(OngoingProblemUpdate))]
[JsonDerivedType(typeof(FinishedProblemUpdate))]
[JsonDerivedType(typeof(TextProblemUpdate))]
[JsonDerivedType(typeof(StartProblemUpdate))]
[JsonDerivedType(typeof(GlyphGridUpdate))]
[JsonDerivedType(typeof(StringGridUpdate))]
/// <summary>
/// Represents an update emitted while solving a problem and streamed to clients.
/// </summary>
public abstract class ProblemUpdate
{
    /// <summary>
    /// Gets the discriminator used by clients to route the update.
    /// </summary>
    public abstract string Type { get; }

    /// <summary>
    /// Gets or sets the identifier of the problem producing this update.
    /// </summary>
    public ProblemId Id { get; set; } = new();
}

