namespace Backend.Problems.Updates;

public class TextProblemUpdate : OngoingProblemUpdate
{
    public override string Type => "text";
    
    public string[]? Lines { get; set; }
    public string? Text { get; set; }
}