using System;
using System.Collections.Generic;
using Backend.Services;

namespace Backend.Problems.Metadata;

public record ProblemSetMetadata(
    string Name,
    string Author,
    DateTime ReleaseTime,
    string? SolutionFilePath,
    List<ProblemMetadata> Problems
);
