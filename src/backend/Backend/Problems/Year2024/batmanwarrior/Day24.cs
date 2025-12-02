using Common;
using Common.Updates;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;
using Lib.Parsing;
using System.Linq;
using Lib.Extensions;

namespace Backend.Problems.Year2024.batmanwarrior;

public class Day24 : ProblemSet
{
    public override DateTime ReleaseTime { get; } =
        new(2024, 12, 24);

    public override List<Problem> Problems { get; } =
    [
        new FirstProblem(),
        new SecondProblem()
    ];

    public override string Name => "Crossed Wires";

    public class FirstProblem : Problem
    {
        public override string Name { get; } = "Part One";

        public override string Description { get; } = "";

        public override Task Solve(string input, Reporter reporter)
        {
            // Create system
            System system = new(input, reporter);

            // Send solution to frontend
            reporter.Report(FinishedProblemUpdate.FromSolution(system.GetBitValue()));
            return Task.CompletedTask;
        }
    }

    public class SecondProblem : Problem
    {
        public override string Name { get; } = "Part Two";

        public override string Description { get; } = "";

        public override Task Solve(string input, Reporter reporter)
        {
            // Create system
            System system = new(input, reporter);

            // Send solution to frontend
            reporter.Report(FinishedProblemUpdate.FromSolution(system.GetWrongGates()));
            return Task.CompletedTask;
        }
    }

    public class System
    {
        private readonly Reporter _reporter;
        private readonly Dictionary<string, State> _wires = [];
        private readonly Dictionary<string, (string, string)> _gates = [];
        private readonly Dictionary<string, string> _gateType = [];

        public System(string input, Reporter reporter)
        {
            // Save for frontend printing
            _reporter = reporter;

            // Parse wires and gates
            foreach (string row in input.SplitLines())
            {
                if (row.Contains(':'))
                {
                    // Add initial wires with values
                    string[] parts = row.Split(": ");
                    _wires[parts[0]] = parts[1] == "1" ? State.HIGH : State.LOW;
                }
                else
                {
                    // Add the gates and their respective wires
                    string[] parts = row.SplitBy([" ", "->"]);

                    // Check if wires exist
                    if (!_wires.ContainsKey(parts[0]))
                        _wires[parts[0]] = State.NONE;
                    if (!_wires.ContainsKey(parts[2]))
                        _wires[parts[2]] = State.NONE;
                    if (!_wires.ContainsKey(parts[3]))
                        _wires[parts[3]] = State.NONE;

                    // Check if gate exist
                    if (!_gates.ContainsKey(parts[3]))
                    {
                        _gates[parts[3]] = (parts[0], parts[2]);
                        _gateType[parts[3]] = parts[1];
                    } 
                }
            }
        }

        public long GetBitValue()
        {
            long value = 0;
            int currZ = 0;
            while (_gates.ContainsKey($"z{currZ:00}"))
            {
                if (GetState($"z{currZ:00}") == State.HIGH)
                    value += (long)Math.Pow(2, currZ);

                currZ++;
            }

            return value;
        }

        public string GetWrongGates()
        {
            // Get highest z gate
            string highestZ = "z00";
            foreach (var pair in _gates)
            {
                // Compare gates that start with z
                if (pair.Key[0] == 'z' && int.Parse(pair.Key[1..]) > int.Parse(highestZ[1..]))
                    highestZ = pair.Key;
            }

            // Save wrong gates
            List<string> wrongGates = [];

            // Check through all gates for wrong ones
            foreach (var pair in _gates)
            {
                // Boom
                if (pair.Key[0] == 'z' && _gateType[pair.Key] != "XOR" && pair.Key != highestZ)
                    wrongGates.Add(pair.Key);
                
                // Boom
                if (_gateType[pair.Key] == "XOR" && pair.Key[0] != 'z' && pair.Value.Item1[0] != 'x' && pair.Value.Item1[0] != 'y' && pair.Value.Item2[0] != 'x' && pair.Value.Item2[0] != 'y')
                    wrongGates.Add(pair.Key);

                // Boom
                if (_gateType[pair.Key] == "AND" && pair.Value.Item1 != "x00" && pair.Value.Item2 != "x00")
                    foreach (var subPair in _gates)
                        if ((pair.Key == subPair.Value.Item1 || pair.Key == subPair.Value.Item2) && _gateType[subPair.Key] != "OR")
                            wrongGates.Add(pair.Key);

                // Boom
                if (_gateType[pair.Key] == "XOR")
                    foreach (var subPair in _gates)
                        if ((pair.Key == subPair.Value.Item1 || pair.Key == subPair.Value.Item2) && _gateType[subPair.Key] == "OR")
                            wrongGates.Add(pair.Key);

                // (I want you in my room)
            }

            // Remove duplicates, sort and return (ez)
            wrongGates = [.. wrongGates.Distinct()];
            wrongGates.Sort();
            return string.Join(",", wrongGates);
        }

        private State GetState(string name)
        {
            // Wire with known state
            if (_wires[name] != State.NONE)
                return _wires[name];

            // Get state of inputs
            State stateOne = GetState(_gates[name].Item1);
            State stateTwo = GetState(_gates[name].Item2);

            // Perform gate operation
            switch (_gateType[name])
            {
                case "AND":
                    if (stateOne == State.LOW || stateTwo == State.LOW)
                    {
                        // If either input is low, gate output low
                        _wires[name] = State.LOW;
                    }
                    else if (stateOne == State.HIGH && stateTwo == State.HIGH)
                    {
                        // If both inputs are high, gate output high
                        _wires[name] = State.HIGH;
                    }
                    else
                    {
                        // Undefined behaviour
                        throw new NotImplementedException();
                    }
                    break;
                case "OR":
                    if (stateOne == State.HIGH || stateTwo == State.HIGH)
                    {
                        // If either input is high, gate output high
                        _wires[name] = State.HIGH;
                    }
                    else if (stateOne == State.LOW && stateTwo == State.LOW)
                    {
                        // If both inputs are low, gate output low
                        _wires[name] = State.LOW;
                    }
                    else
                    {
                        // Undefined behaviour
                        throw new NotImplementedException();
                    }
                    break;
                case "XOR":
                    if (stateOne == State.NONE || stateTwo == State.NONE)
                    {
                        // Undefined behaviour
                        throw new NotImplementedException();
                    }
                    else
                    {
                        // If inputs are same, gate output low
                        // If inputs are different, gate output high
                        _wires[name] = stateOne == stateTwo ? State.LOW : State.HIGH;
                    }
                    break;
            }

            // Return the gate result
            return _wires[name];
        }
    }

    public enum State
    {
        NONE,
        HIGH,
        LOW,
    }
}