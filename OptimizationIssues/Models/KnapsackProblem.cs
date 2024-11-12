namespace OptimizationIssues.Models
{
    public class KnapsackProblem : ProblemBase
    {
        public int KnapsackCapacity { get; set; }
        public List<int> Indexes { get; set; }
        public List<int> Weights { get; set; }
        public List<int> Values { get; set; }

        public KnapsackProblem(int capacity, List<int> indexes, List<int> weights, List<int> values)
        {
            KnapsackCapacity = capacity;
            Weights = weights;
            Values = values;
            Indexes = Enumerable.Range(0, weights.Count).ToList();
        }

        public override int Solve(int a = 0, int b = 0, int c = 0)
        {
            return 0;
        }

        public (int MaxValue, List<(int Index, int Weight, int Value)> SelectedItems, int UsedCapacity) SolveWithDetails()
        {
            int n = Weights.Count;
            int[,] dp = new int[n + 1, KnapsackCapacity + 1];

            for (int i = 0; i <= n; i++)
            {
                for (int w = 0; w <= KnapsackCapacity; w++)
                {
                    if (i == 0 || w == 0)
                        dp[i, w] = 0;
                    else if (Weights[i - 1] <= w)
                        dp[i, w] = Math.Max(dp[i - 1, w], Values[i - 1] + dp[i - 1, w - Weights[i - 1]]);
                    else
                        dp[i, w] = dp[i - 1, w];
                }
            }

            int maxValue = dp[n, KnapsackCapacity];
            List<(int Index, int Weight, int Value)> selectedItems = new List<(int, int, int)>();
            int remainingCapacity = KnapsackCapacity;
            int usedCapacity = 0;

            for (int i = n; i > 0 && remainingCapacity > 0; i--)
            {
                if (dp[i, remainingCapacity] != dp[i - 1, remainingCapacity])
                {
                    selectedItems.Add((Indexes[i - 1], Weights[i - 1], Values[i - 1]));
                    remainingCapacity -= Weights[i - 1];
                    usedCapacity += Weights[i - 1];
                }
            }

            return (maxValue, selectedItems, usedCapacity);
        }
    }
}