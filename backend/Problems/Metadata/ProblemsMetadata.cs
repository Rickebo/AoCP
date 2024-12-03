namespace Backend.Problems.Metadata;

public record ProblemsMetadata
{
    public required Dictionary<int, ProblemCollectionMetadata> Collections { get; init; }   
}