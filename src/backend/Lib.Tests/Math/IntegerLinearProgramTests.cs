using Lib.Math;

namespace Lib.Tests.Math;

public class IntegerLinearProgramTests
{
    [Test]
    public void AddVariable_InvalidBounds_Throws()
    {
        var ilp = new IntegerLinearProgram();

        Assert.Throws<ArgumentOutOfRangeException>(() => ilp.AddVariable(lowerBound: 0, upperBound: -1));
    }

    [Test]
    public void AddConstraint_InvalidVariableIndex_Throws()
    {
        var ilp = new IntegerLinearProgram();
        ilp.AddVariable(0, 1);

        Assert.Throws<ArgumentOutOfRangeException>(() =>
            ilp.AddConstraint([(1, 1L)], IntegerLinearConstraintRelation.Equal, 0));
    }

    [Test]
    public void Solve_FindsOptimalSolutionWithEqualityConstraints()
    {
        var ilp = new IntegerLinearProgram();
        var x = ilp.AddVariable(0, 10, "x");
        var y = ilp.AddVariable(0, 10, "y");

        ilp.AddConstraint([(x, 1L), (y, 1L)], IntegerLinearConstraintRelation.Equal, 10);
        ilp.AddConstraint([(x, 1L), (y, -1L)], IntegerLinearConstraintRelation.Equal, 4);
        ilp.Minimize([(x, 1L), (y, 1L)]);

        var solution = ilp.Solve();

        Assert.Multiple(() =>
        {
            Assert.That(ilp.VariableCount, Is.EqualTo(2));
            Assert.That(solution.Status, Is.EqualTo(IntegerLinearProgramStatus.Optimal));
            Assert.That(solution.GetValue(x), Is.EqualTo(7));
            Assert.That(solution.GetValue(y), Is.EqualTo(3));
            Assert.That(solution.ObjectiveValue, Is.EqualTo(10));
        });
    }

    [Test]
    public void Solve_RespectsStrictInequalities()
    {
        var ilp = new IntegerLinearProgram();
        var x = ilp.AddVariable(0, 10);

        ilp.AddConstraint([(x, 1L)], IntegerLinearConstraintRelation.Greater, 3);
        ilp.AddConstraint([(x, 1L)], IntegerLinearConstraintRelation.Less, 6);
        ilp.Minimize([(x, 1L)]);

        var solution = ilp.Solve();

        Assert.Multiple(() =>
        {
            Assert.That(solution.Status, Is.EqualTo(IntegerLinearProgramStatus.Optimal));
            Assert.That(solution.GetValue(x), Is.EqualTo(4));
            Assert.That(solution.ObjectiveValue, Is.EqualTo(4));
        });
    }

    [Test]
    public void Solve_RespectsNotEqualConstraint()
    {
        var ilp = new IntegerLinearProgram();
        var x = ilp.AddVariable(0, 2);

        ilp.AddConstraint([(x, 1L)], IntegerLinearConstraintRelation.NotEqual, 1);
        ilp.Minimize([(x, 1L)]);

        var solution = ilp.Solve();

        Assert.Multiple(() =>
        {
            Assert.That(solution.Status, Is.EqualTo(IntegerLinearProgramStatus.Optimal));
            Assert.That(solution.GetValue(x), Is.EqualTo(0));
            Assert.That(solution.ObjectiveValue, Is.EqualTo(0));
        });
    }

    [Test]
    public void Solve_ReturnsInfeasibleStatusWhenConstraintsImpossible()
    {
        var ilp = new IntegerLinearProgram();
        var x = ilp.AddVariable(0, 1);

        ilp.AddConstraint([(x, 1L)], IntegerLinearConstraintRelation.GreaterOrEqual, 2);

        var solution = ilp.Solve();

        Assert.Multiple(() =>
        {
            Assert.That(solution.Status, Is.EqualTo(IntegerLinearProgramStatus.Infeasible));
            Assert.That(solution.VariableValues, Has.Count.EqualTo(1));
        });
    }

    [Test]
    public void Solve_WithoutObjective_ReturnsZeroObjectiveValue()
    {
        var ilp = new IntegerLinearProgram();
        var x = ilp.AddVariable(0, 2);

        ilp.AddConstraint([(x, 1L)], IntegerLinearConstraintRelation.Equal, 2);

        var solution = ilp.Solve();

        Assert.Multiple(() =>
        {
            Assert.That(solution.Status, Is.EqualTo(IntegerLinearProgramStatus.Feasible).Or.EqualTo(IntegerLinearProgramStatus.Optimal));
            Assert.That(solution.GetValue(x), Is.EqualTo(2));
            Assert.That(solution.ObjectiveValue, Is.EqualTo(0));
        });
    }

    [Test]
    public void Solve_RecordsParameterStringFromOptions()
    {
        var ilp = new IntegerLinearProgram();
        var x = ilp.AddVariable(0, 1);

        ilp.Minimize([(x, 1L)]);

        var options = new IntegerLinearProgramOptions(SearchWorkers: 4, MaxTimeSeconds: 2.5, EnableSearchProgressLogging: true);

        _ = ilp.Solve(options);

        Assert.That(ilp.LastParameterString, Is.EqualTo("num_search_workers:4,max_time_in_seconds:2.5,log_search_progress:true"));
    }
}
