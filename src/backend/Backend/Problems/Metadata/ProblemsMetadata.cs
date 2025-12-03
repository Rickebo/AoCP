using System.Collections.Generic;

namespace Backend.Problems.Metadata;

public record ProblemsMetadata
{
    public required List<ProblemCollectionMetadata> Collections { get; init; }   
}

