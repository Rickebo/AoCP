using System.Text.Json.Serialization;

namespace Backend.Problems;

[JsonDerivedType(typeof(OngoingProblemUpdate))]
[JsonDerivedType(typeof(FinishedProblemUpdate))]
public abstract class ProblemUpdate
{
    public abstract string Type { get; }
    public ProblemId Id { get; set; } 
}