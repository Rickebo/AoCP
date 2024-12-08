namespace Backend.Problems.Metadata;

public record ProblemCollectionMetadata(int Year, Dictionary<string, List<ProblemSetMetadata>> ProblemSets);