using System.Globalization;
using Google.OrTools.Sat;

namespace Lib.Math;

public sealed class IntegerLinearProgram
{
    private readonly CpModel _model = new();
    private readonly List<IntVar> _variables = [];
    private bool _objectiveSet;

    public int AddVariable(long lowerBound = 0, long upperBound = 0, string? name = null)
    {
        if (upperBound < lowerBound)
            throw new ArgumentOutOfRangeException(nameof(upperBound), "Upper bound must be greater than or equal to lower bound.");

        var variable = _model.NewIntVar(lowerBound, upperBound, name ?? $"x_{_variables.Count}");
        _variables.Add(variable);
        return _variables.Count - 1;
    }

    public void AddConstraint(IEnumerable<(int variableIndex, long coefficient)> terms, IntegerLinearConstraintRelation relation, long rhs)
    {
        var expr = LinearExpr.Sum(BuildTerms(terms));
        switch (relation)
        {
            case IntegerLinearConstraintRelation.Equal:
                _model.Add(expr == rhs);
                break;
            case IntegerLinearConstraintRelation.NotEqual:
                _model.Add(expr != rhs);
                break;
            case IntegerLinearConstraintRelation.LessOrEqual:
                _model.Add(expr <= rhs);
                break;
            case IntegerLinearConstraintRelation.GreaterOrEqual:
                _model.Add(expr >= rhs);
                break;
            case IntegerLinearConstraintRelation.Less:
                _model.Add(expr < rhs);
                break;
            case IntegerLinearConstraintRelation.Greater:
                _model.Add(expr > rhs);
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(relation), relation, "Unsupported constraint relation.");
        }
    }

    public void Minimize(IEnumerable<(int variableIndex, long coefficient)> terms)
    {
        _model.Minimize(LinearExpr.Sum(BuildTerms(terms)));
        _objectiveSet = true;
    }

    public IntegerLinearProgramSolution Solve(IntegerLinearProgramOptions? options = null)
    {
        var solver = new CpSolver();
        var parameterString = BuildParameterString(options);
        if (parameterString.Length > 0)
            solver.StringParameters = parameterString;

        var status = solver.Solve(_model);
        var mappedStatus = MapStatus(status);

        var values = new long[_variables.Count];
        if (mappedStatus != IntegerLinearProgramStatus.Infeasible && mappedStatus != IntegerLinearProgramStatus.Invalid)
        {
            for (var i = 0; i < _variables.Count; i++)
                values[i] = solver.Value(_variables[i]);
        }

        var objectiveValue = _objectiveSet ? (long)solver.ObjectiveValue : 0;
        return new IntegerLinearProgramSolution(mappedStatus, values, objectiveValue);
    }

    private IEnumerable<LinearExpr> BuildTerms(IEnumerable<(int variableIndex, long coefficient)> terms)
    {
        foreach (var (index, coefficient) in terms)
        {
            if (index < 0 || index >= _variables.Count)
                throw new ArgumentOutOfRangeException(nameof(terms), $"Variable index {index} is out of range.");

            yield return _variables[index] * coefficient;
        }
    }

    private static IntegerLinearProgramStatus MapStatus(CpSolverStatus status) => status switch
    {
        CpSolverStatus.Optimal => IntegerLinearProgramStatus.Optimal,
        CpSolverStatus.Feasible => IntegerLinearProgramStatus.Feasible,
        CpSolverStatus.Infeasible => IntegerLinearProgramStatus.Infeasible,
        CpSolverStatus.ModelInvalid => IntegerLinearProgramStatus.Invalid,
        _ => IntegerLinearProgramStatus.Unknown
    };

    private static string BuildParameterString(IntegerLinearProgramOptions? options)
    {
        if (options is null)
            return string.Empty;

        var parts = new List<string>();
        if (options.SearchWorkers is { } workers && workers > 0)
            parts.Add($"num_search_workers:{workers}");
        if (options.MaxTimeSeconds is { } maxTime && maxTime > 0)
            parts.Add($"max_time_in_seconds:{maxTime.ToString(CultureInfo.InvariantCulture)}");
        if (options.EnableSearchProgressLogging)
            parts.Add("log_search_progress:true");

        return string.Join(',', parts);
    }
}

public enum IntegerLinearConstraintRelation
{
    Equal,
    NotEqual,
    LessOrEqual,
    GreaterOrEqual,
    Less,
    Greater,
}

public enum IntegerLinearProgramStatus
{
    Optimal,
    Feasible,
    Infeasible,
    Invalid,
    Unknown
}

public sealed record IntegerLinearProgramSolution(IntegerLinearProgramStatus Status, IReadOnlyList<long> VariableValues, long ObjectiveValue)
{
    public long GetValue(int variableIndex)
    {
        if (variableIndex < 0 || variableIndex >= VariableValues.Count)
            throw new ArgumentOutOfRangeException(nameof(variableIndex));

        return VariableValues[variableIndex];
    }
}

public sealed record IntegerLinearProgramOptions(int? SearchWorkers = null, double? MaxTimeSeconds = null, bool EnableSearchProgressLogging = false);
