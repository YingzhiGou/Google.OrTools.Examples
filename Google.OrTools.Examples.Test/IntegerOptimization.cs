using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Google.OrTools.LinearSolver;

namespace im.irrational.Google.OrTools.Examples.Test
{
    [TestClass]
    public class IntegerOptimization
    {
        /// <summary>
        /// example from https://developers.google.com/optimization/mip/mip_example
        /// 
        /// Maximize x + 10y subject to the following constraints:
        /// <list type="bullet">
        /// <item>x + 7y <= 17.5</item>
        /// <item>x <= 3.5</item>
        /// <item>x >= 0</item>
        /// <item>y >= 0</item>
        /// <item>x,y integers</item>
        /// </list>
        /// </summary>
        [TestMethod]
        public void Example1()
        {
            // Create the linear solver with the SCIP backend.
            Solver solver = Solver.CreateSolver("SCIP");

            // x and y are integer non-negative variables.
            Variable x = solver.MakeIntVar(0.0, double.PositiveInfinity, "x");
            Variable y = solver.MakeIntVar(0.0, double.PositiveInfinity, "y");

            Console.WriteLine($"Number of variables = {solver.NumVariables()}");

            // x + 7 * y <= 17.5.
            solver.Add(x + 7 * y <= 17.5);

            // x <= 3.5.
            solver.Add(x <= 3.5);

            Console.WriteLine($"Number of constraints = {solver.NumConstraints()}");

            // Maximize x + 10 * y.
            solver.Maximize(x + 10 * y);

            Solver.ResultStatus resultStatus = solver.Solve();

            // Check that the problem has an optimal solution.
            if (resultStatus != Solver.ResultStatus.OPTIMAL)
            {
                Console.WriteLine("The problem does not have an optimal solution!");
            }

            Console.WriteLine("Solution:");
            Console.WriteLine("Objective value = " + solver.Objective().Value());
            Console.WriteLine("x = " + x.SolutionValue());
            Console.WriteLine("y = " + y.SolutionValue());

            // assert
            Assert.AreEqual(Solver.ResultStatus.OPTIMAL, resultStatus);
            Assert.AreEqual(23, solver.Objective().Value());
            Assert.AreEqual(3, x.SolutionValue());
            Assert.AreEqual(2, y.SolutionValue());
        }

        /// <summary>
        /// https://developers.google.com/optimization/mip/mip_var_array
        /// </summary>
        [TestMethod]
        public void MipVarArray()
        {
            double[,] constraintCoeffs = {
                { 5, 7, 9, 2, 1 },
                { 18, 4, -9, 10, 12 },
                { 4, 7, 3, 8, 5 },
                { 5, 13, 16, 3, -7 },
            };
            double[] bounds = { 250, 285, 211, 315 };
            double[] objCoeffs = { 7, 8, 2, 9, 6 };
            int numVars = 5;
            int numConstraints = 4;

            // Create the linear solver with the SCIP backend.
            Solver solver = Solver.CreateSolver("SCIP");

            Variable[] x = new Variable[numVars];
            for (int j = 0; j < numVars; j++)
            {
                x[j] = solver.MakeIntVar(0.0, double.PositiveInfinity, $"x_{j}");
            }
            Console.WriteLine("Number of variables = " + solver.NumVariables());

            for (int i = 0; i < numConstraints; ++i)
            {
                Constraint constraint = solver.MakeConstraint(0, bounds[i], "");
                for (int j = 0; j < numVars; ++j)
                {
                    constraint.SetCoefficient(x[j], constraintCoeffs[i, j]);
                }
            }
            Console.WriteLine("Number of constraints = " + solver.NumConstraints());

            Objective objective = solver.Objective();
            for (int j = 0; j < numVars; ++j)
            {
                objective.SetCoefficient(x[j], objCoeffs[j]);
            }
            objective.SetMaximization();

            Solver.ResultStatus resultStatus = solver.Solve();

            // Check that the problem has an optimal solution.
            if (resultStatus != Solver.ResultStatus.OPTIMAL)
            {
                Console.WriteLine("The problem does not have an optimal solution!");
                return;
            }

            Console.WriteLine("Solution:");
            Console.WriteLine("Optimal objective value = " + solver.Objective().Value());

            for (int j = 0; j < numVars; ++j)
            {
                Console.WriteLine("x[" + j + "] = " + x[j].SolutionValue());
            }

            Console.WriteLine("\nAdvanced usage:");
            Console.WriteLine("Problem solved in " + solver.WallTime() + " milliseconds");
            Console.WriteLine("Problem solved in " + solver.Iterations() + " iterations");
            Console.WriteLine("Problem solved in " + solver.Nodes() + " branch-and-bound nodes");

            // asserts
            Assert.AreEqual(Solver.ResultStatus.OPTIMAL, resultStatus);
            Assert.AreEqual(259.99999999999966, solver.Objective().Value());
        }
    }
}
