namespace Backend.Problems;

public class FinishedProblemUpdate : ProblemUpdate
{
    public override string Type => "finished";
    
    public bool Successful { get; set; }
    public string? Solution { get; set; }
    public string? Error { get; set; }
}