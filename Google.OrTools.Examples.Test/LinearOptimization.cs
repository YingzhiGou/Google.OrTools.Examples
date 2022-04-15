using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Google.OrTools.LinearSolver;

namespace im.irrational.Google.OrTools.Examples.Test
{
    [TestClass]
    public class LinearOptimization
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
        [TestMethod]
        public void Example1()
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

            Assert.AreEqual(4.0, objective.Value());
            Assert.AreEqual(1.0, x.SolutionValue());
            Assert.AreEqual(1.0, y.SolutionValue());
        }

        /// <summary>
        /// Example from https://developers.google.com/optimization/lp/lp_example
        /// 
        /// Solving a Linear Programing problem
        /// 
        /// Maximize 3x + 4y subject to the following constraints:
        /// <list type="bullet">
        /// <item>x + 2y <= 14</item>
        /// <item>3x - y >= 0</item>
        /// <item>x - y <= 2</item>
        /// </list>
        /// </summary>
        [TestMethod]
        public void LPProblem()
        {
            Solver solver = Solver.CreateSolver("GLOP");

            // x and y are continuous non-negative variables
            Variable x = solver.MakeNumVar(0.0, double.PositiveInfinity, "x");
            Variable y = solver.MakeNumVar(0.0, double.PositiveInfinity, "y");

            Console.WriteLine($"Number of variables: {solver.NumVariables()}");

            // x + 2y <= 14
            solver.Add(x + 2 * y <= 14.0);

            // 3x -y >= 0
            solver.Add(3 * x - y >= 0.0);

            // x - y <= 2
            solver.Add(x - y <= 2.0);

            Console.WriteLine($"Number of constraints = {solver.NumConstraints()}");

            // objective function: 3x + 4y
            solver.Maximize(3 * x + 4 * y);

            Solver.ResultStatus resultStatus = solver.Solve();

            // check that the problem has an optimal solution
            if (resultStatus != Solver.ResultStatus.OPTIMAL)
            {
                Console.WriteLine("The problem does not have an optimal solution!");
            }

            Assert.AreEqual(Solver.ResultStatus.OPTIMAL, resultStatus);

            // print solution
            Console.WriteLine("Solution:");
            Console.WriteLine($"Objective value = {solver.Objective().Value()}");
            Console.WriteLine($"x = {x.SolutionValue()}");
            Console.WriteLine($"y = {y.SolutionValue()}");

            Console.WriteLine("\nAdvanced usage:");
            Console.WriteLine("Problem solved in " + solver.WallTime() + " milliseconds");
            Console.WriteLine("Problem solved in " + solver.Iterations() + " iterations");

            // assert
            Assert.AreEqual(34.0, Math.Round(solver.Objective().Value(), 6));
            Assert.AreEqual(6.0, Math.Round(x.SolutionValue(), 6));
            Assert.AreEqual(4.0, Math.Round(y.SolutionValue(), 6));
        }
    }
}