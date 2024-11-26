namespace Backend.Problems.Updates;

public class GridUpdate : OngoingProblemUpdate
{
    public override string Type => "grid";
    
    public Dictionary<string, Dictionary<string, string>> Rows { get; set; }
}