using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.Marshalling;
using System.Text;
using System.Threading.Tasks;
using Common;
using Common.Updates;
using Lib;
using Lib.Geometry;
using Lib.Grids;

namespace Backend.Problems.Year2024.Rickebo;

public class Day09 : ProblemSet
{
    public override DateTime ReleaseTime { get; } =
        new(2024, 12, 09);

    public override List<Problem> Problems { get; } =
    [
        new ProblemOne(),
        new ProblemTwo()
    ];

    public override string Name => "Disk Fragmenter";

    private class ProblemOne : Problem
    {
        public override string Name => "Part One";


        public override Task Solve(string input, Reporter reporter)
        {
            reporter.ReportSolution(Checksum(CompactWithFragmentation(Parse(input))));

            return Task.CompletedTask;
        }
    }

    private class ProblemTwo : Problem
    {
        public override string Name => "Part Two";

        public override Task Solve(string input, Reporter reporter)
        {
            reporter.ReportSolution(Checksum(CompactWithoutFragmentation(Parse(input))));

            return Task.CompletedTask;
        }
    }

    private static List<IBlock> Parse(string input)
    {
        var blocks = new List<IBlock>();

        for (var i = 0; i < input.Length; i++)
        {
            var blockLength = input[i] - '0';

            if (i % 2 == 1)
                blocks.Add(new EmptyBlock(blockLength));
            else
                blocks.Add(new Block(i / 2, blockLength));
        }

        return blocks;
    }

    private static List<IBlock> CompactWithFragmentation(List<IBlock> input)
    {
        var blocks = new LinkedList<IBlock>(input);

        while (true)
        {
            var removedBlock = blocks.Last;
            blocks.RemoveLast();

            if (removedBlock?.Value is not Block insert)
                continue;

            var space = insert.Length;
            for (var block = blocks.First; block != null; block = block.Next)
            {
                if (block.Value is not EmptyBlock empty)
                    continue;

                if (empty.Length == insert.Length)
                {
                    block.Value = insert;
                    space = 0;
                    break;
                }

                if (insert.Length < empty.Length)
                {
                    blocks.AddBefore(block, insert);
                    block.Value = new EmptyBlock(empty.Length - insert.Length);
                    space = 0;
                    break;
                }

                block.Value = insert with { Length = empty.Length };
                insert = insert with { Length = insert.Length - empty.Length };
                space -= empty.Length;
            }

            // If, after going through all blocks, there is still space, we can insert it last and break
            if (space < 1) continue;

            blocks.AddLast(insert with { Length = space });
            break;
        }

        return blocks.ToList();
    }

    private static List<IBlock> CompactWithoutFragmentation(List<IBlock> input)
    {
        var blocks = new LinkedList<IBlock>(input);
        
        while (true)
        {
            var biggestSpace = 0;
            LinkedListNode<IBlock>? popNode = null;

            for (var node = blocks.First; node != null; node = node.Next)
            {
                if (node.Value is EmptyBlock)
                    biggestSpace = Math.Max(biggestSpace, node.Value.Length);
                else if (node.Value.Length <= biggestSpace)
                    popNode = node;
            }

            // If there is no block fitting the biggest space, exit
            if (popNode == null) break;

            if (popNode.Value is not Block insert)
                throw new Exception();

            popNode.Value = new EmptyBlock(insert.Length);

            for (var current = blocks.First; current != null; current = current.Next)
            {
                if (current.Value is not EmptyBlock empty)
                    continue;

                if (empty.Length < insert.Length)
                    continue;

                current.Value = insert;

                if (empty.Length > insert.Length)
                    blocks.AddAfter(current, new EmptyBlock(empty.Length - insert.Length));
                    
                break;
            }
        }

        return blocks.ToList();
    }

    private static long Checksum(List<IBlock> blocks)
    {
        var s = 0L;
        var pos = 0;
        foreach (var entry in blocks)
        {
            if (entry is not Block block)
            {
                pos += entry.Length;
                continue;
            }

            for (var i = 0; i < block.Length; i++)
                s += pos++ * block.Id;
        }

        return s;
    }

    private interface IBlock
    {
        int Length { get; }
    }

    private record Block(int Id, int Length) : IBlock;

    private record EmptyBlock(int Length) : IBlock;
}

