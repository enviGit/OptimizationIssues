using OptimizationIssues.Models;

namespace OptimizationIssues.ViewModels
{
    public class KnapsackViewModel
    {
        public int KnapsackCapacity { get; set; }
        public List<int> Indexes { get; set; }
        public List<int> Weights { get; set; }
        public List<int> Values { get; set; }

        public KnapsackViewModel()
        {
            Indexes = new List<int>();
            Weights = new List<int>();
            Values = new List<int>();
        }

        public (int MaxValue, List<(int Index, int Weight, int Value)> SelectedItems, int UsedCapacity) SolveKnapsackWithDetails()
        {
            KnapsackProblem problem = new KnapsackProblem(KnapsackCapacity, Indexes, Weights, Values);
            return problem.SolveWithDetails();
        }
    }
}