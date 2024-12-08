using System.Runtime.InteropServices.Marshalling;
using Common;
using Common.Updates;
using Lib;
using Lib.Coordinate;
using Lib.Grid;

namespace Backend.Problems.Year2024.Rickebo;

public class Day08 : ProblemSet
{
    public override DateTime ReleaseTime { get; } =
        new(2024, 12, 08);

    public override List<Problem> Problems { get; } =
    [
        new ProblemOne(),
        new ProblemTwo()
    ];

    private static readonly string[] _colors =
    [
        "#e6194b", "#3cb44b", "#ffe119", "#4363d8", "#f58231", "#911eb4", "#46f0f0",
        "#f032e6", "#bcf60c", "#fabebe", "#008080", "#e6beff", "#9a6324", "#fffac8",
        "#800000", "#aaffc3", "#808000", "#ffd8b1", "#000075", "#808080", "#ffffff",
        "#000000"
    ];

    public override string Name => "Resonant Collinearity";

    private class ProblemOne : Problem
    {
        public override string Name => "Part One";


        public override Task Solve(string input, Reporter reporter)
        {
            var data = Parse(input);
            reporter.ReportSolution(FindAntinodes(data, reporter).Count.ToString());

            return Task.CompletedTask;
        }
    }

    private class ProblemTwo : Problem
    {
        public override string Name => "Part Two";

        public override Task Solve(string input, Reporter reporter)
        {
            var data = Parse(input);
            reporter.ReportSolution(FindAntinodes(data, reporter, true).Count.ToString());

            return Task.CompletedTask;
        }
    }

    private static HashSet<IntegerCoordinate<int>> FindAntinodes(
        ProblemState state,
        Reporter? reporter,
        bool multiple = false
    )
    {
        var antinodes = new Dictionary<IntegerCoordinate<int>, string>();

        reporter?.ReportStringGridUpdate(
            state.Grid,
            (builder, coordinate, value) => builder
                .WithCoordinate(coordinate)
                .WithText("#000000")
        );

        var fi = 0;
        foreach (var freq in state.Antennas.Keys)
        {
            var freqColor = _colors[fi++ % _colors.Length];

            foreach (var antennaPos in state.Antennas[freq])
            {
                reporter?.ReportGlyphGridUpdate(
                    builder => builder.WithEntry(
                        b => b
                            .WithCoordinate(antennaPos)
                            .WithGlyph("+")
                            .WithForeground(freqColor + "66")
                    )
                );

                foreach (var otherAntennaPos in state.Antennas[freq])
                {
                    if (otherAntennaPos == antennaPos)
                        continue;

                    var delta = antennaPos - otherAntennaPos;
                    var p1 = antennaPos + delta;

                    if (state.Grid.Contains(p1))
                        antinodes[p1] = freqColor;

                    if (!multiple)
                        continue;

                    for (var pc = antennaPos; state.Grid.Contains(pc); pc += delta)
                        antinodes[pc] = freqColor;
                    
                    for (var pc = antennaPos; state.Grid.Contains(pc); pc -= delta)
                        antinodes[pc] = freqColor;
                }
            }
        }

        reporter?.ReportGlyphGridUpdate(
            updateBuilder => updateBuilder
                .WithEntries(
                    antinodes,
                    (glyphBuilder, pair) => glyphBuilder
                        .WithCoordinate(pair.Key)
                        .WithForeground(pair.Value)
                        .WithBackground("#00000000")
                        .WithGlyph("*")
                )
        );

        return antinodes.Keys.ToHashSet();
    }

    private static ProblemState Parse(string input)
    {
        var grid = Parser.ParseCharGrid(input).FlipY();
        var antennas = new Dictionary<char, HashSet<IntegerCoordinate<int>>>();

        foreach (var coordinate in grid.Coordinates)
        {
            var value = grid[coordinate];
            if (value == '.')
                continue;

            if (!antennas.TryGetValue(value, out var antennaSet))
                antennas[value] = antennaSet = new HashSet<IntegerCoordinate<int>>();

            antennaSet.Add(coordinate);
        }

        return new ProblemState(grid, antennas);
    }

    private record ProblemState(
        CharGrid Grid,
        Dictionary<char, HashSet<IntegerCoordinate<int>>> Antennas
    );
}