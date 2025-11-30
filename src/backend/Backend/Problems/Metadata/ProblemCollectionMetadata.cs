using System.Collections.Generic;

namespace Backend.Problems.Metadata;

public record ProblemCollectionMetadata(
    string Source,
    int Year,
    Dictionary<string, List<ProblemSetMetadata>> ProblemSets
);
