using System.Text.Json.Serialization;

namespace Common.Updates;

[JsonDerivedType(typeof(OngoingProblemUpdate))]
[JsonDerivedType(typeof(FinishedProblemUpdate))]
[JsonDerivedType(typeof(TextProblemUpdate))]
[JsonDerivedType(typeof(StartProblemUpdate))]
[JsonDerivedType(typeof(GlyphGridUpdate))]
[JsonDerivedType(typeof(StringGridUpdate))]
public abstract class ProblemUpdate
{
    public abstract string Type { get; }
    public ProblemId Id { get; set; }
}

