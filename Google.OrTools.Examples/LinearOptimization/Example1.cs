using System;
using Google.OrTools.LinearSolver;

namespace im.irrational.Google.OrTools.Examples.LinearOptimization
{
    /// <summary>
    /// Example from https://developers.google.com/optimization/introduction/dotnet
    /// 
    /// Maximize 3x + y subject to the following constraints:
    ///     |-------------|
    ///     | 0 <= x <= 1 |
    ///     | 0 <= y <= 2 |
    ///     | x + y <= 2  |
    ///     |-------------|
    /// </summary>
    internal class Example1
    {
        static public void Solve()
        {
            // create the linear solver with the GLOP back-end
            Solver solver = Solver.CreateSolver("GLOP");

            // create the variable x and y.
            Variable x = solver.MakeNumVar(0.0, 1.0, "x");
            Variable y = solver.MakeNumVar(0.0, 2.0, "y");

            Console.WriteLine("Number of variables = " + solver.NumVariables());

            // create a linear constraint, 0 <= x + y <= 2
            Constraint ct = solver.MakeConstraint(0.0, 2.0, "ct");
            ct.SetCoefficient(x, 1);
            ct.SetCoefficient(y, 1);

            Console.WriteLine("Number of constraints = " + solver.NumConstraints());

            // create the objective function 3 * x + y
            Objective objective = solver.Objective();
            objective.SetCoefficient(x, 3);
            objective.SetCoefficient(y, 1);
            objective.SetMaximization();

            solver.Solve();

            Console.WriteLine("Solution:");
            Console.WriteLine("Objective value = " + objective.Value());
            Console.WriteLine("x = " + x.SolutionValue());
            Console.WriteLine("y = " + y.SolutionValue());
        }
    }
}
