using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Google.OrTools.Sat;
using System.Linq;

namespace im.irrational.Google.OrTools.Examples.Test
{
    [TestClass]
    public class ConstraintOptimization
    {
        /// <summary>
        /// https://developers.google.com/optimization/cp/cp_solver
        /// 
        /// <list type="bullet">
        /// <item>Three variables, x, y, and z, each of which can take on the values: 0, 1, or 2.</item>
        /// <item>One constraint: x ≠ y</item>
        /// </list>
        /// </summary>
        [TestMethod]
        public void SimpleSetProgram()
        {
            // create model
            CpModel model = new CpModel();

            // define variables
            int num_vals = 3;

            IntVar x = model.NewIntVar(0, num_vals - 1, "x");
            IntVar y = model.NewIntVar(0, num_vals - 1, "y");
            IntVar z = model.NewIntVar(0, num_vals - 1, "z");

            // creates the constraints 
            model.Add(x != y);

            // creates a solver and solve the model
            CpSolver solver = new CpSolver();
            CpSolverStatus status = solver.Solve(model);

            if (status == CpSolverStatus.Optimal || status == CpSolverStatus.Feasible)
            {
                Console.WriteLine($"x = {solver.Value(x)}.");
                Console.WriteLine($"y = {solver.Value(y)}.");
                Console.WriteLine($"z = {solver.Value(z)}");
            }
            else
            {
                Console.WriteLine($"No Solution found.");
            }

            Assert.AreEqual(CpSolverStatus.Optimal, status);
        }

        /// <summary>
        /// https://developers.google.com/optimization/cp/cp_solver#all_solutions
        /// </summary>
        [TestMethod]
        public void FindAllSolultions()
        {
            // Creates the model.
            CpModel model = new CpModel();

            // Creates the variables.
            int num_vals = 3;

            IntVar x = model.NewIntVar(0, num_vals - 1, "x");
            IntVar y = model.NewIntVar(0, num_vals - 1, "y");
            IntVar z = model.NewIntVar(0, num_vals - 1, "z");

            // Adds a different constraint.
            model.Add(x != y);

            // Creates a solver and solves the model.
            CpSolver solver = new CpSolver();
            VarArraySolutionPrinter cb = new VarArraySolutionPrinter(new IntVar[] { x, y, z });
            // Search for all solutions.
            solver.StringParameters = "enumerate_all_solutions:true";
            // And solve.
            CpSolverStatus status = solver.Solve(model, cb);

            Console.WriteLine($"Number of solutions found: {cb.SolutionCount()}");

            Assert.AreEqual(18, cb.SolutionCount());
            Assert.AreEqual(CpSolverStatus.Optimal, status);
        }

        class VarArraySolutionPrinter : CpSolverSolutionCallback
        {
            public VarArraySolutionPrinter(IntVar[] variables)
            {
                variables_ = variables;
            }

            public override void OnSolutionCallback()
            {
                {
                    Console.WriteLine(String.Format("Solution #{0}: time = {1:F2} s", solution_count_, WallTime()));
                    foreach (IntVar v in variables_)
                    {
                        Console.WriteLine(String.Format("  {0} = {1}", v.ToString(), Value(v)));
                    }
                    solution_count_++;
                }
            }

            public int SolutionCount()
            {
                return solution_count_;
            }

            private int solution_count_;
            private IntVar[] variables_;
        }

        /// <summary>
        /// https://developers.google.com/optimization/cp/cp_example
        /// 
        /// Maximize 2x + 2y + 3z subject to the following constraints:
        /// <list type="bullet">
        ///     <item>x + 7⁄2 y + 3⁄2 z	≤	25</item>
        ///     <item>3x - 5y + 7z	≤	45</item>
        ///     <item>5x + 2y - 6z	≤	37</item>
        ///     <item>x, y, z	≥	0</item>
        ///     <item>x, y, z integers</item>
        /// </list>
        /// 
        /// </summary>
        [TestMethod]
        public void CpSatExample()
        {
            // create a model
            CpModel model = new CpModel();

            // define variables
            int varUpperBound = new int[] { 50, 45, 37 }.Max();

            IntVar x = model.NewIntVar(0, varUpperBound, "x");
            IntVar y = model.NewIntVar(0, varUpperBound, "y");
            IntVar z = model.NewIntVar(0, varUpperBound, "z");

            // Creates the constraints
            model.Add(2 * x + 7 * y + 3 * z <= 50);
            model.Add(3 * x - 5 * y + 7 * z <= 45);
            model.Add(5 * x + 2 * y - 6 * z <= 37);

            model.Maximize(2 * x + 2 * y + 3 * z); 
            
            // Creates a solver and solves the model.
            CpSolver solver = new CpSolver();
            CpSolverStatus status = solver.Solve(model);

            if (status == CpSolverStatus.Optimal || status == CpSolverStatus.Feasible)
            {
                Console.WriteLine($"Maximum of objective function: {solver.ObjectiveValue}");
                Console.WriteLine("x = " + solver.Value(x));
                Console.WriteLine("y = " + solver.Value(y));
                Console.WriteLine("z = " + solver.Value(z));
            }
            else
            {
                Console.WriteLine("No solution found.");
            }

            Console.WriteLine("Statistics");
            Console.WriteLine($"  conflicts: {solver.NumConflicts()}");
            Console.WriteLine($"  branches : {solver.NumBranches()}");
            Console.WriteLine($"  wall time: {solver.WallTime()}s");

            // assert
            Assert.AreEqual(CpSolverStatus.Optimal, status);
            Assert.AreEqual(35, solver.ObjectiveValue);
            Assert.AreEqual(7, solver.Value(x));
            Assert.AreEqual(3, solver.Value(y));
            Assert.AreEqual(5, solver.Value(z));
        }
    }
}
