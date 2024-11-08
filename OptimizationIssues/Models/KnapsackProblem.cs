namespace OptimizationIssues.Models
{
    public class KnapsackProblem : ProblemBase
    {
        public int KnapsackCapacity { get; set; }
        public List<int> Weights { get; set; }
        public List<int> Values { get; set; }

        public KnapsackProblem(int capacity, List<int> weights, List<int> values)
        {
            KnapsackCapacity = capacity;
            Weights = weights;
            Values = values;
        }

        public override int Solve()
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

            return dp[n, KnapsackCapacity];
        }
    }
}