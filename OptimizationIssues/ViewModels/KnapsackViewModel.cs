using OptimizationIssues.Models;

namespace OptimizationIssues.ViewModels
{
    public class KnapsackViewModel
    {
        public int KnapsackCapacity { get; set; }
        public List<int> Weights { get; set; }
        public List<int> Values { get; set; }

        public KnapsackViewModel()
        {
            Weights = new List<int>();
            Values = new List<int>();
        }

        public int SolveKnapsack()
        {
            KnapsackProblem problem = new KnapsackProblem(KnapsackCapacity, Weights, Values);
            return problem.Solve();
        }
    }
}
