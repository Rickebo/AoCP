using System.Globalization;
using Google.OrTools.Sat;

namespace Lib.Math;

/// <summary>
/// Provides a lightweight wrapper around OR-Tools CP-SAT for integer linear programming problems.
/// </summary>
public sealed class IntegerLinearProgram
{
    private readonly CpModel _model = new();
    private readonly List<IntVar> _variables = [];
    private bool _objectiveSet;
    private string _lastParameterString = string.Empty;

    /// <summary>
    /// Gets the number of variables currently registered on the model.
    /// </summary>
    public int VariableCount => _variables.Count;

    /// <summary>
    /// Gets the parameter string applied to the most recent <see cref="Solve"/> invocation.
    /// </summary>
    public string LastParameterString => _lastParameterString;

    /// <summary>
    /// Adds a new integer variable with the provided bounds.
    /// </summary>
    /// <param name="lowerBound">Inclusive lower bound.</param>
    /// <param name="upperBound">Inclusive upper bound. Must be greater than or equal to <paramref name="lowerBound"/>.</param>
    /// <param name="name">Optional name. When omitted, a generated name is used.</param>
    /// <returns>The zero-based index of the created variable.</returns>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when <paramref name="upperBound"/> is less than <paramref name="lowerBound"/>.</exception>
    public int AddVariable(long lowerBound = 0, long upperBound = 0, string? name = null)
    {
        if (upperBound < lowerBound)
            throw new ArgumentOutOfRangeException(nameof(upperBound), "Upper bound must be greater than or equal to lower bound.");

        var variable = _model.NewIntVar(lowerBound, upperBound, name ?? $"x_{_variables.Count}");
        _variables.Add(variable);
        return _variables.Count - 1;
    }

    /// <summary>
    /// Adds a linear constraint to the model.
    /// </summary>
    /// <param name="terms">Collection of variable indices and coefficients that form the left-hand side expression.</param>
    /// <param name="relation">The relational operator used to compare the expression against <paramref name="rhs"/>.</param>
    /// <param name="rhs">Right-hand side constant value.</param>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when a variable index is out of range or the relation is not supported.</exception>
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

    /// <summary>
    /// Sets the minimization objective for the model using the provided terms.
    /// Replaces any previously defined objective.
    /// </summary>
    /// <param name="terms">Collection of variable indices and coefficients forming the objective expression.</param>
    public void Minimize(IEnumerable<(int variableIndex, long coefficient)> terms)
    {
        _model.Minimize(LinearExpr.Sum(BuildTerms(terms)));
        _objectiveSet = true;
    }

    /// <summary>
    /// Solves the configured integer linear program.
    /// </summary>
    /// <param name="options">Optional solver configuration.</param>
    /// <returns>An <see cref="IntegerLinearProgramSolution"/> containing the status, variable values, and objective value (0 when no objective is set).</returns>
    public IntegerLinearProgramSolution Solve(IntegerLinearProgramOptions? options = null)
    {
        var solver = new CpSolver();
        _lastParameterString = BuildParameterString(options);
        if (_lastParameterString.Length > 0)
            solver.StringParameters = _lastParameterString;

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

    /// <summary>
    /// Builds a collection of OR-Tools terms from the provided variable indices and coefficients.
    /// </summary>
    /// <param name="terms">Variable indices and coefficients.</param>
    /// <returns>A collection of <see cref="LinearExpr"/> instances composing the sum.</returns>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when a variable index is out of range.</exception>
    private IEnumerable<LinearExpr> BuildTerms(IEnumerable<(int variableIndex, long coefficient)> terms)
    {
        foreach (var (index, coefficient) in terms)
        {
            if (index < 0 || index >= _variables.Count)
                throw new ArgumentOutOfRangeException(nameof(terms), $"Variable index {index} is out of range.");

            yield return _variables[index] * coefficient;
        }
    }

    /// <summary>
    /// Maps OR-Tools solver statuses to <see cref="IntegerLinearProgramStatus"/>.
    /// </summary>
    private static IntegerLinearProgramStatus MapStatus(CpSolverStatus status) => status switch
    {
        CpSolverStatus.Optimal => IntegerLinearProgramStatus.Optimal,
        CpSolverStatus.Feasible => IntegerLinearProgramStatus.Feasible,
        CpSolverStatus.Infeasible => IntegerLinearProgramStatus.Infeasible,
        CpSolverStatus.ModelInvalid => IntegerLinearProgramStatus.Invalid,
        _ => IntegerLinearProgramStatus.Unknown
    };

    /// <summary>
    /// Builds the parameter string used to configure <see cref="CpSolver"/>.
    /// </summary>
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

/// <summary>
/// Supported relational operators for linear constraints.
/// </summary>
public enum IntegerLinearConstraintRelation
{
    /// <summary>Expression equals the right-hand side.</summary>
    Equal,
    /// <summary>Expression is different from the right-hand side.</summary>
    NotEqual,
    /// <summary>Expression is less than or equal to the right-hand side.</summary>
    LessOrEqual,
    /// <summary>Expression is greater than or equal to the right-hand side.</summary>
    GreaterOrEqual,
    /// <summary>Expression is strictly less than the right-hand side.</summary>
    Less,
    /// <summary>Expression is strictly greater than the right-hand side.</summary>
    Greater,
}

/// <summary>
/// Represents the outcome of solving an <see cref="IntegerLinearProgram"/>.
/// </summary>
public enum IntegerLinearProgramStatus
{
    /// <summary>A provably optimal solution was found.</summary>
    Optimal,
    /// <summary>A feasible (but not provably optimal) solution was found.</summary>
    Feasible,
    /// <summary>No solution satisfies the constraints.</summary>
    Infeasible,
    /// <summary>The model is invalid.</summary>
    Invalid,
    /// <summary>The solver did not return a definitive status.</summary>
    Unknown
}

/// <summary>
/// Represents the result of solving an <see cref="IntegerLinearProgram"/>.
/// </summary>
/// <param name="Status">The solver status.</param>
/// <param name="VariableValues">Variable assignments in index order.</param>
/// <param name="ObjectiveValue">Objective value when an objective was defined; otherwise 0.</param>
public sealed record IntegerLinearProgramSolution(IntegerLinearProgramStatus Status, IReadOnlyList<long> VariableValues, long ObjectiveValue)
{
    /// <summary>
    /// Gets the value assigned to a variable by index.
    /// </summary>
    /// <param name="variableIndex">Zero-based variable index.</param>
    /// <returns>The value assigned to the variable.</returns>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when the index is out of range.</exception>
    public long GetValue(int variableIndex)
    {
        if (variableIndex < 0 || variableIndex >= VariableValues.Count)
            throw new ArgumentOutOfRangeException(nameof(variableIndex));

        return VariableValues[variableIndex];
    }
}

/// <summary>
/// Solver configuration options for <see cref="IntegerLinearProgram"/>.
/// </summary>
/// <param name="SearchWorkers">Number of search worker threads; ignored when null or non-positive.</param>
/// <param name="MaxTimeSeconds">Maximum solving time in seconds; ignored when null or non-positive.</param>
/// <param name="EnableSearchProgressLogging">Enables OR-Tools search progress logging when true.</param>
public sealed record IntegerLinearProgramOptions(int? SearchWorkers = null, double? MaxTimeSeconds = null, bool EnableSearchProgressLogging = false);
