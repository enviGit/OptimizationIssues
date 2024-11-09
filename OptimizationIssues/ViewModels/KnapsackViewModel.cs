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

        public (int MaxValue, List<(int Weight, int Value)> SelectedItems, int UsedCapacity) SolveKnapsackWithDetails()
        {
            KnapsackProblem problem = new KnapsackProblem(KnapsackCapacity, Weights, Values);
            return problem.SolveWithDetails();
        }
    }
}