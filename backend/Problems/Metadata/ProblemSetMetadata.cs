using Backend.Services;

namespace Backend.Problems.Metadata;

public record ProblemSetMetadata(string Name, DateTime ReleaseTime, List<ProblemMetadata> Problems);