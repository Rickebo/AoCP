using Backend.Services;

namespace Backend.Problems.Metadata;

public record ProblemSetMetadata(DateTime ReleaseTime, List<ProblemMetadata> Problems);