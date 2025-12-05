using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Common;
using Common.Updates;
using Lib.Geometry;
using Lib.Grids;

namespace Backend.Problems.Year2025Codelight.Rickebo;

public class Day21 : ProblemSet
{
    public override DateTime ReleaseTime { get; } =
        new(2025, 11, 21);

    public override List<Problem> Problems { get; } =
    [
        new ProblemOne()
    ];

    public override string Name => "Coffee Quest";


    private class ProblemOne : Problem
    {
        public override string Name => "Part One";

        public override string Description => "";


        private static int Bfs(Position start, CharGrid grid)
        {
            var frontier = new PriorityQueue<Position, int>();
            
            frontier.Enqueue(start, 0);

            while (frontier.TryDequeue(out var pos, out var score))
            {
                var direction = pos.Direction;

                var newDirections = new[]
                {
                    (direction, 1),
                    (direction.Rotate(Rotation.CounterClockwise), 2),
                    (direction.Rotate(Rotation.Clockwise), 3),
                };
                
                foreach (var (newDirection, penalty) in newDirections)
                {
                    var neighbour = pos.Coordinate.Move(newDirection);

                    if (!grid.Contains(neighbour) || grid[neighbour] == '#')
                        continue;

                    if (grid[neighbour] == 'E')
                        return score + penalty;
                    
                    var newPos = new Position(neighbour, newDirection);
                    frontier.Enqueue(newPos, score + penalty);
                }
            }

            throw new Exception("No solution found");
        }
        
        public override Task Solve(string input, Reporter reporter)
        {
            var grid = new CharGrid(input);
            var start = grid.Find(x => x == 'S');
            var pos = new Position(start, Direction.East);
            
            reporter.Report(
                new FinishedProblemUpdate()
                {
                    Solution = Bfs(pos, grid).ToString(),
                }
            );

            return Task.CompletedTask;
        }
        
        public record Position(IntegerCoordinate<int> Coordinate, Direction Direction);
    }
}

