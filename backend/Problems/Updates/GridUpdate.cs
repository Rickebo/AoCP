namespace Backend.Problems.Updates;

public class GridUpdate : OngoingProblemUpdate
{
    public override string Type => "grid";

    public int? Width { get; set; }
    public int? Height { get; set; }
    public bool? Clear { get; set; }
    public Dictionary<string, Dictionary<string, object>> Rows { get; set; }

    public record Cell(string Glyph, string Fg, string Bg);
}