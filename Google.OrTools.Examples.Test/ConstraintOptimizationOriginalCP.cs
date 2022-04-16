using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Google.OrTools.ConstraintSolver; // different package from ConstraintOptimization tests
using System.Linq;

namespace im.irrational.Google.OrTools.Examples.Test
{
    [TestClass]
    public class ConstraintOptimizationOritionalCP
    {

        /// <summary>
        /// https://developers.google.com/optimization/cp/cryptarithmetic
        /// 
        /// <code>
        ///         CP
        /// +       IS
        /// +      FUM
        /// ----------
        /// =     TRUE
        /// </code>
        /// </summary>
        [TestMethod]
        public void CryptarithmeticPuzzle1()
        {
            // Instantiate the solver.
            Solver solver = new Solver("CP is fun!");

            const int kBase = 10;

            // Decision variables.
            IntVar c = solver.MakeIntVar(1, kBase - 1, "C");
            IntVar p = solver.MakeIntVar(0, kBase - 1, "P");
            IntVar i = solver.MakeIntVar(1, kBase - 1, "I");
            IntVar s = solver.MakeIntVar(0, kBase - 1, "S");
            IntVar f = solver.MakeIntVar(1, kBase - 1, "F");
            IntVar u = solver.MakeIntVar(0, kBase - 1, "U");
            IntVar n = solver.MakeIntVar(0, kBase - 1, "N");
            IntVar t = solver.MakeIntVar(1, kBase - 1, "T");
            IntVar r = solver.MakeIntVar(0, kBase - 1, "R");
            IntVar e = solver.MakeIntVar(0, kBase - 1, "E");

            // Group variables in a vector so that we can use AllDifferent.
            IntVar[] letters = new IntVar[] { c, p, i, s, f, u, n, t, r, e };

            // Verify that we have enough digits.
            if (kBase < letters.Length)
            {
                throw new Exception("kBase < letters.Length");
            }

            // Define constraints.
            solver.Add(letters.AllDifferent());

            // CP + IS + FUN = TRUE
            solver.Add(p + s + n + kBase * (c + i + u) + kBase * kBase * f ==
                       e + kBase * u + kBase * kBase * r + kBase * kBase * kBase * t);

            int SolutionCount = 0;
            // Create the decision builder to search for solutions.
            DecisionBuilder db = solver.MakePhase(letters, Solver.CHOOSE_FIRST_UNBOUND, Solver.ASSIGN_MIN_VALUE);
            solver.NewSearch(db);
            while (solver.NextSolution())
            {
                Console.Write("C=" + c.Value() + " P=" + p.Value());
                Console.Write(" I=" + i.Value() + " S=" + s.Value());
                Console.Write(" F=" + f.Value() + " U=" + u.Value());
                Console.Write(" N=" + n.Value() + " T=" + t.Value());
                Console.Write(" R=" + r.Value() + " E=" + e.Value());
                Console.WriteLine();

                // Is CP + IS + FUN = TRUE?

                Assert.AreEqual(p.Value() + s.Value() + n.Value() + kBase * (c.Value() + i.Value() + u.Value()) +
                        kBase * kBase * f.Value(),
                    e.Value() + kBase * u.Value() + kBase * kBase * r.Value() + kBase * kBase * kBase * t.Value());


                SolutionCount++;
            }
            solver.EndSearch();
            Console.WriteLine($"Number of solutions found: {SolutionCount}");

            // assert
            Assert.AreEqual(72, SolutionCount);
        }

        /// <summary>
        /// https://developers.google.com/optimization/cp/queens
        /// </summary>
        [DataTestMethod]
        [DataRow(1, 1)]
        [DataRow(2, -1)]
        [DataRow(3, -1)]
        [DataRow(4, 2)]
        [DataRow(5, 10)]
        [DataRow(6, 4)]
        [DataRow(7, 40)]
        [DataRow(8, 92)]
        [DataRow(9, 352)]
        [DataRow(10, 724)]
        [DataRow(11, 2680)]
        [DataRow(12, 14200)]
        [DataRow(13, 73712)]
        // [DataRow(14, 365596)]
        // [DataRow(15, 1)]
        // [DataRow(16, 1)]
        public void NQueesProblem(int boardSize, int nSolutions)
        {
            // Instantiate the solver.
            Solver solver = new Solver("N-Queens");

            IntVar[] queens = new IntVar[boardSize];
            for (int i = 0; i < boardSize; ++i)
            {
                queens[i] = solver.MakeIntVar(0, boardSize - 1, $"x{i}");
            }

            // Define constraints.
            // All rows must be different.
            solver.Add(queens.AllDifferent());

            // All columns must be different because the indices of queens are all different.
            // No two queens can be on the same diagonal.
            IntVar[] diag1 = new IntVar[boardSize];
            IntVar[] diag2 = new IntVar[boardSize];
            for (int i = 0; i < boardSize; ++i)
            {
                diag1[i] = solver.MakeSum(queens[i], i).Var();
                diag2[i] = solver.MakeSum(queens[i], -i).Var();
            }

            solver.Add(diag1.AllDifferent());
            solver.Add(diag2.AllDifferent());

            // Create the decision builder to search for solutions.
            DecisionBuilder db = solver.MakePhase(queens, Solver.CHOOSE_FIRST_UNBOUND, Solver.ASSIGN_MIN_VALUE);

            // Iterates through the solutions, displaying each.
            int SolutionCount = 0;
            solver.NewSearch(db);
            while (solver.NextSolution())
            {
                Console.WriteLine("Solution " + SolutionCount);
                for (int i = 0; i < boardSize; ++i)
                {
                    for (int j = 0; j < boardSize; ++j)
                    {
                        if (queens[j].Value() == i)
                        {
                            Console.Write("Q");
                        }
                        else
                        {
                            Console.Write("_");
                        }
                        if (j != boardSize - 1)
                            Console.Write(" ");
                    }
                    Console.WriteLine("");
                }
                SolutionCount++;
            }
            solver.EndSearch();

            // Statistics.
            Console.WriteLine("Statistics");
            Console.WriteLine($"  failures: {solver.Failures()}");
            Console.WriteLine($"  branches: {solver.Branches()}");
            Console.WriteLine($"  wall time: {solver.WallTime()} ms");
            Console.WriteLine($"  Solutions found: {SolutionCount}");

            Assert.AreEqual(Math.Max(nSolutions, 0), SolutionCount);
        }
    }
}
