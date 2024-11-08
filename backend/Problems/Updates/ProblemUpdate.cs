using System.Text.Json.Serialization;

namespace Backend.Problems.Updates;

[JsonDerivedType(typeof(OngoingProblemUpdate))]
[JsonDerivedType(typeof(FinishedProblemUpdate))]
[JsonDerivedType(typeof(TextProblemUpdate))]
public abstract class ProblemUpdate
{
    public abstract string Type { get; }
    public ProblemId Id { get; set; }
}